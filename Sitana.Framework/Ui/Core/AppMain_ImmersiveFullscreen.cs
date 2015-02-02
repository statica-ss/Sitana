using System;
using Sitana.Framework.Ui.Core;
using Android.App;
using Sitana.Framework.Diagnostics;
using Microsoft.Xna.Framework;
using Android.Views;
using Sitana.Framework.Cs;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.OS;
using System.Threading;
using Android.Views.InputMethods;
using Android.Content.PM;

namespace Sitana.Framework.Ui.Core
{
	public class AppMain_ImmersiveFullscreen: AppMain
	{
		int _width;
		int _height;

		double _checkTime = 1;

		public AppMain_ImmersiveFullscreen(): base(false)
		{
			Init();
			Display display = Activity.WindowManager.DefaultDisplay;

			global::Android.Graphics.Point outSize = new global::Android.Graphics.Point();
			display.GetRealSize(outSize);

			_width = outSize.X;
			_height = outSize.Y;

            Activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.ImmersiveSticky|SystemUiFlags.LayoutFullscreen|SystemUiFlags.LayoutHideNavigation|SystemUiFlags.Fullscreen);
			Activity.Window.DecorView.RequestLayout();

			Graphics.PreferredBackBufferWidth = _width;
			Graphics.PreferredBackBufferHeight = _height;

			AddView();
		}
	}
}

