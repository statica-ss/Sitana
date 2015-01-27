using System;
using Android.App;
using Android.Content;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.GamerApi
{
    public class GooglePlayServicesActivity: AndroidGameActivity
	{
		protected override void OnStart()
		{
			base.OnStart();
            ((GamerPlatform_GooglePlayServices)Gamer.Instance.GamerPlatform).Start();
		}

		protected override void OnStop()
		{
			base.OnStop();
            ((GamerPlatform_GooglePlayServices)Gamer.Instance.GamerPlatform).Stop();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
            ((GamerPlatform_GooglePlayServices)Gamer.Instance.GamerPlatform).OnActivityResult(requestCode, resultCode, data);
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}

