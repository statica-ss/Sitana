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
using MessageUI;
using Foundation;
using UIKit;
using GameKit;

namespace Sitana.Framework
{
    public static class Platform
    {
        internal class CustomMailComposeDelegate : MFMailComposeViewControllerDelegate
        {
            private Action _onCompleted;


            public CustomMailComposeDelegate(Action onCompleted)
            {
                _onCompleted = onCompleted;
            }

            public override void Finished(MFMailComposeViewController controller,
                                        MFMailComposeResult result, NSError error)
            {
                controller.DismissViewController(true, DoCompletedAction);
            }

            public void DoCompletedAction()
            {
                if (_onCompleted != null)
                {
                    _onCompleted.Invoke();
                }
            }
        }

		private static string _appId;

		private static UIApplication _app;

		public static void Init(UIApplication app, string appId)
		{
			_app = app;
			_appId = appId;
		}

        public static bool CloseApp()
        {
			return true;
        }
		
        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            return IsolatedStorageFile.GetUserStoreForApplication();
        }

        public static void OpenWebsite(string url)
        {
            UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(url));
        }

        public static void OpenMail(string name, string address, string subject, string text, Action onCompleted)
        {
            MFMailComposeViewController mail = new MFMailComposeViewController();

            mail.MailComposeDelegate = new CustomMailComposeDelegate(onCompleted);
            mail.SetToRecipients(new string[] { name + "<" + address + ">" });
            mail.SetSubject(subject);
            mail.SetMessageBody(text, true);

            UIViewController controller = AppMain.Current.Services.GetService(typeof(UIViewController)) as UIViewController;
            controller.PresentViewController(mail, true, null);
        }

        public static void OpenRatingPage()
        {
			string url = string.Format("itms-apps://itunes.apple.com/app/id{0}", _appId);
            UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(url));
        }

        public static string CurrentVersion
        {
            get
            {
				NSObject obj = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString");

				return (obj as NSString).ToString();
            }
        }

		public static string DeviceId
		{
			get
			{
				var selector = new ObjCRuntime.Selector("uniqueIdentifier");

				if (UIDevice.CurrentDevice.RespondsToSelector(selector))
				{
					return UIDevice.CurrentDevice.PerformSelector(selector).ToString();
				}

				return UIDevice.CurrentDevice.IdentifierForVendor.ToString();
			}
		}

		public static string DeviceName
		{
			get
			{
				return UIDevice.CurrentDevice.Name;
			}
		}

		public static string OsVersion
		{
			get
			{
				return OsName + " " + UIDevice.CurrentDevice.SystemVersion;
			}
		}

		public static string OsName
		{
			get
			{
				return UIDevice.CurrentDevice.SystemName;
			}
		}

        public static void DisableLock(bool disable)
        {
            _app.IdleTimerDisabled = disable;
        }

		public static float PixelsToPoints(float pixels)
		{
			float scale = (float)UIScreen.MainScreen.Scale;
			return pixels / scale;
		}

		public static float PointsToPixels(float points)
		{
			float scale = (float)UIScreen.MainScreen.Scale;
			return points * scale;
		}

		public static int KeyboardHeight(bool landscape)
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				if (landscape)
				{
					return (int)(352 * (float)UIScreen.MainScreen.Scale);
				}
				else
				{
					return (int)(264 * (float)UIScreen.MainScreen.Scale);
				}
			}

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				if (landscape)
				{
					return (int)(162 * (float)UIScreen.MainScreen.Scale);
				} 
				else
				{
					return (int)(216 * (float)UIScreen.MainScreen.Scale);
				}
			}

			return 0;
		}
    }
}
