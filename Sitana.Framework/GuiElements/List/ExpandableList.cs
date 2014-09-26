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
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Content;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Sitana.Framework.Gui.List;
using Microsoft.Xna.Framework.Input.Touch;
using Sitana.Framework.Input;

namespace Sitana.Framework.Gui
{
    public partial class ExpandableList : GuiElement
    {
        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event EventHandler<GestureEventArgs> HorizontalDrag;

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event EventHandler<GestureEventArgs> Click;

        public event EventHandler<GestureEventArgs> TouchDown;
        public event EventHandler<GestureEventArgs> TouchUp;

        public event EventHandler<GestureEventArgs> CancelTouch;

        private Dictionary<String, ListItem> _itemTemplates = new Dictionary<String, ListItem>();

        private List<ListItem> _items = new List<ListItem>();
        
        private Dictionary<ListItemData, ListItem> _elements = new Dictionary<ListItemData, ListItem>();

        private Boolean _reverseMode = false;

        private static Dictionary<String, Type> _types = new Dictionary<String, Type>();
        private static Dictionary<String, ExpandableList> _restoreInstances = new Dictionary<String,ExpandableList>();

        private Single _scale;
        private ListItemsContainer _dataContext;
        private Boolean _allowScroll = true;

        private Int32 _maxOneAdd = Int32.MaxValue;
        private Int32 _maxFirstAdd = Int32.MaxValue;

        private VerticalScroller _verticalScroller;

        public ExpandableList()
        {
            RegisterItemType(typeof(ListItem_Label));
            RegisterItemType(typeof(ListItem_Separator));
            RegisterItemType(typeof(ListItem_SelectionRect));
            RegisterItemType(typeof(ListItem_SlideContainer));
            RegisterItemType(typeof(ListItem_Image));
            RegisterItemType(typeof(ListItem_Toolbar));
            RegisterItemType(typeof(ListItem_TextButton));
            RegisterItemType(typeof(ListItem_MultilineLabel));
        }

        public static void RegisterItemType(Type type)
        {
            try
            {
                _types.Add(type.Name, type);
            }
            catch (System.Exception)
            {

            }
        }

        public static ListItemElement Create(String name)
        {
            Type type;

            if (_types.TryGetValue(name, out type))
            {
                return Activator.CreateInstance(type) as ListItemElement;
            }

            return null;
        }

        public static void RemoveInstance(String name)
        {
            _restoreInstances.Remove(name);
        }

        private ListItem FindTemplate(String name)
        {
            ListItem item = null;
            if (_itemTemplates.TryGetValue(name, out item))
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Draws button.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch object used to render textures and texts.</param>
        /// <param name="color">Color to multiply all contents by.</param>
        /// <param name="offset">Offset to move bnutton by.</param>
        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            if (Opacity <= 0)
            {
                return;
            }

            Vector2 position = new Vector2(ElementRectangle.X, ElementRectangle.Y);
            Vector2 offset = ComputeOffsetWithTransition(transition) + topLeft;

            position += offset;

            position.Y -= _allowScroll ? _verticalScroller.Position : 0;

            Color color = ComputeColorWithTransition(transition, Color.White);

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                ListItem item = _items[idx];
                Single height = (Single)item.DrawHeight * _scale;

                if ( position.Y + height >= ElementRectangle.Top )
                {
                    Boolean hideSeparator = idx == _items.Count - 1;

                    if (!hideSeparator)
                    {
                        if (item.Bind.GetType() != _items[idx + 1].Bind.GetType())
                        {
                            hideSeparator = true;
                        }
                    }

                    item.Draw(spriteBatch, position, _scale, (Single)color.A / 255.0f * Opacity, hideSeparator );
                }
                
                position.Y += height;

                if (position.Y > ElementRectangle.Bottom + offset.Y)
                {
                    break;
                }
            }
        }

        private Boolean UpdateDataContext()
        {
            Boolean redraw = false;

            if (_dataContext.ShouldUpdate)
            {
                redraw |= true;

                lock (_dataContext.Lock)
                {
                    Int32 addedElements = 0;
                    Int32 maxAdds = _elements.Count > _maxOneAdd ? _maxOneAdd : _maxFirstAdd;

                    for (Int32 index = 0; index < _dataContext.Count; ++index)
                    {
                        Int32 dataIndex = _reverseMode ? _dataContext.Count - index - 1 : index;
                        ListItemData data = _dataContext[dataIndex];

                        if (!_elements.ContainsKey(data))
                        {
                            var template = FindTemplate(data.Template);

                            if (template != null)
                            {
                                _elements.Add(data, template.Clone(data));
                                data.Update();

                                addedElements++;

                                if (addedElements > maxAdds)
                                {
                                    _dataContext.ShouldUpdate = true;
                                    break;
                                }
                            }
                        }
                    }

                    List<ListItemData> removed = _dataContext.RemovedElements;

                    for (Int32 idx = 0; idx < removed.Count; ++idx)
                    {
                        _elements.Remove(removed[idx]);
                    }

                    removed.Clear();

                    if (addedElements <= maxAdds || _maxFirstAdd == maxAdds)
                    {
                        _items.Clear();

                        for (Int32 elemIndex = 0; elemIndex < _dataContext.Count; ++elemIndex)
                        {
                            Int32 dataIndex = _reverseMode ? _dataContext.Count - elemIndex - 1 : elemIndex;
                            ListItemData data = _dataContext[dataIndex];

                            ListItem item;
                            if (_elements.TryGetValue(data, out item))
                            {
                                _items.Add(item);
                            }
                        }
                    }
                }
            }

            return redraw;
        }

        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = base.Update(gameTime, screenState);

