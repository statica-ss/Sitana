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
using System.Reflection;
using Sitana.Framework.Content;
using Sitana.Framework.Cs;
using Sitana.Framework.Graphics;
using Sitana.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Sitana.Framework.Gui
{
    public abstract class ScreenManager : ToolkitClass
    {
        public static ScreenManager Current {get; private set;}

        public SpriteBatch SpriteBatch { get; private set; }

        public GraphicsDevice GraphicsDevice { get; private set; }

        public String ScreenPostfix { get; set; }

        public Vector2 AreaSize { get; protected set; }

        public Boolean EnableRescale { private get; set; }

        private Single _showMenuSpeed = 0.3f;

        private Double _delayedPreloadDelay = 0;

        private List<LoadResourceInfo> _delayedResourceLoad = new List<LoadResourceInfo>();

        public Random Random{get;private set;}

        // List of existing screens
        private List<Screen> _menuScreens = new List<Screen>();
        private List<Screen> _screens = new List<Screen>();
        private List<ScreenPopup> _popupScreens = new List<ScreenPopup> ();

        public Color BgColor { get; private set; }

        private Color _aspectStripesColor = Color.Black;
        private List<KeyValuePair<String, Object[]>> _navigation = new List<KeyValuePair<String, Object[]>>();
        protected RenderTargetEx _renderTarget;
        protected Viewport _targetViewport;
        private Single _targetScale = 1;
        private Vector2 _offset = Vector2.Zero;
        private Boolean _forceSwitchBackground = false;
        private List<InfoBar> _infoBars = new List<InfoBar>();

        public StringsManager StringsManager { get; private set; }
        public ColorsManager ColorsManager { get; private set; }

        private Dictionary<String, Pair<GuiElement, GuiElement>> _instances = new Dictionary<String, Pair<GuiElement, GuiElement>>();

        public Boolean SupressRedraw {get; private set;}

        public Color ClearColor {get; private set;}

        public Vector2 ScreensOffsetIfMenu { get; private set; }

        private Boolean _redrawMark = true;
        private Object _redrawLock = new Object();

        private Single _menuSize = 0.1f;
        private Boolean _showMenu = false;

        private String _mainMenuPath;

        private Boolean _close = false;
        private Boolean _waitForFlickToCloseMenu = false;

        private List<Action> _dispatcherActions = new List<Action>();
        private List<Action> _dispatcherActionsTemp = new List<Action>();

        private List<Pair<String, Object[]>> _popupQueue = new List<Pair<String, Object[]>> ();

        private Action _onMenuHidden = null;

        public List<AdditionalDraw> _additionalDraws = new List<AdditionalDraw>();

        public Single Scale;

        private String _startupScreen;

        public Screen TopScreen
        {
            get
            {
                return _screens.Count > 0 ? _screens[_screens.Count-1] : null;
            }
        }

        public String PopupPath
        {
            get
            {
                if (_popupScreens.Count > 0)
                {
                    return _popupScreens[_popupScreens.Count-1].LoadPath;
                }

                return null;
            }
        }

        /// <summary>
        /// Construction of new ScreenManager object.
        /// </summary>
        /// <param name="graphicsDevice">Graphics device object initialized in XNA's Game class</param>
        /// <param name="contentManager">Content manager for loading stuff</param>
        public ScreenManager(GameServiceContainer services, String[] contentPaths)
        {
            if (Current != null)
            {
                throw new Exception("There can be only one screen manager.");
            }

            Random = new System.Random();

            ContentLoader.Init(null, contentPaths[0]);
            EnableRescale = true;
            
            StringsManager = new StringsManager();
            ColorsManager = new ColorsManager();

            BgColor = Color.White;
            ScreensOffsetIfMenu = Vector2.Zero;

            UiTask._screenManager = this;
            Current = this;
        }

        public abstract void Initialize(GraphicsDevice graphicsDevice);

        protected void Initialize(String settingsPath, GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = new SpriteBatch(graphicsDevice);

            CsToolkit.WriteLog(MethodBase.GetCurrentMethod(), "Display is {0}x{1} in {2} mode.", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width > GraphicsDevice.Viewport.Height ? "Landscape" : "Portrait");

            // Ensure full path is rovided
            if (!settingsPath.StartsWith(":"))
            {
                throw new InvalidOperationException("Screen paths must be always absolute.");
            }

            // Get screen file directory name
            String directory = Path.GetDirectoryName(settingsPath);

            XmlFileNode node = ContentLoader.Current.Load<XmlFile>(settingsPath);
            LoadSettings(node, directory);
        }

        private void LoadSettings(XmlFileNode node, String directory)
        {
            Point areaSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if (node.Tag != "ScreenManager")
            {
                throw new Exception("Invalid xml node. Expected: ScreenManager");
            }

            ParametersCollection parameters = node["Settings"].Attributes;

            String startScreen = parameters.AsString("StartScreen");

            BgColor = parameters.AsColor("BgColor");
            _aspectStripesColor = parameters.AsColorIfExists("AspectStripesColor") ?? Color.Black;

            Single menuSize = parameters.AsSingle("MenuSize");

            _mainMenuPath = parameters.AsString("Menu");

            String onePixelWhiteTexturePath = parameters.AsString("OnePixelWhiteTexture");

            ContentLoader.Current.OnePixelWhiteTexture = ContentLoader.Current.Load<Texture2D>(onePixelWhiteTexturePath);

            Point minAspect = parameters.ParsePoint("AspectMin");
            Point maxAspect = parameters.ParsePoint("AspectMax");

            if (parameters.HasKey("ShowHideMenuTime"))
            {
                Single duration = parameters.AsSingle("ShowHideMenuTime") / 1000.0f;

                if (duration == 0)
                {
                    _showMenuSpeed = 1000;
                }
                else
                {
                    _showMenuSpeed = 1 / duration;
                }
            }

            Point? resTo = null;

            Double aspect = (Single)areaSize.X / (Single)areaSize.Y;

            if (aspect < (Single)minAspect.X / (Single)minAspect.Y)
            {
                resTo = new Point(areaSize.X, areaSize.X * minAspect.Y / minAspect.X);
            }

            aspect = (Single)areaSize.X / (Single)areaSize.Y;

            if (aspect > (Single)maxAspect.X / (Single)maxAspect.Y)
            {
                resTo = new Point(areaSize.Y * maxAspect.X / maxAspect.Y, areaSize.Y);
            }

            // Read main element.
            if (EnableRescale)
            {
                for (Int32 idx = 0; idx < node.Nodes.Count; ++idx)
                {
                    if (node.Nodes[idx].Tag == "Rescale")
                    {
                        Point currentAreaSize = resTo ?? areaSize;

                        parameters = node.Nodes[idx].Attributes;

                        Point from = parameters.ParsePoint("From");
                        Int32 to = parameters.AsInt32("To");

                        Point minimal = parameters.ParsePoint("Minimal");
                        Point maximum = parameters.ParsePoint("Maximum");

                        if (currentAreaSize == from)
                        {
                            resTo = new Point(to, to * from.Y / from.X);
                            break;
                        }

                        if (currentAreaSize.X < minimal.X)
                        {
                            resTo = new Point(minimal.X, currentAreaSize.Y * minimal.X / currentAreaSize.X);
                            currentAreaSize = resTo.Value;
                        }

                        if (currentAreaSize.Y < minimal.Y)
                        {
                            resTo = new Point(currentAreaSize.X * minimal.Y / currentAreaSize.Y, minimal.Y);
                            currentAreaSize = resTo.Value;
                        }

                        if (currentAreaSize.X > maximum.X)
                        {
                            resTo = new Point(maximum.X, currentAreaSize.Y * maximum.X / currentAreaSize.X);
                            currentAreaSize = resTo.Value;
                        }

                        if (currentAreaSize.Y > maximum.Y)
                        {
                            resTo = new Point(currentAreaSize.X * maximum.Y / currentAreaSize.Y, maximum.Y);
                            currentAreaSize = resTo.Value;
                        }

                        if (resTo.HasValue)
                        {
                            break;
                        }
                    }
                }
            }

            Scale = 1;

            if (resTo.HasValue)
            {
                Scale = (Single)resTo.Value.Y / (Single)areaSize.Y;

                _renderTarget = new RenderTargetEx(GraphicsDevice, resTo.Value.X, resTo.Value.Y);
                _targetViewport = new Viewport(0, 0, resTo.Value.X, resTo.Value.Y);

                _targetScale = (Single)areaSize.X / (Single)resTo.Value.X;

                if (_targetScale * resTo.Value.Y > areaSize.Y)
                {
                    _targetScale = (Single)areaSize.Y / (Single)resTo.Value.Y;
                }

                _offset = new Vector2((Int32)(areaSize.X - resTo.Value.X * _targetScale) / 2, (Int32)(areaSize.Y - resTo.Value.Y * _targetScale) / 2);

                areaSize = resTo.Value;

                if (InputHandler.Current != null)
                {
                    InputHandler.Current.Scale = 1 / _targetScale;
                    InputHandler.Current.Offset = _offset;
                }
            }

            AreaSize = GraphicsHelper.Vector2FromPoint(areaSize);

            _menuSize = AreaSize.X - AreaSize.X * menuSize;

            if (!String.IsNullOrEmpty(startScreen))
            {
                NavigateTo( startScreen);
                TopScreen.InstantShow();
                _startupScreen = startScreen;
            }

            InputHandler.Current.InstallGestureHandler(GestureAdditionalType.TouchDown, GestureType.None, GestureReceiverType.Menu, OnTouchDown);
            InputHandler.Current.InstallGestureHandler(GestureAdditionalType.TouchUp, GestureType.None, GestureReceiverType.Menu, OnTouchUp);
            InputHandler.Current.InstallGestureHandler(GestureAdditionalType.Native, GestureType.Tap, GestureReceiverType.Menu, OnTap);
            InputHandler.Current.InstallGestureHandler(GestureAdditionalType.Native, GestureType.Flick, GestureReceiverType.Menu, OnFlick);
        }

        public void NavigateToStartScreen()
        {
            if ( _startupScreen != null )
            {
                NavigateTo(_startupScreen);
                TopScreen.InstantShow();
            }
        }

        public void ShowMenu(String menuPath)
        {
            _showMenu = true;
            InputHandler.Current.GestureReceiverType = GestureReceiverType.Menu;

            for (Int32 idx = 0; idx < _menuScreens.Count; ++idx)
            {
                _menuScreens[idx].OnRemoved();
            }

            _menuScreens.Clear();

            NavigateMenu(menuPath);
            _menuScreens[_menuScreens.Count - 1].InstantShow();
        }

        public void ShowMenu()
        {
            _showMenu = true;
            InputHandler.Current.GestureReceiverType = GestureReceiverType.Menu;

            for (Int32 idx = 0; idx < _menuScreens.Count; ++idx)
            {
                _menuScreens[idx].OnRemoved();
            }

            _menuScreens.Clear();

            NavigateMenu(_mainMenuPath);
            _menuScreens[_menuScreens.Count - 1].InstantShow();
        }

        public void HideMenu()
        {
            _showMenu = false;
        }

        public void HideMenu(Action onMenuHidden)
        {
            _showMenu = false;
            _onMenuHidden = onMenuHidden;
        }

        public void ToggleMenu()
        {

            if (_showMenu) 
            {
                HideMenu();
            } 
            else 
            {
                ShowMenu();
            }
        }

        private void OnTouchDown(Object sender, EventArgs e)
        {
            GestureEventArgs args = e as GestureEventArgs;

            if (ScreensOffsetIfMenu.X == _menuSize)
            {
                if (args.Sample.Position.X >= _menuSize)
                {
                    _waitForFlickToCloseMenu = true;
                }
            }

            if (ScreensOffsetIfMenu.X != 0)
            {
                args.Handled = true;
            }
        }

        private void OnTouchUp(Object sender, EventArgs e)
        {
            GestureEventArgs args = e as GestureEventArgs;

            if ( _waitForFlickToCloseMenu )
            {
                args.Handled = false;
                _waitForFlickToCloseMenu = false;
            }
        }

        private void OnTap(Object sender, EventArgs e)
        {
            GestureEventArgs args = e as GestureEventArgs;

            if (ScreensOffsetIfMenu.X == _menuSize)
            {
                if ( args.Sample.Position.X >= _menuSize )
                {
                    _showMenu = false;
                }
            }

            if ( ScreensOffsetIfMenu.X != 0 )
            {
                args.Handled = true;
            }
        }

        private void OnFlick(Object sender, EventArgs e)
        {
            GestureEventArgs args = e as GestureEventArgs;

            if ( _waitForFlickToCloseMenu )
            {
                if (args.Sample.Delta.X < 0 && Math.Abs(args.Sample.Delta.X) > Math.Abs(args.Sample.Delta.Y))
                {
                    _showMenu = false;
                }

                args.Handled = true;
                _waitForFlickToCloseMenu = false;
            }
        }

        protected virtual void ProcessMenuInput()
        {
            InputHandler.Current.PointersState.Clear();
            InputHandler.Current.ClearKeys();
        }

        /// <summary>
        /// Process all screens logic
        /// </summary>
        /// <param name="timeSpan">Time elapsed since last frame</param>
        /// <returns></returns>
        public virtual Boolean Update(TimeSpan timeSpan)
        {
            _dispatcherActionsTemp.Clear();

            lock (_dispatcherActions)
            {
                for (Int32 idx = 0; idx < _dispatcherActions.Count; ++idx)
                {
                    _dispatcherActionsTemp.Add(_dispatcherActions[idx]);
                }
                _dispatcherActions.Clear();
            }

            for (Int32 idx = 0; idx < _dispatcherActionsTemp.Count; ++idx)
            {
                _dispatcherActionsTemp[idx].Invoke();
            }

            if (_delayedResourceLoad.Count > 0)
            {
                _delayedPreloadDelay -= timeSpan.TotalSeconds;

                if (_delayedPreloadDelay <= 0)
                {
                    var info = _delayedResourceLoad[0];
                    _delayedResourceLoad.RemoveAt(0);

                    try
                    {
                        if (info.Generator != null)
                        {
                            info.Generator.Generate();
                        }
                        else if (info.Load != null)
                        {
                            Object contentObj = info.Load.Invoke(ContentLoader.Current, new Object[] { info.Path });
                            ContentLoader.Current.AddContent(info.Path, info.ContentType, contentObj);
                        }
                    }
                    catch (Exception) { }

                    _delayedPreloadDelay = 0.1;
                }
            }

            if (_showMenu)
            {
                if ( ScreensOffsetIfMenu.X < _menuSize )
                {
                    ScreensOffsetIfMenu = new Vector2( Math.Min(_menuSize, ScreensOffsetIfMenu.X + (Single)timeSpan.TotalSeconds * _showMenuSpeed * AreaSize.X), 0);
                    Redraw();
                }
            }
            else
            {
                if ( ScreensOffsetIfMenu.X > 0 )
                {
                    ScreensOffsetIfMenu = new Vector2(Math.Max(0, ScreensOffsetIfMenu.X - (Single)timeSpan.TotalSeconds * _showMenuSpeed * AreaSize.X), 0);
                    Redraw();

                    if (ScreensOffsetIfMenu.X == 0)
                    { 
                        if (_onMenuHidden != null)
                        {
                            UiAction(() =>
                            {
                                _onMenuHidden.Invoke();
                                _onMenuHidden = null;
                            });
                        }
                    }
                }
            }

            if (ScreensOffsetIfMenu.X == 0)
            {
                InputHandler.Current.GestureReceiverType = _popupScreens.Count > 0 ? GestureReceiverType.Popup : GestureReceiverType.Screen;
            } else
            {
                InputHandler.Current.GestureReceiverType = GestureReceiverType.Menu;
            }
                
            InputHandler.Current.Update();

            for (Int32 idx = _popupScreens.Count-1; idx >= 0 ; --idx)
            {
                var popupScreen = _popupScreens[idx];
                popupScreen.Update(timeSpan);

                if (popupScreen.ConsumeInput)
                {
                    InputHandler.Current.PointersState.Clear();
                    InputHandler.Current.ClearKeys();
                }

                if (idx == _popupScreens.Count - 1)
                {
                    if (popupScreen.State == Screen.ScreenState.ToRemove || popupScreen.State == Screen.ScreenState.TransitionOut)
                    {
                        if (_popupQueue.Count > 0)
                        {
                            var queued = _popupQueue[0];
                            _popupQueue.RemoveAt(0);
                            ShowPopupInternal(queued.First, queued.Second);
                        }
                    }
                }
            }

            if (ScreensOffsetIfMenu.X != 0)
            {
                // Iterate thru al screens and call Update for'em
                for (Int32 idx = _menuScreens.Count-1; idx >= 0 ; --idx )
                {
                    Screen screen = _menuScreens[idx];

                    screen.Update(timeSpan);

                    // If screen should be removed, remove screen and call OnRemoved handler for screen
                    if (screen.State == Screen.ScreenState.ToRemove)
                    {
                        _menuScreens.RemoveAt(idx);
                        screen.OnRemoved();
                    }
                }

                ProcessMenuInput();
            }
            
            ProcessInput();

            for (Int32 idx = 0; idx < _infoBars.Count; ++idx)
            {
                _infoBars[idx].Update((Single)timeSpan.TotalSeconds);
            }


            // Iterate thru al screens and call Update for'em
            for (Int32 idx = _screens.Count-1; idx >= 0; --idx )
            {
                Screen screen = _screens[idx];
                screen.Update(timeSpan);
            }

            for (Int32 idx = _popupScreens.Count - 1; idx >= 0; --idx)
            {
                var popupScreen = _popupScreens[idx];
                if (popupScreen.State == Screen.ScreenState.ToRemove)
                {
                    _popupScreens.RemoveAt(idx);
                    popupScreen.OnRemoved();
                    Redraw();
                }
            }

            // Iterate thru al screens and call Update for'em
            for (Int32 idx = _screens.Count-1; idx >= 0; --idx )
            {
                Screen screen = _screens[idx];

                // If screen should be removed, remove screen and call OnRemoved handler for screen
                if (screen.State == Screen.ScreenState.ToRemove)
                {
                    _screens.RemoveAt(idx);
                    screen.OnRemoved();
                    Redraw();
                }
            }

            lock (_redrawLock)
            { 
                SupressRedraw = !_redrawMark;
                _redrawMark = false;
            }

            Boolean retValue = _screens.Count > 0 && !_close;
            _close = false;

            return retValue;
        }

        protected virtual void ProcessInput()
        {

        }
        // Draw game screens.
        public void Draw(TimeSpan timeSpan)
        {
            if (SpriteBatch == null)
            {
                return;
            }

            if (_renderTarget != null)
            {
                if (!_renderTarget.IsOk)
                {
                    if (!_renderTarget.Restore())
                    {
                        Redraw();
                        return;
                    }
                }

                try
                {
                    _renderTarget.Begin();
                }
                catch
                {
                    UiAction(()=>
                    {
                        _renderTarget.Release();
                        _renderTarget.Restore();
                    });
                    Redraw();
                    return;
                }
            }

            OnDraw(timeSpan);

            if (_renderTarget != null)
            {
                _renderTarget.End();

                GraphicsDevice.Clear(_aspectStripesColor);

                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);

                SpriteBatch.Draw(_renderTarget.Buffer, _offset, null, Color.White, 0, Vector2.Zero, _targetScale, SpriteEffects.None, 0);
                SpriteBatch.End();
            }
        }

        public void ShowInfo(Int32 barId, InfoBarInfo info)
        {
            _infoBars[barId].PushInfo(info);
        }

        /// <summary>
        /// Draws screens
        /// </summary>
        /// <param name="timeSpan">Time ellapsed since last frame</param>
        protected virtual void OnDraw(TimeSpan timeSpan)
        {
            Color backColor;

            if(ScreensOffsetIfMenu.X > 0)
            {
                if ( _menuScreens[0].BgColor.HasValue )
                {
                    backColor = _menuScreens[0].BgColor.Value;
                }
                else
                {
                    backColor = BgColor;
                }
            }
            else if (_screens.Count > 0 && _screens[0].BgColor.HasValue)
            {
                // Clear screen with solid color.
                backColor = _screens[0].BgColor.Value;
            }
            else
            {
                // Clear screen with solid color.
                backColor = BgColor;
            }

            Color newColor = backColor;

            if (_screens.Count > 1)
            {
                for (Int32 idx = 1; idx < _screens.Count; ++idx)
                {
                    if (_screens[idx].BgColor.HasValue)
                    {
                        newColor = _screens[idx].BgColor.Value;
                        newColor = GraphicsHelper.MixColors(backColor, newColor, (Single)_screens[idx].Transition);
                    }
                }
            }

            ClearColor = newColor;
            GraphicsDevice.Clear(newColor);

            if (ScreensOffsetIfMenu.X > 0)
            {
                // For each screen call Draw method to draw its stuff.
                for (Int32 idx = 0; idx < _menuScreens.Count; ++idx)
                {
                    Screen screen = _menuScreens[idx];
                    screen.Draw(timeSpan);
                }
            }


            Single scale = 1;

            // For each screen call Draw method to draw its stuff.
            for (Int32 idx = 0; idx < _screens.Count; ++idx)
            {
                Screen screen = _screens[idx];
                screen.Draw(timeSpan);
                scale = screen.Scale.X;
            }

            for (Int32 idx = 0; idx < _popupScreens.Count; ++idx)
            {
                Screen screen = _popupScreens[idx];
                screen.Draw(timeSpan);
            }

            for (Int32 idx = 0; idx < _infoBars.Count; ++idx)
            {
                InfoBar infoBar = _infoBars[idx];
                infoBar.Draw(SpriteBatch, scale, AreaSize);
            }
        }

        private ScreenPopup ShowPopupInternal(String path, Object[] arguments)
        {
            try
            {
                ScreenPopup popup = (ScreenPopup)Screen.Load(path, this, arguments, false);

                _popupScreens.Add(popup);

                // Call OnAdded callback
                popup.OnAdded();

                return popup;
            }
            catch (CancelScreenAddException)
            {
                
            }

            return null;
        }

        public void ShowPopup(String path, params Object[] arguments)
        {
            if (_popupScreens.Count != 0 )
            {
                var popup = _popupScreens [_popupScreens.Count - 1];

                if (popup.State == Screen.ScreenState.TransitionIn || popup.State == Screen.ScreenState.Visible)
                {
                    _popupQueue.Add (new Pair<String, Object[]> (path, arguments));
                    return;
                }
            }

            ShowPopupInternal(path, arguments);
        }

        /// <summary>
        /// Navigates to given screen.
        /// </summary>
        /// <param name="path">Path to screen to navigate to.</param>
        public void NavigateTo(String path)
        {
            NavigateTo(path, null);
        }

        /// <summary>
        /// Navigates to given screen.
        /// </summary>
        /// <param name="path">Path to screen to navigate to.</param>
        public void SwitchTo(String path)
        {
            TopScreen.InstantHide();
            NavigateTo(path, null);
            TopScreen.InstantShow();
        }

        /// <summary>
        /// Navigates to given screen.
        /// </summary>
        /// <param name="path">Path to screen to navigate to.</param>
        /// <param name="arguments">Arguments for screen when loading content.</param>
        internal void NavigateToInternal(String path, Boolean back, Object[] arguments)
        {
            if (path == null)
            {
                RemoveAll(true, TransitionType.None);
            }
            else
            {
                if (String.IsNullOrEmpty(_startupScreen))
                {
                    _startupScreen = path;
                }

                String unifiedPath = PathHelper.UnifySeparators(path);
                
                RemoveAll(false, TransitionType.None);

                Screen screen = AddScreen(unifiedPath, arguments, back);

                if (screen == null)
                {
                    return;
                }

                if (screen.ScreenTransition >= TransitionType.SlideLeft)
                {
                    for (Int32 idx = 0; idx < _screens.Count; ++idx)
                    {
                        var screenToRemove = _screens[idx];

                        if (screenToRemove.State == Screen.ScreenState.TransitionOut)
                        {
                            screenToRemove.UpdateCurrentTransition(screen.ScreenTransitionInvert, -1, screen.TimeTransitionIn);
                        }
                    }
                }

                if ( screen.ClearNavigation )
                {
                    _navigation.Clear();
                }

                if (!screen.DisableHistory)
                {
                    _navigation.Add(new KeyValuePair<String, Object[]>(path, arguments));
                }
                else
                {
                    _navigation.Add(new KeyValuePair<String, Object[]>(null, null));
                }
            }
        }

        /// <summary>
        /// Navigates to given screen.
        /// </summary>
        /// <param name="path">Path to screen to navigate to.</param>
        /// <param name="arguments">Arguments for screen when loading content.</param>
        public void NavigateTo(String path, params Object[] arguments)
        {
            NavigateToInternal(path, false, arguments);
        }

        /// <summary>
        /// Navigates to given screen.
        /// </summary>
        /// <param name="path">Path to screen to navigate to.</param>
        /// <param name="arguments">Arguments for screen when loading content.</param>
        public void SwitchTo(String path, params Object[] arguments)
        {
            TopScreen.InstantHide();
            NavigateToInternal(path, false, arguments);
            TopScreen.InstantShow();
        }

        public void NavigateMenu(String path)
        {
            NavigateMenu(path, null);
        }

        public void NavigateMenu(String path, params Object[] arguments)
        {
            Screen screen;

            // For each screen call Remove method to remove screen
            for (Int32 idx = 0; idx < _menuScreens.Count; ++idx)
            {
                screen = _menuScreens[idx];
                screen.Remove();
            }

            // Create desired screen.
            screen = Screen.LoadMenu(path, this, arguments);

            // Add screen to list
            _menuScreens.Add(screen);

            _menuScreens.Sort( (s1,s2) => s1.Zorder - s2.Zorder );

            // Call OnAdded callback
            screen.OnAdded();

            if (_menuScreens.Count == 1)
            {
                screen.InstantShow();
            }
        }

        /// <summary>
        /// Navigates back to previous screen.
        /// </summary>
        internal void NavigateBack()
        {
            TransitionType newTransition = TransitionType.None;
            TransitionType removeTransition = TransitionType.None;
            Double outTime = -1;

            if (TopScreen != null)
            {
                newTransition = TopScreen.ScreenTransitionInvert;
                removeTransition = TopScreen.ScreenTransition;
                outTime = TopScreen.TimeTransitionOut;
            }

            if (_navigation.Count == 1)
            {
                Close();
                return;
            }

            if (_navigation.Count > 0)
            {
                _navigation.RemoveAt(_navigation.Count - 1);
            }

            while (_navigation.Count > 0)
            {
                if (_navigation[_navigation.Count - 1].Key == null)
                {
                    _navigation.RemoveAt(_navigation.Count - 1);
                }
                else
                {
                    RemoveAll(false, removeTransition);
                    var screen = AddScreen(_navigation[_navigation.Count - 1].Key, _navigation[_navigation.Count - 1].Value, true);
                    screen.UpdateCurrentTransition(newTransition, outTime, -1);
                    break;
                }
            }

            if (_navigation.Count == 0)
            {
                RemoveAll(true, removeTransition);
            }
        }

        /// <summary>
        /// Navigates back until selected screen. If no path found, removes all screens.
        /// </summary>
        /// <param name="path">Path to navigate back to.</param>
        internal void NavigateBack(String[] paths)
        {
            TransitionType newTransition = TransitionType.None;
            TransitionType removeTransition = TransitionType.None;
            Double outTime = -1;

            if (TopScreen != null)
            {
                newTransition = TopScreen.ScreenTransitionInvert;
                removeTransition = TopScreen.ScreenTransition;
                outTime = TopScreen.TimeTransitionOut;
            }

            while (_navigation.Count > 0)
            {
                _navigation.RemoveAt(_navigation.Count - 1);

                if (_navigation.Count > 0)
                {
                    foreach (var path in paths)
                    {
                        if (_navigation[_navigation.Count - 1].Key != null && _navigation[_navigation.Count - 1].Key == path)
                        {
                            RemoveAll(false, removeTransition);
                            var screen = AddScreen(_navigation[_navigation.Count - 1].Key, _navigation[_navigation.Count - 1].Value, true);

                            screen.UpdateCurrentTransition(newTransition, outTime, -1);

                            return;
                        }
                    }
                }
                else
                {
                    RemoveAll(true, removeTransition);
                }
            }
        }

        /// <summary>
        /// Adds new screen to manager
        /// </summary>
        /// <param name="screen">Screen to add to manager</param>
        private Screen AddScreen(String path, Object[] arguments, Boolean back)
        {
            try
            {
                // Create desired screen.
                Screen screen = Screen.Load(path, this, arguments, back);

                // Set proper background screen.
                SwitchBackground(screen.BackgroundPath, screen);

                // Add screen to list
                _screens.Add(screen);

                // Call OnAdded callback
                screen.OnAdded();

                return screen;
            }
            catch(CancelScreenAddException)
            {
                return null;
            }
        }

        /// <summary>
        /// Removes all screens.
        /// </summary>
        private void RemoveAll(Boolean removeBackground, TransitionType removeTransition)
        {
            // For each screen call Draw method to draw its stuff
            for (Int32 idx = 0; idx < _screens.Count; ++idx)
            {
                var screen = _screens[idx];

                if (removeBackground || !(screen is ScreenBackground))
                {
                    screen.Remove(removeTransition);
                }
            }
        }

        /// <summary>
        /// Switch background for all screens.
        /// </summary>
        /// <param name="path">Path of new background screen definition.</param>
        private void SwitchBackground(String path, Screen caller)
        {
            // Where to put new background screen.
            Int32 putAt = _screens.Count;

            // New background screen.
            Screen screen = null;

            // Convert path to be matchable with already loaded screens' paths.
            String matchPath = path;

            for (Int32 idx = 0; idx < _screens.Count; ++idx)
            {
                if (_screens[idx] is ScreenBackground)
                {
                    ScreenBackground current = _screens[idx] as ScreenBackground;

                    if (current.State == Screen.ScreenState.Visible || current.State == Screen.ScreenState.TransitionIn)
                    {
                        if (!_forceSwitchBackground && current.IsBackgroundMatching(matchPath))
                        {
                            return;
                        }
                    }

                    _screens[idx].Remove();
                    putAt = idx + 1;
                }
            }

            if (path != "")
            {
                screen = Screen.Load(path, this, new Object[]{caller}, false);

                if (screen is ScreenBackground)
                {
                    _screens.Insert(putAt, screen);
                    screen.OnAdded();
                }
            }
        }

        public virtual void OnAppActivated()
        {
            if (MusicControler.Instance != null)
            {
                MusicControler.Instance.OnActivate();
            }

            Screen[] screensToNotify = _screens.ToArray();

            // For each screen call OnAppActivated method to draw its stuff
            foreach (var screen in screensToNotify)
            {
                screen.UpdateAfterResume();
                screen.OnAppActivated();
            }

            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 59);

            for (Int32 idx = _popupScreens.Count - 1; idx >= 0; --idx)
            {
                _popupScreens[idx].UpdateScreen (timeSpan);

                if (_popupScreens[idx].State == Screen.ScreenState.ToRemove)
                {
                    Screen screen = _popupScreens[idx];
                    _popupScreens.RemoveAt(idx);
                    screen.OnRemoved();
                }
            }

            // Iterate thru al screens and call Update for'em
            for (Int32 idx = 0; idx < _screens.Count; )
            {
                Screen screen = _screens[idx];

                screen.UpdateScreen(timeSpan);

                // If screen should be removed, remove screen and call OnRemoved handler for screen
                if (screen.State == Screen.ScreenState.ToRemove)
                {
                    _screens.RemoveAt(idx);
                    screen.OnRemoved();
                }
                // otherwise increment screen index and continue loop
                else
                {
                    ++idx;
                }
            }
        }

        public virtual void OnAppDeactivated()
        {
            if (MusicControler.Instance != null)
            {
                MusicControler.Instance.OnDeactivate();
            }

            Screen[] screensToNotify = _screens.ToArray();

            // For each screen call OnAppActivated method to draw its stuff
            foreach (var screen in screensToNotify)
            {
                screen.OnAppDeactivated();
            }
        }

        public void OnSizeChanged(Point areaSize)
        {
            if ( GraphicsDevice == null )
            {
                return;
            }

            AreaSize = GraphicsHelper.Vector2FromPoint(areaSize);

            for ( Int32 idx = 0; idx < _screens.Count; ++idx )
            {
                _screens[idx].OnRemoved();
            }

            _screens.Clear();

            AddScreen(_navigation[_navigation.Count - 1].Key, _navigation[_navigation.Count - 1].Value, false);

            for ( Int32 idx = 0; idx < _screens.Count; ++idx )
            {
                _screens[idx].InstantShow();
            }

            if ( _popupScreens.Count > 0 )
            {
                String path = _popupScreens[_popupScreens.Count - 1].LoadPath;

                for ( Int32 idx = 0; idx < _popupScreens.Count; ++idx )
                {
                    _popupScreens[idx].OnRemoved();
                }

                _popupScreens.Clear();

                var popup = ShowPopupInternal(path, null);

                if( popup != null )
                {
                    popup.InstantShow();
                }
            }
        }

        public Int32 AddInfoBar(InfoBarSettings infoBarSettings)
        {
            _infoBars.Add(new InfoBar(infoBarSettings));
            return _infoBars.Count - 1;
        }

        public void Close()
        {
            _close = true;
        }

        internal void AddInstance(GuiElement element)
        {
            String name = element.Instance;

            if (String.IsNullOrEmpty(name))
            {
                element.FirstInstance = element;
                element.SecondInstance = null;
                return;
            }

            if (_instances.ContainsKey(name))
            {
                Pair<GuiElement, GuiElement> pair;
                _instances.TryGetValue(name, out pair);

                if (pair.Second != null)
                {
                    pair = new Pair<GuiElement, GuiElement>(pair.Second, null);
                }

                _instances.Remove(name);

                pair = new Pair<GuiElement, GuiElement>(pair.First, element);

                pair.First.FirstInstance = pair.First;
                pair.First.SecondInstance = pair.Second;

                pair.Second.FirstInstance = pair.First;
                pair.Second.SecondInstance = pair.Second;

                _instances.Add(name, pair);
            }
            else
            {
                _instances.Add(name, new Pair<GuiElement, GuiElement>(element, null));

                element.FirstInstance = element;
                element.SecondInstance = null;
            }
        }

        internal void RemoveInstance(GuiElement element)
        {
            String name = element.Instance;

            if (String.IsNullOrEmpty(name))
            {
                return;
            }

            if (_instances.ContainsKey(name))
            {
                Pair<GuiElement, GuiElement> pair;
                _instances.TryGetValue(name, out pair);

                if (pair.Second == element)
                {
                    if (pair.First != null)
                    {
                        pair.First.FirstInstance = pair.First;
                        pair.First.SecondInstance = null;
                    }

                    pair = new Pair<GuiElement, GuiElement>(pair.First, null);
                }
                else if (pair.First == element)
                {
                    if (pair.Second != null)
                    {
                        pair.Second.FirstInstance = pair.Second;
                        pair.Second.SecondInstance = null;
                    }
                    pair = new Pair<GuiElement, GuiElement>(pair.Second, null);
                }
                else
                {
                    return;
                }

                _instances.Remove(name);

                if (pair.First != null)
                {
                    _instances.Add(name, pair);
                }
            }
        }

        public void Redraw()
        {
            lock (_redrawLock)
            { 
                _redrawMark = true;
            }
        }

        public Boolean IsPopupVisible
        {
            get
            {
                return _popupScreens.Count > 0;
            }
        }

        public Boolean IsMenuVisible
        {
            get
            {
                return _showMenu || ScreensOffsetIfMenu != Vector2.Zero;
            }
        }

        public Boolean IsTopScreen(Screen screen)
        {
            if ( screen.IsMenuScreen)
            {
                return _menuScreens.Count > 0 && _menuScreens[_menuScreens.Count - 1] == screen;
            }
            else
            {
                return _screens.Count > 0 && _screens[_screens.Count - 1] == screen;
            }
        }

        public void UiAction( EmptyArgsVoidDelegate del)
        {
            lock (_dispatcherActions)
            {
                _dispatcherActions.Add(new Action(del));
            }
        }

        public void AdditionalDraw( EmptyArgsVoidDelegate function, Rectangle clip)
        {
            _additionalDraws.Add(new AdditionalDraw(function, clip));
        }

        public  Boolean PopAdditionalDraw(out AdditionalDraw additionalDraw)
        {
            if (_additionalDraws.Count > 0)
            { 
                additionalDraw = _additionalDraws[0];
                _additionalDraws.RemoveAt(0);
                return true;
            }

            additionalDraw = new AdditionalDraw();
            return false;
        }

        internal void AddDelayedResourceLoad(LoadResourceInfo info)
        {
            if (info != null)
            {
                if (!ContentLoader.Current.IsContentLoaded(info.ContentType, info.Path))
                {
                    _delayedResourceLoad.Insert(Random.Next(_delayedResourceLoad.Count + 1), info);
                }
            }
        }

		internal void Release()
		{
			foreach (var screen in _popupScreens)
			{
				screen.OnRemoved();
			}

			foreach (var screen in _screens)
			{
				screen.OnRemoved();
			}

			foreach (var screen in _menuScreens)
			{
				screen.OnRemoved();
			}

			_delayedResourceLoad.Clear();

			lock (_dispatcherActions)
			{
				_dispatcherActions.Clear();
			}

			Current = null;
		}
    }
}
