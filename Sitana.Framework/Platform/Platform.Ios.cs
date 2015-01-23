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

        public static String AppId { private get; set; }

        public static UIApplication App { private get; set; }

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
            UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(url));
        }

        public static void OpenMail(String name, String address, String subject, String text, Action onCompleted)
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
            String url = String.Format("itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id={0}", AppId);
            UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(url));
        }

        public static void OpenAchievements()
        {
            UIViewController controller = AppMain.Current.Services.GetService(typeof(UIViewController)) as UIViewController;

            GKAchievementViewController achievementViewController = new GKAchievementViewController ();
            achievementViewController.DidFinish += 
                (object sender, EventArgs e) => 
                {
                    achievementViewController.DismissViewController(true, null);
                };

            controller.PresentViewController(achievementViewController, true, null);
        }

        public static void OpenLeaderboards()
        {
            UIViewController controller = AppMain.Current.Services.GetService(typeof(UIViewController)) as UIViewController;

            GKLeaderboardViewController leaderboardViewController = new GKLeaderboardViewController ();
            leaderboardViewController.DidFinish += 
                (object sender, EventArgs e) => 
            {
                leaderboardViewController.DismissViewController(true, null);
            };

            controller.PresentViewController(leaderboardViewController, true, null);
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
            App.IdleTimerDisabled = disable;
        }
    }
}
