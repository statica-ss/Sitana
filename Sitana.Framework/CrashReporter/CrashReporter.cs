using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Sitana.Framework.CrashReporter.Data;
using Sitana.Framework.Cs;
using Sitana.Framework.DataTransfer;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Settings;

namespace Sitana.Framework.CrashReporter
{
    public class CrashReporter: Singleton<CrashReporter>
    {
        private static Type[] _crashReporterDataTypes = new Type[]{typeof(DateAndTime), typeof(Crash), typeof(CrashInstance), typeof(Device), typeof(Session), typeof(Application)};

        private object _exceptionsLock = new object();

        private List<Pair<DateTime, string>> _crashes = new List<Pair<DateTime, string>>();
        private string _deviceGuid = string.Empty;
        private bool _deviceSent = false;

        private List<Pair<DateTime, string>> _sentCrashes = new List<Pair<DateTime, string>>();

        private EcsHttpClient _httpClient;

#if ANDROID
        public void StartSession(String url, String appGuid, Context context)
        {
            InitSession(url, appGuid, "Android");
        }
#elif IOS
        public void StartSession(String url, String appGuid)
        {
            InitSession(url, appGuid, "iOS", "", "");
        }
#elif WINDOWS_PHONE
        public void StartSession(String url, String appGuid)
        {
            InitSession(url, appGuid, "Windows Phone 8");
        }
#elif MACOSX
        public void StartSession(String url, String appGuid)
        {
            InitSession(url, appGuid, "Mac OSX");
        }
#elif WINDOWS
        public void StartSession(String url, String appGuid)
        {
            InitSession(url, appGuid, "Windows", System.Environment.MachineName, System.Environment.OSVersion.ToString());
        }
#endif
        private void InitSession(String url, String appGuid, String platform, String deviceName, String osVersion)
        {
            Unserialize();

            AppDomain.CurrentDomain.UnhandledException += OnException;

            if (_deviceGuid == String.Empty)
            {
                _deviceGuid = UuidGenerator.GenerateString();
                Serialize();
            }

            _httpClient = new EcsHttpClient(url, _crashReporterDataTypes);

            _deviceSent = false;
            if (!_deviceSent)
            {
                SendDeviceInfo(deviceName, osVersion, platform);
            }

            SendCrashes(appGuid);
            SendStartSession(appGuid);
        }

        private void OnException(Object sender, UnhandledExceptionEventArgs args)
        {
            String exceptionString = args.ExceptionObject.ToString();

            lock (_exceptionsLock)
            {
                _crashes.Add(new Pair<DateTime, string>(DateTime.UtcNow, exceptionString));
            }

            Serialize();

            ConsoleEx.WriteLine(ConsoleEx.Error, exceptionString);

            if (RemoteConsoleClient.Instance.ConsoleAttached)
            {
                Thread.Sleep(250);
            }
        }

        private void SendDeviceInfo(String deviceName, String osVerion, String platform)
        {
            var deviceInfo = new Device()
            {
                Guid = _deviceGuid,
                Name = deviceName,
                Platform = platform,
                OsVersion = osVerion
            };

            var request = new EcsRequest()
            {
                Action = "ActionDeviceInfo",
                Data = deviceInfo
            };

            _httpClient.Request(request, (res) =>
                {
                    if (res.Status == 0)
                    {
                        _deviceSent = true;
                        Serialize();
                    }
                });
        }

        private void SendCrashes(string appGuid)
        {
            lock (_exceptionsLock)
            {
                _sentCrashes = _crashes;
                _crashes = new List<Pair<DateTime, string>>();
            }

            if (_sentCrashes.Count > CrashReportInfo.MaxCapacity)
            {
                _sentCrashes.RemoveRange(0, _sentCrashes.Count - CrashReportInfo.MaxCapacity);
            }

            var crashReportInfo = new CrashReportInfo()
            {
                ApplicationGuid = appGuid,
                DeviceGuid = _deviceGuid
            };

            foreach (var crash in _sentCrashes)
            {
                var crashReport = new CrashReport()
                {
                    Crash = crash.Second,
                    Time = new DateAndTime(crash.First)
                };

                crashReportInfo.Add(crashReport);
            }

            var request = new EcsRequest()
            {
                Action = "ActionCrashReport",
                Data = crashReportInfo
            };

            _httpClient.Request(request, OnSendCrashes);
        }

        private void OnSendCrashes(EcsResponse response)
        {
            if (response.Status == 0)
            {
                lock (_exceptionsLock)
                {
                    _sentCrashes = null;
                }

                Serialize();
            }
            else
            {
                lock (_exceptionsLock)
                {
                    if (_sentCrashes != null)
                    {
                        _crashes.InsertRange(0, _sentCrashes);
                        _sentCrashes = null;
                    }
                }
            }
        }

        private void SendStartSession(string appGuid)
        {
            var startSession = new Session()
            {
                ApplicationGuid = appGuid,
                DeviceGuid = _deviceGuid,
                Time = new DateAndTime(DateTime.UtcNow)
            };

            var request = new EcsRequest()
            {
                Action = "ActionStartSession",
                Data = startSession
            };

            _httpClient.Request(request, (response) => { });
        }

        private void Unserialize()
        {
            lock (_exceptionsLock)
            {
                try
                {
                    // Create path.
                    var path = Serializator.PathFromType(GetType());

                    // Open isolated storage.
                    using (var isolatedStorageFile = SystemWrapper.GetUserStoreForApplication())
                    {
                        // Open file from storage.
                        using (Stream stream = isolatedStorageFile.OpenFile(path, FileMode.Open))
                        {
                            using (BinaryReader reader = new BinaryReader(stream))
                            {
                                if (reader.ReadString() != "CRD")
                                {
                                    throw new Exception();
                                }

                                _deviceSent = reader.ReadBoolean();
                                _deviceGuid = reader.ReadString();

                                Int32 count = reader.ReadInt32();

                                for (Int32 idx = 0; idx < count; ++idx)
                                {
                                    long time = reader.ReadInt64();
                                    DateTime dt = DateTime.FromBinary(time);

                                    String text = reader.ReadString();

                                    _crashes.Add(new Pair<DateTime,string>(dt, text));
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private void Serialize()
        {
            lock (_exceptionsLock)
            {
                // Create path.
                var path = Serializator.PathFromType(GetType());

                // Open isolated storage.
                using (var isolatedStorageFile = SystemWrapper.GetUserStoreForApplication())
                {
                    // Open file from storage.
                    using (Stream stream = isolatedStorageFile.OpenFile(path, FileMode.Create))
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            int count = _crashes.Count;

                            if ( _sentCrashes != null )
                            {
                                count += _sentCrashes.Count;
                            }

                            writer.Write("CRD");

                            writer.Write(_deviceSent);
                            writer.Write(_deviceGuid);

                            writer.Write(count);

                            if ( _sentCrashes != null )
                            {
                                foreach (var keyVal in _sentCrashes)
                                {
                                    writer.Write(keyVal.First.ToBinary());
                                    writer.Write(keyVal.Second);
                                }
                            }

                            foreach (var keyVal in _crashes)
                            {
                                writer.Write(keyVal.First.ToBinary());
                                writer.Write(keyVal.Second);
                            }
                        }
                    }
                }
            }
        }
    }
}
