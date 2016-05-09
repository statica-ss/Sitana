using Sitana.Framework.Content;
using System.IO;

namespace Sitana.Framework.Ui.Core
{
    public sealed class AppDictionary: StringsManager
    {
        private static volatile AppDictionary _instance;
        private static object syncRoot = new object();

        private AppDictionary()
        {
            
        }

        public static AppDictionary Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppDictionary();
                        }
                    }
                }

                return _instance;
            }
        }

        public void Load(char separator, params string[] paths)
        {
            Clear();
            foreach (var path in paths)
            {
                try
                {
                    using (Stream stream = ContentLoader.Current.Open(path))
                    {
                        Append(stream, separator);
                    }
                }
                catch { }
            }
        }
    }
}
