using Sitana.Framework.Cs;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Serialization
{
    public static class BuiltInSerializatorX
    {
        public static object Deserialize(XNode node)
        {
            string type = node.Attribute("type");
            
            switch(type)
            {
                case "string":
                    return node.Attribute("value");

                case "int":
                    return int.Parse(node.Attribute("value"));

                case "float":
                    {
                        var value = node.Attribute("value");

                        if (value == "max")
                        {
                            return float.MaxValue;
                        }

                        if (value == "min")
                        {
                            return float.MinValue;
                        }

                        return float.Parse(value, CultureInfo.InvariantCulture);
                    }

                case "double":
                    {
                        var value = node.Attribute("value");

                        if (value == "max")
                        {
                            return double.MaxValue;
                        }

                        if (value == "min")
                        {
                            return double.MinValue;
                        }

                        return double.Parse(value, CultureInfo.InvariantCulture);
                    }

                case "long":
                    return long.Parse(node.Attribute("value"));

                case "bool":
                    return bool.Parse(node.Attribute("value"));

                case "dateTime":
                    return DateTime.Parse(node.Attribute("value"), CultureInfo.InvariantCulture);
            }

            return null;
        }

        public static bool DeserializeEnum<T>(XNode node, out T value)
        {
            string type = node.Attribute("type");

            if(type == "enum")
            {
                try
                {
                    value = (T)Enum.Parse(typeof(T), node.Attribute("value"));
                    return true;
                }
                catch
                {}
            }

            value = default(T);

            return false;
        }

        public static void Serialize(XNode node, object value)
        {
            if(value is Enum)
            {
                node.AddAttribute("type", "enum");
                node.AddAttribute("value", Enum.GetName(value.GetType(), value));
            }
            else if (value is string)
            {
                node.AddAttribute("type", "string");
                node.AddAttribute("value", (string)value);
            }
            else if (value is int)
            {
                node.AddAttribute("type", "int");
                node.AddAttribute("value", value.ToString());
            }
            else if (value is float)
            {
                float val = (float)value;

                node.AddAttribute("type", "float");

                string valueText = null;

                if(val >= float.MaxValue)
                {
                    valueText = "max";
                }
                else if (val <= float.MinValue)
                {
                    valueText = "min";
                }
                else
                {
                    valueText = string.Format(CultureInfo.InvariantCulture, "{0}", value);
                }

                node.AddAttribute("value", valueText);
            }
            else if (value is double)
            {
                double val = (double)value;

                node.AddAttribute("type", "double");

                string valueText = null;

                if (val >= double.MaxValue)
                {
                    valueText = "max";
                }
                else if (val <= double.MinValue)
                {
                    valueText = "min";
                }
                else
                {
                    valueText = string.Format(CultureInfo.InvariantCulture, "{0}", value);
                }

                node.AddAttribute("value", valueText);

            }
            else if (value is long)
            {
                node.AddAttribute("type", "long");
                node.AddAttribute("value", value.ToString());
            }
            else if (value is bool)
            {
                node.AddAttribute("type", "bool");
                node.AddAttribute("value", value.ToString());
            }
            else if (value is DateTime)
            {
                node.AddAttribute("type", "DateTime");
                node.AddAttribute("value", string.Format(CultureInfo.InvariantCulture, "{0}", value));
            }
            else
            {
                throw new InvalidOperationException("Cannot serialize type: " + value.GetType().Name);
            }
        }
    }
}
