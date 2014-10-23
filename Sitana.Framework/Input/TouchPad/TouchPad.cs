using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Input.TouchPad
{
    public partial class TouchPad: Singleton<TouchPad>
    {
        const int MouseId = 1;

        struct ListenerInfo
        {
            public GestureType GestureType;
            public IGestureListener Listener;
            public IGestureListener Parent;
        }

        struct LastTap
        {
            public Vector2 Position;
            public DateTime Time;
        }

        public int MinDragSize = 16;
        public int HoldTimeInMs = 1000;
        public int HoldStartTimeInMs = 700;
        public int DoubleTapTimeInMs = 500;

        public GestureType RightClickGesture = GestureType.Hold;

        Dictionary<int, TouchElement> _elements = new Dictionary<int, TouchElement>();

        List<ListenerInfo> _listeners = new List<ListenerInfo>();

        Dictionary<IGestureListener, bool> _listenersMap = new Dictionary<IGestureListener, bool>();

        Gesture _gesture = new Gesture();
        Gesture _gesturePointerCapturedBy = new Gesture();

        Vector2? _rightClick;
        DateTime _rightClickTime;

        LastTap? _lastTap;

        public void AddListener(GestureType gestureType, IGestureListener listener)
        {
            if (_listenersMap.ContainsKey(listener))
            {
                int index = _listeners.FindIndex(el => el.Listener == listener);

                var element = _listeners[index];
                element.GestureType |= gestureType;

                _listeners[index] = element;
            }
            else
            {
                IGestureListener parent;
                int index = FindIndex(listener, out parent);

                _listeners.Insert(index, new ListenerInfo() { GestureType = gestureType, Listener = listener, Parent = parent });
                _listenersMap.Add(listener, true);
            }
        }

        int FindIndex(IGestureListener listener, out IGestureListener listenerParent)
        {
            int index = 0;
            IGestureListener parent = null;

            parent = listener.Parent;

            while(parent != null && !_listenersMap.ContainsKey(parent))
            {
                parent = parent.Parent;
            }

            if ( parent != null )
            {
                int first = _listeners.FindIndex((el) => el.Listener == parent || el.Parent == parent);

                if ( first >= 0 )
                {
                    index = first;
                }
            }

            listenerParent = parent;
            return index;
        }

        public void RemoveListener(IGestureListener listener)
        {
            if (_listenersMap.ContainsKey(listener))
            {
                _listenersMap.Remove(listener);

                int index = _listeners.FindIndex(el => el.Listener == listener);

                if (index >= 0)
                {
                    _listeners.RemoveAt(index);
                }
            }
        }

        internal void Update(float time)
        {
            AnalyzeMouse(time);
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

        void AnalyzeMouse(float time)
        {
            MouseState state = Mouse.GetState();

            if (state.RightButton == ButtonState.Pressed && !_rightClick.HasValue)
            {
                _rightClick = state.ToVector2();
                _rightClickTime = DateTime.Now;
            }

            if ( state.RightButton == ButtonState.Released && _rightClick.HasValue)
            {
                Vector2 move = state.ToVector2() - _rightClick.Value;

                if (move.Length() < MinDragSize)
                {
                    if ((DateTime.Now - _rightClickTime).TotalMilliseconds < HoldTimeInMs)
                    {
                        _gesture.GestureType = RightClickGesture;
                        _gesture.Origin = _rightClick.Value;
                        _gesture.Position = _rightClick.Value;
                        _gesture.Handled = false;
                        _gesture.TouchId = MouseId;
                        _gesture.Offset = Vector2.Zero;

                        OnGesture();
                    }
                }

                _rightClick = null;
            }

            TouchElement element;

            if ( !_elements.TryGetValue(MouseId, out element) )
            {
                element = TouchElement.Invalid;
            }

            if (state.LeftButton == ButtonState.Pressed)
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
            _gesture.Handled = false;
            _gesture.TouchId = id;
            _gesture.Offset = Vector2.Zero;

            OnGesture();

            element.LockedListener = _gesture.PointerCapturedBy;

            _elements.Add(id, element);
        }

        void ProcessMove(int id, Vector2 position, float time)
        {
            TouchElement element = _elements[id];
            Vector2 move = position - element.Position;

            element.Position = position;

            _gesture.GestureType = GestureType.Move;
            _gesture.Origin = element.Origin;
            _gesture.Position = position;
            _gesture.Handled = false;
            _gesture.TouchId = id;
            _gesture.Offset = move;
            _gesture.PointerCapturedBy = element.LockedListener;

            if (move != Vector2.Zero)
            {
                OnGesture();

                if (!_gesture.Handled)
                {
                    AnalyzeMoveGestures(id, ref element, ref move);
                }
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
            _gesture.Handled = false;
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
                            if ( (DateTime.Now - _lastTap.Value.Time).TotalMilliseconds < DoubleTapTimeInMs )
                            {
                                if ( (element.Position-_lastTap.Value.Position).Length() < MinDragSize )
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
                        }

                        _gesture.GestureType = GestureType.Tap;
                        OnGesture();

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
                                    Time = DateTime.Now
                                };
                            }
                        }
                    }
                }
            }
        }

        void AnalyzeMoveGestures(int id, ref TouchElement element, ref Vector2 move)
        {
            Vector2 moveHorz = new Vector2(move.X, 0);
            Vector2 moveVert = new Vector2(0, move.Y);

            if ( (element.LockedGesture & (GestureType.HorizontalDrag|GestureType.VerticalDrag|GestureType.Hold|GestureType.HoldStart|GestureType.Down)) == GestureType.None )
            {
                Vector2 drag = element.Position - element.Origin;

                if ( Math.Abs(drag.X) > MinDragSize && Math.Abs(drag.X) > Math.Abs(drag.Y) )
                {
                    element.LockedGesture |= GestureType.HorizontalDrag;
                    moveHorz = new Vector2(drag.X, 0);
                }
                else if (Math.Abs(drag.Y) > MinDragSize && Math.Abs(drag.Y) > Math.Abs(drag.X))
                {
                    element.LockedGesture |= GestureType.VerticalDrag;
                    moveVert = new Vector2(0, drag.Y);
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

                IGestureListener captureBy = _gesture.PointerCapturedBy;

                for (int idx = 0; idx < _listeners.Count; ++idx)
                {
                    var listener = _listeners[idx];

                    if (_gesture.PointerCapturedBy != null && _gesture.PointerCapturedBy != listener.Listener)
                        continue;

                    if ((listener.GestureType & element.LockedGesture) != GestureType.None)
                    {
                        _gesture.GestureType = element.LockedGesture & listener.GestureType;

                        switch (_gesture.GestureType)
                        {
                            case GestureType.HorizontalDrag:
                                _gesture.Offset = moveHorz;
                                break;

                            case GestureType.VerticalDrag:
                                _gesture.Offset = moveVert;
                                break;

                            case GestureType.FreeDrag:
                                _gesture.Offset = move;
                                break;

                            default:
                                throw new InvalidOperationException("Element cannot listen to FreeDrag gesture and Horizontal or Vertical drag at one time.");
                        }

                        listener.Listener.OnGesture(_gesture);

                        if (_gesture.PointerCapturedBy != captureBy)
                        {
                            SendCapturedGesture(_gesture.TouchId, _gesture.PointerCapturedBy);
                            return;
                        }

                        if (_gesture.Handled)
                        {
                            return;
                        }
                    }
                }
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

        void SendCapturedGesture(int touchId, IGestureListener capture)
        {
            _gesturePointerCapturedBy.GestureType = GestureType.CapturedByOther;
            _gesturePointerCapturedBy.TouchId = touchId;

            for (int idx2 = 0; idx2 < _listeners.Count; ++idx2)
            {
                var listener = _listeners[idx2];

                if (listener.Listener != capture)
                {
                    listener.Listener.OnGesture(_gesturePointerCapturedBy);
                }
            }
        }

        void OnGesture()
        {
            if (_gesture.PointerCapturedBy != null)
            {
                _gesture.PointerCapturedBy.OnGesture(_gesture);
                return;
            }

            for(int idx = 0; idx < _listeners.Count; ++idx )
            {
                var listener = _listeners[idx];

                if ( listener.GestureType.HasFlag(_gesture.GestureType) )
                {
                    listener.Listener.OnGesture(_gesture);

                    if (_gesture.PointerCapturedBy != null)
                    {
                        SendCapturedGesture(_gesture.TouchId, _gesture.PointerCapturedBy);
                        return;
                    }

                    if ( _gesture.Handled )
                    {
                        return;
                    }
                }
            }
        }
    }
}
