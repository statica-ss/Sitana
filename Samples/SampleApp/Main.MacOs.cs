using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Content;
using Sitana.Framework;

namespace SampleApp
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
            _appMain.Window.Window.DidResize += Window_DidResize;

            ContentLoader.Init(_appMain.Services, "Assets");

            StylesManager.Instance.LoadStyles("Ui/AppStyles", true);
            _appMain.LoadView("Ui/MainView");

            _appMain.Window.AllowUserResizing = true;

            _appMain.Graphics.IsFullScreen = false;

            _appMain.IsMouseVisible = true;
            _appMain.ContentLoading += MainController.OnLoadContent;

            _appMain.Run();
        }

        void Window_DidResize(object sender, EventArgs e)
        {
            UiTask.BeginInvoke(() =>
                {
                    double unit = Math.Min((double)AppMain.Current.GraphicsDevice.Viewport.Width / 640.0,
                        (double)AppMain.Current.GraphicsDevice.Viewport.Height / 480.0);

                    unit = Math.Min(1, unit);

                    UiUnit.FontUnit = UiUnit.Unit = unit;

                    _appMain.SizeChanged();
                });
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
    }
}


