using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Content;
using Microsoft.Xna.Framework;

namespace TestApp
{
	[Register("AppDelegate")]
	class Program : UIApplicationDelegate
	{
		public override void FinishedLaunching(UIApplication app)
		{
			var main = new AppMain();

			ContentLoader.Init(main.Content, "Assets");

            StylesManager.Instance.LoadStyles("Ui/AppStyles", true);

			main.LoadView("Ui/MainView");

			main.Graphics.IsFullScreen = true;
			main.Graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight | DisplayOrientation.PortraitDown;

			main.OnLoadContent += TestController.OnLoadContent;

			main.Run();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			UIApplication.Main(args, null, "AppDelegate");
		}
	} 
}
