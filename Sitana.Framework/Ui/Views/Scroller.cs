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
        float _scrollPositionX = 0;
        float _scrollPositionY = 0;

        int _touchIdX = 0;
        int _touchIdY = 0;

        int _maxScrollX = 0;
        int _maxScrollY = 0;

        float _scrollSpeedX = 0;
        float _scrollSpeedY = 0;

        double? _lastMoveTime = null;

        UiView _view;

        Rectangle _bounds = new Rectangle(0,0,1,1);

        public Point ScrollPosition
        {
            get
            {
                return new Point((int)_scrollPositionX, (int)_scrollPositionY);
            }
        }

        public Point MaxScroll
        {
            set
            {
                _maxScrollX = value.X;
                _maxScrollY = value.Y;
            }
        }

        public Scroller(UiView view, bool horizontal, bool vertical)
        {
            _view = view;

            if (horizontal && vertical)
            {
                TouchPad.Instance.AddListener(GestureType.FreeDrag | GestureType.Down | GestureType.Up, _view);
            }
            else if (vertical)
            {
                TouchPad.Instance.AddListener(GestureType.VerticalDrag | GestureType.Down | GestureType.Up, _view);
            }
            else if ( horizontal )
            {
                TouchPad.Instance.AddListener(GestureType.HorizontalDrag | GestureType.Down | GestureType.Up, _view);
            }
        }

        public void Remove()
        {
            TouchPad.Instance.RemoveListener(_view);
        }

        public void Update(float time, Rectangle bounds)
        {
            if (_bounds != bounds)
            {
                float factorX = 1;
                float factorY = 1;
                factorX = (float)bounds.Width/(float)_bounds.Width;
                factorY = (float)bounds.Height / (float)_bounds.Height;

                _bounds = bounds;

                _scrollPositionX *= factorX;
                _scrollPositionY *= factorY;
            }

            float desiredScrollX = Math.Max(0, Math.Min(_maxScrollX - bounds.Width, _scrollPositionX));
            float desiredScrollY = Math.Max(0, Math.Min(_maxScrollY - bounds.Height, _scrollPositionY));

            if (Math.Abs(desiredScrollX - _scrollPositionX) > bounds.Width / 5)
            {
                _scrollSpeedX = 0;
                _touchIdX = 0;
            }

            if (Math.Abs(desiredScrollY - _scrollPositionY) > bounds.Height / 5)
            {
                _scrollSpeedY = 0;
                _touchIdY = 0;
            }

            _scrollPositionX = ComputeScroll(time, _scrollPositionX, _maxScrollX, bounds.Width);
            _scrollPositionY = ComputeScroll(time, _scrollPositionY, _maxScrollY, bounds.Height);

            if ( _touchIdX == 0)
            {
                _scrollPositionX += _scrollSpeedX * time;
                _scrollSpeedX -= _scrollSpeedX * time * 10;

                if (Math.Abs(_scrollSpeedX) < 1)
                {
                    _scrollSpeedX = 0;
                }
            }

            if(_touchIdY == 0)
            {
                _scrollPositionY += _scrollSpeedY * time;
                _scrollSpeedY -= _scrollSpeedY * time * 10;

                if (Math.Abs(_scrollSpeedY) < 1)
                {
                    _scrollSpeedY = 0;
                }
            }
        }

        private float ComputeScroll(float time, float scrollPosition, float maxScroll, int size)
        {
            float desiredScroll = Math.Max(0, Math.Min(maxScroll - size, scrollPosition));

            if (desiredScroll != scrollPosition)
            {
                int sign = Math.Sign(desiredScroll - scrollPosition);

                for (int idx = 0; idx < 10; ++idx)
                {
                    scrollPosition = time * desiredScroll + (1 - time) * scrollPosition;
                }

                if (Math.Abs(desiredScroll - scrollPosition) < 1)
                {
                    scrollPosition = desiredScroll;
                }

                if (Math.Sign(desiredScroll - scrollPosition) != sign)
                {
                    scrollPosition = desiredScroll;
                }
            }

            return scrollPosition;
        }

        public void OnGesture(Gesture gesture)
        {
            switch (gesture.GestureType)
            {
                case GestureType.Up:
                    if (_touchIdX == gesture.TouchId)
                    {
                        _touchIdX = 0;
                        gesture.Handled = true;
                        _lastMoveTime = null;
                    }

                    if (_touchIdY == gesture.TouchId)
                    {
                        _touchIdY = 0;
                        gesture.Handled = true;
                        _lastMoveTime = null;
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
                            _scrollPositionX -= gesture.Offset.X;
                        }

                        if (_touchIdY != 0)
                        {
                            _scrollPositionY -= gesture.Offset.Y;
                        }

                        if (_lastMoveTime != null)
                        {
                            double time = AppMain.Current.TotalGameTime - _lastMoveTime.Value;

                            if (_touchIdX != 0)
                            {
                                _scrollSpeedX = (_scrollSpeedX + -gesture.Offset.X / (float)time) / 2;
                            }

                            if (_touchIdY != 0)
                            {
                                _scrollSpeedY = (_scrollSpeedY + -gesture.Offset.Y / (float)time) / 2;
                            }
                        }
                        else
                        {
                            _scrollSpeedX = 0;
                            _scrollSpeedY = 0;
                        }

                        _lastMoveTime = AppMain.Current.TotalGameTime;
                    }
                    break;
            }
        }
    }
}
