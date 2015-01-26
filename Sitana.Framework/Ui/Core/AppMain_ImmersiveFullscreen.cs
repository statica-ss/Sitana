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

			Activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)Platform.ImmersiveModeFlags;
			Activity.Window.DecorView.RequestLayout();

			Graphics.PreferredBackBufferWidth = _width;
			Graphics.PreferredBackBufferHeight = _height;

			AddView();
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
				if (!IsSizeOk(Activity.Window.DecorView.Width,Activity.Window.DecorView.Height))
				{
					Activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)Platform.ImmersiveModeFlags;
					Activity.Window.DecorView.RequestLayout();
				}

				_checkTime = 1;
			}
		}
	}
}

