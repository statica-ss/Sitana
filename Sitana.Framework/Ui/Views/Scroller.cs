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
        float _scrollPosition = 0;

        int _touchId = 0;
        int _maxScroll = 0;

        float _scrollSpeed = 0;

        double? _lastMoveTime = null;

        IGestureListener _listener;

        bool _vertical = false;

        Rectangle _bounds = new Rectangle(0,0,1,1);

        public int ScrollPosition
        {
            get
            {
                return (int)_scrollPosition;
            }
        }

        public int MaxScroll
        {
            set
            {
                _maxScroll = value;
            }
        }

        public Scroller(IGestureListener listener, bool vertical)
        {
            _listener = listener;
            _vertical = vertical;

            if (_vertical)
            {
                TouchPad.Instance.AddListener(GestureType.VerticalDrag | GestureType.Down | GestureType.Up, _listener);
            }
            else
            {
                TouchPad.Instance.AddListener(GestureType.HorizontalDrag | GestureType.Down | GestureType.Up, _listener);
            }
        }

        public void Remove()
        {
            TouchPad.Instance.RemoveListener(_listener);
        }

        public void Update(float time, Rectangle bounds)
        {
            if (_bounds != bounds)
            {
                float factor = 1;
                factor = _vertical ? (float)bounds.Height/(float)_bounds.Height : (float)bounds.Width/(float)_bounds.Width;
                _bounds = bounds;

                _scrollPosition *= factor;
            }

            int size = _vertical ? bounds.Height : bounds.Width;

            float desiredScroll = Math.Max(0, Math.Min(_maxScroll - size, _scrollPosition));

            if (Math.Abs(desiredScroll - _scrollPosition) > bounds.Height / 5)
            {
                _touchId = 0;
                _scrollSpeed = 0;
            }

            if (desiredScroll != _scrollPosition)
            {
                int sign = Math.Sign(desiredScroll - _scrollPosition);

                for (int idx = 0; idx < 10; ++idx)
                {
                    _scrollPosition = time * desiredScroll + (1 - time) * _scrollPosition;
                }

                if (Math.Abs(desiredScroll - _scrollPosition) < 1)
                {
                    _scrollPosition = desiredScroll;
                }

                if (Math.Sign(desiredScroll - _scrollPosition) != sign)
                {
                    _scrollPosition = desiredScroll;
                }
            }
            else if ( _touchId == 0)
            {
                _scrollPosition += _scrollSpeed * time;
                _scrollSpeed -= _scrollSpeed * time * 10;

                if (Math.Abs(_scrollSpeed) < 1)
                {
                    _scrollSpeed = 0;
                }
            }
        }

        public void OnGesture(Gesture gesture, Rectangle screenBounds)
        {
            switch (gesture.GestureType)
            {
                case GestureType.Down:
                    if (_touchId == 0)
                    {
                        if (screenBounds.Contains(gesture.Position.ToPoint()))
                        {
                            _touchId = gesture.TouchId;
                            gesture.Handled = true;
                            gesture.LockedListener = _listener;
                            _lastMoveTime = null;
                            _scrollSpeed = 0;
                        }
                    }
                    break;

                case GestureType.Up:
                    if (_touchId == gesture.TouchId)
                    {
                        _touchId = 0;
                        gesture.Handled = true;
                        _lastMoveTime = null;
                    }
                    break;

                case GestureType.VerticalDrag:
                    if (_touchId == gesture.TouchId)
                    {
                        _scrollPosition -= gesture.Offset.Y;
                        gesture.Handled = true;

                        if (_lastMoveTime != null)
                        {
                            double time = AppMain.Current.TotalGameTime - _lastMoveTime.Value;
                            _scrollSpeed = -gesture.Offset.Y / (float)time;
                        }
                        else
                        {
                            _scrollSpeed = 0;
                        }

                        _lastMoveTime = AppMain.Current.TotalGameTime;
                    }
                    break;

                case GestureType.HorizontalDrag:
                    if (_touchId == gesture.TouchId)
                    {
                        _scrollPosition -= gesture.Offset.X;
                        gesture.Handled = true;

                        if (_lastMoveTime != null)
                        {
                            double time = AppMain.Current.TotalGameTime - _lastMoveTime.Value;
                            _scrollSpeed = (_scrollSpeed + -gesture.Offset.X / (float)time) / 2;
                        }
                        else
                        {
                            _scrollSpeed = 0;
                        }

                        _lastMoveTime = AppMain.Current.TotalGameTime;
                    }
                    break;
            }
        }
    }
}
