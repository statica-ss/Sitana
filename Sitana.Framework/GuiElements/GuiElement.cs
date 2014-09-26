/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos;
using Ebatianos.Content;
using Ebatianos.Cs;
using Ebatianos.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;

namespace Ebatianos.Gui
{
    public class GuiElement
    {
        protected class InitializeParams
        {
            public XmlFileNode Node;
            public Dictionary<String, String> NamespaceAliases;
            public ParametersCollection Parameters;
            public String Directory;
            public Vector2 Scale;
            public Vector2 AreaSize;
            public Vector2 Offset;
            public Screen Owner;
        }

        public String Id { get; set; }

        public Object Tag { get; set; }

        public TransitionType TransitionIn { get; private set; }
        public TransitionType TransitionOut { get; private set; }

        public Single TransitionSizeIn { protected get; set; }
        public Single TransitionSizeOut { protected get; set; }

        public Boolean TopScreenUpdate { get; protected set; }
        public Boolean UsesSpriteBatch {get; protected set;}

        public Vector2 Scale = Vector2.One;
        public Rectangle ElementRectangle { get; protected set; }

        public Boolean Visible { get; set; }

        public Boolean AdditiveDraw { get; protected set; }

        public String Instance { get; private set; }

        private Single _opacity = 1;
        private Single _visiblityOpacity = 1;

        private Single _transitionOpacityPower = 0;

        public Single Opacity
        {
            get
            {
                return _visiblityOpacity * _opacity;
            }
        }

        private Single _disabledOpacity = 0.5f;

        protected Screen Owner { get; private set; }

        public Boolean Enabled { get; set; }

        public GuiElement FirstInstance { get; internal set; }
        public GuiElement SecondInstance { get; internal set; }

        public Boolean  ClipToElement {get; protected set;}

        protected Int32 Order { get; set; }

        public Single ShowHideSpeed { private get; set; }

        public virtual Pair<Int32, Int32> DrawLevels
        {
            get
            {
                Int32 def = (Order * 2) + (AdditiveDraw ? 0 : 1);
                return new Pair<Int32, Int32>(def, def);
            }
        }

        protected GuiElement()
        {
            TransitionIn = TransitionType.None;
            TransitionOut = TransitionType.None;
            TopScreenUpdate = true;
            ElementRectangle = new Rectangle(0, 0, 0, 0);
            Visible = true;
            ShowHideSpeed = 4;
            Enabled = true;
            UsesSpriteBatch = true;
        }

        protected Boolean DrawLevel(Int32 level)
        {
            return AdditiveDraw ? level == Order * 2 : level == Order * 2 + 1;
        }

