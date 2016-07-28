﻿// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Diagnostics;
using System;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Sitana.Framework.Ui.Views
{
    public class UiButton: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Text"] = parser.ParseString("Text");
            file["Icon"] = parser.ParseResource<Texture2D>("Icon");
            file["Click"] = parser.ParseDelegate("Click");
            file["Hold"] = parser.ParseDelegate("Hold");

            file["Push"] = parser.ParseDelegate("Push");
            file["Release"] = parser.ParseDelegate("Release");

            file["Enabled"] = parser.ParseBoolean("Enabled");
            file["Disabled"] = parser.ParseBoolean("Disabled");

            file["RepeatStart"] = parser.ParseInt("RepeatStart");
            file["RepeatInterval"] = parser.ParseInt("RepeatInterval");

            file["PushSound"] = parser.ParseResource<SoundEffect>("PushSound");
            file["ReleaseSound"] = parser.ParseResource<SoundEffect>("ReleaseSound");
            file["ActionSound"] = parser.ParseResource<SoundEffect>("ActionSound");

            file["GestureMargin"] = parser.ParseMargin("GestureMargin");

            file["Mode"] = parser.ParseEnum<UiButtonMode>("Mode");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiButton.Drawables":
                        ParseDrawables(cn, file, typeof(ButtonDrawable));
                        break;
                }
            }
        }

        protected UiButtonMode ButtonMode
        {
            set
            {
                _mode = value;
            }
        }

        public bool IsPushed {get; private set;}

        SharedValue<bool> _enabledFlag;
        bool _enabledFlagInvert = false;
        protected Margin _gestureMargin;

        public bool Enabled
        {
            get
            {
                return _enabledFlagInvert ? !_enabledFlag.Value : _enabledFlag.Value;
            }

            set
            {
                _enabledFlag.Value = _enabledFlagInvert ? !value : value;
            }
        }

        float _delayTime = 0.5f;
        
        float _repeatInterval = 0.5f;
        float _repeatStart = 1f;

        float _waitForAction = 0;

        UiButtonMode _mode = UiButtonMode.Release;

        protected int _touchId = TouchPad.InvalidTouchId;

        private Dictionary<int, bool> _touches = new Dictionary<int, bool>();

        protected List<ButtonDrawable> _drawables = new List<ButtonDrawable>();

        protected SharedString _text;

        protected Rectangle _checkRect;

        public SharedValue<Texture2D> Icon;

        private SoundEffect _pushSound;
        private SoundEffect _releaseSound;
        protected SoundEffect _actionSound;

        private DateTime? _holdTime = null;

        private bool _processHold = false;

        public SharedString Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }

        public virtual ButtonState ButtonState
        {
            get
            {
                return Enabled ? (IsPushed ? ButtonState.Pushed : ButtonState.None) : ButtonState.Disabled;
            }
        }

        protected override void OnAdded()
        {
            SetPushed(false, false);
            OnPushedChanged();

            EnabledGestures = (GestureType.Down | GestureType.Up | GestureType.Move);
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if (_holdTime.HasValue)
            {
                if ((DateTime.Now - _holdTime.Value).TotalMilliseconds >= TouchPad.Instance.HoldTimeInMs)
                {
                    CallDelegate("Hold");
                    _holdTime = null;
                    SetPushed(false, false);
                }
            }

            if(IsPushed && !Enabled)
            {
                SetPushed(false, false);
                _waitForAction = 0;
            }

            if( _waitForAction > 0 )
            {
                _waitForAction -= time;

                if ( _waitForAction <= 0 )
                {
                    if (_mode == UiButtonMode.Repeat)
                    {
                        if (IsPushed)
                        {
                            DoAction();
                            _waitForAction += _repeatInterval;
                        }
                    }
                    else
                    {
                        DoAction();
                        _waitForAction = 0;
                    }
                }
            }
        }

        protected override void OnViewDisplayChanged(bool isDisplayed)
        {
            base.OnViewDisplayChanged(isDisplayed);

            if (!isDisplayed)
            {
                SetPushed(false, false);
            }
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            _enabledFlag.ValueChanged -= OnEnabledChanged;
        }

        protected override void OnGesture(Gesture gesture)
        {
            if (!Enabled)
            {
                return;
            }

            int tolerance = Math.Max(_gestureMargin.Left, Math.Max(_gestureMargin.Right, Math.Max(_gestureMargin.Top, _gestureMargin.Bottom)));

            Rectangle bounds = ScreenBounds;

            bounds.X -= _gestureMargin.Left;
            bounds.Width += _gestureMargin.Width;

            bounds.Y -= _gestureMargin.Top;
            bounds.Height += _gestureMargin.Height;

            switch(gesture.GestureType)
            {
                case GestureType.CapturedByOther:

                    if (_touchId == gesture.TouchId)
                    {
                        SetPushed(false, true);
                    }
                    break;

                case GestureType.Hold:
                    if(_touchId == gesture.TouchId && _processHold)
                    {
                        gesture.SetHandled();
                    }
                    break;

                case GestureType.Down:

                    if (bounds.Contains(gesture.Origin.ToPoint()))
                    {
						if (Parent.IsPointInsideView(gesture.Origin, tolerance))
                        {
                            if (_mode == UiButtonMode.Game)
                            {
                                if (!_touches.ContainsKey(gesture.TouchId))
                                {
                                    _touches.Add(gesture.TouchId, true);
                                }
                                break;
                            }

                            if (_touchId == 0)
                            {
                                _touchId = gesture.TouchId;

                                if (_processHold)
                                {
                                    _holdTime = gesture.Time;
                                }

                                SetPushed(true, _mode != UiButtonMode.Press);
                                _checkRect = bounds;

                                gesture.SetHandled();

                                if (_mode == UiButtonMode.Press || _mode == UiButtonMode.Repeat)
                                {
                                    DoAction();

                                    if(_mode == UiButtonMode.Repeat)
                                    {
                                        _waitForAction = _repeatStart;
                                    }
                                }
                                else if (_mode == UiButtonMode.Delayed)
                                {
                                    _waitForAction = _delayTime;
                                }
                            }
                        }
                    }
                    break;

                case GestureType.Move:

                    if (_mode == UiButtonMode.Game)
                    {
						if (IsPointInsideView(gesture.Position, tolerance))
                        {
                            if (!_touches.ContainsKey(gesture.TouchId))
                            {
                                _touches.Add(gesture.TouchId, true);
                            }
                            SetPushed(true, true);
                        }
                        else
                        {
                            _touches.Remove(gesture.TouchId);

                            if (_touches.Count == 0 )
                            {
                                SetPushed(false, true);
                            }
                        }
                        break;
                    }
                    

                    if (_touchId == gesture.TouchId)
                    {
                        if(_holdTime.HasValue)
                        {
                            if( (gesture.Origin - gesture.Position).Length() >= TouchPad.Instance.MinDragSize )
                            {
                                _holdTime = null;
                                SetPushed(false, false);
                                break;
                            }
                        }

                        SetPushed(_checkRect.Contains(gesture.Position), true);
                    }
                    break;

                case GestureType.Up:

                    if (_mode == UiButtonMode.Game)
                    {
                        _touches.Remove(gesture.TouchId);

                        if (_touches.Count == 0)
                        {
                            SetPushed(false, true);
                        }
                        break;
                    }

                    if ( _touchId == gesture.TouchId)
                    {
                        if ( IsPushed && _mode == UiButtonMode.Release)
                        {
                            gesture.SetHandled();
                            DoAction();
                        }

                        SetPushed(false, false);
                    }
                    break;
            }
        }
        
        protected virtual void OnPushedChanged()
        {
            
        }

        void SetPushed(bool pushed, bool playSound)
        {
            if ( IsPushed != pushed)
            {
                if (playSound)
                {
                    SoundEffect sound = pushed ? _pushSound : _releaseSound;

                    if (sound != null)
                    {
                        sound.Play();
                    }
                }

                IsPushed = pushed;

                CallDelegate(pushed ? "Push" : "Release");

                OnPushedChanged();
            }

            if (!pushed)
            {
				_holdTime = null;
                _touchId = 0;
            }
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiButton));

            Icon = DefinitionResolver.GetShared<Texture2D>(Controller, Binding, file["Icon"], null);

            if(Icon==null)
            {
                Icon = new SharedValue<Texture2D>();
            }

            _gestureMargin = DefinitionResolver.Get<Margin>(Controller, Binding, file["GestureMargin"], Margin.None);

            _text = DefinitionResolver.GetSharedString(Controller, Binding, file["Text"]);

            if (_text == null)
            {
                _text = new SharedString();
            }

            if (file["Hold"] != null)
            {
                RegisterDelegate("Hold", file["Hold"]);
                _processHold = true;
            }

            RegisterDelegate("Release", file["Release"]);
            RegisterDelegate("Push", file["Push"]);

            RegisterDelegate("Click", file["Click"]);

            List<DefinitionFile> drawableFiles = file["Drawables"] as List<DefinitionFile>;

            if (file["Disabled"] != null && file["Enabled"] == null)
            {
                _enabledFlag = DefinitionResolver.GetShared(Controller, Binding, file["Disabled"], false);
                _enabledFlagInvert = true;
            }
            else
            {
                _enabledFlag = DefinitionResolver.GetShared(Controller, Binding, file["Enabled"], true);
                _enabledFlagInvert = false;
            }

            _enabledFlag.ValueChanged += OnEnabledChanged;

            if ( drawableFiles != null )
            {
                foreach (var def in drawableFiles)
                {
                    var drawable = def.CreateInstance(Controller, Binding) as ButtonDrawable;

                    if (drawable != null)
                    {
                        _drawables.Add(drawable);
                    }
                }
            }

            _pushSound = DefinitionResolver.Get<SoundEffect>(Controller, Binding, file["PushSound"], null);
            _releaseSound = DefinitionResolver.Get<SoundEffect>(Controller, Binding, file["ReleaseSound"], null);
            _actionSound = DefinitionResolver.Get<SoundEffect>(Controller, Binding, file["ActionSound"], null);

            ButtonMode = DefinitionResolver.Get<UiButtonMode>(Controller, Binding, file["Mode"], UiButtonMode.Release);

            _repeatStart = (float)DefinitionResolver.Get<int>(Controller, Binding, file["RepeatStart"], 0) / 1000f;
            _repeatInterval = (float)DefinitionResolver.Get<int>(Controller, Binding, file["RepeatInterval"], 0) / 1000f;

            return true;
        }

        void OnEnabledChanged(bool value)
        {
            if (!value)
            {
                SetPushed(false, false);
            }
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (Math.Abs(opacity) < float.Epsilon)
            {
                return;
            }

            var batch = parameters.DrawBatch;

            var drawInfo = new DrawButtonInfo();

            drawInfo.Text = _text;
            drawInfo.ButtonState = ButtonState;

            drawInfo.Target = ScreenBounds;
            drawInfo.Opacity = opacity;
            drawInfo.EllapsedTime = parameters.EllapsedTime;
            drawInfo.Icon = Icon.Value;

            for (int idx = 0; idx < _drawables.Count; ++idx)
            {
                var drawable = _drawables[idx];
                drawable.Draw(batch, drawInfo);
            }
        }

        protected virtual void DoAction()
        {
            CallDelegate("Click");

            if (_actionSound != null)
            {
                _actionSound.Play();
            }
        }
    }
}
