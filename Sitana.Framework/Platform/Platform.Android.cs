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
using Android.Content.PM;
using Java.Lang.Reflect;

namespace Sitana.Framework
{
    public static class Platform
    {
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
            Context context = AppMain.Activity;
            Android.Net.Uri uri = Android.Net.Uri.Parse("market://details?id=" + context.PackageName);

            Intent goToMarket = new Intent(Intent.ActionView, uri);

            try 
            {
                context.StartActivity(goToMarket);
            } 
            catch (ActivityNotFoundException) 
            {
                uri = Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=" + context.PackageName);

                goToMarket = new Intent(Intent.ActionView, uri);
                context.StartActivity(goToMarket);
            }
        }

        public static string CurrentVersion
        {
            get
            {
                try
                {
                    PackageInfo pInfo = AppMain.Activity.PackageManager.GetPackageInfo(AppMain.Activity.PackageName, (PackageInfoFlags)0);
                    return pInfo.VersionName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

		public static string OsName
		{
			get
			{
				return string.Format("android ({0})", Android.OS.Build.Board);
			}
		}

		public static string DeviceName
		{
			get
			{
				return Android.OS.Build.Device;
			}
		}

		public static string OsVersion
		{
			get
			{
				return "Android " + Android.OS.Build.VERSION.Release;
			}
		}

		public static string DeviceId
		{
			get
			{
				string deviceId = Android.Provider.Settings.Secure.GetString(Game.Activity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
				return deviceId;
			}
		}

        public static void DisableLock(Boolean disable)
        {
            
        }

        public static int KeyboardHeight(bool landscape)
        {
            return 0;
        }
    }
}
