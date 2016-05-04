using Sitana.Framework.IO;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sitana.Framework.Diagnostics
{
    public static class ServiceMode
    {
        class ServiceModeUpdatable: IUpdatable
        {
            void IUpdatable.Update(float time)
            {
                ServiceMode.Update();
            }
        }

        static object _serviceModeLock = new object();

        static string[] _groups = new string[32];

        static int _enabledGroups = 0;

        static StringBuilder _cache = new StringBuilder();

        static bool _flush = false;
        static bool _enabled = false;

        static ServiceModeUpdatable _updatable;

        public static bool Enabled
        {
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;

                    if (_enabled)
                    {
                        _updatable = new ServiceModeUpdatable();
                        AppMain.Current.RegisterUpdatable(_updatable);
                    }
                    else
                    {
                        AppMain.Current.UnregisterUpdatable(_updatable);
                    }
                }
            }

            get
            {
                return _enabled;
            }
        }

        public static int RegisterGroup(string name, bool enabled = false)
        {
            for(int idx = 0; idx < 32; ++idx)
            {
                if(_groups[idx] == null)
                {
                    _groups[idx] = name;
                    EnableGroup(idx, enabled);
                    return idx;
                }
            }

            throw new Exception("Cannot register more than 32 service mode groups.");
        }

        public static void EnableGroup(int group, bool enabled)
        {
            int value = 1 << group;

            if(enabled)
            {
                _enabledGroups |= value;
            }
            else
            {
                _enabledGroups &= ~value;
            }
        }

        public static List<string> Groups
        {
            get
            {
                List<string> groups = new List<string>();
                for (int idx = 0; idx < 32; ++idx)
                {
                    if(_groups[idx] == null)
                    {
                        break;
                    }

                    groups.Add(_groups[idx]);
                }

                return groups;
            }
        }

        public static void WriteLine(int group, string format, params object[] args)
        {
            if (_enabled && group >= 0)
            {
                int groupValue = 1 << group;

                if ((_enabledGroups & groupValue) != 0)
                {
                    DateTime now = DateTime.Now;

                    lock (_serviceModeLock)
                    {
                        _cache.AppendFormat("{1:0000}-{2:00}-{3:00} {4:00}:{5:00}:{6:00} [{0}] ", _groups[group], now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                        if(args != null)
                        {
                            _cache.AppendFormat(format, args);
                        }
                        else
                        {
                            _cache.AppendLine(format);
                        }
                        _cache.Append('\n');
                        _flush = true;
                    }
                }
            }
        }

        public static string LoadLog(bool removeExisting)
        {
            using (var storageManager = new IsolatedStorageManager())
            {
                string text;
                using (var stream = storageManager.OpenFile("ServiceMode.log", IO.OpenFileMode.Open))
                {
                    var reader = new StreamReader(stream);
                    text = reader.ReadToEnd();   
                }

                if(removeExisting)
                {
                    storageManager.DeleteFile("ServiceMode.log");
                }

                return text;
            }
        }

        internal static void Update()
        {
            if (_flush)
            {
                string data = null;
                lock (_serviceModeLock)
                {
                    data = _cache.ToString();
                    _cache.Clear();
                    _flush = false;
                }

                using(var storageManager = new IsolatedStorageManager())
                {
                    using(var stream = storageManager.OpenFile("ServiceMode.log", IO.OpenFileMode.Append))
                    {
                        var writer = new StreamWriter(stream);
                        writer.AutoFlush = true;
                        writer.Write(data);
                    }
                }
            }
        }
    }
}
