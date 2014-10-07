// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Sitana.Framework.Input;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Controllers;
using System.Collections.Generic;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Xml;
using Sitana.Framework.Input.TouchPad;

namespace Sitana.Framework.Ui.Views
{
    /// <summary>
    /// Parameters:
    /// Id
    /// Visible
    /// MinSize
    /// Opacity
    /// </summary>
    public abstract class UiView : IDefinitionClass, IGestureListener
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Id"] = node.Attribute("Id");
            file["Controller"] = Type.GetType(node.Attribute("Controller"));

            file["Visible"] = parser.ParseBoolean("Visible");
            file["BackgroundColor"] = parser.ParseColor("BackgroundColor");

            file["Opacity"] = parser.ParseInt("Opacity");

            file["ViewRemoved"] = parser.ParseDelegate("ViewRemoved");
            file["ViewAdded"] = parser.ParseDelegate("ViewAdded");

            file["ViewActivated"] = parser.ParseDelegate("ViewActivated");
            file["ViewDeactivated"] = parser.ParseDelegate("ViewDeactivated");

            file["ViewResized"] = parser.ParseDelegate("ViewResized");

            file["MinWidth"] = parser.ParseLength("MinWidth", false);
            file["MinHeight"] = parser.ParseLength("MinHeight", false);

            PositionParameters.Parse(node, file);

