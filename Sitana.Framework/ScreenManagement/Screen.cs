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
using System.Collections.Generic;
using System.IO;
using Sitana.Framework.Content;
using Sitana.Framework.Cs;
using Sitana.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Sitana.Framework.Gui
{
    public class Screen
    {
        // Possible screen states
        public enum ScreenState
        {
            ToRemove,         // Screen should be removed from manager
            TransitionIn,     // Screen is showing
            Visible,          // Screen is visible
            TransitionOut     // Screen is hiding
        }

        public enum FitMode
        {
            Both,
            Width,
            Height
        }

        // Screen state
        private ScreenState _state = ScreenState.TransitionIn;

        // Screen state
        public ScreenState State
        {
            get
            {
                return _state;
            }

            private set
            {
                _state = value;
            }
        }

        // Manager containing screen
        public ScreenManager ScreenManager { get; protected set; }

        // Current transition (0..1)
        public Double Transition { get; private set; }

        // type of screen transition.
        private TransitionType _transitionTypeIn;
        private TransitionType _transitionTypeOut;

        internal Int32 Zorder {get; private set;}

        // Size of screen
        public Vector2 AreaSize { get; private set; }

        // SpriteBatch object from ScreenManager should be used for drawing sprites
        protected SpriteBatch SpriteBatch { get; private set; }

        // List of buttons on the screen.
        private List<GuiElement> _elements = new List<GuiElement>();

        private Rectangle FadeSizes = new Rectangle(0, 0, 0, 0);

        public Vector2 DesignSize = Vector2.Zero;

        public Vector2 Scale { get; private set; }

        // Times for transition in and out for screen. These values cannot be 0.
        public Double TimeTransitionIn { get; protected set; }
        public Double TimeTransitionOut { get; protected set; }

        public String LoadPath { get; private set; }
        public Object[] LoadArguments {get; private set;}

        private Dictionary<String, ParametersCollection> _parameters = new Dictionary<String, ParametersCollection>();

        // Path of background screen.
        public String BackgroundPath { get; private set; }

        // Action on back button.
        protected Action BackAction = null;

        // Should ScreenManager disable adding to history.
        public Boolean DisableHistory { get; private set; }

        // Color of background when screen is at the most bottom position.
        public Color? BgColor { get; set; }

        private static Boolean _canCreateInstance = false;

        private RasterizerState _scissorsRasterizer = new RasterizerState();

        public Boolean IsMenuScreen { get; private set; }
        public Boolean IsPopupScreen { get; internal set; }

        public Vector2 ScreenOffset { get; set; }

        private Vector2 _screenOffset = Vector2.Zero;

        private Boolean _gestureHandlersInstalled = false;
        private List<RegisteredGesture> _gestureHandlers = new List<RegisteredGesture>();

        public Boolean InvertTransitions {get; private set;}
        private Boolean _invertTransitionsOnBack = false;

        public Boolean ClearNavigation { get; protected set; }

        public TransitionType ScreenTransition { get; private set; }

        public TransitionType ScreenTransitionInvert
        {
            get
            {
                switch(ScreenTransition)
                {
                    case TransitionType.SlideTop:
                        return TransitionType.SlideBottom;

                    case TransitionType.SlideBottom:
                        return TransitionType.SlideTop;

                    case TransitionType.SlideLeft:
                        return TransitionType.SlideRight;

                    case TransitionType.SlideRight:
                        return TransitionType.SlideLeft;
                }

                return TransitionType.None;
            }
        }

        private TransitionType _currentTransition = TransitionType.None;

        private Int32 _skipFrames = 0;

        // Single float value of transition used in drawing methods
        protected Single TransitionF
        {
            get
            {
                return (Single)Transition;
            }
        }

        public IEnumerable<GuiElement> Elements
        {
            get
            {
                return _elements;
            }
        }

        /// <summary>
        /// Construct screen object
        /// </summary>
        public Screen()
        {
            if (!_canCreateInstance)
            {
                throw new Exception("Cannot create instance Screen instance using new.");
            }

            // Screen should show itself after add
            State = ScreenState.TransitionIn;

            // It's hidden
            Transition = 0;

            // Set transition times to half of second
            TimeTransitionIn = 0.5;
            TimeTransitionOut = 0.5;

            Scale = new Vector2(1, 1);

            _scissorsRasterizer.ScissorTestEnable = true;
            IsMenuScreen = false;
            IsPopupScreen = false;
            ClearNavigation = false;

            _skipFrames = 2;

            ScreenTransition = TransitionType.None;
        }

        internal void UpdateCurrentTransition(TransitionType transition, Double inTime, Double outTime)
        {
            if (inTime >= 0)
            {
                TimeTransitionIn = inTime;
            }

            if (outTime >= 0)
            {
                TimeTransitionOut = outTime;
            }

            _currentTransition = transition;
        }

        public void SetParameters(String id, ParametersCollection parameters)
        {
            if (_parameters.ContainsKey(id))
            {
                _parameters[id].Add(parameters);
            }
            else
            {
                _parameters.Add(id, parameters);
            }
        }

        /// <summary>
        /// Returns collection of parameters.
        /// </summary>
        /// <param name="id">Id of collection</param>
        /// <returns>Collection of parameters.</returns>
        public ParametersCollection Parameters(String id)
        {
            return _parameters[id];
        }

        /// <summary>
        /// Returns collection of unnamed parameters.
        /// </summary>
        /// <returns>Collection of parameters.</returns>
        public ParametersCollection Parameters()
        {
            return _parameters[""];
        }

        /// <summary>
        /// Informs screen to remove
        /// </summary>
        public void Remove(TransitionType transition = TransitionType.None)
        {
            if (TimeTransitionOut == 0)
            {
                // Set to remove to instantly hide screen.
                State = ScreenState.ToRemove;
            }
            else
            {
                _skipFrames = 2;

                // Set transition out state to hide the screen
                State = ScreenState.TransitionOut;
                _currentTransition = transition == TransitionType.None ? _transitionTypeOut : transition;
            }
        }

        /// <summary>
        /// Initializes screen
        /// </summary>
        /// <param name="screenManager">ScreenManager object that will contain screen</param>
        protected virtual void Init(ScreenManager screenManager, params Object[] arguments)
        {
            // Set screen manager object and SpriteBatch object
            ScreenManager = screenManager;
            SpriteBatch = screenManager.SpriteBatch;

            // Compute screen area
            AreaSize = screenManager.AreaSize;

            ScreenManager.Redraw();
        }

        public void UpdateAfterResume()
        {
            SpriteBatch = ScreenManager.SpriteBatch;

            // Compute screen area
            AreaSize = ScreenManager.AreaSize;

            ScreenManager.Redraw();
        }

        public void UpdateScreen(TimeSpan timeSpan)
        {
            if (_skipFrames > 0)
            {
                _skipFrames--;
                return;
            }

            switch (State)
            {
            // If screen is transitioning in, increment transition and if it's fully transitioned
            // set its state to Visible
            case ScreenState.TransitionIn:

                Transition += timeSpan.TotalSeconds / TimeTransitionIn;

                if (Transition >= 1)
                {
                    Transition = 1;
                    State = ScreenState.Visible;

                    if (InvertTransitions)
                    {
                        InvertTransitions = false;
                        SwitchTransitions();
                    }
                }

                ScreenManager.Redraw();
                break;

            // If screen is transitioning in, decrement transition and if it's fully transitioned out
            // set its state to ToRemove
            case ScreenState.TransitionOut:

                Transition -= timeSpan.TotalSeconds / TimeTransitionOut;

                if (Transition <= 0)
                {
                    Transition = 0;
                    State = ScreenState.ToRemove;
                }

                ScreenManager.Redraw();

                break;

            case ScreenState.Visible:

                if (InputHandler.Current.GetKeyState(Keys.Escape) == KeyInfo.PressState.Pressed)
                {
                    Boolean isTop = IsPopupScreen;

                    if (!isTop)
                    {
                        isTop = ScreenManager.IsTopScreen(this) && !ScreenManager.IsPopupVisible;
                    }

                    if (BackAction != null && isTop)
                    {
                        BackAction.Invoke();
                        ScreenManager.Redraw();
                    }
                }
                break;
            }

            if ( State == Screen.ScreenState.Visible )
            {
                if ( !_gestureHandlersInstalled )
                {
                    for ( Int32 idx = 0; idx < _gestureHandlers.Count; ++idx )
                    {
                        var handler = _gestureHandlers[idx];
                        InputHandler.Current.InstallGestureHandler(handler);
                    }

                    _gestureHandlersInstalled = true;
                }
            }
            else
            {
                if(_gestureHandlersInstalled)
                {
                    UninstallGestureHandlers();
                }
            }
        }

        /// <summary>
        /// Processes screen logic
        /// </summary>
        /// <param name="timeSpan">Time ellapsed since last frame</param>
        public virtual void Update(TimeSpan timeSpan)
        {
            if ( ScreenOffset != Vector2.Zero )
            {
                InputHandler.Current.OffsetPointers(-ScreenOffset);
            }

            UpdateScreen(timeSpan);
            Boolean redraw = _screenOffset != ScreenOffset;

            if ( redraw )
            {
                Single time = (Single)timeSpan.TotalSeconds;

                if ( time < 0.125f )
                {
                    _screenOffset = (1 - time * 8) * _screenOffset + time * 8 * ScreenOffset;

                    if ( Math.Abs(_screenOffset.X-ScreenOffset.X) < 1 && Math.Abs(_screenOffset.Y-ScreenOffset.Y) < 1)
                    {
                        _screenOffset = ScreenOffset;
                    }
                }
                else
                {
                    _screenOffset = ScreenOffset;
                }
            }

            for (Int32 idx = _elements.Count - 1; idx >= 0; --idx)
            {
                redraw |= _elements[idx].Update(timeSpan, State);
            }

            if (redraw)
            {
                ScreenManager.Redraw();
            }

            if (ScreenOffset != Vector2.Zero)
            {
                InputHandler.Current.OffsetPointers(ScreenOffset);
            }
        }

        /// <summary>
        /// Loads screen.
        /// </summary>
        /// <param name="name">Screen name (full path).</param>
        internal static Screen LoadMenu(String name, ScreenManager screenManager, Object[] arguments)
        {
            return LoadInternal(name, screenManager, true, arguments, false);
        }

        /// <summary>
        /// Loads screen.
        /// </summary>
        /// <param name="name">Screen name (full path).</param>
        internal static Screen Load(String name, ScreenManager screenManager, Object[] arguments, Boolean back)
        {
            return LoadInternal(name, screenManager, false, arguments, back);
        }

        /// <summary>
        /// Loads screen.
        /// </summary>
        /// <param name="name">Screen name (full path).</param>
        private static Screen LoadInternal(String name, ScreenManager screenManager, Boolean isMenu, Object[] arguments, Boolean back)
        {
            // Get screen file directory name
            String directory = Path.GetDirectoryName(name);


            XmlFileNode screenNode;

            try
            {
                // Open stream with screen description with postfix name.
                screenNode = ContentLoader.Current.Load<XmlFile>(Name(name, screenManager));
            }
            catch (Exception)
            {
                // Open stream with screen description without postfix name.
                screenNode = ContentLoader.Current.Load<XmlFile>(name);
            }

            if (screenNode.Tag != "Screen")
            {
                throw new Exception("Invalid xml node, expected: Screen");
            }

            // Create dictionary with parameters.
            ParametersCollection screenParameters = screenNode.Attributes;

            // Find type with specyfied name.
            Type type = Type.GetType(screenParameters.AsString("Class"));

            // If found, create instance.
            if (type != null)
            {
                _canCreateInstance = true;

                object obj = Activator.CreateInstance(type);

                _canCreateInstance = false;

                Screen screen = obj as Screen;

                screen.IsMenuScreen = isMenu;
                screenNode.ValueSource = screen;
                screenNode.ColorsManager = screenManager.ColorsManager;

                // Init screen to fit current ScreenManager screens.
                screen.Init(screenManager, arguments);

                // Set load path.
                screen.LoadPath = name;
                screen.LoadArguments = arguments;

                // Load screen elements.
                screen.Load(screenNode, directory, back);

                screen.LoadContent(arguments);

                return screen;
            }

            throw new Exception("Error creating screen class object: " + screenParameters.AsString("Class") + ".");
        }

        /// <summary>
        /// Returns screen file name with postfix.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="screenManager"></param>
        /// <returns></returns>
        public static String Name(String name, ScreenManager screenManager)
        {
            if (screenManager != null)
            {
                if (!String.IsNullOrEmpty(screenManager.ScreenPostfix))
                {
                    name = name + "-" + screenManager.ScreenPostfix;
                }
            }

            return name;
        }

        /// <summary>
        /// Returns screen file name with postfix.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public String Name(String name)
        {
            return Name(name, ScreenManager);
        }

        /// <summary>
        /// Loads screen elements.
        /// </summary>
        /// <param name="stream">Stream used to read file.</param>
        /// <param name="contentLoader">Content loader used to load textures, fonts, etc.</param>
        private void Load(XmlFileNode screenNode, String directory, Boolean back)
        {
            ParametersCollection screenParameters = screenNode.Attributes;

            DesignSize = AreaSize;
            FitMode fitMode = FitMode.Both;

            _transitionTypeIn = screenParameters.AsEnum<TransitionType>("TransitionIn", TransitionType.None);
            _transitionTypeOut = screenParameters.AsEnum<TransitionType>("TransitionOut", TransitionType.None);

            if (screenParameters.AsBoolean("ClearNavigation"))
            {
                ClearNavigation = true;
            }

            if (_transitionTypeIn >= TransitionType.SlideLeft)
            {
                ScreenTransition = _transitionTypeIn;
            }

            _currentTransition = _transitionTypeIn;

            _invertTransitionsOnBack = screenParameters.AsBoolean("InvertTransitionsOnBack");
            InvertTransitions = back ? _invertTransitionsOnBack : false;
            
            Zorder = screenParameters.AsInt32("Zorder");

            DisableHistory = screenParameters.AsBoolean("DisableHistory", false);

            // Get design size to determine screen's scale for current resolution.
            DesignSize.X = screenParameters.AsInt32("DesignWidth");
            DesignSize.Y = screenParameters.AsInt32("DesignHeight");

            // Get fit mode for scale computing method.
            fitMode = screenParameters.AsEnum<FitMode>("FitMode", FitMode.Both);

            // Get transition times.
            TimeTransitionIn = (Double)screenParameters.AsInt32("TimeTransitionIn") / 1000.0;
            TimeTransitionOut = (Double)screenParameters.AsInt32("TimeTransitionOut") / 1000.0;

            BgColor = screenParameters.AsColorIfExists("BgColor");

            if (TimeTransitionIn == 0)
            {
                State = ScreenState.Visible;
                Transition = 1;
            }

            // Obtain action on back button.
            BackAction = screenParameters.AsAction("OnBack", null);

            // Compute screen scale for current resolution.
            Vector2 scale = AreaSize / DesignSize;

            switch (fitMode)
            {
            case FitMode.Both:
                scale.X = Math.Min(scale.X, scale.Y);
                scale.Y = scale.X;
                break;

            case FitMode.Width:
                scale.Y = scale.X;
                break;

            case FitMode.Height:
                scale.X = scale.Y;
                break;
            }

            Scale = scale;

            // Get background screen path.
            BackgroundPath = screenParameters.AsString("Background");

            LoadInside(screenNode, directory);

            // Compute element transitions.
            ComputeTransitions();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="directory"></param>
        private void LoadInside(XmlFileNode mainNode, String directory)
        {
            // Create dictionary for aliases for namespaces of elements used in screens.
            Dictionary<String, String> namespaceAliases = new Dictionary<String, String>();

            // Read GuiElements.
            for (Int32 idx = 0; idx < mainNode.Nodes.Count; ++idx )
            {
                XmlFileNode node = mainNode.Nodes[idx];
                // Get class name.
                String className = node.Tag;

                // Create dictionary with parameters.
                ParametersCollection parameters = node.Attributes;

                // If namespace declaration, parse it and add to dictionary.
                if (className == "Namespace")
                {
                    String id = parameters.AsString("Id");
                    String value = parameters.AsString("Value");

                    if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(value))
                    {
                        namespaceAliases.Add(id, value);
                    }
                }
                else if (className == "Include")
                {
                    LoadInclude(parameters.AsString("Path"), directory);
                }
                // Otherwise if class name is not empty.
                else if (className != "")
                {
                    GuiElement element = GuiElement.CreateElement(node, namespaceAliases, directory, Scale, AreaSize, this);

                    if ( element != null )
                    {
                        AddElement(element);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directory"></param>
        private void LoadInclude(String path, String directory)
        {
            XmlFileNode node;

            try
            {
                // Open stream with screen description with postfix name.
                node = ContentLoader.Current.Load<XmlFile>(Name(path, ScreenManager));
            }
            catch (Exception)
            {
                // Open stream with screen description without postfix name.
                node = ContentLoader.Current.Load<XmlFile>(path);
            }

            if ( node.Tag != "Partial")
            {
                throw new Exception("Invalid xml node. Expected: Partial");
            }

            node.ValueSource = this;
            node.ColorsManager = ScreenManager.ColorsManager;

            directory = Path.GetDirectoryName(path);

            LoadInside(node, directory);
        }

        /// <summary>
        /// Computes transition sizes.
        /// </summary>
        private void ComputeTransitions()
        {
            Int32 top = 0;
            Int32 bottom = (Int32)AreaSize.Y;
            Int32 left = 0;
            Int32 right = (Int32)AreaSize.X;

            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                var element = _elements[idx];

                switch (element.TransitionIn)
                {
                case TransitionType.Top:
                    top = Math.Max(top, element.ElementRectangle.Bottom);
                    break;

                case TransitionType.Bottom:
                    bottom = Math.Min(bottom, element.ElementRectangle.Top);
                    break;

                case TransitionType.Left:
                    left = Math.Max(left, element.ElementRectangle.Right);
                    break;

                case TransitionType.Right:
                    right = Math.Min(right, element.ElementRectangle.Left);
                    break;
                }

                switch (element.TransitionOut)
                {
                    case TransitionType.Top:
                        top = Math.Max(top, element.ElementRectangle.Bottom);
                        break;

                    case TransitionType.Bottom:
                        bottom = Math.Min(bottom, element.ElementRectangle.Top);
                        break;

                    case TransitionType.Left:
                        left = Math.Max(left, element.ElementRectangle.Right);
                        break;

                    case TransitionType.Right:
                        right = Math.Min(right, element.ElementRectangle.Left);
                        break;
                }
            }

            bottom = (Int32)AreaSize.Y - bottom;
            right = (Int32)AreaSize.X - right;

            FadeSizes = new Rectangle(left, top, right - left, bottom - top);

            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                var element = _elements[idx];

                switch (element.TransitionIn)
                {
                case TransitionType.Top:
                    element.TransitionSizeIn = FadeSizes.Top;
                    break;

                case TransitionType.Bottom:
                    element.TransitionSizeIn = FadeSizes.Bottom;
                    break;

                case TransitionType.Left:
                    element.TransitionSizeIn = FadeSizes.Left;
                    break;

                case TransitionType.Right:
                    element.TransitionSizeIn = FadeSizes.Right;
                    break;

                default:
                    element.TransitionSizeIn = 0;
                    break;
                }

                switch (element.TransitionOut)
                {
                case TransitionType.Top:
                    element.TransitionSizeOut = FadeSizes.Top;
                    break;

                case TransitionType.Bottom:
                    element.TransitionSizeOut = FadeSizes.Bottom;
                    break;

                case TransitionType.Left:
                    element.TransitionSizeOut = FadeSizes.Left;
                    break;

                case TransitionType.Right:
                    element.TransitionSizeOut = FadeSizes.Right;
                    break;

                default:
                    element.TransitionSizeOut = 0;
                    break;
                }
            }
        }

        /// <summary>
        /// Adds element to the screen.
        /// </summary>
        /// <param name="button">Button to be added.</param>
        protected void AddElement(GuiElement element)
        {
            _elements.Add(element);
            element.OnAdded();
        }

        /// <summary>
        /// Removes element from the screen.
        /// </summary>
        /// <param name="id">Id of element to remove.</param>
        protected void RemoveElement(String id)
        {
            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                if (_elements[idx].Id == id)
                {
                    _elements[idx].OnRemoved();
                    _elements.RemoveAt(idx);
                    break;
                }
            }
        }

        protected void DrawElements(Single transition)
        {
            DrawElements(transition, Vector2.Zero);
        }

        /// <summary>
        /// Draws screen elements.
        /// </summary>
        /// <param name="color">Color to mix with buttons.</param>
        /// <param name="offset">Offset for buttons.</param>
        protected void DrawElements(Single transition, Vector2 offset)
        {
            DrawElements(transition, _elements, offset);
        }

        private Boolean CalculateTransitionOffset(out Vector2 offset)
        {
            offset = Vector2.Zero;

            Single transition = TransitionF;

            if (transition == 1)
            {
                return false;
            }

            TransitionType transitionType = _currentTransition;

            if ( transitionType != TransitionType.None && !ScreenManager.IsTopScreen(this) )
            {
                if (transitionType < TransitionType.SlideLeft)
                {
                    return true;
                }
            }

            switch(transitionType)
            {
            case TransitionType.Bottom:
                case TransitionType.SlideBottom:
                offset = new Vector2(0, AreaSize.Y * (1 - transition));
                return true;

            case TransitionType.Top:
                case TransitionType.SlideTop:
                offset = new Vector2(0, -AreaSize.Y * (1 - transition));
                return true;

            case TransitionType.Right:
                case TransitionType.SlideRight:
                offset = new Vector2(AreaSize.X * (1 - transition), 0);
                return true;

            case TransitionType.Left:
            case TransitionType.SlideLeft:
                offset = new Vector2(-AreaSize.X * (1 - transition), 0);
                return true;
            }

            return false;
        }

        private void StartDefaultSpriteBatch(Int32 level, RasterizerState defaultState)
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, (level % 2) == 0 ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, defaultState);
        }

        protected void DrawElements(Single transition, List<GuiElement> elements, Vector2 topLeft)
        {
            Vector2 offset;

            Rectangle screenClip = new Rectangle(0, 0, (Int32)AreaSize.X, (Int32)AreaSize.Y);

            RasterizerState defaultState = RasterizerState.CullNone;

            if ( CalculateTransitionOffset(out offset))
            {
                screenClip.X += (Int32)offset.X;
                screenClip.Y += (Int32)offset.Y;

                Rectangle bounds = new Rectangle(0, 0, (Int32)AreaSize.X, (Int32)AreaSize.Y);

                screenClip = GraphicsHelper.IntersectRectangle(ref screenClip, ref bounds);

                defaultState = _scissorsRasterizer;

                transition = 1;
                topLeft += offset;
            }

            topLeft += _screenOffset;

            Int32 minLevel = Int32.MaxValue;
            Int32 maxLevel = 0;

            if (!IsMenuScreen && !IsPopupScreen)
            {
                topLeft += ScreenManager.ScreensOffsetIfMenu;
            }

            for (Int32 idx = 0; idx < elements.Count; ++idx)
            {
                Pair<Int32, Int32> levels = elements[idx].DrawLevels;
                minLevel = Math.Min(levels.First, minLevel);
                maxLevel = Math.Max(levels.Second, maxLevel);
            }

            for (Int32 level = minLevel; level <= maxLevel; ++level)
            {
                SpriteBatch.GraphicsDevice.ScissorRectangle = screenClip;
                StartDefaultSpriteBatch(level, defaultState);

                for (Int32 idx = 0; idx < elements.Count; ++idx)
                {
                    var element = elements[idx];

                    if (element.ClipToElement)
                    {
                        SpriteBatch.End();

                        Rectangle scissors = new Rectangle((Int32)(topLeft.X + element.ElementRectangle.X), (Int32)(topLeft.Y + element.ElementRectangle.Y), element.ElementRectangle.Width, element.ElementRectangle.Height);
                        Rectangle bounds = screenClip;

                        Vector2 move = element.ComputeOffsetWithTransition(transition) + _screenOffset;

                        scissors.X += (Int32)move.X;
                        scissors.Y += (Int32)move.Y;

                        SpriteBatch.GraphicsDevice.ScissorRectangle = GraphicsHelper.IntersectRectangle(ref bounds, ref scissors);

                        SpriteBatch.Begin(SpriteSortMode.Deferred, (level % 2) == 0 ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, _scissorsRasterizer);

                        element.Draw(level, SpriteBatch, topLeft, transition);

                        SpriteBatch.End();

                        SpriteBatch.GraphicsDevice.ScissorRectangle = screenClip;
                        StartDefaultSpriteBatch(level, defaultState);
                    }
                    else
                    {
                        if (!element.UsesSpriteBatch)
                        {
                            SpriteBatch.End();
                            element.Draw(level, null, topLeft, transition);
                            StartDefaultSpriteBatch(level, defaultState);
                        }
                        else
                        {
                            element.Draw(level, SpriteBatch, topLeft, transition);
                        }
                    }

                    Boolean spriteBatchStarted = true;

                    AdditionalDraw draw;

                    while (ScreenManager.PopAdditionalDraw(out draw))
                    {
                        if (spriteBatchStarted)
                        {
                            SpriteBatch.End();
                            spriteBatchStarted = false;
                        }

                        Rectangle scissors = new Rectangle((Int32)(topLeft.X + draw.Clip.X), (Int32)(topLeft.Y + draw.Clip.Y), draw.Clip.Width, draw.Clip.Height);
                        Rectangle bounds = screenClip;

                        SpriteBatch.GraphicsDevice.ScissorRectangle = GraphicsHelper.IntersectRectangle(ref bounds, ref scissors);

                        SpriteBatch.Begin(SpriteSortMode.Deferred, (level % 2) == 0 ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, _scissorsRasterizer);

                        draw.DrawFunction();

                        SpriteBatch.End();
                    }

                    if (!spriteBatchStarted)
                    {
                        SpriteBatch.Begin(SpriteSortMode.Deferred, (level % 2) == 0 ? BlendState.Additive : BlendState.AlphaBlend);
                    }
                }

                SpriteBatch.End();
            }
        }

        private void UninstallGestureHandlers()
        {
            for (Int32 idx = 0; idx < _gestureHandlers.Count; ++idx)
            {
                InputHandler.Current.UninstallGestureHandler(_gestureHandlers[idx]);
            }
            _gestureHandlersInstalled = false;
        }

        protected void InstallGestureHandler(GestureAdditionalType additionalType, GestureType type, EventHandler<GestureEventArgs> handler)
        {
            GestureReceiverType rtype = GestureReceiverType.Screen;

            if (IsPopupScreen)
            {
                rtype = GestureReceiverType.Popup;
            } 
            else if (IsMenuScreen)
            {
                rtype = GestureReceiverType.Menu;
            }

            _gestureHandlers.Add(new RegisteredGesture(additionalType, type, rtype, handler));
        }

        internal void InstallGestureHandlerInternal(GestureAdditionalType additionalType, GestureType type, EventHandler<GestureEventArgs> handler)
        {
            InstallGestureHandler(additionalType, type, handler);
        }

        /// <summary>
        /// Shows screen immediately.
        /// </summary>
        protected void ShowImmediately()
        {
            State = ScreenState.Visible;
            Transition = 1;
        }

        protected void SwitchTransitions()
        {
            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                _elements[idx].SwitchInOutTransitions();
            }
        }

        public void GoBack()
        {
            GoBack(null);
        }

        public void GoBack(String argument)
        {
            if(State == ScreenState.TransitionOut || ScreenManager.TopScreen != this)
            {
                return;
            }

            if (_invertTransitionsOnBack)
            {
                SwitchTransitions();
            }

            if (argument != null)
            {
                String[] paths = argument.Split('|');
                ScreenManager.NavigateBack(paths);
            }
            else
            {
                ScreenManager.NavigateBack();
            }
        }

        public void NavigateToLikeBack(String path, params Object[] args)
        {
            SwitchTransitions();

            ScreenManager.NavigateToInternal(path, true, args);
        }

        public virtual void Back(Object sender, String argument)
        {
            GoBack(argument);
        }

        public virtual void Back(Object sender)
        {
            GoBack();
        }

        public virtual void Open(Object sender, String argument)
        {
            ScreenManager.NavigateTo(argument);
        }

        public void OpenWeb(Object sender, String argument)
        {
            ScreenManager.UiAction(() => SystemWrapper.OpenWebsite(argument));
        }

        public GuiElement Find(String id)
        {
            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                var element = _elements[idx];

                if (element.Id == id)
                {
                    return element;
                }
            }

            return null;
        }

        /// <summary>
        /// Find the specified element.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="T">Type of element.</typeparam>
        public T Find<T>(String id) where T: GuiElement
        {
            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                var element = _elements[idx];

                if (element.Id == id && element is T)
                {
                    return element as T;
                }
            }

            return default(T);
        }

        /// <summary>
        /// This is called when screen is added to manager
        /// </summary>
        public virtual void OnAdded()
        {
            
        }

        /// <summary>
        /// This is called when screen is removed from manager
        /// </summary>
        public virtual void OnRemoved()
        {
            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                var element = _elements[idx];
                element.OnRemoved();
            }

            UninstallGestureHandlers();
        }

        /// <summary>
        /// Called when app is activated.
        /// </summary>
        public virtual void OnAppActivated()
        {
            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                var element = _elements[idx];
                element.OnActivated();
            }
        }

        /// <summary>
        /// Called when app is activated.
        /// </summary>
        public virtual void OnAppDeactivated()
        {
            for (Int32 idx = 0; idx < _elements.Count; ++idx)
            {
                var element = _elements[idx];
                element.OnDeactivated();
            }
        }

        /// <summary>
        /// Loads screen content
        /// </summary>
        public virtual void LoadContent(Object[] arguments)
        {

        }

        /// <summary>
        /// Draws screen
        /// </summary>
        /// <param name="timeSpan">Time ellapsed since last frame</param>
        public virtual void Draw(TimeSpan timeSpan)
        {
            DrawElements(TransitionF);
        }

        public void InstantShow()
        {
            Transition = 1;
        }

        public void InstantHide()
        {
            Transition = 0.0001;
            State = ScreenState.TransitionOut;
        }
    }
}
