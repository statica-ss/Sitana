using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Content;
using Sitana.Framework;

namespace GameEditor
{
    static class Program
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
         
            using( var p = new NSAutoreleasePool() ) {
                NSApplication.SharedApplication.Delegate = new AppDelegate();
                NSApplication.Main(args);
            }
        }
    }

    class AppDelegate : NSApplicationDelegate
    {
        AppMain _appMain;

        public override void FinishedLaunching(MonoMac.Foundation.NSObject notification)
        {
            _appMain = new AppMain();

            _appMain.Window.Window.CollectionBehavior = NSWindowCollectionBehavior.FullScreenPrimary;

            ContentLoader.Init(_appMain.Services, "Assets");

            StylesManager.Instance.LoadStyles("AppStyles", true);

            _appMain.LoadView("MainView");

            _appMain.Window.AllowUserResizing = true;

            _appMain.CanClose = (a) =>
            {
                if (!MainController.Current.CanClose)
                {
                    MainController.Current.Exit();
                }

                return MainController.Current.CanClose;
            };

            _appMain.Graphics.IsFullScreen = false;

            if (EditorSettings.Instance.WindowWidth != 0 || EditorSettings.Instance.WindowHeight != 0)
            {
                _appMain.Graphics.PreferredBackBufferWidth = EditorSettings.Instance.WindowWidth;
                _appMain.Graphics.PreferredBackBufferHeight = EditorSettings.Instance.WindowHeight;
            }

            _appMain.IsMouseVisible = true;
            _appMain.OnLoadContent += MainController.OnLoadContent;

            _appMain.OnLoadedView += (a) =>
            {
                if (EditorSettings.Instance.WindowWidth == 0 || EditorSettings.Instance.WindowHeight == 0)
                {
                    a.ResizeToView();
                }
            };

            _appMain.Resized += (w, h) =>
            {
                if ( !EditorSettings.Instance.Fullscreen )
                {
                    EditorSettings.Instance.WindowWidth = w;
                    EditorSettings.Instance.WindowHeight = h;
                }
            };

            _appMain.Window.Window.WillEnterFullScreen += (o, e) =>
            {
                EditorSettings.Instance.Fullscreen = true;
            };

            _appMain.Window.Window.WillExitFullScreen += (o, e) =>
            {
                EditorSettings.Instance.Fullscreen = false;
            };

            _appMain.Window.Window.WillClose += (o, e) => EditorSettings.Instance.Serialize();

            _appMain.InactiveSleepTime = TimeSpan.FromSeconds(1);

            _appMain.Run();

            if ( EditorSettings.Instance.Fullscreen )
            {
                _appMain.Window.Window.ToggleFullScreen(null);
            }
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
    }
}


