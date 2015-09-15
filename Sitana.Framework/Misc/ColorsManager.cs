using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Sitana.Framework.Cs;
using Sitana.Framework.Helpers;
using Sitana.Framework.Content;

namespace Sitana.Framework
{
    public class ColorsManager: Singleton<ColorsManager>
    {
        private Dictionary<string, ColorWrapper> _dictionary = new Dictionary<string, ColorWrapper>();

        public ColorWrapper this[string id]
        {
            get
            {
                ColorWrapper color;

                if (!id.StartsWith(":"))
                {
                    return null;
                }

                if (_dictionary.TryGetValue(id, out color))
                {
                    return color;
                }

                return null;
            }
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Append(string path)
        {
            using (Stream stream = ContentLoader.Current.Open(path))
            {
                return Append(stream);
            }
        }

        public bool Append(Stream stream)
        {
            try
            {
                // Create stream reader.
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);

                // Read whole file as text.
                string text = streamReader.ReadToEnd();

                // Split into lines.
                string[] lines = text.Split("\n".ToCharArray());

                // Iterate through lines.
                foreach (var line in lines)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    // Split texts in line.
                    string[] texts = line.Split("^ \t\r".ToCharArray());

                    string key = null;  // Key string.
                    string value = null;  // Localized string.

                    // Iterate thru all phrases and assign first the key and then value.
                    foreach (var phrase in texts)
                    {
                        if (phrase != "")
                        {
                            if (key == null)
                            {
                                key = phrase;
                            }
                            else
                            {
                                if (value != "" && value != null)
                                {
                                    value += " ";
                                }

                                value += phrase.Replace('|', '\n');
                            }
                        }
                    }

                    // If line contains key and value, add them to dictionary.
                    if (key != null && value != null)
                    {
                        value.Replace(" ", string.Empty);

                        ColorWrapper destination = this[key];

                        if(destination == null)
                        {
                            destination = new ColorWrapper();
                        }

                        if (value.StartsWith(":"))
                        {
                            destination.Value = this[value].Value;
                        }
                        else
                        {
                            Color? col = ColorParser.Parse(value);

                            if (col.HasValue)
                            {
                                destination.Value = col.Value;
                            }
                        }

                        _dictionary[key] = destination;
                    }
                }
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }
    }
}