        protected ParametersCollection PrepareParametersCollection(ParametersCollection collection)
        {
            ParametersCollection parameters = collection;

            Boolean parametersChanged = false;

            for (Int32 idx = 1; idx < 10; ++idx)
            {
                String groupIndex = idx == 1 ? "GroupId" : "GroupId" + idx.ToString();
                String groupId = parameters.AsString( groupIndex);

                if (groupId != "")
                {
                    if (!parametersChanged)
                    { 
                        parameters = new ParametersCollection(parameters.ValueSource, true);
                        parametersChanged = true;
                        parameters.Add(collection);
                        parameters.MethodCallIndexArgument = collection.MethodCallIndexArgument;
                        parameters.ColorsManager = collection.ColorsManager;
                    }

                    try
                    {
                        ParametersCollection paramsCollection = Owner.Parameters(groupId);
                        parameters.Add(paramsCollection);
                        parameters.Remove(groupIndex);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Undefined group: " + groupId);
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        /// Initializes element from parameters.
        /// </summary>
        /// <param name="node">XML node entity.</param>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="scale">Current screen scale.</param>
        /// <param name="areaSize">Size of the area.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        public Boolean Init(XmlFileNode node, Dictionary<String, String> namespaceAliases, String directory, Vector2 scale, Vector2 areaSize, Screen owner)
        {
            Owner = owner;

            ParametersCollection parameters = PrepareParametersCollection(node.Attributes);

            String containerId = parameters.AsString("Container");

            Vector2 offset = Vector2.Zero;

            if (!String.IsNullOrEmpty(containerId))
            {
                var container = owner.Find<ControlsContainer>(containerId);

                if (container != null)
                {
                    offset = GraphicsHelper.Vector2FromPoint(container.ElementRectangle.Location);
                    areaSize = new Vector2(container.ElementRectangle.Width, container.ElementRectangle.Height);
                }
            }

            return Initialize( 
                new InitializeParams()
                {
                    Node = node,
                    NamespaceAliases = namespaceAliases,
                    Parameters = parameters,
                    Directory = directory,
                    Scale = scale,
                    AreaSize = areaSize,
                    Offset = offset,
                    Owner = owner
                });
        }

        public virtual GuiElement Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes element from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected virtual Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            Vector2 scale = initParams.Scale;
            Screen owner = initParams.Owner;

            Scale = scale;
            TransitionIn = TransitionType.None;
            TransitionOut = TransitionType.None;

            Id = parameters.AsString("Id");
            Instance = parameters.AsString("Instance").ToUpperInvariant();

            Tag = parameters.AsObject<Object>("Tag");

            if (parameters.HasKey("Transition"))
            {
                TransitionIn = TransitionOut = parameters.AsEnum<TransitionType>("Transition", TransitionType.None);
            }
            else
            {
                TransitionIn = parameters.AsEnum<TransitionType>("TransitionIn", TransitionType.None);
                TransitionOut = parameters.AsEnum<TransitionType>("TransitionOut", TransitionType.None);

                if (owner.InvertTransitions)
                {
                    TransitionType tin = TransitionOut;
                    TransitionOut = TransitionIn;
                    TransitionIn = tin;
                }
            }

            _transitionOpacityPower = parameters.AsSingle("TransitionOpacityPower");

            if (parameters.HasKey("Opacity"))
            {
                _opacity = parameters.AsSingle("Opacity");
            }

            if (parameters.HasKey("ShowHideTime"))
            {
                Single showHideTime = parameters.AsSingle("ShowHideTime") / 1000.0f;

                if (showHideTime > 0)
                {
                    ShowHideSpeed = 1 / showHideTime;
                }
                else
                {
                    ShowHideSpeed = 1000;
                }
            }


            if ( !parameters.AsBoolean("Visible", true) )
            {
                Visible = false;
                _visiblityOpacity = 0;
            }

            if ( !parameters.AsBoolean("Enabled", true) )
            {
                Enabled = false;
            }

            if ( parameters.HasKey("DisabledOpacity") )
            {
                _disabledOpacity = parameters.AsSingle("DisabledOpacity");
            }

            switch (parameters.AsString("Order").ToUpperInvariant())
            {
                case "BACKGROUND":
                    Order = 0;
                    break;

                case "CONTENT":
                    Order = 1;
                    break;

                case "OVERLAY":
                    Order = 2;
                    break;

                default:
                    Order = 1;
                    break;
            }

            AdditiveDraw = parameters.AsBoolean("AdditiveDraw");

            return true;
        }

        protected virtual void OnCloned(GuiElement source)
        {
            Scale = source.Scale;
            TransitionIn = source.TransitionIn;
            TransitionOut = source.TransitionOut;
            TransitionSizeIn = source.TransitionSizeIn;
            TransitionSizeOut = source.TransitionSizeOut;
            Instance = source.Instance;
            Order = source.Order;
            Owner = source.Owner;
            AdditiveDraw = source.AdditiveDraw;
            ElementRectangle = source.ElementRectangle;
            ClipToElement = source.ClipToElement;
        }

        /// <summary>
        /// Computes rectangle of element from given coordinates and alignment.
        /// </summary>
        /// <param name="position">Anchor point.</param>
        /// <param name="size">Size of element.</param>
        /// <param name="align">Alignment of element.</param>
        /// <returns></returns>
        public static Rectangle RectangleFromAlignAndSize(Point position, Point size, Align align, Vector2 offset)
        {
            // Separate placement flags.
            Align center = align & Align.Center;
            Align right = align & Align.Right;
            Align vcenter = align & Align.Middle;
            Align bottom = align & Align.Bottom;

            // Compute position if button is to be centered.
            if (center == Align.Center)
            {
                position.X -= size.X / 2;
            }
            // Compute position if button is to be aligned right.
            else if (right == Align.Right)
            {
                position.X -= size.X;
            }

            // Compute position if button is to be centerd vertically.
            if (vcenter == Align.Middle)
            {
                position.Y -= size.Y / 2;
            }
            // COmpute position if button is to be aligned bottom.
            else if (bottom == Align.Bottom)
            {
                position.Y -= size.Y;
            }

            return new Rectangle((Int32)(position.X + offset.X), (Int32)(position.Y + offset.Y), size.X, size.Y);
        }

        public static Boolean IsAlign(Align align, Align desired)
        {
            return (align & desired) == desired;
        }

        /// <summary>
        /// Draws GuiElement.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch object used to render textures and texts.</param>
        /// <param name="color">Color to multiply all contents by.</param>
        /// <param name="offset">Offset to move element by.</param>
        public virtual void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
        }

        /// <summary>
        /// Updates element state.
        /// </summary>
        public virtual Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = false;

            Single desiredOpacity = Visible ? (Enabled ? 1 : _disabledOpacity) : 0;

            if ( desiredOpacity != _visiblityOpacity )
            {
                if ( desiredOpacity > _visiblityOpacity )
                {
                    _visiblityOpacity += (Single)gameTime.TotalSeconds * ShowHideSpeed;
                    _visiblityOpacity = Math.Min(desiredOpacity, Opacity);
                    redraw = true;
                }
                
                if ( desiredOpacity < _visiblityOpacity )
                {
                    _visiblityOpacity -= (Single)gameTime.TotalSeconds * ShowHideSpeed;
                    _visiblityOpacity = Math.Max(desiredOpacity, _visiblityOpacity);
                    redraw = true;
                }
            }

            return redraw;
        }

        /// <summary>
        /// Called when element is removed either by screen or whole owner screen is removed.
        /// </summary>
        public virtual void OnRemoved()
        {
            Owner.ScreenManager.RemoveInstance(this);
        }

        public virtual void OnAdded()
        {
            Owner.ScreenManager.AddInstance(this);
        }

        public Vector2 ComputeOffsetWithTransition(Single transition)
        {
            Screen.ScreenState screenState = Owner.State;

            Vector2 offset = Vector2.Zero;

            TransitionType type = (screenState == Screen.ScreenState.TransitionIn) ? TransitionIn : TransitionOut;
            Single transitionSize = (screenState == Screen.ScreenState.TransitionIn) ? TransitionSizeIn : TransitionSizeOut;

            switch (type)
            {
                case TransitionType.Bottom:
                    offset = new Vector2(0, (1 - transition) * (Single)transitionSize * 2.5f);
                    break;

                case TransitionType.Top:
                    offset = new Vector2(0, -(1 - transition) * (Single)transitionSize * 2.5f);
                    break;

                case TransitionType.Right:
                    offset = new Vector2((1 - transition) * (Single)transitionSize * 2.5f, 0);
                    break;

                case TransitionType.Left:
                    offset = new Vector2(-(1 - transition) * (Single)transitionSize * 2.5f, 0);
                    break;

                case TransitionType.SlideBottom:
                    offset = new Vector2(0, (1 - transition) * (Single)Owner.AreaSize.Y);
                    break;

                case TransitionType.SlideTop:
                    offset = new Vector2(0, -(1 - transition) * (Single)Owner.AreaSize.Y);
                    break;

                case TransitionType.SlideRight:
                    offset = new Vector2((1 - transition) * (Single)Owner.AreaSize.X, 0);
                    break;

                case TransitionType.SlideLeft:
                    offset = new Vector2(-(1 - transition) * (Single)Owner.AreaSize.X, 0);
                    break;
            }

            return offset;
        }

        protected Color ComputeColorWithTransition(Single transition, Color color)
        {
            Screen.ScreenState screenState = Owner.State;
            TransitionType type = screenState == Screen.ScreenState.TransitionIn ? TransitionIn : TransitionOut;

            switch (type)
            {
                case TransitionType.Always:
                    break;

                case TransitionType.Fade:
                    {
                        Single trans = (Single)Math.Max(0, (transition - 0.75) * 4);
                        color *= trans;
                    }
                    break;

                case TransitionType.AlphaBlend:
                    {
                        Single trans = (Single)Math.Max(0, (transition - 0.5) * 2);
                        trans = trans * trans;

                        color *= trans;
                    }

                    break;

            case TransitionType.SlideBottom:
            case TransitionType.SlideTop:
            case TransitionType.SlideRight:
            case TransitionType.SlideLeft:
                break;

            case TransitionType.None:
                {
                    Single trans = (Single)Math.Max(0, (transition - 0.9) * 10);
                    color *= trans;
                }
                break;

                default:
                    {
                        Single trans = (Single)Math.Max(0, (transition - 0.5) * 2);
                        color *= trans;
                    }
                    break;
            }

            return color * (Single)Math.Pow(transition, _transitionOpacityPower);
        }

        protected Point FindPosition(ParametersCollection parameters, Point areaSize, Vector2 scale)
        {
            Point position = ParsePosition(parameters, "X", "Y", areaSize, scale);

            if (parameters.HasKey("X1") && parameters.HasKey("X2"))
            {
                Point position1 = ParsePosition(parameters, "X1", "Y", areaSize, scale);
                Point position2 = ParsePosition(parameters, "X2", "Y", areaSize, scale);

                position.X = (position1.X + position2.X) / 2;
            }

            if (parameters.HasKey("Y1") && parameters.HasKey("Y2"))
            {
                Point position1 = ParsePosition(parameters, "X", "Y1", areaSize, scale);
                Point position2 = ParsePosition(parameters, "X", "Y2", areaSize, scale);

                position.Y = (position1.Y + position2.Y) / 2;
            }

            return position;
        }

        public static Rectangle FindElementRectangle(ParametersCollection parameters, Point areaSize, Vector2 scale, Vector2 offset)
        {
            Point position = ParsePosition(parameters, "X", "Y", areaSize, scale);

            Align totalAlign = Align.Left;

            Int32 width = 0;
            Int32 height = 0;

            if (parameters.HasKey("X1") && parameters.HasKey("X2"))
            {
                Point position1 = ParsePosition(parameters, "X1", "Y", areaSize, scale);
                Point position2 = ParsePosition(parameters, "X2", "Y", areaSize, scale);

                position.X = position1.X;
                width = position2.X - position1.X;
            }
            else
            {
                Align align = parameters.AsAlign("Align", String.Empty, Align.Left);
                width = (Int32)(parameters.AsInt32("Width") * scale.X);

                totalAlign |= align;
            }

            if (parameters.HasKey("Y1") && parameters.HasKey("Y2"))
            {
                Point position1 = ParsePosition(parameters, "X", "Y1", areaSize, scale);
                Point position2 = ParsePosition(parameters, "X", "Y2", areaSize, scale);

                position.Y = position1.Y;
                height = position2.Y - position1.Y;
            }
            else
            {
                Align valign = parameters.AsAlign(String.Empty, "Valign", Align.Top);
                height = (Int32)(parameters.AsInt32("Height") * scale.Y);

                totalAlign |= valign;
            }

            return RectangleFromAlignAndSize(position, new Point(width, height), totalAlign, offset);
        }

        public static Vector2 ParsePosition(ParametersCollection collection, String xid, String yid, Vector2 areaSize, Vector2 scale)
        {
            return GraphicsHelper.Vector2FromPoint(
               ParsePosition(collection, xid, yid, GraphicsHelper.PointFromVector2(areaSize), scale)
            );
        }

        public static Point ParsePosition(ParametersCollection collection, String xid, String yid, Point areaSize, Vector2 scale)
        {
            String xstr = collection.AsString(xid);
            String ystr = collection.AsString(yid);

            Boolean centerX = false;
            Boolean centerY = false;

            if (xstr.StartsWith("C"))
            {
                xstr = xstr.Substring(1);
                centerX = true;

                if (xstr.StartsWith("+"))
                {
                    xstr = xstr.Substring(1);
                }

                if (xstr == "")
                {
                    xstr = "0";
                }
            }

            if (ystr.StartsWith("C"))
            {
                ystr = ystr.Substring(1);
                centerY = true;

                if (ystr.StartsWith("+"))
                {
                    ystr = ystr.Substring(1);
                }

                if (ystr == "")
                {
                    ystr = "0";
                }
            }

            Boolean fromRight = false;
            Boolean fromBottom = false;

            if (xstr.StartsWith("-"))
            {
                fromRight = true;
            }

            if (ystr.StartsWith("-"))
            {
                fromBottom = true;
            }

            Int32 posX;
            Int32 posY;

            Int32.TryParse(xstr, out posX);
            Int32.TryParse(ystr, out posY);

            posX = (Int32)(posX * scale.X);
            posY = (Int32)(posY * scale.Y);

            if (centerX)
            {
                posX = areaSize.X / 2 + posX;
            }
            else if (fromRight)
            {
                posX = areaSize.X + posX;
            }

            if (centerY)
            {
                posY = areaSize.Y / 2 + posY;
            }
            else if (fromBottom)
            {
                posY = areaSize.Y + posY;
            }

            return new Point(posX, posY);
        }

        public void NoTransitionAtExit(Boolean fade)
        {
            TransitionOut = fade ? TransitionType.Fade : TransitionType.Always;
        }

        public static GuiElement CreateElement(XmlFileNode node, Dictionary<String, String> namespaceAliases, String directory, Vector2 scale, Vector2 areaSize, Screen owner)
        {
            String className = node.Tag;
            ParametersCollection parameters = node.Attributes;

            // Get class name parts (namespace.class)
            String[] classNameParts = className.Split(".".ToCharArray());

            // Name of the namespace alias.
            String namespaceName;

            // Only one namespace alias is valid for class, so there should be 2 parts. Search for namespace alias.
            if (classNameParts.Length == 2 && namespaceAliases.TryGetValue(classNameParts[0], out namespaceName))
            {
                // Get namespace and module parts.
                String[] nameAndModule = namespaceName.Split(',');

                String module = null;
                String name = null;

                // Only two strings is valid - namespace and module.
                if (nameAndModule.Length == 2)
                {
                    name = nameAndModule[0];
                    module = nameAndModule[1];

                    // Get name of class.
                    String nameOfClass = classNameParts[1];

                    // Change name of class to be fully decorated with namespace and module name.
                    className = name + "." + nameOfClass + "," + module;
                }
            }

            // Find type with specyfied name.
            Type type = Type.GetType(className);

            // If found, create instance.
            if (type != null)
            {
                GuiElement element = null;

                object obj = Activator.CreateInstance(type);

                // If it's GuiElement (should be!)
                if (obj is GuiElement)
                {
                    // Unserialize from parameters.
                    if ((obj as GuiElement).Init(node, namespaceAliases, directory, scale, areaSize, owner))
                    {
                        element = obj as GuiElement;
                    }
                    else
                    {
                        return null;
                    }

                    Action action = parameters.AsAction("OnLoaded", obj);

                    if (action != null)
                    {
                        action.Invoke();
                    }

                    return element;
                }
                else
                {
                    throw new Exception("Class is not a GuiElement.");
                }
            }
            else
            {
                throw new Exception("Error creating element: " + className + ".");
            }
        }

        protected void InstallGestureHandler(GestureAdditionalType additionalType, GestureType type, EventHandler<GestureEventArgs> handler)
        {
            Owner.InstallGestureHandlerInternal(additionalType, type, handler);
        }

        internal void SwitchInOutTransitions()
        {
            TransitionType tin = TransitionOut;
            TransitionOut = TransitionIn;
            TransitionIn = tin;

            Single tinSize = TransitionSizeOut;
            TransitionSizeOut = TransitionSizeIn;
            TransitionSizeIn = tinSize;
        }

        public virtual void UpdatePosition(Point position)
        {
            ElementRectangle = new Rectangle(position.X, position.Y, ElementRectangle.Width, ElementRectangle.Height);
        }

        public virtual void UpdatePosition(Rectangle position)
        {
            ElementRectangle = position;
        }

        public virtual void OnActivated()
        {
        }

        public virtual void OnDeactivated()
        {
        }

        public static Vector2 OffsetByAlign(Align align, Vector2 size)
        {
            Vector2 move = Vector2.Zero;

            if (IsAlign(align, Align.Bottom))
            {
                move.Y = size.Y;
            }
            else if (IsAlign(align, Align.Middle))
            {
                move.Y = size.Y / 2;
            }

            if (IsAlign(align, Align.Right))
            {
                move.X = size.X;
            }
            else if (IsAlign(align, Align.Center))
            {
                move.X = size.X / 2;
            }

            return move;
        }
    }
}
