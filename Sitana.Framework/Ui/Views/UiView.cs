﻿// SITANA - Copyright (C) The Sitana Team.
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
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
using Sitana.Framework.Diagnostics;

namespace Sitana.Framework.Ui.Views
{
    /// <summary>
    /// Parameters:
    /// Id
    /// Visible
    /// MinSize
    /// Opacity
    /// </summary>
    public abstract class UiView: IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Id"] = node.Attribute("Id");
            file["Controller"] = Type.GetType(node.Attribute("Controller"));

            file["Visible"] = parser.ParseBoolean("Visible", true);
            file["BackgroundColor"] = parser.ParseColor("BackgroundColor");

            file["Opacity"] = parser.ParseInt("Opacity", 100);

            file["ViewRemoved"] = parser.ParseDelegate("ViewRemoved");
            file["ViewAdded"] = parser.ParseDelegate("ViewAdded");
        }

        public string Id { get; set; }

        public virtual Rectangle Bounds { get; set; }

        public Boolean Visible { get; set; }

        public virtual Point MinSize {get; protected set;}

        public PositionParameters PositionParameters { get; private set; }

        Dictionary<string, object> _delegates = new Dictionary<string, object>();

        public float Opacity { get; set; }
        
        UiContainer _parent = null;

        public Margin Margin { get { return PositionParameters.Margin; } set { PositionParameters.Margin = value; } }

        private UiController _controller = null;

        public object Binding { get; private set; }

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
            MinSize = new Point(10, 10);
            PositionParameters = null;
        }

        internal void ViewDraw(ref UiViewDrawParameters parameters)
        {
            Draw(ref parameters);
        }

        internal void ViewUpdate(float time)
        {
            float opacity = Visible ? Opacity : 0;
            DisplayOpacity = time * 4 * opacity + (1 - time * 4) * DisplayOpacity;

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
            CallDelegate("ViewAdded");
            OnAdded();
        }

        public void ViewRemoved()
        {
            CallDelegate("ViewRemoved");
            OnRemoved();
        }

        protected virtual void Draw(ref UiViewDrawParameters parameters)
        {
            if (DisplayOpacity == 0 || BackgroundColor.A == 0)
            {
                return;
            }

            parameters.DrawBatch.DrawRectangle(ScreenBounds, BackgroundColor * DisplayOpacity);
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

        protected void Init(ref UiController controller, object binding, DefinitionFile file)
        {
            Type controllerType = file["Controller"] as Type;

            if (controllerType != null)
            {
                var newController = Activator.CreateInstance(controllerType) as UiController;

                if (newController != null)
                {
                    newController.AttachView(this);
                    Controller = newController;
                    controller = newController;
                }
            }

            Id = (string)file["Id"];
            Visible = DefinitionResolver.GetBoolean(controller, binding, file["Visible"]);

            int opacity = DefinitionResolver.Get<int>(controller, binding, file["Opacity"]);
            Opacity = (float)opacity / 100.0f;

            if (Visible)
            {
                DisplayOpacity = Opacity;
            }

            BackgroundColor = DefinitionResolver.GetColor(controller, binding, file["BackgroundColor"]) ?? Color.Transparent;
            RegisterDelegate("ViewRemoved", file["ViewRemoved"]);
            RegisterDelegate("ViewAdded", file["ViewAdded"]);
        }

        protected abstract void Init(UiController controller, object binding, DefinitionFile file);

        public void CreatePositionParameters(UiController controller, object binding, DefinitionFile file, Type type)
        {
            PositionParameters = (PositionParameters)Activator.CreateInstance(type);

            if ( file.AdditionalParameters.TryGetValue(type, out file))
            {
                (PositionParameters as IDefinitionClass).Init(controller, binding, file);
            }
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
    }
}
