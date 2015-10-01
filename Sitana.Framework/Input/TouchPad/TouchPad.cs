using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.Views;

namespace Sitana.Framework.Input.TouchPad
{
    public partial class TouchPad: Singleton<TouchPad>
    {
#if __MACOS__
        const float MouseWheelScrollFactor = 10000;
#else
        const float MouseWheelScrollFactor = 100;
#endif

        const int MouseId = 1;

        public delegate void OnTouchDelegate(int id, Vector2 position);

        struct LastTap
        {
            public Vector2 Position;
            public double  Time;
        }

        public int MinDragSize = 16;

        public int HoldTimeInMs = 1000;
        public int HoldStartTimeInMs = 700;

        public int DoubleTapSize = 32;

        public int DoubleTapTimeInMs = 1000;

        public GestureType RightClickGesture = GestureType.Hold;

        public event OnTouchDelegate TouchDown;
        public event OnTouchDelegate Tap;

		private int _scrollWheelValue = 0;

        Dictionary<int, TouchElement> _elements = new Dictionary<int, TouchElement>();

        Gesture _gesture = new Gesture();

        Vector2? _rightClick;

        DateTime _rightClickTime;

        LastTap? _lastTap;

        List<IGestureListener> _listeners = new List<IGestureListener>();

		public TouchPad()
		{
			_scrollWheelValue = Mouse.GetState().ScrollWheelValue;
		}

