// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using Android.App;
using Android.Views;
using Microsoft.Xna.Framework;
using Sitana.Framework.Diagnostics;

namespace Sitana.Framework.Ui.Core
{
	public partial class AppMain
	{
		public AppMain(Activity activity): this()
		{
			View view = (View)Services.GetService(typeof(View));
			activity.SetContentView(view);
		}
	}
}
