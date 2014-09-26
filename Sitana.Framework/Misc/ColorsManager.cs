using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Sitana.Framework
{
    public class ColorsManager
    {
        private Dictionary<String, Color> _dictionary = new Dictionary<String, Color>();

        public Color? this[String id]
        {
            get
            {
                Color color;

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

        public Boolean Append(Stream stream)
        {
            try
            {
                // Create stream reader.
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);

                // Read whole file as text.
                String text = streamReader.ReadToEnd();

                // Split into lines.
                String[] lines = text.Split("\n".ToCharArray());

                // Iterate through lines.
                foreach (var line in lines)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    // Split texts in line.
                    String[] texts = line.Split("^ \t\r".ToCharArray());

                    String key = null;  // Key string.
                    String value = null;  // Localized string.

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
                        value.Replace(" ", String.Empty);

                        Color? color = null;

                        if (value.StartsWith(":"))
                        {
                            color = this[value];
                        }
                        else
                        {
                            color = ParseColor(value, true);
                        }

                        if (color.HasValue)
                        {
                            try
                            {
                                _dictionary.Add(key, color.Value);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }

        private Color? ParseColor(String value, Boolean premultiplied)
        {
            String[] rgb = value.Split(',');

            if (rgb.Length == 3)
            {
                return new Color(Int32.Parse(rgb[0]), Int32.Parse(rgb[1]), Int32.Parse(rgb[2]));
            }

            if (rgb.Length == 4)
            {
                if (premultiplied)
                {
                    return new Color(Int32.Parse(rgb[1]), Int32.Parse(rgb[2]), Int32.Parse(rgb[3])) * ((Single)Int32.Parse(rgb[0]) / 255.0f);
                }
                else
                {
                    return new Color(Int32.Parse(rgb[1]), Int32.Parse(rgb[2]), Int32.Parse(rgb[3]), Int32.Parse(rgb[0]));
                }
            }

            return null;
        }
    }
}
