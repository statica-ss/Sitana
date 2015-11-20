using Sitana.Framework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE_APP
using Windows.UI.Xaml;
#endif

#if ANDROID
using Android.Runtime;
#endif

namespace Sitana.Framework.Diagnostics
{
    public class CrashReporter
    {
        public readonly static CrashReporter Instance = new CrashReporter();

        private string _appVersion;

        private object _crashesLock = new object();

        CrashService _service;

        List<ExceptionData> _unhandledExceptions = new List<ExceptionData>();

        public string AppName
        {
            get
            {
                return _service.AppName;
            }
        }

        private CrashReporter()
        {
        }

        public void Init(CrashService service)
        {
            _service = service;
            _appVersion = Platform.CurrentVersion;

			LoadLastSession();
			Send();

			//TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

			#if ANDROID
			AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledException;
            #elif WINDOWS_PHONE_APP
			#else
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			#endif
        }

        async void Send()
        {
            await _service.Start();

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
                using (var store = new IsolatedStorageManager())
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

		#if ANDROID
		void AndroidEnvironment_UnhandledException(object sender, RaiseThrowableEventArgs args)
		{
			lock (_crashesLock)
			{
				_unhandledExceptions.Add(new ExceptionData(_appVersion, DateTime.UtcNow, args.Exception));
			}

			Serialize();
		}
		#elif !WINDOWS_PHONE_APP
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
        #else
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (!args.Handled)
            {
                lock (_crashesLock)
                {
                    _unhandledExceptions.Add(new ExceptionData(_appVersion, DateTime.UtcNow, args.Exception));
                }

                Serialize();
            }
        }
		#endif

		void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
		{
			if (args.Exception == null)
			{
				return;
			}

			lock (_crashesLock)
			{
				_unhandledExceptions.Add(new ExceptionData(_appVersion, DateTime.UtcNow, args.Exception));
			}

			Serialize();
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
            using (var store = new IsolatedStorageManager())
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
