using System;
using Android.App;
using Android.Content;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.GamerApi
{
	public class GamerActivity: AndroidGameActivity
	{
		protected override void OnStart()
		{
			base.OnStart();
			Gamer.Instance.Handler.GameHelper.Start();
		}

		protected override void OnStop()
		{
			base.OnStop();
			Gamer.Instance.Handler.GameHelper.Stop();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			Gamer.Instance.Handler.GameHelper.OnActivityResult (requestCode, resultCode, data);
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}

