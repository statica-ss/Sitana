
namespace Sitana.Framework.Xml
{
    public struct XNodeAttributes
    {
        XNode _node;
        public XNodeAttributes(XNode node)
        {
            _node = node;
        }

        public bool HasAttribute(string name)
        {
            return _node.HasAttribute(name);
        }

        public int AsInt32(string name, int defaultValue)
        {
            int value;

            if (int.TryParse(_node.Attribute(name), out value))
            {
                return value;
            }

            return defaultValue;
        }

        public string AsString(string name, string defaultValue = "")
        {
            if (HasAttribute(name))
            {
                return _node.Attribute(name);
            }
            return defaultValue;
        }

        public bool AsBoolean(string name, bool defaultValue = false)
        {
            bool value;

            if (bool.TryParse(_node.Attribute(name), out value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
