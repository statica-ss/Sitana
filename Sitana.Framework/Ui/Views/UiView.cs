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
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views
{
    /// <summary>
    /// Parameters:
    /// Id
    /// Visible
    /// MinSize
    /// Opacity
    /// </summary>
    public abstract class UiView : IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Id"] = parser.ParseString("Id");

            string controller = node.Attribute("Controller");
            Type controllerType = null;

            if (!string.IsNullOrEmpty(controller))
            {
                controllerType = Type.GetType(controller);
                if (controllerType == null)
                {
                    throw new Exception(string.Format("Cannot find controller type: {0}.", controller));
                }
            }

            file["Controller"] = controllerType;

            file["Binding"] = parser.ParseDelegate("Binding");

            file["Modal"] = parser.ParseBoolean("Modal");

            file["Visible"] = parser.ParseBoolean("Visible");
            file["Hidden"] = parser.ParseBoolean("Hidden");

            file["BackgroundColor"] = parser.ParseColor("BackgroundColor");

            file["Opacity"] = parser.ParseDouble("Opacity");

            file["ViewRemoved"] = parser.ParseDelegate("ViewRemoved");
            file["ViewAdded"] = parser.ParseDelegate("ViewAdded");

            file["ViewActivated"] = parser.ParseDelegate("ViewActivated");
            file["ViewDeactivated"] = parser.ParseDelegate("ViewDeactivated");

            file["ViewResized"] = parser.ParseDelegate("ViewResized");

            file["MinWidth"] = parser.ParseLength("MinWidth", false);
            file["MinHeight"] = parser.ParseLength("MinHeight", false);

            file["ShowHideTime"] = parser.ParseDouble("ShowHideTime");

            file["HideTime"] = parser.ParseDouble("HideTime");
            file["ShowTime"] = parser.ParseDouble("ShowTime");

            file["Tag"] = parser.ParseString("Tag");

            PositionParameters.Parse(node, file);

            foreach (var cn in node.Nodes)
            {
                if (cn.Tag == "UiView.BackgroundDrawable")
                {
                    if (cn.Nodes.Count != 1)
                    {
                        string error = node.NodeError("UiView.BackgroundDrawable must have exactly 1 child.");
                        if (DefinitionParser.EnableCheckMode)
                        {
                            ConsoleEx.WriteLine(error);
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }

                    file["BackgroundDrawable"] = DefinitionFile.LoadFile(cn.Nodes[0]);
                }

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

                if (cn.Tag == "UiView.ParentShowTransitionEffect")
                {
                    if (cn.Nodes.Count != 1)
                    {
                        string error = node.NodeError("UiView.NavigateToTransitionEffect must have exactly 1 child.");
                        if (DefinitionParser.EnableCheckMode)
                        {
                            ConsoleEx.WriteLine(error);
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }

                    file["ParentShowTransitionEffect"] = DefinitionFile.LoadFile(cn.Nodes[0]);
                }

                if (cn.Tag == "UiView.ParentHideTransitionEffect")
                {
                    if (cn.Nodes.Count != 1)
                    {
                        string error = node.NodeError("UiView.NavigateFromTransitionEffect must have exactly 1 child.");
                        if (DefinitionParser.EnableCheckMode)
                        {
                            ConsoleEx.WriteLine(error);
                        }
                        else
                        {
                            throw new Exception(error);
                        }
                    }

                    file["ParentHideTransitionEffect"] = DefinitionFile.LoadFile(cn.Nodes[0]);
                }
            }
        }

        public static void ParseDrawables(XNode node, DefinitionFile file, Type drawableType, string targetId = "Drawables")
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

            if (file[targetId] != null)
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
                file[targetId] = list;
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
                InvalidateScreenBounds();
            }
        }

        public delegate void ViewDisplayChangedDelegate(bool visible);

        public delegate void ViewSizeChangedDelegate(Rectangle bounds);

        protected SharedValue<bool> _visiblityFlag { private get; set;}

        public GestureType EnabledGestures = GestureType.None;

        public static int DefaultShowTime = 250;
        public static int DefaultHideTime = 250;

        public virtual Point MinSize
        {
            get
            {
                return new Point(_minWidth.Compute(), _minHeight.Compute());
            }
        }

        public PositionParameters PositionParameters { get; private set; }

        Dictionary<string, object> _delegates = new Dictionary<string, object>();

        public SharedValue<double> Opacity { get; set; }
        
        UiContainer _parent = null;

        public Margin Margin { get { return PositionParameters.Margin; } set { PositionParameters.Margin = value; } }

        private UiController _controller = null;

        public IBackgroundDrawable BackgroundDrawable { get; set; }

        protected Length _minWidth;
        protected Length _minHeight;

        protected float _showSpeed;
        protected float _hideSpeed;

        protected bool _modal;

        protected Rectangle _bounds = new Rectangle();

        public object Binding { get; private set; }

        private Rectangle _lastSize = Rectangle.Empty;

        protected TransitionEffect _showTransitionEffect = null;
        protected TransitionEffect _hideTransitionEffect = null;

        internal TransitionEffect _parentShowTransitionEffect = null;
        internal TransitionEffect _parentHideTransitionEffect = null;

        protected bool _isViewDisplayed = false;

        public SharedString Tag { get; private set;}

        bool _updateController = false;
        bool _visibleIsHidden = false;

        Rectangle _screenBounds = new Rectangle();
        bool _screenBoundsInvalid = true;

        bool _wasViewDisplayed = false;

        public event ViewDisplayChangedDelegate ViewDisplayChanged;
        public event ViewSizeChangedDelegate ViewSizeChanged;

        public bool IsViewDisplayed
        {
            get
            {
                return _isViewDisplayed;
            }
        }

        public void InvalidateScreenBounds()
        {
            _screenBoundsInvalid = true;
        }

		public bool IsPointInsideView(Vector2 point, int tolerance = 0)
        {
			bool ret = false;

			if (tolerance == 0)
			{
				ret = ScreenBounds.Contains(point);
			} 
			else
			{
				Rectangle bounds = ScreenBounds;
				bounds.Inflate(tolerance, tolerance);

				ret = bounds.Contains(point);
			}

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

        public virtual float DisplayVisibility { get; protected set; }

        private ColorWrapper _backgroundColor = new ColorWrapper(Color.Transparent);
        private InvokeParameters _invokeParameters = new InvokeParameters();

        internal int OffsetBoundsVertical
        {
            set
            {
                _bounds.Y = value;
                _screenBounds = _bounds;
            }

            get
            {
                return _bounds.Y;
            }
        }

        public bool Visible
        {
            get
            {
                return _visibleIsHidden ? !_visiblityFlag.Value : _visiblityFlag.Value;
            }

            set
            {
                _visiblityFlag.Value = _visibleIsHidden ? !value : value;
            }
        }

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
                if (_screenBoundsInvalid)
                {
                    CalculateScreenBounds(out _screenBounds);
                }
                
                return _screenBounds;
            }
        }

        internal void CalculateScreenBounds(out Rectangle bounds)
        {
            if(!_screenBoundsInvalid)
            {
                bounds = _screenBounds;
            }
            else if (Parent != null)
            {
                Parent.CalculateScreenBounds(out bounds);

                bounds.X += Bounds.X;
                bounds.Y += Bounds.Y;

                bounds.Width = Bounds.Width;
                bounds.Height = Bounds.Height;
            } 
            else
            {
                bounds = Bounds;
            }
        }

        public UiView()
        {
            Bounds = Rectangle.Empty;
            PositionParameters = null;
        }

        public void ViewDraw(ref UiViewDrawParameters parameters)
        {
            _isViewDisplayed = Visible && Math.Abs(parameters.Transition) < 0.000001;

            if (DisplayVisibility == 0)
            {
                return;
            }

            TransitionEffect transitionEffect = null;
            TransitionEffect transitionEffectShowHide = null;

            switch (parameters.TransitionMode)
            {
                case TransitionMode.Show:
                    transitionEffect = _parentShowTransitionEffect;
                    break;

                case TransitionMode.Hide:
                    transitionEffect = _parentHideTransitionEffect;
                    break;

                case TransitionMode.None:
                    transitionEffectShowHide = DisplayVisibility == 1 ? null : (Visible ? _showTransitionEffect : _hideTransitionEffect);
                    break;
            }

            if (transitionEffect != null || transitionEffectShowHide != null)
            {
                UiViewDrawParameters drawParameters = parameters;

                float opacity;
                Matrix transform;

                Matrix targetTransform = Matrix.Identity;
                float targetOpacity = 1;

                if (transitionEffect != null)
                {
                    transitionEffect.Get(parameters.Transition, parameters.TransitionRectangle, ScreenBounds, out transform, out opacity);
                    targetOpacity *= opacity;
                    targetTransform *= transform;
                }

                if (transitionEffectShowHide != null)
                {
                    transitionEffectShowHide.Get(1 - DisplayVisibility, Parent != null ? Parent.ScreenBounds : ScreenBounds, ScreenBounds, out transform, out opacity);
                    targetOpacity *= opacity;
                    targetTransform *= transform;
                }

                drawParameters.DrawBatch.PushTransform(targetTransform);
                drawParameters.Opacity *= (float)Opacity.Value * targetOpacity;

                Draw(ref drawParameters);

                drawParameters.DrawBatch.PopTransform();
            }
            else
            {
                UiViewDrawParameters drawParameters = parameters;
                drawParameters.Opacity *= DisplayVisibility * (float)Opacity.Value;

                Draw(ref drawParameters);
            }
        }

        internal virtual void ResetViewDisplayed()
        {
            _isViewDisplayed = false;
        }

        internal virtual void ProcessAfterDraw()
        {
            if (_isViewDisplayed != _wasViewDisplayed)
            {
                _wasViewDisplayed = _isViewDisplayed;
                if (ViewDisplayChanged != null)
                {
                    ViewDisplayChanged(_isViewDisplayed);
                }
            }
        }

        internal void ViewUpdate(float time)
        {
            if ( _updateController && _controller != null)
            {
                _controller.UpdateInternal(time);
            }

            float opacity = Visible ? 1 : 0;

            bool visible = DisplayVisibility > 0;

            float oldDisplayVisibility = DisplayVisibility;

            if (DisplayVisibility < opacity)
            {
                DisplayVisibility += _showSpeed * time;
                DisplayVisibility = Math.Min(DisplayVisibility, opacity);
            }
            else if (DisplayVisibility > opacity)
            {
                DisplayVisibility -= _hideSpeed * time;
                DisplayVisibility = Math.Max(DisplayVisibility, opacity);
            }

            if(DisplayVisibility != oldDisplayVisibility)
            {
                AppMain.Redraw();
            }

            if (visible != DisplayVisibility > 0)
            {
                if ( Parent != null )
                {
                    Parent.RecalcLayout();
                }
            }

            if ( _lastSize != Bounds)
            {
                if (_lastSize.Height != Bounds.Height || _lastSize.Width != Bounds.Width)
                {
                    CallDelegate("ViewResized", new InvokeParam("bounds", Bounds), new InvokeParam("size", new Point(Bounds.Width, Bounds.Height)),
                        new InvokeParam("width", Bounds.Width), new InvokeParam("height", Bounds.Height));

                    OnSizeChanged();

                    if(ViewSizeChanged != null)
                    {
                        ViewSizeChanged(Bounds);
                    }
                }

                _lastSize = Bounds;

                AppMain.Redraw();
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

        internal void RegisterView()
        {
            if (!Id.IsNullOrWhiteSpace())
            {
                Controller.Register(Id, this);
            }
        }

        public void ViewAdded()
        {
            CallDelegate("ViewAdded");
            OnAdded();

            if(_modal)
            {
                TouchPad.Instance.TouchDown += ModalTouchDown;
            }
        }

        public void ViewRemoved()
        {
            if (!Id.IsNullOrWhiteSpace())
            {
                Controller.Unregister(Id, this);
            }

            CallDelegate("ViewRemoved");
            if(_updateController)
            {
                Controller.OnViewDetached();
            }
            OnRemoved();

            if (_modal)
            {
                TouchPad.Instance.TouchDown -= ModalTouchDown;
            }
        }

        protected void DrawBackground(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            Color backgroundColor = BackgroundColor * opacity;

            if (backgroundColor.A > 0)
            {
                if (BackgroundDrawable != null)
                {
                    BackgroundDrawable.Draw(parameters.DrawBatch, ScreenBounds, backgroundColor);
                }
                else
                {
                    parameters.DrawBatch.DrawRectangle(ScreenBounds, backgroundColor);
                }
            }
        }

        protected virtual void OnSizeChanged()
        {

        }

        protected virtual void Draw(ref UiViewDrawParameters parameters)
        {
            DrawBackground(ref parameters);
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
                _updateController = true;
            }
        }

        protected virtual bool Init(object controller, object binding, DefinitionFile definition)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiView));

            Type controllerType = file["Controller"] as Type;

            _controller = controller as UiController;

            if (controllerType != null)
            {
                var newController = Activator.CreateInstance(controllerType) as UiController;

                if (newController != null)
                {
                    newController.Parent = _controller;
                    Controller = newController;
                }
            }

            Binding = binding;

            object bindParameter = file["Binding"];

            if (bindParameter != null)
            {
                Object bind = DefinitionResolver.GetValueFromMethodOrField(Controller, binding, bindParameter);

                if (bind != null)
                {
                    Binding = bind;
                }
            }

            Id = DefinitionResolver.GetString(Controller, Binding, file["Id"]);

            if (file["Hidden"] != null && file["Visible"] == null)
            {
                _visiblityFlag = DefinitionResolver.GetShared<bool>(Controller, Binding, file["Hidden"], false);
                _visibleIsHidden = true;
            }
            else
            {
                _visiblityFlag = DefinitionResolver.GetShared<bool>(Controller, Binding, file["Visible"], true);
            }

            Tag = DefinitionResolver.GetSharedString(Controller, Binding, file["Tag"]);

            Opacity = DefinitionResolver.GetShared<double>(Controller, Binding, file["Opacity"], 1);

            DisplayVisibility = Visible ? 1 : 0;

            _modal = DefinitionResolver.Get<bool>(Controller, Binding, file["Modal"], false);

            RegisterDelegate("ViewRemoved", file["ViewRemoved"]);
            RegisterDelegate("ViewAdded", file["ViewAdded"]);
            RegisterDelegate("ViewActivated", file["ViewActivated"]);
            RegisterDelegate("ViewDeactivated", file["ViewDeactivated"]);
            RegisterDelegate("ViewResized", file["ViewResized"]);

            _minWidth = DefinitionResolver.Get<Length>(Controller, Binding, file["MinWidth"], Length.Zero);
            _minHeight = DefinitionResolver.Get<Length>(Controller, Binding, file["MinHeight"], Length.Zero);

            _showSpeed = (float) Math.Max(
                    DefinitionResolver.Get<double>(Controller, Binding, file["ShowHideTime"], -1),
                    DefinitionResolver.Get<double>(Controller, Binding, file["ShowTime"], -1));

            if ( _showSpeed < 0 )
            {
                _showSpeed = DefaultShowTime;
            }

            _showSpeed /= 1000.0f;

            _showSpeed = _showSpeed > 0 ? 1 / _showSpeed : float.MaxValue;

            _hideSpeed = (float) Math.Max(
                DefinitionResolver.Get<double>(Controller, Binding, file["ShowHideTime"], -1),
                DefinitionResolver.Get<double>(Controller, Binding, file["HideTime"], -1));

            if ( _hideSpeed < 0 )
            {
                _hideSpeed = DefaultHideTime;
            }

            _hideSpeed /= 1000.0f;

            _hideSpeed = _hideSpeed > 0 ? 1 / _hideSpeed : float.MaxValue;

            CreatePositionParameters(Controller, Binding, definition);

            DefinitionFile backgroundDrawable = file["BackgroundDrawable"] as DefinitionFile;

            Color defaultBackgroundColor = Color.Transparent;

            if (backgroundDrawable != null)
            {
                BackgroundDrawable = backgroundDrawable.CreateInstance(Controller, Binding) as IBackgroundDrawable;

                if (BackgroundDrawable != null)
                {
                    defaultBackgroundColor = Color.White;
                }
            }

            _backgroundColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["BackgroundColor"]) ?? new ColorWrapper(defaultBackgroundColor);

            DefinitionFile showTransitionEffectFile = file["ShowTransitionEffect"] as DefinitionFile;
            DefinitionFile hideTransitionEffectFile = file["HideTransitionEffect"] as DefinitionFile;
            DefinitionFile parentShowTransitionEffectFile = file["ParentShowTransitionEffect"] as DefinitionFile;
            DefinitionFile parentHideTransitionEffectFile = file["ParentHideTransitionEffect"] as DefinitionFile;

            if (showTransitionEffectFile != null)
            {
                _showTransitionEffect = showTransitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
            }

            if (hideTransitionEffectFile != null)
            {
                _hideTransitionEffect = hideTransitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
            }

            if (parentShowTransitionEffectFile != null)
            {
                _parentShowTransitionEffect = parentShowTransitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
            }

            if (parentHideTransitionEffectFile != null)
            {
                _parentHideTransitionEffect = parentHideTransitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
            }

            return true;
        }

        void CreatePositionParameters(UiController controller, object binding, DefinitionFile file)
        {
            PositionParameters = new PositionParameters();
            PositionParameters.Init(controller, binding, file);
        }

        bool IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            return Init(controller, binding, file);
        }

        internal void OnNeighboursInited()
        {
            if(!PositionParameters.BindHeightId.IsNullOrWhiteSpace() && Parent != null)
            {
                UiView view = Parent.FindChild(PositionParameters.BindHeightId);
                view.ViewSizeChanged += BindHeight_ViewSizeChanged;
            }

            if (!PositionParameters.BindWidthId.IsNullOrWhiteSpace() && Parent != null)
            {
                UiView view = Parent.FindChild(PositionParameters.BindWidthId);
                view.ViewSizeChanged += BindWidth_ViewSizeChanged;
            }
        }

        void BindWidth_ViewSizeChanged(Rectangle bounds)
        {
            PositionParameters.Width = new Length(pixels: bounds.Width);

            if (Bounds.Width != bounds.Width)
            {
                Parent.ShouldRecalcLayout();
            }
        }

        void BindHeight_ViewSizeChanged(Rectangle bounds)
        {
            PositionParameters.Height = new Length(pixels: bounds.Height);

            if (Bounds.Height != bounds.Height)
            {
                Parent.ShouldRecalcLayout();
            }
        }

        protected void RegisterDelegate(string id, object definition)
        {
            if (definition != null)
            {
                _delegates.Add(id, definition);
            }
        }

        protected virtual object CallDelegate(string id, params InvokeParam[] args)
        {
            _invokeParameters.Clear();

            foreach (var param in args)
            {
                _invokeParameters.Set(param);
            }

            _invokeParameters.Set(new InvokeParam("binding", Binding));
            _invokeParameters.Set(new InvokeParam("sender", this));

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

            if (size.X == 0 && PositionParameters.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                size.X = width;
            }

            if (size.Y == 0 && PositionParameters.VerticalAlignment == VerticalAlignment.Stretch)
            {
                size.Y = height;
            }

            return size;
        }

        internal virtual void ViewGesture(Gesture gesture)
        {
            if (gesture.GestureType == GestureType.CapturedByOther)
            {
                if (gesture.PointerCapturedBy != this)
                {
                    OnGesture(gesture);
                }
            }
            else
            {
                if (_isViewDisplayed)
                {
                    if (gesture.PointerCapturedBy == null || gesture.PointerCapturedBy == this)
                    {
                        if ((gesture.GestureType & EnabledGestures) != GestureType.None)
                        {
                            GestureType originalType = gesture.GestureType;
                            gesture.GestureType = gesture.GestureType & EnabledGestures;
                            OnGesture(gesture);
                            gesture.GestureType = originalType;
                        }
                    }
                }

                if (_modal && Visible)
                {
                    gesture.Skip();
                }
            }
        }

        protected virtual void OnGesture(Gesture gesture)
        {
        }

        public virtual void Move(Point offset)
        {
            Bounds = new Rectangle(Bounds.X + offset.X, Bounds.Y + offset.Y, Bounds.Width, Bounds.Height);
        }

        void ModalTouchDown(int id, Vector2 position)
        {
            if(_modal && Visible)
            {
                if(!ScreenBounds.Contains(position.ToPoint()))
                {
                    Visible = false;
                }
            }
        }
    }
}
