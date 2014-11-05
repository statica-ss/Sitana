using System;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Ui.Interfaces;
using Sitana.Framework.Input.TouchPad;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Sitana.Framework.Ui.Views.ButtonDrawables;

namespace Sitana.Framework.Ui.Views
{
    public class UiScrollBar: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Context"] = parser.ParseString("Context");
            file["Mode"] = parser.ParseEnum<Mode>("Mode");
            file["AlwaysVisible"] = parser.ParseBoolean("AlwaysVisible");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                case "UiScrollBar.ThumbDrawables":
                    ParseDrawables(cn, file, typeof(ButtonDrawable), "ThumbDrawables");
                    break;

                case "UiScrollBar.TrackDrawables":
                    ParseDrawables(cn, file, typeof(ButtonDrawable), "TrackDrawables");
                    break;
                }
            }
        }

        enum Mode
        {
            Horizontal,
            Vertical
        }

        bool _vertical = false;
        IScrolledElement _element;
        protected int _touchId = 0;

        float _scrollPositionOnDown;
        bool _alwaysVisible = false;

        protected List<ButtonDrawable> _thumbDrawables = new List<ButtonDrawable>();
        protected List<ButtonDrawable> _trackDrawables = new List<ButtonDrawable>();

        protected override void OnAdded()
        {
            base.OnAdded();

            GestureType drag = _vertical ? GestureType.VerticalDrag : GestureType.HorizontalDrag;
            TouchPad.Instance.AddListener(GestureType.Down | GestureType.Up | GestureType.Move | drag, this);
        }

        protected override void OnRemoved()
        {
            TouchPad.Instance.RemoveListener(this);
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }
            Rectangle screen = ScreenBounds;
            Rectangle thumb = ThumbRect;

            if (thumb != screen || _alwaysVisible)
            {

                var batch = parameters.DrawBatch;

                var drawInfo = new DrawButtonInfo();
                drawInfo.Text = null;
                drawInfo.ButtonState = _touchId != 0 ? ButtonState.Pushed : ButtonState.None;

                drawInfo.Target = screen;
                drawInfo.Opacity = opacity;
                drawInfo.EllapsedTime = parameters.EllapsedTime;

                for (int idx = 0; idx < _trackDrawables.Count; ++idx)
                {
                    var drawable = _trackDrawables[idx];
                    drawable.Draw(batch, drawInfo);
                }

                drawInfo.Target = thumb;

                for (int idx = 0; idx < _thumbDrawables.Count; ++idx)
                {
                    var drawable = _thumbDrawables[idx];
                    drawable.Draw(batch, drawInfo);
                }
            }
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiScrollBar));

            string id = DefinitionResolver.GetString(Controller, Binding, file["Context"]);

            _element = Controller.Find(id) as IScrolledElement;
            _vertical = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Horizontal) == Mode.Vertical;
            _alwaysVisible = DefinitionResolver.Get<bool>(Controller, Binding, file["AlwaysVisible"], false);

            List<DefinitionFile> drawableFiles = file["ThumbDrawables"] as List<DefinitionFile>;

            if ( drawableFiles != null )
            {
                foreach (var def in drawableFiles)
                {
                    ButtonDrawable drawable = def.CreateInstance(Controller, binding) as ButtonDrawable;

                    if (drawable != null)
                    {
                        _thumbDrawables.Add(drawable);
                    }
                }
            }

            drawableFiles = file["TrackDrawables"] as List<DefinitionFile>;

            if ( drawableFiles != null )
            {
                foreach (var def in drawableFiles)
                {
                    ButtonDrawable drawable = def.CreateInstance(Controller, binding) as ButtonDrawable;

                    if (drawable != null)
                    {
                        _trackDrawables.Add(drawable);
                    }
                }
            }
        }

        protected override void OnGesture(Gesture gesture)
        {
            if (ThumbRect == ScreenBounds)
            {
                return;
            }

            switch(gesture.GestureType)
            {
            case GestureType.CapturedByOther:

                if (_touchId == gesture.TouchId)
                {
                    _touchId = 0;
                }
                break;

            case GestureType.Down:

                if ( ThumbRect.Contains(gesture.Origin.ToPoint()))
                {
                    if (_touchId == 0)
                    {

                        _touchId = gesture.TouchId;

                        gesture.Handled = true;
                        _scrollPositionOnDown = _vertical ? _element.ScrollingService.ScrollPositionY : _element.ScrollingService.ScrollPositionX;
                    }
                }
                break;

            case GestureType.Move:

                if (_touchId == gesture.TouchId)
                {
                    UpdatePosition(gesture.Origin, gesture.Position);
                }
                break;

            case GestureType.Up:
                if ( _touchId == gesture.TouchId)
                {
                    _touchId = 0;
                }
                break;

            case GestureType.VerticalDrag:
            case GestureType.HorizontalDrag:
                if ( _touchId == gesture.TouchId)
                {
                    gesture.Handled = true;
                    gesture.CapturePointer(this);
                }
                break;
            }
        }

        void UpdatePosition(Vector2 origin, Vector2 position)
        {
            float maxScroll = _element.MaxScrollY;
            float size = _vertical ? Bounds.Height : Bounds.Width;

            float unit = maxScroll / size;

            Vector2 move = (position - origin) * unit;

            float newPosition = _scrollPositionOnDown + (_vertical ? move.Y : move.X);

            newPosition = Math.Max(0, Math.Min(maxScroll-_element.ScreenBounds.Height, newPosition));

            if ( _vertical )
            {
                _element.ScrollingService.ScrollPositionY = newPosition;
            }
            else
            {
                _element.ScrollingService.ScrollPositionX = newPosition;
            }
        }

        Rectangle ThumbRect
        {
            get
            {
                Rectangle screenBounds = ScreenBounds;

                if (_vertical)
                {
                    int maxScroll = _element.MaxScrollY;
                    int size = _element.ScreenBounds.Height;

                    int thumbSize = (int)Math.Ceiling( (float)Bounds.Height * (float)size / (float)maxScroll);
                    int position = Bounds.Height * (int)_element.ScrollingService.ScrollPositionY / maxScroll;

                    var rect = GraphicsHelper.IntersectRectangle(ScreenBounds, 
                        new Rectangle(screenBounds.X, screenBounds.Y + position, screenBounds.Width, thumbSize));

                    if ( rect.Height < rect.Width )
                    {
                        rect.Height = rect.Width;
                        rect.Y = Math.Min(rect.Y, screenBounds.Bottom - rect.Height);
                    }

                    return rect;
                }
                else
                {
                    int maxScroll = _element.MaxScrollX;
                    int size = _element.ScreenBounds.Width;

                    int thumbSize = (int)Math.Ceiling( (float)Bounds.Width * (float)size / (float)maxScroll);
                    int position = (Bounds.Width - thumbSize) * (int)_element.ScrollingService.ScrollPositionX / maxScroll;

                    var rect = GraphicsHelper.IntersectRectangle(ScreenBounds, 
                        new Rectangle(screenBounds.X + position, screenBounds.Y, thumbSize, screenBounds.Height));

                    if ( rect.Width < rect.Height )
                    {
                        rect.Width = rect.Height;
                        rect.X = Math.Min(rect.X, screenBounds.Right - rect.Width);
                    }

                    return rect;
                }

            }
        }
    }
}