            foreach (var cn in node.Nodes)
            {
                if (cn.Tag == "UiView.ShowTransitionEffect")
                {
                    if (cn.Nodes.Count != 1)
                    {
                        string error = node.NodeError("UiView.ShowTransitionEffect must have exactly 1 child.");
                        if (DefinitionParser.EnableCheckMode)
                        {
                            ConsoleEx.WriteLine(error);
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }

                    file["ShowTransitionEffect"] = DefinitionFile.LoadFile(cn.Nodes[0]);
                }

                if (cn.Tag == "UiView.HideTransitionEffect")
                {
                    if (cn.Nodes.Count != 1)
                    {
                        string error = node.NodeError("UiView.HideTransitionEffect must have exactly 1 child.");
                        if (DefinitionParser.EnableCheckMode)
                        {
                            ConsoleEx.WriteLine(error);
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }

                    file["HideTransitionEffect"] = DefinitionFile.LoadFile(cn.Nodes[0]);
                }
            }
        }

        protected static void ParseDrawables(XNode node, DefinitionFile file, Type drawableType)
        {
            List<DefinitionFile> list = new List<DefinitionFile>();

            for (int idx = 0; idx < node.Nodes.Count; ++idx)
            {
                XNode childNode = node.Nodes[idx];
                DefinitionFile newFile = DefinitionFile.LoadFile(childNode);

                if (!newFile.Class.IsSubclassOf(drawableType))
                {
                    string error = node.NodeError(String.Format("Drawable must inherit from {0} type.", drawableType.Name));
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

        public string Id { get; set; }

        public virtual Rectangle Bounds
        {
            get
            {
                return _bounds;
            }

            set
            {
                _bounds = value;
                _enableGestureHandling = false;
            }
        }

        public Boolean Visible { get; set; }

        public virtual Point MinSize
        {
            get
            {
                return new Point(_minWidth.Compute(), _minHeight.Compute());
            }
        }

        public PositionParameters PositionParameters { get; private set; }

        Dictionary<string, object> _delegates = new Dictionary<string, object>();

        public float Opacity { get; set; }
        
        UiContainer _parent = null;

        public Margin Margin { get { return PositionParameters.Margin; } set { PositionParameters.Margin = value; } }

        private UiController _controller = null;

        protected Length _minWidth;
        protected Length _minHeight;

        protected Rectangle _bounds = new Rectangle();

        public object Binding { get; private set; }

        private Rectangle _lastSize = Rectangle.Empty;

        protected TransitionEffect _showTransitionEffect = null;
        protected TransitionEffect _hideTransitionEffect = null;

        protected bool _enableGestureHandling = false;

        public bool IsPointInsideView(Vector2 point)
        {
            bool ret = ScreenBounds.Contains(point);

            if (ret && Parent != null)
            {
                return Parent.IsPointInsideView(point);
            }

            return ret;
        }

        public bool IsPointInsideView(Point point)
        {
            bool ret = ScreenBounds.Contains(point);

            if (ret && Parent != null)
            {
                return Parent.IsPointInsideView(point);
            }

            return ret;
        }

        public virtual UiContainer Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                if (_parent != value)
                {
                    if (_parent != null)
                    {
                        _parent.Remove(this);
                    }

                    _parent = value;

                    if (_parent != null)
                    {
                        _parent.Add(this);
                    }
                }
            }
        }

        protected float DisplayOpacity { get; private set; }

        private ColorWrapper _backgroundColor = new ColorWrapper(Color.Transparent);
        private InvokeParameters _invokeParameters = new InvokeParameters();

        public virtual Color BackgroundColor
        {
            get
            {
                return _backgroundColor.Value;
            }

            set
            {
                _backgroundColor.Value = value;
            }
        }

        public Rectangle ScreenBounds
        {
            get
            {
                if (Parent != null)
                {
                    return new Rectangle(Parent.ScreenBounds.X + Bounds.X, Parent.ScreenBounds.Y + Bounds.Y, Bounds.Width, Bounds.Height);
                }

                return Bounds;
            }
        }

        public UiView()
        {
            Bounds = Rectangle.Empty;
            PositionParameters = null;
        }

        internal void ViewDraw(ref UiViewDrawParameters parameters)
        {
            _enableGestureHandling = Visible && parameters.Transition == 0;

            TransitionEffect transitionEffect = parameters.TransitionModeHide ? _hideTransitionEffect : _showTransitionEffect;

            if (transitionEffect != null)
            {
                UiViewDrawParameters drawParameters = parameters;

                float opacity;
                Matrix transform;

                transitionEffect.Get(drawParameters.Transition, parameters.TransitionRectangle, ScreenBounds, out transform, out opacity);

                drawParameters.Opacity *= opacity;

                drawParameters.DrawBatch.PushTransform(transform);

                Draw(ref drawParameters);

                drawParameters.DrawBatch.PopTransform();
            }
            else
            {
                Draw(ref parameters);
            }
        }

        internal void ViewUpdate(float time)
        {
            float opacity = Visible ? Opacity : 0;
            DisplayOpacity = time * 4 * opacity + (1 - time * 4) * DisplayOpacity;

            if ( _lastSize != Bounds)
            {
                CallDelegate("ViewResized", new InvokeParam("bounds", Bounds), new InvokeParam("size", new Point(Bounds.Width, Bounds.Height)),
                    new InvokeParam("width", Bounds.Width), new InvokeParam("height", Bounds.Height));

                _lastSize = Bounds;
            }

            Update(time);
        }

        internal void ViewActivated()
        {
            CallDelegate("ViewActivated");
        }

        internal void ViewDeactivated()
        {
            CallDelegate("ViewDeactivated");
        }

        public void ViewAdded()
        {
            if (!Id.IsNullOrWhiteSpace())
            {
                Controller.Register(Id, this);
            }
            CallDelegate("ViewAdded");
            OnAdded();
        }

        public void ViewRemoved()
        {
            if (!Id.IsNullOrWhiteSpace())
            {
                Controller.Unregister(Id, this);
            }

            CallDelegate("ViewRemoved");
            OnRemoved();
        }

        protected virtual void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = DisplayOpacity * parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            Color backgroundColor = BackgroundColor * opacity;

            if (backgroundColor.A > 0)
            {
                parameters.DrawBatch.Texture = null;
                parameters.DrawBatch.DrawRectangle(ScreenBounds, backgroundColor);
            }
        }

        protected virtual void Update(float time)
        {
        }

        protected virtual void OnAdded()
        {
        }

        protected virtual void OnRemoved()
        {
        }

        public UiController Controller
        { 
            get
            {
                if ( _controller == null && Parent != null )
                {
                    return Parent.Controller;
                }

                return _controller;
            }

            set
            {
                _controller = value;
                _controller.AttachView(this);
            }
        }

        protected virtual void Init(object controller, object binding, DefinitionFile definition)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiView));

            Type controllerType = file["Controller"] as Type;

            _controller = controller as UiController;

            if (controllerType != null)
            {
                var newController = Activator.CreateInstance(controllerType) as UiController;

                if (newController != null)
                {
                    newController.AttachView(this);
                    Controller = newController;
                }
            }

            Binding = binding;

            Id = (string)file["Id"];
            Visible = DefinitionResolver.Get<bool>(Controller, binding, file["Visible"], true);

            int opacity = DefinitionResolver.Get<int>(Controller, binding, file["Opacity"], 100);
            Opacity = (float)opacity / 100.0f;

            if (Visible)
            {
                DisplayOpacity = Opacity;
            }

            BackgroundColor = DefinitionResolver.GetColor(Controller, binding, file["BackgroundColor"]) ?? Color.Transparent;

            RegisterDelegate("ViewRemoved", file["ViewRemoved"]);
            RegisterDelegate("ViewAdded", file["ViewAdded"]);
            RegisterDelegate("ViewActivated", file["ViewActivated"]);
            RegisterDelegate("ViewDeactivated", file["ViewDeactivated"]);
            RegisterDelegate("ViewResized", file["ViewResized"]);

            _minWidth = DefinitionResolver.Get<Length>(Controller, binding, file["MinWidth"], Length.Zero);
            _minHeight = DefinitionResolver.Get<Length>(Controller, binding, file["MinHeight"], Length.Zero);

            CreatePositionParameters(Controller, binding, definition);

            DefinitionFile showTransitionEffectFile = file["ShowTransitionEffect"] as DefinitionFile;
            DefinitionFile hideTransitionEffectFile = file["HideTransitionEffect"] as DefinitionFile;

            if (showTransitionEffectFile != null)
            {
                _showTransitionEffect = showTransitionEffectFile.CreateInstance(Controller, binding) as TransitionEffect;
            }

            if (hideTransitionEffectFile != null)
            {
                _hideTransitionEffect = hideTransitionEffectFile.CreateInstance(Controller, binding) as TransitionEffect;
            }
        }

        void CreatePositionParameters(UiController controller, object binding, DefinitionFile file)
        {
            PositionParameters = new PositionParameters();
            PositionParameters.Init(controller, binding, file);
        }

        void IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            Init(controller, binding, file);
        }

        protected void RegisterDelegate(string id, object definition)
        {
            if (definition != null)
            {
                _delegates.Add(id, definition);
            }
        }

        protected object CallDelegate(string id, params InvokeParam[] args)
        {
            _invokeParameters.Clear();

            foreach (var param in args)
            {
                _invokeParameters.Set(param);
            }

            _invokeParameters.Set(new InvokeParam("binding", Binding));

            object definition;

            if (_delegates.TryGetValue(id, out definition))
            {
                return DefinitionResolver.InvokeMethod(Controller, Binding, _delegates[id], _invokeParameters);
            }

            return null;
        }

        protected bool HasDelegate(string id)
        {
            return _delegates.ContainsKey(id);
        }

        protected T CallDelegate<T>(string id, params InvokeParam[] args)
        {
            object result = CallDelegate(id, args);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public virtual Point ComputeSize(int width, int height)
        {
            var size = new Point(PositionParameters.Width.Compute(width-PositionParameters.Margin.Width), PositionParameters.Height.Compute(height-PositionParameters.Margin.Height));

            if (size.X == 0 && PositionParameters.Align.HasFlag(Align.StretchHorz))
            {
                size.X = width;
            }

            if (size.Y == 0 && PositionParameters.Align.HasFlag(Align.StretchVert))
            {
                size.Y = height;
            }

            return size;
        }

        void IGestureListener.OnGesture(Gesture gesture)
        {
            if (_enableGestureHandling)
            {
                OnGesture(gesture);
            }
        }

        protected virtual void OnGesture(Gesture gesture)
        {
        }

        public virtual void Move(Point offset)
        {
            Bounds = new Rectangle(Bounds.X + offset.X, Bounds.Y + offset.Y, Bounds.Width, Bounds.Height);
            _enableGestureHandling = false;
        }

        
    }
}
