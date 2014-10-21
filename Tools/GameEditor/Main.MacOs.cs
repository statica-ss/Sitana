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

            UiUnit.Unit = 1;
            UiUnit.FontUnit = 1;
            UiUnit.EnableFontScaling = false;

            StylesManager.Instance.LoadStyles("Ui/AppStyles", true);
            _appMain.LoadView("Ui/MainView");

            _appMain.Window.AllowUserResizing = true;

            _appMain.Graphics.IsFullScreen = false;

            _appMain.IsMouseVisible = true;
            _appMain.OnLoadContent += MainController.OnLoadContent;
            _appMain.OnLoadedView += (a) => a.ResizeToView();

            _appMain.Run();

        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
    }
}


