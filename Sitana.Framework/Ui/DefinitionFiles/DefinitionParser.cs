// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;

namespace Sitana.Framework.Essentials.Ui.DefinitionFiles
{
    public struct DefinitionParser
    {
        public static bool EnableCheckMode = false;

        readonly XNode _node;

        public DefinitionParser(XNode node)
        {
            _node = node;
        }

        Exception Error(string id, string format, params object[] args)
        {
            string message = _node.NodeError(String.Format("{1}: {0}", String.Format(format, args), id));

            if ( EnableCheckMode )
            {
                ConsoleEx.WriteLine(message);
                return null;
            }
            else
            {
                return new Exception(message);
            }
        }

        public string Value(string attribute)
        {
            return _node.Attribute(attribute);
        }

        object ParseMethodOrField(string name)
        {
            if (name.StartsWith("{"))
            {
                bool binding = name.StartsWith("{{");
                name = name.Trim('{', '}');

                int index = name.IndexOf('(');

                if (index > 0)
                {
                    string[] elements = name.Substring(index + 1).Trim(')').Split(',');

                    object[] parameters;
                    if (elements[0].IsNullOrWhiteSpace())
                    {
                        parameters = new object[0];
                    }
                    else
                    {
                        parameters = new object[elements.Length];
                    }

                    for (int idx = 0; idx < parameters.Length; ++idx)
                    {
                        parameters[idx] = ParseParameter(name, elements[idx]);
                    }

                    name = name.Substring(0, index);

                    return new MethodName() { Name = name, Parameters = parameters, Binding = binding };
                }


                index = name.IndexOf('[');
                {
                    string[] elements = name.Substring(index + 1).Trim(']').Split(',');
                    object[] parameters = null;

                    if ( index > 0 )
                    {
                        parameters = new object[elements.Length];
                    }

                    if (parameters != null)
                    {
                        for (int idx = 0; idx < parameters.Length; ++idx)
                        {
                            parameters[idx] = ParseParameter(name, elements[idx]);
                        }
                    }

                    if (index > 0)
                    {
                        name = name.Substring(0, index);
                    }

                    return new FieldName() { Name = name, Parameters = parameters, Binding = binding };
                }


            }

            return null;
        }

        object ParseParameter(string methodDef, string val)
        {
            if ( val.StartsWith("\'"))
            {
                val = val.Trim('\'');
                return val;
            }

            val = val.Trim();

            int intVal;
            if (int.TryParse(val, out intVal))
            {
                return intVal;
            }

            val = val.Trim();
            return new ReflectionParameter(val);
        }

        public object ParseDelegate(string name)
        {
            name = Value(name);
            
            object result = ParseMethodOrField(name);

            if (result is MethodName || result is FieldName)
            {
                return result;
            }

            return null;
        }

        public object ParseString(string name)
        {
            name = Value(name);
            object method = ParseMethodOrField(name);

            if (method != null)
            {
                return method;
            }

            return name;
        }

        public object ParseColor(string id)
        {
            string name = Value(id);
            object method = ParseMethodOrField(name);

            if (method != null)
            {
                return method;
            }

            if (name.IsNullOrEmpty())
            {
                return null;
            }

            int r, g, b, a;

            if (name.StartsWith("#"))
            {
                if (name.Length == 7 || name.Length == 9)
                {
                    int color;
                    if (int.TryParse(name.Replace("#", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color))
                    {
                        a = (color >> 24) & 0xff;
                        r = (color >> 16) & 0xff;
                        g = (color >> 8) & 0xff;
                        b = color & 0xff;

                        if (name.Length == 7)
                        {
                            a = 255;
                        }

                        return Color.FromNonPremultiplied(r, g, b, a);
                    }
                }
            }

            string[] vals = name.Replace(" ", "").Split(',');

            if (vals.Length == 3)
            {
                if (Int32.TryParse(vals[0], out r) && Int32.TryParse(vals[1], out g) && Int32.TryParse(vals[2], out b))
                {
                    return new Color(r, g, b);
                }
            }

            if (vals.Length == 4)
            {
                if (Int32.TryParse(vals[0], out r) && Int32.TryParse(vals[1], out g) && Int32.TryParse(vals[2], out b) && Int32.TryParse(vals[3], out a))
                {
                    return Color.FromNonPremultiplied(r, g, b, a);
                }
            }

            Exception ex = Error(id, "Invalid format. Color formats are: '#aarrggbb' '#rrggbb' 'r,g,b' 'r,g,b,a'.");
            if ( ex != null) throw ex;

            return null;
        }

