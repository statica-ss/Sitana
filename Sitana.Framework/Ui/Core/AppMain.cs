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

        public UiView MainView { get; set; }

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
            var newSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if (_lastSize != newSize)
            {
                if (MainView != null)
                {
                    MainView.Bounds = new Rectangle(0, 0, newSize.X, newSize.Y);
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

            MainView = (UiView)obj;
            MainView.CreatePositionParameters(MainView.Controller, null, _mainView, typeof(PositionParameters));

            (MainView as UiContainer).RecalculateAll();

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
            XNode node = ContentLoader.Current.Load<XFile>(path);
            _mainView = DefinitionFile.LoadFile(node);
        }
    }
}
