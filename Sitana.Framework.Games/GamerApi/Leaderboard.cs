using System;
using System.Collections.Generic;
using Sitana.Framework.Xml;

namespace Sitana.Framework.GamerApi
{
    public class Leaderboard
    {
        public readonly string Id;
        public int Score { get; internal set;}

        private Dictionary<string, string> _title = new Dictionary<string, string>();

        public Leaderboard(XNode node)
        {
            Id = node.Attribute("Id");

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

