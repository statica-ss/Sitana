// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Input.TouchPad;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Content;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Ui.Interfaces;
using System.Collections.Generic;
using Sitana.Framework.Input;
using Sitana.Framework.Input.GamePad;
using Sitana.Framework.Cs;
using Sitana.Framework.Media;
using Sitana.Framework.Misc;
using System.Threading;

namespace Sitana.Framework.Ui.Core
{
    public partial class AppMain: Game, IGestureListener
    {
        public delegate void LoadDelegate(AppMain main);
        public delegate bool CloseDelegate(AppMain main);
        public delegate void ResizedDelegate(int width, int height);

        public static AppMain Current { get; private set; }

        private GraphicsDeviceManager _graphics;

        private AdvancedDrawBatch _drawBatch;

        public event LoadDelegate ContentLoading;
        public event LoadDelegate ViewLoaded;
        public event ResizedDelegate Resized;

        public event EmptyArgsVoidDelegate AppActivated;
        public event EmptyArgsVoidDelegate AppDeactivated;

        public CloseDelegate CanClose { get; set;}

        private Point _lastSize = Point.Zero;

        private int _desiredVerticalOffset = 0;
        private float _currentVerticalOffset = 0;

        private int _top = 0;
        private bool _inited = false;

        public double TotalGameTime { get; private set; }

        public bool TouchTestEnable = false;

        public List<IUpdatable> _updatables = new List<IUpdatable>();

        public int RedrawFrequency
        {
            set
            {
                if (value == 0)
                {
					_redrawPeriod = 0;
                }
                else
                {
                    _redrawPeriod = 1.0 / (double)value;
                }
            }
        }

        IFocusable _currentFocus = null;

        double _redrawPeriod = 0;
        double _timeTillRedraw = 0;

        bool _shouldRedraw = false;
        bool _redrawInNextFrame = false;

        public GraphicsDeviceManager Graphics
        {
            get
            {
                return _graphics;
            }
        }

        public AdvancedDrawBatch DrawBatch
        {
            get
            {
                return _drawBatch;
            }
        }

        public UiContainer MainView { get; private set; }

        private DefinitionFile _mainView;

        float _cumulativeFrameTime = 0;

        // Move this to proper partial class!
        #if !ANDROID
        public AppMain()
        {
            Init();
        }
        #endif

        int _viewUpdateCounter = 0;
        int _viewDrawCounter = 0;

        int _fullUpdateCounter = 0;

