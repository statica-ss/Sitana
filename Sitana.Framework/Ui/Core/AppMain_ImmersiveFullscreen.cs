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
		Activity _activity;

		public AppMain_ImmersiveFullscreen(Activity activity): this(activity, true)
		{
			_activity = activity;

			Display display = activity.WindowManager.DefaultDisplay;

			global::Android.Graphics.Point outSize = new global::Android.Graphics.Point();
			display.GetRealSize(outSize);

			_width = outSize.X;
			_height = outSize.Y;

			_activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)Platform.ImmersiveModeFlags;
			_activity.Window.DecorView.RequestLayout();
		}

		public AppMain_ImmersiveFullscreen(Activity activity, bool addToLayout): base()
		{
			_activity = activity;

			Display display = activity.WindowManager.DefaultDisplay;

			global::Android.Graphics.Point outSize = new global::Android.Graphics.Point();
			display.GetRealSize(outSize);

			_width = outSize.X;
			_height = outSize.Y;

			_activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)Platform.ImmersiveModeFlags;
			_activity.Window.DecorView.RequestLayout();

			Graphics.PreferredBackBufferWidth = activity.Window.DecorView.Width;
			Graphics.PreferredBackBufferHeight = activity.Window.DecorView.Height;

			if (addToLayout)
			{
				View view = (View)Services.GetService(typeof(View));
				_activity.SetContentView(view);
			}
		}

		protected override void OnSize(int width, int height)
		{
			if (width > height)
			{
				width = Math.Max(_width, _height);
				height = Math.Min(_width, _height);
			} 
			else
			{
				width = Math.Min(_width, _height);
				height = Math.Max(_width, _height);
			}

			var rect = new Rectangle(0, 0, width, height);

			PerformanceProfiler.Instance.ComputeContentRect(ref rect);

			MainView.Bounds = rect;

			CallResized(rect.Width, rect.Height);

		}

		private bool IsSizeOk(int width, int height)
		{
			return (width == _width && height == _height) ||
				   (height == _width && width == _height);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_checkTime -= gameTime.ElapsedGameTime.TotalSeconds;

			if (_checkTime >= 0)
			{
				if (!IsSizeOk(_activity.Window.DecorView.Width,_activity.Window.DecorView.Height))
				{
					_activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)Platform.ImmersiveModeFlags;
					_activity.Window.DecorView.RequestLayout();
				}

				_checkTime = 1;
			}
		}
	}
}

