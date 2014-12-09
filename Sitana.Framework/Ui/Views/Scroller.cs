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
            BothDrag = HorizontalDrag | VerticalDrag,
			HorizontalWheel = 4,
			VerticalWheel = 8
        }

        int _touchIdX = 0;
        int _touchIdY = 0;

        double? _lastMoveTime = null;

        UiView _view;
        ScrollingService _service;

        Vector2 _scrollSpeed = Vector2.Zero;

		bool _wheelScrollsHorizontal = false;
		float _wheelSpeed = 0;

        Mode _mode;

		public Scroller(UiView view, Mode mode, ScrollingService scrollingService, float wheelSpeed)
			: this(view, mode, scrollingService)
		{
			_wheelSpeed = wheelSpeed;
		}

        public Scroller(UiView view, Mode mode, ScrollingService scrollingService)
        {
            _mode = mode;
            _view = view;
            _service = scrollingService;

            if (mode.HasFlag(Mode.BothDrag))
            {
                view.EnabledGestures = (GestureType.FreeDrag | GestureType.Down | GestureType.Up);
            }
            else if (mode.HasFlag(Mode.VerticalDrag))
            {
                view.EnabledGestures = (GestureType.VerticalDrag | GestureType.Down | GestureType.Up);
            }
            else if (mode.HasFlag(Mode.HorizontalDrag))
            {
                view.EnabledGestures = (GestureType.HorizontalDrag | GestureType.Down | GestureType.Up);
            }

			if (mode.HasFlag(Mode.HorizontalWheel) || mode.HasFlag(Mode.VerticalWheel))
			{
				view.EnabledGestures |= (GestureType.MouseWheel);
				_wheelScrollsHorizontal = mode.HasFlag(Mode.HorizontalWheel);
			}
        }

        public void OnGesture(Gesture gesture)
        {
            switch(gesture.GestureType)
            {
			case GestureType.MouseWheel:

				if (_view.IsPointInsideView(gesture.Position))
				{
					float offset = -gesture.Offset.Y * _wheelSpeed;

					if (_wheelScrollsHorizontal)
					{
						float maxScroll = _service.ScrolledElement.MaxScrollX - _service.ScrolledElement.ScreenBounds.Width;

						if ( ( offset < 0 && _service.ScrollPositionX > 0) || (offset > 0 && _service.ScrollPositionX < maxScroll))
						{
							_service.ScrollPositionX += offset;
						}
					}
					else
					{
						float maxScroll = _service.ScrolledElement.MaxScrollY - _service.ScrolledElement.ScreenBounds.Height;

						if ( (offset < 0 && _service.ScrollPositionY > 0) || (offset > 0 && _service.ScrollPositionY < maxScroll))
						{
							_service.ScrollPositionY += offset;
						}
					}
				}

				break;

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

                    if (_touchIdX != 0 && _mode.HasFlag(Mode.HorizontalDrag))
                    {
                        _service.ScrollPositionX -= gesture.Offset.X;
                    }

                    if (_touchIdY != 0 && _mode.HasFlag(Mode.VerticalDrag))
                    {
                        _service.ScrollPositionY -= gesture.Offset.Y;
                    }

                    if (_lastMoveTime != null)
                    {
                        double time = AppMain.Current.TotalGameTime - _lastMoveTime.Value;

                        if (_touchIdX != 0 && _mode.HasFlag(Mode.HorizontalDrag))
                        {
                            _scrollSpeed.X = (_service.ScrollSpeedX + -gesture.Offset.X / (float)time) / 2;
                        }

                        if (_touchIdY != 0 && _mode.HasFlag(Mode.VerticalDrag))
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
