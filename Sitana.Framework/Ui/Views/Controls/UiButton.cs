// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;

namespace Sitana.Framework.Ui.Views
{
    public class UiButton: UiView, IGestureListener
    {
        delegate void OnClickDelegate(UiButton sender);

        enum Delegates
        {
            OnClick
        }

        public enum State
        {
            Disabled,
            Released,
            Pushed
        }

        protected UiButtonMode ButtonMode
        {
            set
            {
                _mode = value;
            }
        }

        public bool IsPushed {get; private set;}

        public bool Enabled { get; set; }

        float _delayTime = 0.5f;
        float _waitForAction = 0;

        UiButtonMode _mode = UiButtonMode.Release;

        private int _touchId = 0;

        private Dictionary<int, bool> _touches = new Dictionary<int, bool>();

        protected override void OnAdded()
        {
            SetPushed(false);
            OnPushedChanged();

            TouchPad.Instance.AddListener(GestureType.Down | GestureType.Up | GestureType.Move | GestureType.HorizontalDrag, this);
        }

        protected override void OnRemoved()
        {
            TouchPad.Instance.RemoveListener(this);
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if( _waitForAction > 0 )
            {
                _waitForAction -= time;

                if ( _waitForAction <= 0 )
                {
                    DoAction();
                    _waitForAction = 0;
                }
            }
        }

        void IGestureListener.OnGesture(Gesture gesture)
        {
            Rectangle bounds = ScreenBounds;

            switch(gesture.GestureType)
            {
                case GestureType.Down:

                    if (bounds.Contains(gesture.Origin))
                    {
                        if ( _mode == UiButtonMode.Game)
                        {
                            if ( !_touches.ContainsKey(gesture.TouchId))
                            {
                                _touches.Add(gesture.TouchId, true);
                            }
                            break;
                        }

                        if (_touchId == 0)
                        {
                            _touchId = gesture.TouchId;

                            SetPushed(true);
                            gesture.Handled = true;

                            if (_mode == UiButtonMode.Press)
                            {
                                DoAction();
                            }
                            else if (_mode == UiButtonMode.Delayed)
                            {
                                _waitForAction = _delayTime;
                            }
                        }
                    }
                    break;

                case GestureType.Move:

                    if (_mode == UiButtonMode.Game)
                    {
                        if (bounds.Contains(gesture.Origin))
                        {
                            if (!_touches.ContainsKey(gesture.TouchId))
                            {
                                _touches.Add(gesture.TouchId, true);
                            }
                            SetPushed(true);
                        }
                        else
                        {
                            _touches.Remove(gesture.TouchId);

                            if (_touches.Count == 0 )
                            {
                                SetPushed(false);
                            }
                        }
                        break;
                    }
                    

                    if (_touchId == gesture.TouchId)
                    {
                        SetPushed(bounds.Contains(gesture.Position));
                        //gesture.Handled = true;
                    }
                    break;

                case GestureType.Up:

                    if (_mode == UiButtonMode.Game)
                    {
                        _touches.Remove(gesture.TouchId);

                        if (_touches.Count == 0)
                        {
                            SetPushed(false);
                        }
                        break;
                    }

                    if ( _touchId == gesture.TouchId)
                    {
                        if ( IsPushed && _mode == UiButtonMode.Release)
                        {
                            DoAction();
                        }

                        _touchId = 0;
                        SetPushed(false);
                        //gesture.Handled = true;
                    }
                    break;
            }
        }
        
        protected virtual void OnPushedChanged()
        {
            BackgroundColor = IsPushed ? Color.Red : Color.Green;
        }

        void SetPushed(bool pushed)
        {
            if ( IsPushed != pushed)
            {
                IsPushed = pushed;
                OnPushedChanged();
            }
        }

        protected override void Init(UiController controller, object binding, DefinitionFile file)
        {
            base.Init(ref controller, binding, file);
        }

        void DoAction()
        {
            CallDelegate("Click");
        }
    }
}
