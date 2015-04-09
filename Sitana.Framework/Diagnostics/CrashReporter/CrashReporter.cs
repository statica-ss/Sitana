using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Diagnostics
{
    public class CrashReporter
    {
        public readonly static CrashReporter Instance = new CrashReporter();

        private string _appVersion;

        private object _crashesLock = new object();

        CrashService _service;

        List<ExceptionData> _unhandledExceptions = new List<ExceptionData>();

        private CrashReporter()
        {
        }

        public void Init(CrashService service)
        {
            _service = service;
            _appVersion = Platform.CurrentVersion;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            LoadLastSession();
            Send();
        }

        async void Send()
        {
            while(_unhandledExceptions.Count > 0)
            {
                ExceptionData data = await _service.SendOne(_unhandledExceptions[0]);

                if(data == null)
                {
                    _unhandledExceptions.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            Serialize();
        }

        void LoadLastSession()
        {
            try
            {
                using (var store = Platform.GetUserStoreForApplication())
                {
                    if (store.FileExists("CrashReporter"))
                    {
                        using (Stream stream = store.OpenFile("CrashReporter", FileMode.Open))
                        {
                            BinaryReader reader = new BinaryReader(stream);

                            int count = reader.ReadInt32();

                            for (int idx = 0; idx < count; ++idx)
                            {
                                _unhandledExceptions.Add(new ExceptionData(reader));
                            }
                        }
                    }
                }
            }
            catch { }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args.IsTerminating)
            {
                lock (_crashesLock)
                {
                    _unhandledExceptions.Add(new ExceptionData(_appVersion, DateTime.UtcNow, args.ExceptionObject));
                }

                Serialize();
            }
        }

        public void Clear()
        {
            lock (_crashesLock)
            {
                _unhandledExceptions.Clear();
            }

            Serialize();
        }

        private void Serialize()
        {
            lock (_crashesLock)
            {
                using (var store = Platform.GetUserStoreForApplication())
                {
                    using (Stream stream = store.OpenFile("CrashReporter", FileMode.Create))
                    {
                        BinaryWriter writer = new BinaryWriter(stream);

                        writer.Write(_unhandledExceptions.Count);

                        foreach (var crash in _unhandledExceptions)
                        {
                            crash.Serialize(writer);
                        }
                    }
                }
            }
        }
    }
}
