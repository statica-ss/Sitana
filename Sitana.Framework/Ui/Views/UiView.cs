/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (C)2013-2014 Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF SEBASTIAN SEJUD AND IS NOT TO BE 
/// RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT THE EXPRESSED WRITTEN 
/// CONSENT OF SEBASTIAN SEJUD.
/// 
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. SEBASTIAN SEJUD GRANTS
/// TO YOU (ONE SOFTWARE DEVELOPER) THE LIMITED RIGHT TO USE THIS SOFTWARE 
/// ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// essentials@sejud.com
/// sejud.com/essentials-library
/// 
///---------------------------------------------------------------------------
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
    public class UiView: IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Id"] = node.Attribute("Id");
            file["Controller"] = Type.GetType(node.Attribute("Controller"));

            file["Visible"] = parser.ParseBoolean("Visible", true);
            file["BackgroundColor"] = parser.ParseColor("BackgroundColor");

            file["Opacity"] = parser.ParseInt("Opacity", 100);
        }

        delegate void NoArgsVoid();

        enum Delegates
        {
            OnAdded,
            OnRemoved,
            OnActivated,
            OnDeactivated
        }

        public string Id { get; set; }

        public virtual Rectangle Bounds { get; set; }

        public Boolean Visible { get; set; }

        public virtual Point MinSize {get; protected set;}

        public PositionParameters PositionParameters { get; private set; }

        DelegatesMap _delegates;

        public float Opacity { get; set; }
        
        UiContainer _parent = null;

        public Margin Margin { get { return PositionParameters.Margin; } set { PositionParameters.Margin = value; } }

        private UiController _controller = null;

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

        void CallDelegate(Enum id)
        {
            var del = GetDelegate<NoArgsVoid>(id);
            if (del != null) del();
        }

        internal void ViewActivated()
        {
            CallDelegate(Delegates.OnActivated);
        }

        internal void ViewDeactivated()
        {
            CallDelegate(Delegates.OnDeactivated);
        }

        public void ViewAdded()
        {
            CallDelegate(Delegates.OnAdded);
            OnAdded();
        }

        public void ViewRemoved()
        {
            CallDelegate(Delegates.OnRemoved);
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

        protected void RegisterDelegate(Enum id, Type delegateType, string methodName)
        {
            if (_delegates == null) _delegates = new DelegatesMap();
            _delegates.RegisterDelegate(id, delegateType, methodName);
        }

        protected DELEGATE GetDelegate<DELEGATE>(Enum id)
        {
            if (_delegates == null) return default(DELEGATE);
            return (DELEGATE)(object)_delegates.FindMethod(id, Controller);
        }

        protected virtual void Init(ref object context, DefinitionFile file)
        {
            Type controllerType = file["Controller"] as Type;

            if ( controllerType != null )
            {
                var controller = Activator.CreateInstance(controllerType) as UiController;

                if (controller != null)
                {
                    controller.AttachView(this);
                    Controller = controller;
                    context = controller;
                }
            }

            Id = (string)file["Id"];
            Visible = DefinitionResolver.GetBoolean(context, file["Visible"]);

            int opacity = DefinitionResolver.Get<int>(context, file["Opacity"]);
            Opacity = (float)opacity / 100.0f;

            if ( Visible )
            {
                DisplayOpacity = Opacity;
            }

            BackgroundColor = DefinitionResolver.GetColor(context, file["BackgroundColor"]) ?? Color.Transparent;
        }

        public void CreatePositionParameters(object context, DefinitionFile file, Type type)
        {
            PositionParameters = (PositionParameters)Activator.CreateInstance(type);

            if ( file.AdditionalParameters.TryGetValue(type, out file))
            {
                (PositionParameters as IDefinitionClass).Init(context, file);
            }
        }

        void IDefinitionClass.Init(object context, DefinitionFile file)
        {
            Init(ref context, file);
        }
    }
}
