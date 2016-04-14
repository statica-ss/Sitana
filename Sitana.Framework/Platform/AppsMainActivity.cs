using System;
using Microsoft.Xna.Framework;
using Android.OS;
using Android.Views;
using Sitana.Framework.Input;

namespace Sitana.Framework.Ui.Core
{
    public class AppsMainActivity: AndroidGameActivity, ViewTreeObserver.IOnGlobalLayoutListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.DecorView.ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }

        void ViewTreeObserver.IOnGlobalLayoutListener.OnGlobalLayout()
        {
            NativeInput.CurrentFocus?.UpdateLayout();
        }
    }
}

