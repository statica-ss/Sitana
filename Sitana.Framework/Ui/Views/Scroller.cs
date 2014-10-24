using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Input.TouchPad;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views
{
    public class Scroller
    {
        [Flags]
        public enum Mode
        {
            None = 0,
            HorizontalDrag = 1,
            VerticalDrag = 2,
            BothDrag = HorizontalDrag | VerticalDrag
        }

        int _touchIdX = 0;
        int _touchIdY = 0;

        double? _lastMoveTime = null;

        UiView _view;
        ScrollingService _service;

        Vector2 _scrollSpeed = Vector2.Zero;

        public Scroller(UiView view, Mode mode, ScrollingService scrollingService)
        {
            _view = view;
            _service = scrollingService;

            if (mode.HasFlag(Mode.BothDrag))
            {
                TouchPad.Instance.AddListener(GestureType.FreeDrag | GestureType.Down | GestureType.Up, _view);
            }
            else if (mode.HasFlag(Mode.VerticalDrag))
            {
                TouchPad.Instance.AddListener(GestureType.VerticalDrag | GestureType.Down | GestureType.Up, _view);
            }
            else if (mode.HasFlag(Mode.HorizontalDrag))
            {
                TouchPad.Instance.AddListener(GestureType.HorizontalDrag | GestureType.Down | GestureType.Up, _view);
            }
            else
            {
                TouchPad.Instance.AddListener(GestureType.CapturedByOther, _view);
            }
        }

        public void Remove()
        {
            TouchPad.Instance.RemoveListener(_view);
        }

        public void OnGesture(Gesture gesture)
        {
            switch(gesture.GestureType)
            {
            case GestureType.Up:
                if (_touchIdX == gesture.TouchId)
                {
                    _touchIdX = 0;
                    _lastMoveTime = null;
                    _service.ScrollSpeedX = _scrollSpeed.X;
                }

                if (_touchIdY == gesture.TouchId)
                {
                    _touchIdY = 0;
                    _lastMoveTime = null;
                    _service.ScrollSpeedY = _scrollSpeed.Y;
                }
                break;

            case GestureType.HorizontalDrag:
            case GestureType.VerticalDrag:
            case GestureType.FreeDrag:

                if (gesture.PointerCapturedBy == null)
                {
                    if (_touchIdX == 0 && _touchIdY == 0)
                    {
                        if (_view.IsPointInsideView(gesture.Origin))
                        {
                            _touchIdX = _touchIdY = gesture.TouchId;
                            gesture.CapturePointer(_view);
                        }
                    }
                }

                if (_touchIdX == gesture.TouchId || _touchIdY == gesture.TouchId)
                {
                    gesture.Handled = true;

                    if (_touchIdX != 0)
                    {
                        _service.ScrollPositionX -= gesture.Offset.X;
                    }

                    if (_touchIdY != 0)
                    {
                        _service.ScrollPositionY -= gesture.Offset.Y;
                    }

                    if (_lastMoveTime != null)
                    {
                        double time = AppMain.Current.TotalGameTime - _lastMoveTime.Value;

                        if (_touchIdX != 0)
                        {
                            _scrollSpeed.X = (_service.ScrollSpeedX + -gesture.Offset.X / (float)time) / 2;
                        }

                        if (_touchIdY != 0)
                        {
                            _scrollSpeed.Y = (_service.ScrollSpeedY + -gesture.Offset.Y / (float)time) / 2;
                        }
                    }
                    else
                    {
                        _scrollSpeed.X = 0;
                        _scrollSpeed.Y = 0;
                    }

                    _lastMoveTime = AppMain.Current.TotalGameTime;
                }
                break;
            }
        }
    }
}
