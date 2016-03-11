using Sitana.Framework.Diagnostics;
using Sitana.Framework.Ui.Binding;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Views
{
    public class UiItemsStack : UiStackPanel, IItemsConsumer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiStackPanel.Parse(node, file);
            DefinitionParser parser = new DefinitionParser(node);

            file["Items"] = parser.ParseDelegate("Items");

            Dictionary<Type, DefinitionFile> additionalTemplates = new Dictionary<Type, DefinitionFile>();

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiItemsStack.ItemTemplate":
                        {
                            if (cn.Nodes.Count != 1)
                            {
                                string error = node.NodeError("UiItemsStack.ItemTemplate must have exactly 1 child.");

                                if (DefinitionParser.EnableCheckMode)
                                {
                                    ConsoleEx.WriteLine(error);
                                }
                                else
                                {
                                    throw new Exception(error);
                                }
                            }

                            if (string.IsNullOrEmpty(cn.Attribute("DataType")))
                            {
                                if (file["Template"] != null)
                                {
                                    string error = node.NodeError("UiItemsStack default template already defined.");

                                    if (DefinitionParser.EnableCheckMode)
                                    {
                                        ConsoleEx.WriteLine(error);
                                    }
                                    else
                                    {
                                        throw new Exception(error);
                                    }
                                }

                                file["Template"] = DefinitionFile.LoadFile(cn.Nodes[0]);
                            }
                            else
                            {
                                Type type = Type.GetType(cn.Attribute("DataType"));

                                if (type == null)
                                {
                                    string error = node.NodeError("Cannot find type: {0}", cn.Attribute("DataType"));

                                    if (DefinitionParser.EnableCheckMode)
                                    {
                                        ConsoleEx.WriteLine(error);
                                    }
                                    else
                                    {
                                        throw new Exception(error);
                                    }
                                }

                                additionalTemplates.Add(type, DefinitionFile.LoadFile(cn.Nodes[0]));
                            }
                        }
                        break;
                }

                if(!cn.Tag.Contains('.'))
                {
                    throw new Exception("UiItemsStack cannot have any children.");
                }
            }

            if (additionalTemplates.Count > 0)
            {
                file["AdditionalTemplates"] = additionalTemplates;
            }
        }

        DefinitionFile _template;
        IItemsProvider _items = null;
        Dictionary<Type, DefinitionFile> _additionalTemplates;

        object _childrenLock = new object();

        Dictionary<object, UiView> _bindingToElement = new Dictionary<object, UiView>();

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiItemsStack));

            _template = (DefinitionFile)file["Template"];

            _additionalTemplates = file["AdditionalTemplates"] as Dictionary<Type, DefinitionFile>;

            _items = (IItemsProvider)DefinitionResolver.GetValueFromMethodOrField(Controller, Binding, file["Items"]);
            _items.Subscribe(this);

            return true;
        }

        protected override void Update(float time)
        {
            base.Update(time);
            bool recalcLayout = false;

            for(int idx = 0; idx < _items.Count; ++idx)
            {
                if(!_bindingToElement.ContainsKey(_items.ElementAt(idx)))
                {
                    InsertItem(_items.ElementAt(idx), idx);
                    recalcLayout = true;
                }
            }

            if(recalcLayout)
            {
                //RecalculateAll();

                if (Parent != null)
                {
                    SetForceRecalcFlag();
                    Parent.RecalcLayout();
                }
            }
        }

        void InsertItem(object item, int index)
        {
            lock (_childrenLock)
            {
                DefinitionFile template;

                if (_additionalTemplates == null || !_additionalTemplates.TryGetValue(item.GetType(), out template))
                {
                    template = _template;
                }

                var view = (UiView)template.CreateInstance(Controller, item);

                if(view == null)
                {
                    return;
                }

                lock (_childrenLock)
                {
                    _bindingToElement.Add(item, view);
                    _children.Insert(index, view);
                }

                view.Parent = this;
                view.RegisterView();
                view.ViewAdded();
            }
        }

        void IItemsConsumer.Recalculate()
        {
            UiTask.BeginInvoke(() => RecalculateAll());
        }

        void IItemsConsumer.Added(object item, int index)
        {
            UiTask.BeginInvoke(() => RecalculateAll());
            InsertItem(item, index);
        }

        void IItemsConsumer.RemovedAll()
        {
            lock (_childrenLock)
            {
                foreach(var child in _children)
                {
                    child.ViewRemoved();
                }
                _children.Clear();
                _bindingToElement.Clear();
            }

            UiTask.BeginInvoke(() => RecalculateAll());
        }

        void IItemsConsumer.Removed(object item)
        {
            lock (_childrenLock)
            {
                UiView view;
                if (_bindingToElement.TryGetValue(item, out view))
                {
                    view.ViewRemoved();
                    _children.Remove(view);
                    _bindingToElement.Remove(item);
                }
            }

            UiTask.BeginInvoke(() => RecalculateAll());
        }
    }
}