        public void AddListener(IGestureListener listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(IGestureListener listener)
        {
            _listeners.Remove(listener);
        }

        public void LockListener(int pointerId, object listener)
        {
            UiTask.BeginInvoke( ()=>
                {
                    _gesture.GestureType = GestureType.CapturedByOther;
                    _gesture.TouchId = pointerId;
					_gesture.ResetState();
                    _gesture.CapturePointer(listener);

                    OnGesture();
                });
        }

        internal void Update(float time, bool active)
        {
			if (_lastTap.HasValue)
			{
				LastTap tap = _lastTap.Value;
				tap.Time += time;

				if (_lastTap.Value.Time > (double)DoubleTapTimeInMs / 1000.0f)
				{
					_lastTap = null;
				} 
				else
				{
					_lastTap = tap;
				}
			}

            AnalyzeMouse(time, active);
            AnalyzeTouch(time);
        }

        void AnalyzeTouch(float time)
        {
            TouchCollection touch = TouchPanel.GetState();

            for(int idx = 0; idx < touch.Count; ++idx )
            {
                var tp = touch[idx];
                AnalyzeTouchPoint(ref tp, time);
            }

            var keys = _elements.Keys;

            for(int idx = 0; idx < keys.Count; )
            {
                int id = keys.ElementAt(idx);

                TouchLocation tl;

                if ( id != MouseId && !touch.FindById(id, out tl) )
                {
                    ProcessUp(id, _elements[id].Position, time);
                }
                else
                {
                    ++idx;
                }
            }
        }

        void AnalyzeTouchPoint(ref TouchLocation touch, float time)
        {
            TouchElement element;

            if (!_elements.TryGetValue(touch.Id, out element))
            {
                element = TouchElement.Invalid;
            }

            if (touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved)
            {
                if (!element.Valid)
                {
                    ProcessDown(touch.Id, touch.Position);
                }
                else
                {
                    ProcessMove(touch.Id, touch.Position, time);
                }
            }
            else if (element.Valid)
            {
                ProcessUp(touch.Id, touch.Position, time);
            }
        }

        void AnalyzeMouse(float time, bool active)
        {
            MouseState state = Mouse.GetState();

			int scrollWheel = state.ScrollWheelValue - _scrollWheelValue;
			_scrollWheelValue = state.ScrollWheelValue;

			if (scrollWheel != 0)
			{
				_gesture.GestureType = GestureType.MouseWheel;
				_gesture.Origin = state.ToVector2();
				_gesture.Position = state.ToVector2();
				_gesture.TouchId = MouseId;
                _gesture.Offset = new Vector2(0, (float)scrollWheel / MouseWheelScrollFactor);

				OnGesture();
			}

            if (active && state.RightButton == ButtonState.Pressed && !_rightClick.HasValue)
            {
                _rightClick = state.ToVector2();
                _rightClickTime = DateTime.Now;

				_gesture.GestureType = GestureType.RightButtonDown;
				_gesture.Origin = _rightClick.Value;
				_gesture.Position = _rightClick.Value;
				_gesture.TouchId = MouseId;
				_gesture.Offset = Vector2.Zero;

				OnGesture();
            }

            if ( active && state.RightButton == ButtonState.Released && _rightClick.HasValue)
            {
				_gesture.GestureType = GestureType.RightButtonUp;
				_gesture.Origin = _rightClick.Value;
				_gesture.Position = _rightClick.Value;
				_gesture.TouchId = MouseId;
				_gesture.Offset = Vector2.Zero;

				OnGesture();

				if (RightClickGesture != GestureType.None)
				{
					Vector2 move = state.ToVector2() - _rightClick.Value;

					if (move.Length() < MinDragSize)
					{
						if ((DateTime.Now - _rightClickTime).TotalMilliseconds < HoldTimeInMs)
						{
							_gesture.GestureType = RightClickGesture;
							_gesture.Origin = _rightClick.Value;
							_gesture.Position = _rightClick.Value;
							_gesture.TouchId = MouseId;
							_gesture.Offset = Vector2.Zero;

							OnGesture();
						}
					}
				}

                _rightClick = null;
            }

            _gesture.GestureType = GestureType.MouseMove;
            _gesture.Origin = state.ToVector2();
            _gesture.Position = state.ToVector2();
            _gesture.TouchId = 0;
            _gesture.Offset = Vector2.Zero;

            OnGesture();

            TouchElement element;

            if ( !_elements.TryGetValue(MouseId, out element) )
            {
                element = TouchElement.Invalid;
            }

            if (active && state.LeftButton == ButtonState.Pressed)
            {
                if (!element.Valid)
                {
                    ProcessDown(MouseId, state.ToVector2());
                }
                else
                {
                    ProcessMove(MouseId, state.ToVector2(), time);
                }
            }
            else if ( element.Valid )
            {
                ProcessUp(MouseId, state.ToVector2(), time);
            }
        }

        void ProcessDown(int id, Vector2 position)
        {
            TouchElement element = new TouchElement()
            {
                Origin = position,
                Position = position,
                Valid = true,
                LockedGesture = GestureType.None,
                DownTime = DateTime.Now
            };

            _gesture.GestureType = GestureType.Down;
            _gesture.Origin = position;
            _gesture.Position = position;
            _gesture.TouchId = id;
            _gesture.Offset = Vector2.Zero;

            OnGesture();

            element.LockedListener = _gesture.PointerCapturedBy;

            _elements.Add(id, element);

            if ( TouchDown != null )
            {
                TouchDown(id, position);
            }
        }

        void ProcessMove(int id, Vector2 position, float time)
        {
            TouchElement element = _elements[id];
            Vector2 move = position - element.Position;

            element.Position = position;

            _gesture.GestureType = GestureType.Move;
            _gesture.Origin = element.Origin;
            _gesture.Position = position;
            _gesture.TouchId = id;
            _gesture.Offset = move;
            _gesture.PointerCapturedBy = element.LockedListener;

            if (move != Vector2.Zero)
            {
                OnGesture();
            }

            if (!_gesture.Handled)
            {
                AnalyzeMoveGestures(id, ref element, ref move);
            }

            element.LockedListener = _gesture.PointerCapturedBy;

            _elements.Remove(id);
            _elements.Add(id, element);
        }

        void ProcessUp(int id, Vector2 position, float time)
        {
            TouchElement element = _elements[id];
            Vector2 move = position - element.Position;

            element.Position = position;
            element.LockedListener = null;

            _elements.Remove(id);

            _gesture.GestureType = GestureType.Up;
            _gesture.Origin = element.Origin;
            _gesture.Position = position;
            _gesture.PointerCapturedBy = element.LockedListener;
            _gesture.TouchId = id;
            _gesture.Offset = move;

            OnGesture();

            if ( element.LockedGesture == GestureType.HoldStart )
            {
                _gesture.GestureType = GestureType.HoldCancel;
                OnGesture();
            }

            if (!_gesture.Handled)
            {
                if (element.LockedGesture == GestureType.None)
                {
                    if ((element.Origin - element.Position).Length() < MinDragSize && (DateTime.Now - element.DownTime).TotalMilliseconds < HoldStartTimeInMs )
                    {
                        bool doubleTap = false;

                        if ( _lastTap.HasValue )
                        {
                            if ( (element.Position-_lastTap.Value.Position).Length() < DoubleTapSize )
                            {
                                _gesture.GestureType = GestureType.DoubleTap;

                                OnGesture();

                                doubleTap = true;

                                if (_gesture.Handled)
                                {
                                    _lastTap = null;
                                    return;
                                }
                            }
                        }

                        _gesture.GestureType = GestureType.Tap;
                        OnGesture();

                        if(Tap!=null)
                        {
                            Tap(_gesture.TouchId, _gesture.Position);
                        }

                        if ( !_gesture.Handled )
                        {
                            if ( doubleTap )
                            {
                                _lastTap = null;
                            }
                            else
                            {
                                _lastTap = new LastTap()
                                {
                                    Position = _gesture.Position,
                                    Time = 0
                                };
                            }
                        }
                    }
                }
            }
        }

        void AnalyzeMoveGestures(int id, ref TouchElement element, ref Vector2 move)
        {
            if ( (element.LockedGesture & (GestureType.HorizontalDrag|GestureType.VerticalDrag|GestureType.Hold|GestureType.HoldStart|GestureType.Down)) == GestureType.None )
            {
                Vector2 drag = element.Position - element.Origin;

                if ( Math.Abs(drag.X) > MinDragSize && Math.Abs(drag.X) > Math.Abs(drag.Y) )
                {
                    element.LockedGesture |= GestureType.HorizontalDrag;
                }
                else if (Math.Abs(drag.Y) > MinDragSize && Math.Abs(drag.Y) > Math.Abs(drag.X))
                {
                    element.LockedGesture |= GestureType.VerticalDrag;
                }

                if ( drag.Length() > MinDragSize )
                {
                    element.LockedGesture |= GestureType.FreeDrag;
                    move = drag;
                }
            }

            if ((element.LockedGesture & (GestureType.HorizontalDrag | GestureType.FreeDrag | GestureType.VerticalDrag)) != GestureType.None)
            {
                _gesture.TouchId = id;
                _gesture.Position = element.Position;
                _gesture.Origin = element.Origin;
                _gesture.Offset = move;
                _gesture.GestureType = element.LockedGesture;

                OnGesture();
            }
            else
            {
                _gesture.TouchId = id;
                _gesture.Position = element.Position;
                _gesture.Origin = element.Origin;
                _gesture.Offset = Vector2.Zero;

                if (element.LockedGesture == GestureType.None)
                {
                    TimeSpan ellapsed = DateTime.Now - element.DownTime;

                    if (ellapsed.TotalMilliseconds > HoldStartTimeInMs)
                    {
                        element.LockedGesture = GestureType.HoldStart;
                        _gesture.GestureType = GestureType.HoldStart;

                        OnGesture();
                    }
                }
                else if (element.LockedGesture == GestureType.HoldStart)
                {
                    TimeSpan ellapsed = DateTime.Now - element.DownTime;

                    if (move.Length() > MinDragSize)
                    {
                        element.LockedGesture = GestureType.Hold;
                        _gesture.GestureType = GestureType.HoldCancel;

                        OnGesture();
                        return;
                    }

                    if (ellapsed.TotalMilliseconds > HoldTimeInMs)
                    {
                        element.LockedGesture = GestureType.Hold;
                        _gesture.GestureType = GestureType.Hold;

                        OnGesture();
                    }
                }
            }
        }

        void OnGesture()
        {
            _gesture.ResetState();
            _gesture.PointerCapturedBy = null;

            for (int idx = 0; idx < _listeners.Count; ++idx)
            {
                _listeners[idx].OnGesture(_gesture);

                if (_gesture.Handled || _gesture.SkipRest)
                {
                    break;
                }
            }

            if (_gesture.PointerCapturedBy != null && _gesture.GestureType != GestureType.CapturedByOther)
            {
                _gesture.ResetState();
                _gesture.GestureType = GestureType.CapturedByOther;

                for (int idx = 0; idx < _listeners.Count; ++idx)
                {
                    _listeners[idx].OnGesture(_gesture);
                }
            }
        }
    }
}
