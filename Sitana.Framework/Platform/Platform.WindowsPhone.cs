// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO.IsolatedStorage;
using System.Reflection;
using System;
using System.Windows;

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
                //Version version = Assembly.GetEntryAssembly().GetName().Version;
                return "1.0";// version.ToString();
            }
        }

		public static string DeviceId
        {
            get
            {
                return DeviceStatus.DeviceName;
            }
        }
		
        public static void DisableLock(bool disable)
        {
            
        }
		
    }
}
