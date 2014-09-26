/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ebatianos
{
    public class StringsManager
    {
        public Dictionary<String, String> _dictionary = new Dictionary<String, String>();

        public String this[String id]
        {
            get
            {
                String text;

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
                        _dictionary.Add(key, value);
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