            for (Int32 idx = 0; idx < _items.Count; idx++ )
            {
                ListItem item = _items[idx];
                redraw |= item.UpdateUi((Single)gameTime.TotalSeconds);
            }

            redraw |= UpdateDataContext();

            Int32 height = 0;

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                height += _items[idx].Height;
            }

            Single time = (Single)gameTime.TotalSeconds;

            redraw |= _verticalScroller.Update(time, height * Scale.Y);

            Single pos = ElementRectangle.Y - _verticalScroller.Position;

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                ListItem item = _items[idx];
                Single size = (Single)item.DrawHeight * _scale;

                if ( pos + size >= ElementRectangle.Top )
                {
                    redraw |= item.Bind.ShouldUpdate ? item.Update() : false;
                }
                
                pos += size;

                if (pos > ElementRectangle.Bottom)
                {
                    break;
                }
            }

            if (!_allowScroll)
            {
                _verticalScroller.Position = 0;
            }


            _verticalScroller.Enable = Visible;
            return redraw;
        }

        /// <summary>
        /// Initializes image from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        /// <summary>
        /// Initializes accordion from parameters.
        /// </summary>
        /// <param name="node">XML node entity.</param>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="scale">Current screen scale.</param>
        /// <param name="areaSize">Size of the area.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected override Boolean Initialize(InitializeParams initParams)
        {
            XmlFileNode node = initParams.Node;
            ParametersCollection parameters = initParams.Parameters;
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            Point position1 = ParsePosition(parameters, "X1", "Y1", GraphicsHelper.PointFromVector2(areaSize), scale);
            Point position2 = ParsePosition(parameters, "X2", "Y2", GraphicsHelper.PointFromVector2(areaSize), scale);

            ElementRectangle = RectangleFromAlignAndSize(position1, new Point(position2.X - position1.X, position2.Y - position1.Y), Align.Left | Align.Top, offset);

            Boolean templateLoaded = false;

            for ( Int32 idx = 0; idx < node.Nodes.Count; ++idx )
            {
                XmlFileNode tempNode = node.Nodes[idx];

                if ( tempNode.Tag == "ItemTemplate")
                {
                    tempNode.ValueSource = parameters.ValueSource;
                    tempNode.ColorsManager = parameters.ColorsManager;

                    LoadTemplate(tempNode, directory);
                    templateLoaded = true;
                }
            }

            if (!templateLoaded)
            {
                String path = parameters.AsString("Template");
                
                XmlFileNode templateNode = ContentLoader.Current.Load<XmlFile>(path);

                if (templateNode.Tag != "ListTemplate")
                {
                    throw new Exception("Invalid xml node. Expected: ListTemplate");
                }

                directory = Path.GetDirectoryName(path);

                templateNode.ValueSource = parameters.ValueSource;
                templateNode.ColorsManager = parameters.ColorsManager;

                for (Int32 idx = 0; idx < templateNode.Nodes.Count; ++idx)
                {
                    LoadTemplate(templateNode.Nodes[idx], directory);
                }
            }

            _scale = scale.X;

            _dataContext = (ListItemsContainer)parameters.AsObject("Binding");
            _dataContext.ShouldUpdate = true;

            ClipToElement = true;

            _reverseMode = parameters.AsBoolean("ReverseMode");

            if (parameters.HasKey("MaxNewItemsOnFirstSync"))
            {
                _maxFirstAdd = parameters.AsInt32("MaxNewItemsOnFirstSync");
            }

            if (parameters.HasKey("MaxNewItemsOnSync"))
            {
                _maxOneAdd = parameters.AsInt32("MaxNewItemsOnSync");
            }

            String restore = parameters.AsString("RestoreInstance");

            if (!String.IsNullOrEmpty(restore))
            {
                ExpandableList list;

                if (_restoreInstances.TryGetValue(restore, out list))
                {
                    if (list._dataContext == _dataContext)
                    {
                        _verticalScroller = list._verticalScroller;
                        _elements = list._elements;
                    }
                }

                _restoreInstances[restore] = this;
            }
            
            if ( _verticalScroller == null )
            {
                _verticalScroller = new VerticalScroller(ElementRectangle, Scale.Y * 100);
            }

            _allowScroll = !parameters.AsBoolean("DisableScroll", false);

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.HorizontalDrag, HandleHorizontalDragGesture);
            InstallGestureHandler(GestureAdditionalType.Native, GestureType.VerticalDrag, HandleVerticalDragGesture);
            InstallGestureHandler(GestureAdditionalType.Native, GestureType.FreeDrag, HandleHorizontalDragGesture);
            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Tap, HandleClickGesture);

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Flick, HandleFlickGesture);

            InstallGestureHandler(GestureAdditionalType.TouchDown, GestureType.None, HandleTouchDownGesture);
            InstallGestureHandler(GestureAdditionalType.TouchUp, GestureType.None, HandleTouchUpGesture);

            return true;
        }

        private void HandleTouchDownGesture(Object sender, GestureEventArgs args)
        {
            if (!Visible)
            {
                return;
            }

             _verticalScroller.HandleTouchDownGesture(sender, args);
            
            Vector2 offset;
            Int32? index = IndexFromPoint(false, args.Sample.Position, out offset);

            if (index.HasValue)
            {
                if (_items[index.Value].OnGesture(GestureType.None, offset / _scale, GestureAdditionalType.TouchDown))
                {
                    args.Handled = true;
                }
            }
            
            if (!args.Handled)
            {
                if (TouchDown != null)
                {
                    TouchDown(this, args);
                }
            }
        }

        private void HandleTouchUpGesture(Object sender, GestureEventArgs args)
        {
            if (!Visible)
            {
                return;
            }

            _verticalScroller.HandleTouchUpGesture(sender, args);
            
            Vector2 offset;
            Int32? index = IndexFromPoint(false, args.Sample.Position, out offset);

            if (index.HasValue)
            {
                if (_items[index.Value].OnGesture(GestureType.None, offset / _scale, GestureAdditionalType.TouchUp))
                {
                    args.Handled = true;
                }
            }

            if (!args.Handled && TouchUp != null && _verticalScroller.LastGesture == GestureType.None)
            {
                TouchUp(this, args);
            }

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                _items[idx].OnGesture(GestureType.None, Vector2.Zero, GestureAdditionalType.Cancel);
            }
        }

        private void HandleHorizontalDragGesture(Object sender, GestureEventArgs args)
        {
            if (!Visible)
            {
                return;
            }

            CancelTouchDown(args);

            if (_verticalScroller.LastGesture != GestureType.HorizontalDrag)
            {
                if (!_verticalScroller.CanDoGesture)
                {
                    return;
                }
            }

            Vector2 offset;
            Int32? index = IndexFromPoint(false, args.Sample.Position, out offset);

            if (index.HasValue)
            {
                if (_items[index.Value].OnGesture(args.Sample.GestureType, offset / _scale, args.Sample.Delta, false))
                {
                    args.Handled = true;
                }
            }

            if (!args.Handled && HorizontalDrag != null)
            {
                HorizontalDrag(this, args);
            }

            _verticalScroller.HandleHorizontalDragGesture(sender, args);
        }

        private void LoadTemplate(XmlFileNode node, String directory)
        {
            if ( node.Tag != "ItemTemplate")
            {
                throw new Exception("Invalid xml node. Expected: ItemTemplate");
            }

            var template = new ListItem(node, (Int32)((Single)ElementRectangle.Height / Scale.Y));
            String name = node.Attributes.AsString("Name");
            _itemTemplates.Add(name, template);
        }

        private void HandleClickGesture(Object sender, GestureEventArgs args)
        {
            if (!Visible)
            {
                return;
            }

            CancelTouchDown(args);

            GestureEventArgs click = args as GestureEventArgs;

            Point pos = GraphicsHelper.PointFromVector2(click.Sample.Position);

            if (!ElementRectangle.Contains(pos))
            {
                return;
            }

            Vector2 offset;
            Int32? index = IndexFromPoint(false, args.Sample.Position, out offset);

            if (index.HasValue)
            {
                if (_items[index.Value].OnGesture(GestureType.Tap, offset / _scale))
                {
                    args.Handled = true;
                }
            }

            if (!args.Handled && Click != null)
            {
                Click(this, args);
            }

            _verticalScroller.HandleClickGesture(sender, args);
        }

        public void ExpandAll(Boolean expand)
        {
            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                var item = _items[idx];
                item.IsExpanded = expand;
                item.Update();
            }
        }

        public void ToggleExpanded(Boolean dataIndex, Int32 index, Boolean moveToBottom=true)
        {
            if (dataIndex)
            {
                index = Index(index);
            }

            ListItem item = _items[index];

            item.IsExpanded = !item.IsExpanded;

            if (item.IsExpanded)
            {
                Single pos = ItemPosition(false, index);
                Single position = pos + item.Height * _scale;
                    
                if ( !moveToBottom )
                {
                    position = Math.Min(pos + item.CollapsedHeight * _scale * 2, position);
                }

                if (position > ElementRectangle.Bottom)
                {
                    _verticalScroller.AddPosition(position - ElementRectangle.Bottom);
                }
            }

            item.Update();
        }

        private void CancelTouchDown(GestureEventArgs args)
        {
            if (!Visible)
            {
                return;
            }

            if (CancelTouch != null && _verticalScroller.LastGesture == GestureType.None && _verticalScroller.CanDoGesture)
            {
                CancelTouch(this, args);
            }

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                _items[idx].OnGesture(GestureType.None, Vector2.Zero, GestureAdditionalType.Cancel);
            }
        }

        private void HandleVerticalDragGesture(Object sender, GestureEventArgs args)
        {
            if (!Visible)
            {
                return;
            }

            CancelTouchDown(args);
            _verticalScroller.HandleVerticalDragGesture(sender, args);
        }

        private void HandleFlickGesture(Object sender, GestureEventArgs args)
        {
            if (!Visible)
            {
                return;
            }

            CancelTouchDown(args);
            _verticalScroller.HandleFlickGesture(sender, args);
        }

        public Int32 CollapsedHeight(Boolean dataIndex, Int32 index)
        {
            if (dataIndex)
            {
                index = Index(index);
            }

            return _items[index].CollapsedHeight;
        }

        public Single ItemPosition(Boolean dataIndex, Int32 index)
        {
            if (dataIndex)
            {
                index = Index(index);
            }

            Single pos = ElementRectangle.Y - _verticalScroller.Position;

            for (Int32 idx = 0; idx < index; ++idx)
            {
                ListItem item = _items[idx];
                Single size = (Single)item.Height * _scale;
                pos += size;
            }

            return pos;
        }

        public void ProcessClickGesture(GestureEventArgs args)
        {
            if (!Visible)
            {
                return;
            }

            Vector2 offset;
            Int32? index = IndexFromPoint(false, args.Sample.Position, out offset);

            if (index.HasValue)
            {
                _items[index.Value].OnGesture(GestureType.Tap, offset / _scale);
            }
        }

        public Int32? IndexFromPoint(Boolean dataIndex, Vector2 point, out Vector2 offset)
        {
            offset = Vector2.Zero;

            Vector2 position = new Vector2(ElementRectangle.X, ElementRectangle.Y);

            //spriteBatch.Draw(_onePixelParticle, position, null, Color.Black * 0.2f, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            Point pos = GraphicsHelper.PointFromVector2(point);

            if (!ElementRectangle.Contains(pos))
            {
                return null;
            }

            position.Y -= _verticalScroller.Position;

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                ListItem item = _items[idx];
                Single height = (Single)item.Height * _scale;

                if (position.Y + height >= ElementRectangle.Top)
                {
                    if ( pos.Y >= position.Y && pos.Y < position.Y + height )
                    {
                        offset = point - position;
                        return dataIndex ? Index(idx) : idx;
                    }
                }

                position.Y += height;

                if (position.Y > ElementRectangle.Bottom)
                {
                    break;
                }
            }

            return null;
        }

        public Object GetDataElement(Boolean dataIndex, Int32 index)
        {
            if (dataIndex)
            {
                index = Index(index);
            }

            return _items[index].Bind;
        }

        public void UpdateRect(Rectangle rect)
        {
            ElementRectangle = rect;
            _verticalScroller.UpdateElementRectangle(rect);
        }

        private Int32 Index(Int32 index)
        {
            if (_reverseMode)
            {
                return _items.Count - index - 1;
            }

            return index;
        }

        public void ScrollTo(Object data)
        {
            UpdateDataContext();
            Single position = 0;
            ListItem scrollTo = null;

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                ListItem item = _items[idx];
                Single height = (Single)item.Height * _scale;

                if (item.Bind == data)
                {
                    scrollTo = item;
                    break;
                }

                position += height;
            }

            if (scrollTo != null)
            {
                Single height = ElementRectangle.Height;
                Single itemHeight = scrollTo.Height * _scale;

                if (position < _verticalScroller.Position)
                {
                    _verticalScroller.Position = position;
                }
                else if (position + itemHeight >= _verticalScroller.Position + height)
                {
                    _verticalScroller.Position = position + itemHeight  - height;
                }
            }
        }
    }
}
