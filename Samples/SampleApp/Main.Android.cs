using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Content;

namespace SampleApp
{
    [Activity (
        Label="SampleApp",
        MainLauncher = true,
        Icon = "@drawable/Icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        ScreenOrientation = ScreenOrientation.Portrait | ScreenOrientation.Landscape)
    ]
    public class MainActivity : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var main = new AppMain();

            ContentLoader.Init(main.Services, string.Empty);
            StylesManager.Instance.LoadStyles("Ui/AppStyles", true);

            main.LoadView("Ui/MainView");

            main.Graphics.IsFullScreen = true;
            main.Graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            main.ContentLoading += MainController.OnLoadContent;
            main.Run();
        }
    }
}


