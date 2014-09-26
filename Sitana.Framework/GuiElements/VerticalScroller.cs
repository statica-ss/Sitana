// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
using System;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using Ebatianos;
using Ebatianos.Input;

namespace Ebatianos.Gui
{
    public class VerticalScroller
    {
        public Single Position
        { 
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        public GestureType LastGesture
        {
            get
            {
                return _lastGestureType;
            }
        }

        public Boolean CanDoGesture
        {
            get 
            {
                return _canDoGesture;
            }

            set
            {
                _canDoGesture = value;
            }
        }

        public Boolean Enable { get; set; }

        private GestureType _lastGestureType = GestureType.None;
        private Boolean _canDoGesture = false;

        private Single _position;
        private Single _moveSpeed = 0;

        private Single _addPosition = 0;
        private Single _maxPosition = 0;

        private Single _maxOver = 0;

        private Rectangle _elementRectangle;

        private Single _dragSpeed = 0;
        private TimeSpan _lastDragTime = TimeSpan.Zero;

        public VerticalScroller(Rectangle elementRectangle, Single maxScrollOver)
        {
            _maxOver = maxScrollOver;
            _elementRectangle = elementRectangle;
            Enable = true;
        }

        public void UpdateElementRectangle(Rectangle rect)
        {
            _elementRectangle = rect;
        }

        public Boolean Update(Single time, Single contentHeight)
        {
            if (!Enable)
            {
                return false;
            }

            _maxPosition = Math.Max(0, contentHeight - _elementRectangle.Height);

            Single lastPos = _position;

            if (_addPosition != 0)
            {
                Single add = (Single)(
                    Math.Sign(_addPosition) * Math.Min(Math.Abs(_addPosition), time * 500)
                );

                _addPosition -= add;
                _position += add;
            }

            if ( !_canDoGesture || _lastGestureType != GestureType.VerticalDrag)
            {
                _position += _moveSpeed * time ;

                if (_position > _maxPosition + _maxOver)
                {
                    _position = _maxPosition + _maxOver;
                }

                if (_position < -_maxOver)
                {
                    _position = -_maxOver;
                }

                Single maxPosition = _maxPosition;
                Single minPosition = 0;

                Single desiredPos = Math.Max(minPosition, Math.Min(maxPosition, _position));

                _position = time * 4 * desiredPos + (1 - time * 4) * _position;
                _position = time * 4 * desiredPos + (1 - time * 4) * _position;

                if (Math.Abs(desiredPos - _position) < 1)
                {
                    _position = desiredPos;
                }

                if (_moveSpeed != 0)
                {
                    Single inverse = _moveSpeed * time * 4;

                    if (desiredPos != _position)
                    {
                        inverse *= 3;
                    }

                    if (Math.Abs(inverse) > Math.Abs(_moveSpeed))
                    {
                        inverse = _moveSpeed;
                    }

                    _moveSpeed -= inverse;

                    if (Math.Abs(_moveSpeed) < 2)
                    {
                        _moveSpeed = 0;
                    }
                }
            }

            return lastPos != _position;
        }

        public void AddPosition(Single add)
        {
            _addPosition += add;
        }

        public void HandleTouchDownGesture(Object sender, GestureEventArgs args)
        {
            if (!Enable)
            {
                return;
            }

            Point pos = GraphicsHelper.PointFromVector2(args.Sample.Position);

            _canDoGesture = _elementRectangle.Contains(pos);
            _lastGestureType = GestureType.None;
            _moveSpeed = 0;
            _dragSpeed = 0;
            _lastDragTime = TimeSpan.Zero;
        }

        public void HandleTouchUpGesture(Object sender, GestureEventArgs args)
        {
            _canDoGesture = false;
        }

        public void HandleHorizontalDragGesture(Object sender, EventArgs args)
        {
            if (!Enable)
            {
                return;
            }

            if (_lastGestureType != GestureType.HorizontalDrag)
            {
                if (!_canDoGesture)
                {
                    return;
                }
            }

            _lastGestureType = GestureType.HorizontalDrag;
        }

        public void HandleClickGesture(Object sender, GestureEventArgs args)
        {
            if (!Enable)
            {
                return;
            }

            Point pos = GraphicsHelper.PointFromVector2(args.Sample.Position);

            if (!_elementRectangle.Contains(pos))
            {
                return;
            }

            _lastGestureType = GestureType.Tap;
        }

        public void HandleVerticalDragGesture(Object sender, GestureEventArgs args)
        {
            if (!Enable)
            {
                return;
            }

            if ( _lastGestureType != GestureType.VerticalDrag )
            {
                if (!_canDoGesture)
                {
                    return;
                }
            }

            _position -= args.Sample.Delta.Y;

            if ( _lastDragTime != TimeSpan.Zero )
            {
                TimeSpan diff = args.Sample.Timestamp - _lastDragTime;

                if (diff.TotalSeconds > 0)
                {
                    _dragSpeed = (Single)(-args.Sample.Delta.Y / diff.TotalSeconds);
                }
            }

            _lastDragTime = args.Sample.Timestamp;

            if (_position > _maxPosition + _maxOver)
            {
                _position = _maxPosition + _maxOver;
            }

            if (_position < -_maxOver)
            {
                _position = -_maxOver;
            }

            if (args.Sample.Delta.Y != 0)
            { 
                args.Handled = true;
            }

            _lastGestureType = GestureType.VerticalDrag;
        }

        public void HandleFlickGesture(Object sender, GestureEventArgs args)
        {
            if (!Enable)
            {
                return;
            }

            if ( _lastGestureType != GestureType.VerticalDrag )
            {
                _lastGestureType = GestureType.None;
                return;
            }

            _lastGestureType = GestureType.None;

            _moveSpeed = _dragSpeed;
            _dragSpeed = 0;

            if (args.Sample.Delta.Y != 0)
            {
                args.Handled = true;
            }

            _lastGestureType = GestureType.Flick;
        }
    }
}

