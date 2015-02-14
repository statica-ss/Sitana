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
        double _checkTime = 1;

		public AppMain_ImmersiveFullscreen(): base(false)
		{
			Init();

            Activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.ImmersiveSticky|SystemUiFlags.Fullscreen|SystemUiFlags.HideNavigation|SystemUiFlags.Fullscreen);
            Activity.Window.DecorView.RequestLayout();

			AddView();

            Graphics.PreferredBackBufferWidth = Activity.Window.DecorView.Width;
            Graphics.PreferredBackBufferHeight = Activity.Window.DecorView.Height;
		}

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

//            _checkTime -= gameTime.ElapsedGameTime.TotalSeconds;
//
//            if (_checkTime <= 0)
//            {
//                if(!Activity.Window.DecorView.SystemUiVisibility.HasFlag((StatusBarVisibility)(SystemUiFlags.ImmersiveSticky|SystemUiFlags.Fullscreen|SystemUiFlags.HideNavigation|SystemUiFlags.Fullscreen)))
//                {
//                    Activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.ImmersiveSticky|SystemUiFlags.Fullscreen|SystemUiFlags.HideNavigation|SystemUiFlags.Fullscreen);
//                    Activity.Window.DecorView.RequestLayout();
//                }
//
//                _checkTime = 1;
//            }
        }

	}
}

