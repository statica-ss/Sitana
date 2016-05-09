// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace Sitana.Framework
{
    public class StringsManager
    {
        public Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public string this[string id]
        {
            get
            {
                string text;

                if (!id.StartsWith(":"))
                {
                    return id;
                }

                if (_dictionary.TryGetValue(id, out text))
                {
                    return text;
                }

                return id.Replace('_', ' ');
            }
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Append(Stream stream, char separator)
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
                    string[] texts = line.Split(separator);

                    string key = null;  // Key string.
                    string value = null;  // Localized string.

                    // Iterate thru all phrases and assign first the key and then value.
                    foreach (var phrase in texts)
                    {
                        if (phrase != "")
                        {
                            if (key == null)
                            {
                                key = phrase.Trim();
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
                        _dictionary.Add(key, value);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
