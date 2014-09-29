// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
using Sitana.Framework.Diagnostics;
using System;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Ui.Views
{
    public class UiButton: UiView, IGestureListener
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Text"] = parser.ParseString("Text");
            file["Click"] = parser.ParseDelegate("Click");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiButton.Drawables":
                        ParseDrawables(cn, file);
                        break;
                }
            }
        }

        protected static void ParseDrawables(XNode node, DefinitionFile file)
        {
            List<DefinitionFile> list = new List<DefinitionFile>();

            for (int idx = 0; idx < node.Nodes.Count; ++idx)
            {
                XNode childNode = node.Nodes[idx];
                DefinitionFile newFile = DefinitionFile.LoadFile(childNode);

                if (!newFile.Class.IsSubclassOf(typeof(ButtonDrawable)))
                {
                    string error = node.NodeError("Button Drawable must inherit from ButtonDrawable type.");
                    if (DefinitionParser.EnableCheckMode)
                    {
                        ConsoleEx.WriteLine(error);
                    }
                    else
                    {
                        throw new Exception(error);
                    }
                }

                list.Add(newFile);
            }

            if (file["Drawables"] != null)
            {
                string error = node.NodeError("Drawables already defined");
                if (DefinitionParser.EnableCheckMode)
                {
                    ConsoleEx.WriteLine(error);
                }
                else
                {
                    throw new Exception(error);
                }
            }
            else
            {
                file["Drawables"] = list;
            }
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

        private List<ButtonDrawable> _drawables = new List<ButtonDrawable>();

        private SharedString _text;

        public State ButtonState
        {
            get
            {
                return Enabled ? (IsPushed ? State.Pushed : State.Released) : State.Disabled;
            }
        }

        public UiButton()
        {
            Enabled = true;
        }

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

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiButton));

            _text = DefinitionResolver.GetSharedString(Controller, binding, file["Text"]);

            RegisterDelegate("Click", file["Click"]);

            List<DefinitionFile> drawableFiles = file["Drawables"] as List<DefinitionFile>;

            if ( drawableFiles != null )
            {
                foreach (var def in drawableFiles)
                {
                    ButtonDrawable drawable = def.CreateInstance(Controller, binding) as ButtonDrawable;

                    if (drawable != null)
                    {
                        _drawables.Add(drawable);
                    }
                }
            }
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            if (DisplayOpacity == 0)
            {
                return;
            }

            var batch = parameters.DrawBatch;

            for (int idx = 0; idx < _drawables.Count; ++idx)
            {
                var drawable = _drawables[idx];

                drawable.Draw(batch, ScreenBounds, DisplayOpacity, ButtonState, _text);
            }
        }

        void DoAction()
        {
            CallDelegate("Click", new InvokeParam("sender", this));
        }
    }
}
