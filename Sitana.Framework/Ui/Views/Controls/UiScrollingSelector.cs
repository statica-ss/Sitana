using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using System.Collections.Generic;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views
{
    public class UiScrollingSelector : UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Context"] = parser.ParseDelegate("Context");
            file["ElementHeight"] = parser.ParseLength("ElementHeight");
            file["Spacing"] = parser.ParseLength("Spacing");
            file["SelectedPositionOffset"] = parser.ParseLength("SelectedPositionOffset");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "ScrollingSelector.ItemDrawables":
                        ParseDrawables(cn, file, typeof(ButtonDrawable));
                        break;
                }
            }
        }

        private ISelectorContext _context;

        private SharedString _captionText = new SharedString();

        private Length _elementHeight;

        private Length _spacing;

        private int _selectedPosition = 0;

        private float _scroll = 0;

        private bool _isScrolling = false;

        private float _scrollTo = Single.NaN;

        private float _speed = 0;

        private int _touchId = 0;

        private Length _selectedPositionOffset;

        double? _lastMoveTime = null;

        protected List<ButtonDrawable> _drawables = new List<ButtonDrawable>();

        protected override bool Init(object controller, object binding, DefinitionFiles.DefinitionFile definition)
        {
            if(!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiScrollingSelector));

            _context = DefinitionResolver.GetValueFromMethodOrField(Controller, Binding, file["Context"]) as ISelectorContext;

            _elementHeight = DefinitionResolver.Get<Length>(Controller, Binding, file["ElementHeight"], Length.Zero);
            _spacing = DefinitionResolver.Get<Length>(Controller, Binding, file["Spacing"], Length.Zero);

            _selectedPositionOffset = DefinitionResolver.Get<Length>(Controller, Binding, file["SelectedPositionOffset"], Length.Zero);

            List<DefinitionFile> drawableFiles = file["Drawables"] as List<DefinitionFile>;

            if (drawableFiles != null)
            {
                foreach (var def in drawableFiles)
                {
                    ButtonDrawable drawable = def.CreateInstance(Controller, Binding) as ButtonDrawable;

                    if (drawable != null)
                    {
                        _drawables.Add(drawable);
                    }
                }
            }

            EnabledGestures = GestureType.Tap | GestureType.Up | GestureType.Down | GestureType.VerticalDrag;

            return true;
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            base.Draw(ref parameters);

            AdvancedDrawBatch batch = parameters.DrawBatch;
            Rectangle bounds = ScreenBounds;

            _selectedPosition = bounds.Center.Y + _selectedPositionOffset.Compute(bounds.Height);

            int size = _elementHeight.Compute(bounds.Height) + _spacing.Compute();

            int start = 0;
            int center = _selectedPosition + (int)(_scroll * size);

            Rectangle rect = bounds;
            rect.Height = _elementHeight.Compute(bounds.Height);

            while (center > bounds.Top - size / 2)
            {
                center -= size;
                start--;
            }

            rect.Y = center - _elementHeight.Compute(bounds.Height) / 2;

            DrawButtonInfo drawInfo = new DrawButtonInfo();
            drawInfo.Icon = null;
            drawInfo.Opacity = parameters.Opacity;
            drawInfo.EllapsedTime = parameters.EllapsedTime;
            drawInfo.Text = _captionText;

            while (rect.Bottom  <= bounds.Bottom + size )
            {   
                ButtonState state = _touchId != 0 ? ButtonState.Pushed : ButtonState.None;

                if (center == _selectedPosition && !_isScrolling && float.IsNaN(_scrollTo))
                {
                    state |= ButtonState.Checked;
                }

                if (start == (int)((_scroll > 0 ? -0.5 : 0.5) - _scroll))
                {
                    state |= ButtonState.Checked;
                }

                bool enabled = false;
                _context.GetData(start, _captionText.StringBuilder, out enabled);

                if(!enabled)
                {
                    state |= ButtonState.Disabled;
                    state &= ~ButtonState.Pushed;
                }

                drawInfo.Target = rect;
                drawInfo.ButtonState = state;

                for (int idx = 0; idx < _drawables.Count; ++idx)
                {
                    var drawable = _drawables[idx];
                    drawable.Draw(batch, drawInfo);
                }

                start++;
                center += size;
                rect.Y += size;
            }
        }

        protected override void Update(float time)
        {
            base.Update(time);

            Single factor = 0;

            if (_context.ShouldUpdateSelection)
            {
                ComputeScrollTo();
            }

            if (!float.IsNaN(_scrollTo))
            {
                factor = Math.Min(8, 1.0f / time);

                _scroll = (1 - time * factor) * _scroll + time * factor * _scrollTo;

                if (Math.Abs(_scroll - _scrollTo) < 0.02)
                {
                    _context.SetCurrent(-(Int32)_scrollTo);

                    _scroll = 0;
                    _scrollTo = Single.NaN;
                }
            }
        }

        protected override void OnGesture(Gesture gesture)
        {
            Rectangle bounds = ScreenBounds;
            Point elementsSize = new Point(bounds.Width, _elementHeight.Compute(bounds.Height));
            int spacing = _spacing.Compute();

            switch (gesture.GestureType)
            {
                case GestureType.Tap:
                    if (!bounds.Contains(gesture.Position.ToPoint()))
                    {
                        return;
                    }

                    if (float.IsNaN(_scrollTo))
                    {
                        Point pos = gesture.Position.ToPoint();

                        float size = elementsSize.Y + spacing;

                        _touchId = 0;
                        _isScrolling = false;

                        int center = _selectedPosition + (int)(_scroll * size);
                        int beginCenter = center;

                        while (center > bounds.Top - elementsSize.Y / 2)
                        {
                            center -= (elementsSize.Y + spacing);
                        }

                        while (center < bounds.Bottom + elementsSize.Y / 2)
                        {
                            center += (elementsSize.Y + spacing);

                            if (center > bounds.Top - elementsSize.Y / 2)
                            {
                                Rectangle rect = new Rectangle(bounds.X, center - (Int32)elementsSize.Y / 2, bounds.Width, (Int32)elementsSize.Y);

                                if (rect.Contains(pos))
                                {
                                    _scrollTo = (Int32)((beginCenter - center) / (elementsSize.Y + spacing));
                                }
                            }
                        }
                    }
                    break;

                case GestureType.Up:

                    if (_touchId == gesture.TouchId)
                    {
                        if (_isScrolling)
                        {
                            ComputeScrollTo();
                        }
                        _touchId = 0;
                        _isScrolling = false;
                        _lastMoveTime = null;
                    }
                    break;

                case GestureType.Down:

                    if (_touchId == 0)
                    {
                        if (bounds.Contains(gesture.Position.ToPoint()))
                        {
                            _scrollTo = float.NaN;
                            _touchId = gesture.TouchId;
                            gesture.Handled = true;
                            _touchId = gesture.TouchId;
                            _lastMoveTime = null;
                        }
                    }
                    break;

                case GestureType.VerticalDrag:

                    if (_touchId == gesture.TouchId)
                    {
                        if (_lastMoveTime.HasValue)
                        {
                            double time = AppMain.Current.TotalGameTime - _lastMoveTime.Value;
                            _speed = (float)(gesture.Offset.Y / (elementsSize.Y + spacing) / time);
                        }

                        _isScrolling = true;
                        _scroll += gesture.Offset.Y / (elementsSize.Y + spacing);
                        gesture.Handled = true;
                    }

                    break;
            }
        }

        private void ComputeScrollTo()
        {
            Single internalFactor = _scroll - (Int32)_scroll;

            Boolean enabled = false;

            Single newScrollTo = (Int32)(_scroll + Math.Sign(_scroll) * 0.5f);

            Int32 scrollTo = (Int32)newScrollTo;

            Int32 sign = internalFactor < 0.5f ? -1 : 1;
            Single add = 1;

            while (!enabled)
            {
                _context.GetData(-scrollTo, _captionText.StringBuilder, out enabled);

                if (!enabled)
                {
                    scrollTo = (Int32)newScrollTo + (Int32)add * sign;

                    add += 0.5f;
                    sign *= -1;
                }
            }

            _scrollTo = scrollTo;
        }
    }
}
