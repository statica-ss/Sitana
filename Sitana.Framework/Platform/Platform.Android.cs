// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using System.Reflection;
using System;
using Sitana.Framework.Ui.Core;
using Android.Content;
using Android.App;

namespace Sitana.Framework
{
    public static class Platform
    {
        public const int ImmersiveModeFlags = 4102;

        public static String AppId { private get; set; }

        public static Boolean CloseApp()
        {
			return true;
        }
		
        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            return IsolatedStorageFile.GetUserStoreForApplication();
        }

        public static void OpenWebsite(String url)
        {
			Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
			AppMain.Activity.StartActivity(intent);
        }

        public static void OpenMail(String name, String address, String subject, String text, Action onCompleted)
        {
            
        }

        public static void OpenRatingPage()
        {
            
        }

        public static String CurrentVersion
        {
            get
            {
                Version version = Assembly.GetEntryAssembly().GetName().Version;
                return version.ToString();
            }
        }

        public static void DisableLock(Boolean disable)
        {
            
        }
    }
}
