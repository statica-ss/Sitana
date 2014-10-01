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

namespace Sitana.Framework.Ui.Core
{
    public partial class AppMain: Game
    {
        public delegate void LoadDelegate(AppMain main);


        public static AppMain Current { get; private set; }

        private GraphicsDeviceManager _graphics;

        private AdvancedDrawBatch _drawBatch;

        public event LoadDelegate OnLoadContent;
        public event LoadDelegate OnLoadedView;

        private Point _lastSize = Point.Zero;

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
            Draw();
            GraphicsDevice.Viewport = viewport;
        }

        public bool Draw()
        {
            if (_drawBatch == null)
            {
                return false;
            }

            var drawParameters = new UiViewDrawParameters()
            {
                DrawBatch = _drawBatch
            };

            GraphicsDevice.Clear(MainView.BackgroundColor);
            MainView.ViewDraw(ref drawParameters);
            _drawBatch.Flush();

            return true;
        }

        protected override void Update(GameTime gameTime)
        {
            UiTask.Process();

            var newSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if (_lastSize != newSize)
            {
                if (MainView != null)
                {
                    OnSize(newSize.X, newSize.Y);
                }

                _lastSize = newSize;
            }

            TouchPad.Instance.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            MainView.ViewUpdate((float)gameTime.ElapsedGameTime.TotalSeconds);
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
            MainView.CreatePositionParameters(MainView.Controller, null, _mainView, typeof(PositionParameters));

            MainView.RecalculateAll();

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
    }
}