        protected void Init()
        {
            if (Current != null)
            {
                throw new Exception("There can be only one AppMain instance.");
            }

            Current = this;

            InactiveSleepTime = TimeSpan.FromMilliseconds(100);

            IsFixedTimeStep = false;

            _graphics = new GraphicsDeviceManager(this);

            Graphics.IsFullScreen = true;
            Graphics.SynchronizeWithVerticalRetrace = true;

            MusicController.Instance.Initialize();
            TouchPad.Instance.AddListener(this);

            TotalGameTime = 0;

            PlatformInit();

            _fullUpdateCounter = PerformanceProfiler.Instance.AddCounter(10);
            _viewUpdateCounter = PerformanceProfiler.Instance.AddCounter(10);
            _viewDrawCounter = PerformanceProfiler.Instance.AddCounter(10);

            PerformanceProfiler.Instance.EnableCounter(_fullUpdateCounter, true);
            PerformanceProfiler.Instance.EnableCounter(_viewUpdateCounter, true);
            PerformanceProfiler.Instance.EnableCounter(_viewDrawCounter, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (MainView != null)
            {
                MainView.ViewRemoved();
                MainView = null;
            }

            base.Dispose(disposing);
            Current = null;
        }

        void DrawTouchTest()
        {
            var touchElements = TouchPad.Instance.TouchElements;

            foreach (var element in touchElements)
            {
                Point p0 = element.Value.Position.ToPoint();
                
                Point pL = p0;
                Point pR = p0;

                Point pT = p0;
                Point pB = p0;

                pL.X = 0;
                pR.X = 10000;

                pT.Y = 0;
                pB.Y = 10000;

                _drawBatch.DrawLine(pL, pR, Color.Red);
                _drawBatch.DrawLine(pT, pB, Color.Red);
            }

            _drawBatch.Flush();
        }

        protected override void Draw(GameTime gameTime)
        {
			if(!IsActive)
			{
				return;
			}

            if (disableUpdate > 0)
            {
                PerformanceProfiler.Instance.BeginCounter(_viewDrawCounter);
                if (MainView != null)
                {
                    MainView.ResetViewDisplayed();
                }

                Viewport viewport = GraphicsDevice.Viewport;

                Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
                GraphicsDevice.Viewport = viewport;

                if (MainView != null)
                {
                    MainView.ProcessAfterDraw();
                }
                PerformanceProfiler.Instance.EndCounter(_viewDrawCounter);

                if (_drawBatch != null && TouchTestEnable)
                {
                    DrawTouchTest();
                }
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
            }

            if (_drawBatch != null)
            {
                PerformanceProfiler.Instance.Draw(_drawBatch);
                _drawBatch.Flush();
            }


        }

        public bool Draw(float ellapsedTime)
        {
            if (_drawBatch == null)
            {
                return false;
            }

            var drawParameters = new UiViewDrawParameters()
            {
                DrawBatch = _drawBatch,
                Opacity = 1,
                Transition = 0,
                EllapsedTime = ellapsedTime
            };
            
            GraphicsDevice.Clear(MainView.BackgroundColor);

            _drawBatch.Reset();
            MainView.ViewDraw(ref drawParameters);
            _drawBatch.Flush();

            return true;
        }

		double updatesCount = 0;
		double secTime = 0;

        double disableUpdate = 10;

		protected override void Update(GameTime gameTime)
        {
            PerformanceProfiler.Instance.Update(gameTime.ElapsedGameTime);
            PerformanceProfiler.Instance.BeginCounter(_fullUpdateCounter);

			if(!IsActive)
			{
				return;
			}

            bool shouldRedraw = _redrawInNextFrame;
            _redrawInNextFrame = false;

            _timeTillRedraw -= gameTime.ElapsedGameTime.TotalSeconds;

            if(_timeTillRedraw <= 0)
            {
                _timeTillRedraw = _redrawPeriod;
                shouldRedraw = true;
            }

            _shouldRedraw = false;

            PlatformUpdate(gameTime);

            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            TotalGameTime = gameTime.TotalGameTime.TotalSeconds;

            //Accelerators.Instance.Process(_currentFocus == null);
            if (disableUpdate > 0)
            {
                UiTask.Process();
                DelayedActionInvoker.Instance.Update(time);

                var newSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

                if (_lastSize != newSize)
                {
                    if (MainView != null)
                    {
                        OnSize(newSize.X, newSize.Y);
                        _top = MainView.Bounds.Top;
                    }

                    _lastSize = newSize;
                    _shouldRedraw = true;
                    Redraw(true);
                    RedrawNextFrame();
                }
            }

            if (_currentVerticalOffset != _desiredVerticalOffset)
            {
                if (time >= 0.2)
                {
                    _currentVerticalOffset = _desiredVerticalOffset;
                }
                else
                {
                    _currentVerticalOffset = _desiredVerticalOffset * time * 5 + (1 - time * 5) * _currentVerticalOffset;
                    _currentVerticalOffset = _desiredVerticalOffset * time * 5 + (1 - time * 5) * _currentVerticalOffset;
                    _currentVerticalOffset = _desiredVerticalOffset * time * 5 + (1 - time * 5) * _currentVerticalOffset;
                    _currentVerticalOffset = _desiredVerticalOffset * time * 5 + (1 - time * 5) * _currentVerticalOffset;
                }

                if (Math.Abs(_currentVerticalOffset - _desiredVerticalOffset) < 10)
                {
                    _currentVerticalOffset = _desiredVerticalOffset;
                }
            }

            if (MainView != null && MainView.OffsetBoundsVertical != (int)_currentVerticalOffset + _top)
            {
                MainView.OffsetBoundsVertical = (int)_currentVerticalOffset + _top;
            }

            if (disableUpdate > 0)
            {
                TouchPad.Instance.Update(time, IsActive); 
                GamePads.Instance.Update();
            }

			bool shouldUpdate = shouldRedraw;
			_cumulativeFrameTime += time;

            //disableUpdate -= gameTime.ElapsedGameTime.TotalSeconds;

            if (shouldUpdate && disableUpdate >0)
            {
				for(int idx = 0; idx < _updatables.Count; ++idx)
				{
					_updatables[idx].Update(time);
				}

				if (MainView != null)
				{
                    PerformanceProfiler.Instance.BeginCounter(_viewUpdateCounter);
					MainView.ViewUpdate(_cumulativeFrameTime);
                    PerformanceProfiler.Instance.EndCounter(_viewUpdateCounter);
				}
                _cumulativeFrameTime = 0;
				updatesCount++;
            }

			secTime += gameTime.ElapsedGameTime.TotalSeconds;

			if (secTime >= 1)
			{
				Console.WriteLine("Updates per sec: {0:0.00}", updatesCount / secTime);
				updatesCount -= updatesCount / secTime;
				secTime = 0;
			}

			_redrawInNextFrame |= _shouldRedraw;
            shouldRedraw |= _shouldRedraw;

            if (!shouldRedraw)
            {
                SuppressDraw();
#if !WINDOWS_PHONE_APP
                Thread.Sleep(10);
#endif
            }

            PerformanceProfiler.Instance.EndCounter(_fullUpdateCounter);
        }

        public void ReloadView(string path)
        {
            DefinitionFile file = ContentLoader.Current.Load<DefinitionFile>(path);
            _mainView = file;

            IDefinitionClass obj = _mainView.CreateInstance(null, null);

            MainView = (UiContainer)obj;

            MainView.RecalculateAll();

            MainView.RegisterView();
            MainView.ViewAdded();

            
            ViewLoaded?.Invoke(this);
            

            MainView.Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        protected override void LoadContent()
        {
            _inited = true;
            if (_mainView == null)
            {
                return;
            }

            _drawBatch = new AdvancedDrawBatch(GraphicsDevice);

            ContentLoading?.Invoke(this);

            IDefinitionClass obj = _mainView.CreateInstance(null, null);

            MainView = (UiContainer)obj;

            MainView.RecalculateAll();

            MainView.RegisterView();
            MainView.ViewAdded();

            ViewLoaded?.Invoke(this);

            MainView.Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            _shouldRedraw = true;
            if (MainView != null)
            {
                MainView.ViewActivated();
            }

            TouchPad.Instance.AppDeactivated();

            AppActivated?.Invoke();
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            if (MainView != null)
            {
                MainView.ViewDeactivated();
            }

            TouchPad.Instance.AppDeactivated();

            AppDeactivated?.Invoke();
        }

        public void LoadView(string path)
        {
            DefinitionFile file = ContentLoader.Current.Load<DefinitionFile>(path);
            _mainView = file;

            if(_inited)
            {
                LoadContent();
            }
        }

        public void SizeChanged()
        {
            _lastSize = Point.Zero;
            Redraw(true);
        }

        public void RegisterUpdatable(IUpdatable updatable)
        {
            _updatables.Add(updatable);
        }

        public void UnregisterUpdatable(IUpdatable updatable)
        {
            _updatables.Remove(updatable);
        }

        public int SetFocus(IFocusable focus)
        {
            if ( _currentFocus != null )
            {
                _currentFocus.Unfocus();
            }
                
            _currentFocus = focus;
            return ComputeFocus();
        }

        public void ReleaseFocus(IFocusable focus)
        {
            if ( _currentFocus == focus )
            {
                _currentFocus = null;
                focus.Unfocus();
            }

            ComputeFocus();
        }

        int ComputeFocus()
        {
            if(_currentFocus != null)
            {
                int offset = GraphicsDevice.Viewport.Height - Platform.KeyboardHeight(GraphicsDevice.Viewport.Height < GraphicsDevice.Viewport.Width);

                offset -= _currentFocus.Bottom;
                offset = Math.Min(0, offset);

                _desiredVerticalOffset = offset;
                _currentVerticalOffset = offset;
                MainView.OffsetBoundsVertical = offset;
                return offset;
            }
            else
            {
                _desiredVerticalOffset = 0;
            }
            return 0;
        }

        protected void CallResized(int width, int height)
        {
            Resized?.Invoke(width, height);
        }

        public void CloseApp()
        {
#if __ANDROID__
            Activity.MoveTaskToBack(true);
#elif __IOS__
            throw new NotImplementedException("Cannot exit app on iOS.");
#else
            base.Exit();
#endif
        }

        public new void Exit()
        {
            throw new Exception("Don't call Exit! Call CloseApp instead.");
        }

        void IGestureListener.OnGesture(Gesture gesture)
        {
            switch(gesture.GestureType)
            {
                case GestureType.Down:
                case GestureType.Up:
                    Redraw(true);
                    break;

                case GestureType.Move:
                    if(gesture.Offset != Vector2.Zero)
                    {
                        Redraw(true);
                    }
                    break;
            }

            MainView.ViewGesture(gesture);
        }

		public static void Redraw(UiView caller)
        {
			if(caller != null && caller.IsViewDisplayed)
			{
            	Current._shouldRedraw = true;
			}
        }

		public static void Redraw(bool test)
		{
			Current._shouldRedraw = true;
		}

        public static void RedrawNextFrame()
        {
            Current._redrawInNextFrame = true;
        }
    }
}
