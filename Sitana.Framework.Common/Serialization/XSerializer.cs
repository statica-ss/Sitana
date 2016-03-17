using Sitana.Framework.Xml;
using Sitana.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sitana.Framework.Serialization
{
    public class XSerializer
    {
        XNode _file;

        public XSerializer(XNode node)
        {
            _file = node;
        }

        public void AddContentBinary(string name, byte[] binary)
        {
            var node = new XNode(_file, name);
            node.BinaryValue = binary;

            _file.Nodes.Add(node);
        }

        public void AddContentString(string name, string value)
        {
            var node = new XNode(_file, name);
            node.Value = value;

            _file.Nodes.Add(node);
        }

        public byte[] GetContentBinary(string name)
        {
            var node = _file.Nodes.Find(n => n.Tag == name);

            if (node != null)
            {
                if(node.BinaryValue == null)
                {
                    try
                    {
                        return Convert.FromBase64String(node.Value);
                    }
                    catch
                    {

                    }
                }
                return node.BinaryValue;
            }

            return null;
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
            if(list == null)
            {
                return;
            }

            var node = new XNode(_file, name);
            _file.Nodes.Add(node);

            foreach(var el in list)
            {
                Serialize(node, "Element", el);
            }
        }

        public List<T> DeserializeList<T>(string name, T defaultValue = default(T), List<T> list = null)
        {
            if (list != null)
            {
                list.Clear();
            }

            var node = _file.Nodes.Find(n => n.Tag == name);

            if(node != null)
            {
                if(list == null)
                {
                    list = new List<T>();
                }

                foreach(var cn in node.Nodes)
                {
                    T val = (T)Deserialize(cn, defaultValue, typeof(T));
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
            if (root == null)
            {
                root = _file;
            }

            if(obj == null)
            {
                return;
            }

            if (!SerializeBuiltIn(root, name, obj))
            {
                XNode node = new XNode(root, name);
                root.Nodes.Add(node);
                
                string type = obj.GetType().FullName + "," + obj.GetType().GetTypeInfo().Assembly.GetName().Name;

                node.AddAttribute("XSerializer.SerializedType", type);

                if(obj is IXSerializable)
                {
                    XNode serializableNode = new XNode(node, "IXSerializable");
                    node.Nodes.Add(serializableNode);

                    (obj as IXSerializable).Serialize(serializableNode);
                }

                SerializeProperties(node, name, obj);
                SerializeFields(node, name, obj);
            }
        }

        void SerializeFields(XNode root, string name, object obj)
        {
            XNode node = null;

            Type type = obj.GetType();

            foreach(var info in type.GetRuntimeFields())
            {
                Attribute attr = info.GetCustomAttribute(typeof(XSerializableAttribute));

                if(attr != null)
                {
                    if(node == null)
                    {
                        node = new XNode(root, "Fields");
                        root.Nodes.Add(node);
                    }

                    string id = info.Name;
                    object value = info.GetValue(obj);

                    Serialize(node, id, value);
                }
            }
        }

        void SerializeProperties(XNode root, string name, object obj)
        {
            XNode node = null;

            Type type = obj.GetType();

            foreach (var info in type.GetProperties())
            {
                Attribute attr = info.GetCustomAttribute(typeof(XSerializableAttribute));

                if (attr != null)
                {
                    if (node == null)
                    {
                        node = new XNode(root, "Properties");
                        root.Nodes.Add(node);
                    }

                    string id = info.Name;
                    object value = info.GetValue(obj);

                    Serialize(node, id, value);
                }
            }
        }

        bool SerializeBuiltIn(XNode root, string name, object obj)
        {
            var node = new XNode(root, name);
            if(!BuiltInSerializatorX.Serialize(node, obj))
            {
                return false;
            }

            root.Nodes.Add(node);
            return true;
        }
        

        void DeserializeProperties(object obj, XNode root)
        {
            XNode node = root.Nodes.Find(n => n.Tag == "Properties");
            Type type = obj.GetType();

            if(node != null)
            {
                foreach(var child in node.Nodes)
                {
                    PropertyInfo info = type.GetProperty(child.Tag);
                    object value = Deserialize(child, null, info.PropertyType);

                    info.SetValue(obj, value);
                }
            }
        }

        void DeserializeFields(object obj, XNode root)
        {
            XNode node = root.Nodes.Find(n => n.Tag == "Fields");
            Type type = obj.GetType();

            if (node != null)
            {
                foreach (var child in node.Nodes)
                {
                    FieldInfo info = type.GetField(child.Tag);
                    object value = Deserialize(child, null, info.FieldType);

                    info.SetValue(obj, value);
                }
            }
        }

        public T Deserialize<T>(string name, T defaultValue = default(T))
        {
            var node = _file.Nodes.Find(n => n.Tag == name);
            return (T)Deserialize(node, defaultValue, typeof(T));
        }

        object Deserialize(XNode node, object defaultValue, Type asType)
        {
            if(node != null)
            {
                string typeName = node.Attribute("XSerializer.SerializedType");
                Type type = string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName, true);

                if(type == null)
                {
                    object enumValue;
                    if (BuiltInSerializatorX.DeserializeEnum(node, asType, out enumValue))
                    {
                        return enumValue;
                    }

                    object value = BuiltInSerializatorX.Deserialize(node);

                    if(value == null)
                    {
                        return defaultValue;
                    }

                    return value;
                }

                object obj = Activator.CreateInstance(type);

                DeserializeProperties(obj, node);
                DeserializeFields(obj, node);

                if(obj is IXSerializable)
                {
                    XNode serializableNode = node.Nodes.Find(n => n.Tag == "IXSerializable");

                    if (serializableNode != null)
                    {
                        IXSerializable serializable = obj as IXSerializable;
                        serializable.Deserialize(serializableNode);
                    }
                }

                return obj;
            }

            return defaultValue;
        }
    }
}
