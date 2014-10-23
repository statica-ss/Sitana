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

namespace Sitana.Framework.Ui.Core
{
    public partial class AppMain: Game
    {
        public delegate void LoadDelegate(AppMain main);
        public delegate bool CloseDelegate(AppMain main);
        public delegate void ResizedDelegate(int width, int height);

        public static AppMain Current { get; private set; }

        private GraphicsDeviceManager _graphics;

        private AdvancedDrawBatch _drawBatch;

        public event LoadDelegate OnLoadContent;
        public event LoadDelegate OnLoadedView;
        public event ResizedDelegate Resized;

        public CloseDelegate CanClose { get; set;}

        private Point _lastSize = Point.Zero;

        public double TotalGameTime { get; private set; }

        public List<IUpdatable> _updatables = new List<IUpdatable>();

        public GraphicsDeviceManager Graphics
        {
            get
            {
                return _graphics;
            }
        }

        public UiContainer MainView { get; private set; }

        private DefinitionFile _mainView;

        public AppMain()
        {
            if (Current != null)
            {
                throw new Exception("There can be only one AppMain instance.");
            }

            Current = this;

            InactiveSleepTime = TimeSpan.FromMilliseconds(100);

            _graphics = new GraphicsDeviceManager(this);

            Graphics.IsFullScreen = true;
            Graphics.SynchronizeWithVerticalRetrace = true;

            MusicController.Instance.Initialize();

            TotalGameTime = 0;

            PlatformInit();
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

        protected override void Draw(GameTime gameTime)
        {
            Viewport viewport = GraphicsDevice.Viewport;
            Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            GraphicsDevice.Viewport = viewport;
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

            PerformanceProfiler.Instance.Draw(_drawBatch);
            _drawBatch.Flush();

            return true;
        }

        protected override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PerformanceProfiler.Instance.Update(gameTime.ElapsedGameTime);

            TotalGameTime = gameTime.TotalGameTime.TotalSeconds;

            UiTask.Process();
            DelayedActionInvoker.Instance.Update(time);

            var newSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if (_lastSize != newSize)
            {
                if (MainView != null)
                {
                    OnSize(newSize.X, newSize.Y);
                }

                _lastSize = newSize;
            }


            TouchPad.Instance.Update(time, IsActive);


            for(int idx = 0; idx < _updatables.Count; ++idx)
            {
                _updatables[idx].Update(time);
            }

            MainView.ViewUpdate(time);
        }

        protected override void LoadContent()
        {
            _drawBatch = new AdvancedDrawBatch(GraphicsDevice);

            if (OnLoadContent != null)
            {
                OnLoadContent(this);
            }

            IDefinitionClass obj = _mainView.CreateInstance(null, null);

            MainView = (UiContainer)obj;

            MainView.RecalculateAll();

            MainView.RegisterView();
            MainView.ViewAdded();

            if ( OnLoadedView != null )
            {
                OnLoadedView(this);
            }

            MainView.Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            if (MainView != null)
            {
                MainView.ViewActivated();
            }
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            if (MainView != null)
            {
                MainView.ViewDeactivated();
            }
        }

        public void LoadView(string path)
        {
            DefinitionFile file = ContentLoader.Current.Load<DefinitionFile>(path);
            _mainView = file;
        }

        public void SizeChanged()
        {
            _lastSize = Point.Zero;
        }

        public void RegisterUpdatable(IUpdatable updatable)
        {
            _updatables.Add(updatable);
        }

        public void UnregisterUpdatable(IUpdatable updatable)
        {
            _updatables.Remove(updatable);
        }
    }
}