        public object ParseBoolean(string id)
        {
            string name = Value(id);
            object method = ParseMethodOrField(name);

            if (method != null)
            {
                return method;
            }

            if (name.IsNullOrEmpty())
            {
                return null;
            }

            bool value;

            if(bool.TryParse(name, out value))
            {
                return value;
            }

            Exception ex = Error(id, "Invalid format. Expected true or false or Method/Property name.");
            if (ex != null) throw ex;

            return null;
        }

        public object ParseNinePatchImage(string id)
        {
            string name = Value(id);
            object method = ParseMethodOrField(name);

            if (method != null)
            {
                return method;
            }

            try
            {
                return ContentLoader.Current.Load<NinePatchImage>(name);
            }
            catch(Exception ex)
            {
                if (!EnableCheckMode) throw ex;
                return null;
            }
        }

        public object ParseEnum<T>(string id) where T: struct
        {
            string name = Value(id);

            if (name.IsNullOrEmpty())
            {
                return null;
            }

            T value;
            if (Enum.TryParse<T>(name, true, out value))
            {
                return value;
            }

            Exception ex = Error(id, "Error while parsing enumeration: {0}.", typeof(T).FullName);
            if (ex != null) throw ex;

            return null;
        }

        public object ParseMargin(string id)
        {
            string name = Value(id);
            object method = ParseMethodOrField(name);

            if (method != null)
            {
                return method;
            }

            if (name.IsNullOrEmpty())
            {
                return null;
            }

            string[] elements = name.Replace(" ", "").Split(',');

            if ( elements.Length == 1 )
            {
                int value;
                if ( int.TryParse(elements[0], out value))
                {
                    return new Margin(value);
                }
            }
            else if ( elements.Length == 4 )
            {
                int left, top, right, bottom;
                if ( int.TryParse(elements[0], out left) &&
                     int.TryParse(elements[1], out top) &&
                     int.TryParse(elements[2], out right) &&
                     int.TryParse(elements[3], out bottom))
                {
                    return new Margin(left, top, right, bottom);
                }
            }

            Exception ex = Error(id, "Margin format is 'left,top,right,bottom' or 'all'.");
            if (ex != null) throw ex;

            return null;
        }

        public object ParseInt(string id)
        {
            string name = Value(id);
            object method = ParseMethodOrField(name);

            if (method != null)
            {
                return method;
            }

            if (name.IsNullOrEmpty())
            {
                return null;
            }

            int value;

            if (int.TryParse(name, out value))
            {
                return value;
            }

            Exception ex = Error(id, "Invalid format. Expected Integer.");
            if (ex != null) throw ex;

            return null;
        }

        public object ParseLength(string id)
        {
            string name = Value(id);
            object method = ParseMethodOrField(name);

            if (method != null)
            {
                return method;
            }

            if ( name.IsNullOrEmpty())
            {
                return new Length(true);
            }

            name = name.Replace(" ", "");

            bool percent = name.EndsWith("%");
            Length.Mode mode = Length.Mode.Begining;

            if (name.StartsWith("@"))
            {
                mode = Length.Mode.End;
            }
            else if (name.StartsWith("C"))
            {
                mode = Length.Mode.Center;
            }

            name = name.Trim('%', '@', 'C');
            int length;

            if (String.IsNullOrEmpty(name))
            {
                return new Length(0, percent, mode);
            }

            if ( int.TryParse(name, out length))
            {
                return new Length(length, percent, mode);
            }

            if ( name.ToLowerInvariant() == "auto" )
            {
                return new Length(true);
            }

            Exception ex = Error(id, "Length format is integer with optional '%' sign at the end.");
            if (ex != null) throw ex;

            return new Length(true);
        }
    }
}
