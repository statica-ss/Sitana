using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ebatianos.Content;
using Ebatianos.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Ebatianos.Gui
{
    public class Toolbar: GuiElement
    {
        class ToolbarElement
        {
            public ToolbarElement()
            {
            }

            public ToolbarElement(ParametersCollection parameters)
            {
                Id = parameters.AsString("Id");

                Action = parameters.AsAction("OnClick", Id);

                String path = parameters.AsString("Icon");

                Icon = ContentLoader.Current.Load<Texture2D>(path);

                Position = parameters.AsAlign("", "Position", Align.Top);
            }

            public Action Action = null;
            public Texture2D Icon = null;
            public Align Position = Align.Top;
            public String Id = null;

            public Int32? Offset;
            public Boolean IsPushed = false;

            public Boolean Disabled = true;
        }

        private Texture2D _fill;
        private Texture2D _anchor;
        
        private Align _align;
        private Single _scale;

        private Single _showProgress = 0;
        private Boolean _show = false;

        private Single _showSpeed = 1;

        private Int32 _anchorPosition = 0;

        private Int32 _separatorSize = 0;
        private Int32 _margin = 0;
        private Int32 _spacing = 0;

        private Int32 _allWidth;

        private Color _iconColor;
        private Color _pushedColor;
        private Color _disabledColor;

        private Action _showAgain = null;

        private Int32? _pushedItem;

        public Object Context { get; private set; }

        private List<ToolbarElement> _items = new List<ToolbarElement>();

        public Boolean IsShown
        {
            get
            {
                return _show;
            }
        }

        public override GuiElement Clone()
        {
            throw new NotImplementedException();
        }

        protected override void OnCloned(GuiElement source)
        {
            base.OnCloned(source);
        }

        /// <summary>
        /// Draws label.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch object used to render textures and texts.</param>
        /// <param name="color">Color to multiply all contents by.</param>
        /// <param name="offset">Offset to move bnutton by.</param>
        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (_showProgress == 0)
            {
                return;
            }

            if (!DrawLevel(level))
            {
                return;
            }

            if (Opacity <= 0)
            {
                return;
            }

            if (SecondInstance != null)
            {
                if (this == SecondInstance)
                {
                    transition = 1;
                }
                else
                {
                    return;
                }
            }

            Single hide = 1 - _showProgress;

            Single offset = -hide * (Single)_allWidth;

            Vector2 position = new Vector2(offset, 0);
            Vector2 originFill = Vector2.Zero;
            Vector2 originAnchor = Vector2.Zero;

            

            if (IsAlign(_align, Align.Right))
            {
                offset = -offset;
                position.X = Owner.AreaSize.X + offset;
                originFill.X = _fill.Width;
                originAnchor.X = _anchor.Width;
            }

            Vector2 anchorPosition = new Vector2(position.X, _anchorPosition - (Int32)(_anchor.Height * _scale / 2));
            Vector2 fillScale1 = new Vector2(_scale, anchorPosition.Y);
            Vector2 fillScale2 = new Vector2(_scale, Owner.AreaSize.Y - (anchorPosition.Y + (Int32)(_anchor.Height * _scale)));

            Vector2 fill2Position = new Vector2(position.X, anchorPosition.Y + _anchor.Height * _scale);

            spriteBatch.Draw(_fill, position, null, Color.White, 0, originFill, fillScale1, SpriteEffects.None, 0);
            spriteBatch.Draw(_anchor, anchorPosition, null, Color.White, 0, originAnchor, _scale, SpriteEffects.None, 0);
            spriteBatch.Draw(_fill, fill2Position, null, Color.White, 0, originFill, fillScale2, SpriteEffects.None, 0);


            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                var item = _items[idx];

                if (item.Offset.HasValue)
                {
                    Vector2 pos = new Vector2(ElementRectangle.Center.X + offset, item.Offset.Value);
                    Vector2 origin = new Vector2((Single)item.Icon.Width / 2, 0);

                    Color color = item.IsPushed ? _pushedColor : _iconColor;

                    if (item.Disabled)
                    {
                        color = _disabledColor;
                    }

                    spriteBatch.Draw(item.Icon, pos, null, color, 0, origin, _scale, SpriteEffects.None, 0);
                }

            }
        }

        /// <summary>
        /// Initializes label from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
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

            String path = parameters.AsString("Fill");
            _fill = ContentLoader.Current.Load<Texture2D>(path);

            path = parameters.AsString("Anchor");
            _anchor = ContentLoader.Current.Load<Texture2D>(path);

            _iconColor = parameters.AsColor("IconColor");
            _pushedColor = parameters.AsColor("IconColorPushed");
            _disabledColor = parameters.AsColor("IconColorDisabled");
            Align align = parameters.AsAlign("Align", "", Align.Left);

            Int32 width = (Int32)(_anchor.Width * scale.X);
            _scale = (Single)width / (Single)_anchor.Width;

            _allWidth = width;

            width = (Int32)(_fill.Width * _scale);

            Point position = Point.Zero;

            if (IsAlign(align, Align.Right))
            {
                position.X = (Int32)areaSize.X;
            }

            Point size = new Point(width, (Int32)areaSize.Y);
            _align = align;

            ElementRectangle = RectangleFromAlignAndSize(position, size, _align, offset);

            _showSpeed = 1 / (parameters.AsSingle("ShowTime") / 1000.0f);

            _separatorSize = (Int32)(parameters.AsSingle("SeparatorSize") * scale.Y);
            _margin = (Int32)(parameters.AsSingle("Margin") * scale.Y);
            _spacing = (Int32)(parameters.AsSingle("Spacing") * scale.Y);

            for (Int32 idx = 0; idx < node.Nodes.Count; ++idx)
            {
                if (node.Nodes[idx].Tag == "Items")
                {
                    AddItems(node.Nodes[idx]);
                }
            }

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Tap, ProcessGestureToHandled);
            InstallGestureHandler(GestureAdditionalType.TouchDown, GestureType.None, ProcessTouchDown);
            InstallGestureHandler(GestureAdditionalType.TouchUp, GestureType.None, ProcessTouchUp);
            InstallGestureHandler(GestureAdditionalType.Native, GestureType.HorizontalDrag, ProcessGestureToHandled);
            InstallGestureHandler(GestureAdditionalType.Native, GestureType.VerticalDrag, ProcessGestureToHandled);

            return true;
        }

        private void ProcessTouchDown(Object sender, GestureEventArgs args)
        {
            if (_showProgress < 1)
            {
                return;
            }

            Point pos = GraphicsHelper.PointFromVector2(args.Sample.Position);
            _pushedItem = null;

            if (ElementRectangle.Contains(pos))
            {
                args.Handled = true;

                if (_showProgress == 1)
                {
                    Int32? index = ItemFromPoint(pos);

                    if (index.HasValue)
                    {
                        if (!_items[index.Value].Disabled)
                        {
                            SetPushed(index);
                            InputHandler.Current.PointerInvalidated += CancelDown;
                        }
                    }
                }
            }
        }

        private void CancelDown(Object sender, EventArgs args)
        {
            SetPushed(null);
            InputHandler.Current.PointerInvalidated -= CancelDown;
        }

        private void ProcessTouchUp(Object sender, GestureEventArgs args)
        {
            if (_showProgress < 1)
            {
                return;
            }

            Point pos = GraphicsHelper.PointFromVector2(args.Sample.Position);

            if (ElementRectangle.Contains(pos))
            {
                args.Handled = true;

                if (_pushedItem != null)
                {
                    if (ButtonContainsPoint(_pushedItem.Value, pos))
                    {
                        _items[_pushedItem.Value].Action.Invoke();
                    }
                }
            }

            SetPushed(null);
        }

        private void ProcessGestureToHandledAndHide(Object sender, GestureEventArgs args)
        {
            if (_showProgress > 0)
            {
                args.Handled = true;
                Hide();
            }
        }

        private void ProcessGestureToHandled(Object sender, GestureEventArgs args)
        {
            if (_showProgress > 0)
            {
                args.Handled = true;
            }
        }

        private void AddItems(XmlFileNode node)
        {
            for (Int32 idx = 0; idx < node.Nodes.Count; ++idx)
            {
                String type = node.Nodes[idx].Tag;
                var parameters = node.Nodes[idx].Attributes;

                switch(type)
                {
                    case "Action":
                        _items.Add(new ToolbarElement(parameters));
                        break;

                    case "Separator":
                        _items.Add(new ToolbarElement());
                        break;

                    default:
                        throw new InvalidOperationException("Node name is invalid. Correct are: Action, Seperator.");
                }
            }
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean ret = base.Update(gameTime, screenState);

            if (_show)
            {
                if (_showProgress < 1)
                {
                    _showProgress += (Single)gameTime.TotalSeconds * _showSpeed;
                    _showProgress = Math.Min(1, _showProgress);
                    ret = true;
                }
            }
            else
            {
                if (_showProgress > 0)
                {
                    _showProgress -= (Single)gameTime.TotalSeconds * _showSpeed;
                    _showProgress = Math.Max(0, _showProgress);

                    if (_showProgress == 0)
                    {
                        if (_showAgain != null)
                        {
                            _showProgress = Single.Epsilon;
                            _showAgain.Invoke();
                            _showAgain = null;
                        }
                    }

                    ret = true;
                }
            }

            return ret;
        }

        public void Show(Int32 anchorPosition, Object context, String[] disabledElements, String[] hiddenElements)
        {
            _showAgain = new Action(() =>
            {
                Context = context;
                _anchorPosition = anchorPosition;
                _show = true;

                PlaceItems(disabledElements, hiddenElements);
            });

            if (_showProgress < 0.001f || context == Context)
            {
                _showAgain.Invoke();
                _showAgain = null;
            }
            else
            {
                Hide();
            }
        }

        public Boolean OnTap(Object sender, GestureEventArgs args)
        {
            if (_showProgress > 0)
            {
                Point pos = GraphicsHelper.PointFromVector2(args.Sample.Position);

                if (!ElementRectangle.Contains(pos))
                {
                    Hide();
                    return true;
                }

                args.Handled = true;
            }

            return false;
        }

        private Boolean ButtonContainsPoint(Int32 idx, Point pos)
        {
            var item = _items[idx];

            if (item.Offset.HasValue)
            {
                Int32 top = item.Offset.Value;
                Int32 height = (Int32)(item.Icon.Height * _scale);

                Rectangle rect = new Rectangle(ElementRectangle.Left, top, ElementRectangle.Width, height);

                if (rect.Contains(pos))
                {
                    return true;
                }
            }

            return false;
        }

        private Int32? ItemFromPoint(Point pos)
        {
            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                if (ButtonContainsPoint(idx, pos))
                {
                    return idx;
                }
            }

            return null;
        }

        public void Hide()
        {
            _show = false;
        }

        private void SetPushed(Int32? index)
        {
            _pushedItem = index;

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                if (idx == index)
                {
                    _items[idx].IsPushed = true;
                }
                else
                {
                    _items[idx].IsPushed = false;
                }
            }
        }

        private void PlaceItems(String[] disabledElements, String[] hiddenElements)
        {
            Boolean lastWasSeparator = true;
            Int32 position = _margin;

            for (Int32 idx = 0; idx < _items.Count; ++idx)
            {
                var item = _items[idx];

                if ( IsAlign(item.Position, Align.Bottom ))
                {
                    break;
                }

                if (item.Id == null)
                {
                    if (!lastWasSeparator)
                    {
                        position += _separatorSize;
                        lastWasSeparator = true;
                    }
                }
                else if (!hiddenElements.Contains(item.Id))
                {
                    if (!lastWasSeparator)
                    {
                        position += _spacing;
                    }

                    item.Offset = position;
                    position += (Int32)(item.Icon.Height * _scale);

                    item.Disabled = disabledElements.Contains(item.Id);
                    lastWasSeparator = false;
                }
                else
                {
                    item.Offset = null;
                }
            }

            lastWasSeparator = true;
            position = (Int32)Owner.AreaSize.Y - _margin;

            for (Int32 idx = _items.Count-1; idx >= 0; --idx)
            {
                var item = _items[idx];

                if (item.Id == null)
                {
                    if (!lastWasSeparator)
                    {
                        position -= _separatorSize;
                        lastWasSeparator = true;
                    }
                }
                else if (!IsAlign(item.Position, Align.Bottom))
                {
                    break;
                }
                else if (!hiddenElements.Contains(item.Id))
                {
                    if (!lastWasSeparator)
                    {
                        position -= _spacing;
                    }

                    position -= (Int32)(item.Icon.Height * _scale);
                    item.Offset = position;

                    item.Disabled = disabledElements.Contains(item.Id);

                    lastWasSeparator = false;

                }
                else
                {
                    item.Offset = null;
                }
            }
        }
    }
}
