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

        public static String CurrentVersion
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

        public static void DisableLock(Boolean disable)
        {
            
        }
    }
}
