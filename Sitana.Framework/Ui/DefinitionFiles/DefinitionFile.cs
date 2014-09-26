using System;
using System.Collections.Generic;
using System.Reflection;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Controllers;

namespace Sitana.Framework.Ui.DefinitionFiles
{
    public class DefinitionFile
    {
        public readonly Type Class;
        public readonly string Anchor;

        Dictionary<string, object> _values = new Dictionary<string, object>();

        bool _locked = false;

        public DefinitionFile(Type type, string anchor)
        {
            Anchor = anchor;
            Class = type;
        }

        public void Lock()
        {
            _locked = true;
        }

        public object this[string id]
        {
            get
            {
                object value;
                _values.TryGetValue(id, out value);

                return value;
            }

            set
            {
                if ( _locked )
                {
                    throw new Exception("Modifying values is not allowed.");
                }

                _values[id] = value;
            }
        }

        public readonly Dictionary<Type, DefinitionFile> AdditionalParameters = new Dictionary<Type, DefinitionFile>();

        public static Type GetType(XNode node)
        {
            string ns = node.Namespace;
            string cl = node.Tag;

            if ( ns.StartsWith("namespace:"))
            {
                string[] vals = ns.Substring(10).Split(',');
                string name = String.Format("{0}.{1},{2}", vals[0], cl, vals[1]);

                return Type.GetType(name);
            }

            return null;
        }

        static Type[] ParseMethodTypes = new Type[] { typeof(XNode), typeof(DefinitionFile) };

        public static DefinitionFile LoadFile(XNode node)
        {
            Type type = GetType(node);

            DefinitionFile file = null;
            MethodInfo method = type.GetMethod("Parse", ParseMethodTypes);

            if (method != null)
            {
                file = new DefinitionFile(type, node.Owner.Name);
                method.Invoke(null, new object[] { node, file });
                file.ParseAdditionalParameters(node);
            }

            return file;
        }

        public static DefinitionFile CreateFile(Type type, XNode attributesNode)
        {
            DefinitionFile file = null;
            MethodInfo method = type.GetMethod("Parse", ParseMethodTypes);

            if (method != null)
            {
                file = new DefinitionFile(type, "");
                method.Invoke(null, new object[] { attributesNode, file });
                file.ParseAdditionalParameters(attributesNode);
            }

            return file;
        }

        void ParseAdditionalParameters(XNode node)
        {
            Dictionary<Type, DefinitionFile> parameters = AdditionalParameters;

            foreach (var attrib in node.Attributes)
            {
                if (attrib.StartsWith("type:"))
                {
                    string[] elements = attrib.Substring(5).Split(':');

                    var type = Type.GetType(elements[0].Trim());

                    if (type == null)
                    {
                        string error = node.NodeError("Cannot find type defined in parameter namespace.");

                        if (DefinitionParser.EnableCheckMode)
                        {
                            ConsoleEx.WriteLine(error);
                            continue;
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }

                    if (!parameters.ContainsKey(type))
                    {
                        XNode newNode = node.EnucleateAttributes("type:" + elements[0] + ":");
                        DefinitionFile newFile = DefinitionFile.CreateFile(type, newNode);

                        if (newFile != null)
                        {
                            parameters.Add(type, newFile);
                        }
                    }
                }
            }
        }

        public IDefinitionClass CreateInstance(UiController controller, object context)
        {
            IDefinitionClass obj = (IDefinitionClass)Activator.CreateInstance(Class);
            obj.Init(controller, context, this);

            return obj;
        }
    }
}
