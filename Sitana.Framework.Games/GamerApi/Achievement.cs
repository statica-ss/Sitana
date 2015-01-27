using System;
using System.Collections.Generic;
using Sitana.Framework.Xml;

namespace Sitana.Framework.GamerApi
{
    public class Achievement
    {
		public const int Completed = int.MaxValue;

        public readonly string Id;
        public readonly bool Hidden;

        public int Completion
        { 
            get
            {
                lock(_lock)
                {
                    return _completion;
                }
            }

            internal set
            {
                lock(_lock)
                {
                    _completion = value;
                }
            }
        }

        int _completion = 0;
        object _lock = new object();

        private Dictionary<string, string> _title = new Dictionary<string, string>();

        public Achievement(string id, bool hidden)
        {
            Id = id;
            Hidden = hidden;
        }

        public Achievement(XNode node)
        {
            Id = node.Attribute("Id");
            Hidden = node.Attribute("Hidden").ToUpperInvariant() == "TRUE";

            foreach(var lang in node.Nodes)
            {
                Title(lang.Tag, lang.Attribute("Title"));
            }
        }

        public void Title(string language, string title)
        {
            _title[language] = title;
        }

        public string Title(string language)
        {
            return _title[language];
        }
    }
}

