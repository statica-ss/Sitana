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
using Android.OS;
using Android.Telephony;
using Java.Util;
using Android.Net.Wifi;
using System.Globalization;
using Android.Widget;
using Java.IO;
using System.Net;
using System.IO;

namespace Sitana.Framework
{
    public static class Platform
    {
		static bool _fileDownloading = false;

        public static String AppId { private get; set; }

        public static Boolean CloseApp()
        {
			return true;
        }

		public static Assembly MainAssembly
		{
			get
			{
				return Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
			}
		}

		[Obsolete("GetUserStoreForApplication is deprecated, please use IsolatedStorageManager instead.", true)]
		public static IsolatedStorageFile GetUserStoreForApplication()
		{
			throw new InvalidOperationException();
		}

		internal static IsolatedStorageFile UserStore
		{
			get
			{
				return IsolatedStorageFile.GetUserStoreForApplication();
			}
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

		public static string UniqueDeviceId
		{
			get
			{
				ISharedPreferences sharedPrefs = AppMain.Activity.GetSharedPreferences(AppMain.Activity.PackageName, FileCreationMode.MultiProcess);

				string uniqueId = sharedPrefs.GetString("UniqueId", null);

				if (uniqueId == null)
				{
					uniqueId = GenerateDeviceId();
				}

				if (uniqueId == null) 
				{
					uniqueId = UuidGenerator.GenerateString();

					ISharedPreferencesEditor editor =  sharedPrefs.Edit();
					editor.PutString("UniqueId", uniqueId);
					editor.Commit();
				}
					
				System.Diagnostics.Debug.WriteLine(string.Format("DeviceId: {0}", uniqueId));
				return uniqueId;
			}
		}

		static string GenerateDeviceId()
		{
			string androidId = Android.Provider.Settings.Secure.GetString(AppMain.Activity.ApplicationContext.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

			if (CheckIdValid(androidId))
			{
				return FormatLikeGuid(androidId);
			}

			try
			{
				WifiManager wifiMan = (WifiManager)AppMain.Activity.GetSystemService(Context.WifiService);
				WifiInfo wifiInf = wifiMan.ConnectionInfo;
				String macAddr = wifiInf.MacAddress;

				if (CheckIdValid(macAddr))
				{
					return FormatLikeGuid(macAddr);
				}
			}
			catch{}

			return null;
		}

		static string FormatLikeGuid(string id)
		{
			long plus = 0;

			id = id.ToUpperInvariant();

			for (int index = 0; index < id.Length; ++index )
			{
				char ch = id[index];
				if (!char.IsDigit(ch) && (ch < 'A' || ch > 'F') )
				{
					id = id.Replace(ch.ToString(), string.Empty);
					plus += (int)ch;
				}
			}

			string part1 = id.Substring(0, id.Length / 2);
			string part2 = id.Substring(id.Length / 2);

			part1 = part1.Substring(0, Math.Min(16, part1.Length));
			part2 = part2.Substring(0, Math.Min(16, part2.Length));

			long part1Long;
			long part2Long;

			if(long.TryParse(part1, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out part1Long))
			{
				if(long.TryParse(part2, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out part2Long))
				{
					part2 += plus;
					return new UUID(part1Long, part2Long).ToString();
				}
			}

			return null;
		}

		static bool CheckIdValid(string id)
		{
			List<char> characters = new List<char>();

			foreach (var ch in id)
			{
				if(char.IsLetterOrDigit(ch) && !characters.Contains(ch))
				{
					characters.Add(ch);
				}
			}

			return characters.Count > 4;
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

        public static void DisableLock(bool disable)
        {
			if (disable)
			{
				AppMain.Activity.Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
			} 
			else
			{
				AppMain.Activity.Window.ClearFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
			}
        }

        public static int KeyboardHeight(bool landscape)
        {
            return 0;
        }

		public static void DoRestart(Context context) 
		{
			try 
			{
				//check if the context is given
				if (context != null) 
				{
					//fetch the packagemanager so we can get the default launch activity 
					// (you can replace this intent with any other activity if you want
					PackageManager pm = context.PackageManager;
					//check if we got the PackageManager
					if (pm != null) 
					{
						//create the intent with the default start activity for your application
						Intent mStartActivity = pm.GetLaunchIntentForPackage(context.PackageName);

						if (mStartActivity != null) 
						{
							
							mStartActivity.AddFlags(ActivityFlags.ClearTop);
							//create a pending intent so the application is restarted after System.exit(0) was called. 
							// We use an AlarmManager to call this intent in 100ms

							int mPendingIntentId = 223344;

							PendingIntent mPendingIntent = PendingIntent.GetActivity(context, mPendingIntentId, mStartActivity, PendingIntentFlags.CancelCurrent);

							AlarmManager mgr = (AlarmManager)context.GetSystemService(Context.AlarmService);

							mgr.Set(AlarmType.Rtc, Java.Lang.JavaSystem.CurrentTimeMillis() + 100, mPendingIntent);
							//kill the application
							Java.Lang.JavaSystem.Exit(0);

						} 

					}

				}
			}
			catch
			{
				Java.Lang.JavaSystem.Exit(0);
			}
		}
			

		public static void PlayVideo(string path, bool local)
		{
			Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(path));
			intent.SetDataAndType(Android.Net.Uri.Parse(path), "video/mp4");

			AppMain.Activity.StartActivity(intent);
		}

		public static void DownloadAndOpenFile(string url)
		{
			if (_fileDownloading)
			{
				return;
			}

			_fileDownloading = true;
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.BeginGetResponse(OnFileDownloaded, request);
		}

		static void OnFileDownloaded(IAsyncResult state)
		{
			try
			{
				HttpWebRequest request = state.AsyncState as HttpWebRequest;

				WebResponse response = request.EndGetResponse(state);

				string fileName = Path.GetFileName(request.RequestUri.ToString());

				try
				{
					fileName = response.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty);
				}
				catch
				{
				}

				string contentType = response.ContentType;

				var file = AppMain.Activity.GetFileStreamPath(fileName);
				file.Mkdirs();
				file.Delete();

				using (var openFileOutput = AppMain.Activity.OpenFileOutput(fileName, FileCreationMode.WorldReadable))
				{
					Stream responseStream = response.GetResponseStream();

					byte[] buffer = new byte[1024 * 1024];

					for (;;)
					{
						int read = responseStream.Read(buffer, 0, 1024 * 1024);

						if (read == 0)
						{
							break;
						}

						openFileOutput.Write(buffer, 0, read);
					}

					openFileOutput.Flush();
				}
					
				file.DeleteOnExit();

				try
				{
					Intent viewDoc = new Intent(Intent.ActionView);

					viewDoc.SetDataAndType(Android.Net.Uri.FromFile(file), contentType);
					viewDoc.SetFlags(ActivityFlags.NewTask);

					PackageManager pm = AppMain.Activity.PackageManager;

					IList<ResolveInfo> apps = pm.QueryIntentActivities(viewDoc, PackageInfoFlags.MatchDefaultOnly);

					if (apps.Count > 0)
					{
						AppMain.Activity.StartActivity(viewDoc);
					}
				}
				catch(Exception ex)
				{
					System.Console.WriteLine("{0}", ex);
				}
			}
			finally
			{
				_fileDownloading = false;
			}
		}
    }
}
