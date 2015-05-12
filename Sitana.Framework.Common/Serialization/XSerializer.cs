using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sitana.Framework.Serialization
{
    public class XSerializer
    {
        XNode _file;

        public XSerializer(XNode node)
        {
            _file = node;
        }

        public void AddContentString(string name, string value)
        {
            var node = new XNode(_file, name);
            node.Value = value;

            _file.Nodes.Add(node);
        }

        public string GetContentString(string name)
        {
            var node = _file.Nodes.Find(n => n.Tag == name);

            if (node != null)
            {
                return node.Value;
            }

            return null;
        }

        public void SerializeList<T>(string name, List<T> list)
        {
            var node = new XNode(_file, name);
            _file.Nodes.Add(node);

            foreach(var el in list)
            {
                Serialize(node, "Element", el);
            }
        }

        public List<T> DeserializeList<T>(string name, T defaultValue = default(T))
        {
            List<T> list = new List<T>();

            var node = _file.Nodes.Find(n => n.Tag == name);

            if(node != null)
            {
                foreach(var cn in node.Nodes)
                {
                    T val = Deserialize<T>(cn, defaultValue);
                    list.Add(val);
                }
            }

            return list;
        }

        public void Serialize(string name, object obj)
        {
            Serialize(null, name, obj);
        }

        void Serialize(XNode root, string name, object obj)
        {
            if(obj is IXSerializable)
            {
                Serialize(root, name, obj as IXSerializable);
            }
            else
            {
                SerializeBuiltIn(root, name, obj);
            }
        }

        void SerializeBuiltIn(XNode root, string name, object obj)
        {
            if (root == null)
            {
                root = _file;
            }

            var node = new XNode(root, name);
            BuiltInSerializatorX.Serialize(node, obj);

            root.Nodes.Add(node);
        }

        void Serialize(XNode root, string name, IXSerializable obj)
        {
            string type = obj.GetType().FullName + "," + obj.GetType().Assembly.GetName().Name;

            if(root == null)
            {
                root = _file;
            }

            var node = new XNode(root, name);

            node.AddAttribute("XSerializer.SerializedType", type);
            obj.Serialize(node);

            root.Nodes.Add(node);
        }

        public T Deserialize<T>(string name, T defaultValue = default(T))
        {
            var node = _file.Nodes.Find(n => n.Tag == name);
            return Deserialize<T>(node, defaultValue);
        }

        T Deserialize<T>(XNode node, T defaultValue)
        {
            if(node != null)
            {
                string typeName = node.Attribute("XSerializer.SerializedType");
                Type type = string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName, true, true);

                if(type == null)
                {
                    T enumValue;
                    if (BuiltInSerializatorX.DeserializeEnum<T>(node, out enumValue))
                    {
                        return enumValue;
                    }

                    object value = BuiltInSerializatorX.Deserialize(node);

                    if(value == null)
                    {
                        return defaultValue;
                    }

                    return (T)value;
                }

                object obj = Activator.CreateInstance(type);

                if(obj is IXSerializable)
                {
                    IXSerializable serializable = obj as IXSerializable;
                    serializable.Deserialize(node);

                    return (T)serializable;
                }
            }

            return defaultValue;
        }
    }
}
