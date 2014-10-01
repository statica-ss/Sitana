using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Content;
using Microsoft.Xna.Framework;

namespace TestGame
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

			main.OnLoadContent += GameController.OnLoadContent;

			// Compute scale factor.
			double scale = Math.Min((double)main.Graphics.PreferredBackBufferWidth / 960.0, (double)main.Graphics.PreferredBackBufferHeight / 640.0);
			scale = Math.Round(scale, 1);

			UiUnit.FontUnit = scale;
			UiUnit.Unit = scale;

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
