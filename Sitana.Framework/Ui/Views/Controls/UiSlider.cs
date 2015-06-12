using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using Sitana.Framework.Cs;
using Sitana.Framework.Input.TouchPad;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.Parameters;

namespace Sitana.Framework.Ui.Views
{
    public class UiSlider: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Mode"] = parser.ParseEnum<Mode>("Mode");
            file["MaxValue"] = parser.ParseInt("MaxValue");
            file["MinValue"] = parser.ParseInt("MinValue");
            file["Value"] = parser.ParseInt("Value");
            file["ValueChanged"] = parser.ParseDelegate("ValueChanged");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiSlider.ThumbDrawables":
                        ParseDrawables(cn, file, typeof(ButtonDrawable), "ThumbDrawables");
                        break;

                    case "UiSlider.TrackDrawables":
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

        protected List<ButtonDrawable> _thumbDrawables = new List<ButtonDrawable>();
        protected List<ButtonDrawable> _trackDrawables = new List<ButtonDrawable>();

        bool _vertical = false;
        protected int _touchId = 0;

        float _scrollPositionOnDown = 0;

        SharedValue<int> _value = null;
        SharedValue<int> _maxValue = null;
        SharedValue<int> _minValue = null;

        protected override void OnAdded()
        {
            base.OnAdded();

            GestureType drag = _vertical ? GestureType.VerticalDrag : GestureType.HorizontalDrag;
            EnabledGestures = (GestureType.Down | GestureType.Up | GestureType.Move | drag);
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

        protected override void OnGesture(Gesture gesture)
        {
            if (ThumbRect == ScreenBounds)
            {
                return;
            }

            switch (gesture.GestureType)
            {
                case GestureType.CapturedByOther:

                    if (_touchId == gesture.TouchId)
                    {
                        _touchId = 0;
                    }
                    break;

                case GestureType.Down:

                    if (ThumbRect.Contains(gesture.Origin.ToPoint()))
                    {
                        if (_touchId == 0)
                        {

                            _touchId = gesture.TouchId;

                            gesture.SetHandled();
                            _scrollPositionOnDown = Value;
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
                    if (_touchId == gesture.TouchId)
                    {
                        _touchId = 0;
                    }
                    break;

                case GestureType.VerticalDrag:
                case GestureType.HorizontalDrag:
                    if (_touchId == gesture.TouchId)
                    {
                        gesture.SetHandled();
                        gesture.CapturePointer(this);
                    }
                    break;
            }
        }

        void UpdatePosition(Vector2 origin, Vector2 position)
        {
            float maxScroll = MaxValue;
            float size = _vertical ? Bounds.Height : Bounds.Width;

            float unit = maxScroll / size;

            Vector2 move = (position - origin) * unit;

            float newPosition = _scrollPositionOnDown + (_vertical ? move.Y : move.X);

            newPosition = Math.Max(0, Math.Min(maxScroll, newPosition));
            Value = (int)newPosition;
        }

        int MaxValue
        {
            get
            {
                return _maxValue.Value - _minValue.Value;
            }
        }

        int Value
        {
            get
            {
                return _value.Value - _minValue.Value;
            }

            set
            {
                _value.Value = value + _minValue.Value;
                OnValueChanged();
            }
        }

        Rectangle ThumbRect
        {
            get
            {
                Rectangle screenBounds = ScreenBounds;

                if (_vertical)
                {
                    int maxScroll = MaxValue;
                    int size = Bounds.Width;

                    int thumbSize = size;
                    int position = Bounds.Height * Value / maxScroll;

                    var rect = GraphicsHelper.IntersectRectangle(ScreenBounds,
                        new Rectangle(screenBounds.X, screenBounds.Y + position, screenBounds.Width, thumbSize));

                    if (rect.Height < rect.Width)
                    {
                        rect.Height = rect.Width;
                        rect.Y = Math.Min(rect.Y, screenBounds.Bottom - rect.Height);
                    }

                    return rect;
                }
                else
                {
                    int maxScroll = MaxValue;
                    int size = Bounds.Height;

                    int thumbSize = size;
                    int position = (Bounds.Width - thumbSize) * Value / maxScroll;

                    var rect = GraphicsHelper.IntersectRectangle(ScreenBounds,
                        new Rectangle(screenBounds.X + position, screenBounds.Y, thumbSize, screenBounds.Height));

                    if (rect.Width < rect.Height)
                    {
                        rect.Width = rect.Height;
                        rect.X = Math.Min(rect.X, screenBounds.Right - rect.Width);
                    }

                    return rect;
                }

            }
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiSlider));

            _vertical = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Horizontal) == Mode.Vertical;

            _maxValue = DefinitionResolver.GetShared<int>(Controller, Binding, file["MaxValue"], 100);
            _minValue = DefinitionResolver.GetShared<int>(Controller, Binding, file["MinValue"], 0);
            _value = DefinitionResolver.GetShared<int>(Controller, Binding, file["Value"], 0);

            RegisterDelegate("ValueChanged", file["ValueChanged"]);

            List<DefinitionFile> drawableFiles = file["ThumbDrawables"] as List<DefinitionFile>;

            if (drawableFiles != null)
            {
                foreach (var def in drawableFiles)
                {
                    ButtonDrawable drawable = def.CreateInstance(Controller, Binding) as ButtonDrawable;

                    if (drawable != null)
                    {
                        _thumbDrawables.Add(drawable);
                    }
                }
            }

            drawableFiles = file["TrackDrawables"] as List<DefinitionFile>;

            if (drawableFiles != null)
            {
                foreach (var def in drawableFiles)
                {
                    ButtonDrawable drawable = def.CreateInstance(Controller, Binding) as ButtonDrawable;

                    if (drawable != null)
                    {
                        _trackDrawables.Add(drawable);
                    }
                }
            }

            return true;
        }

        void OnValueChanged()
        {
            CallDelegate("ValueChanged", new InvokeParam("sender", this), new InvokeParam("value", _value.Value));
        }
    }
}
