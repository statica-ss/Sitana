// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Reflection;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using System.Windows.Media;

namespace Ebatianos
{
    public static class SystemWrapper
    {
        private static Action<Char> _onCharacterAction;
        private static Action _onKeyboardHidden;

        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            return IsolatedStorageFile.GetUserStoreForApplication();
        }

        public static void OpenWebsite(String url)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(url, UriKind.RelativeOrAbsolute);
            task.Show();
        }

        public static void OpenMail(String name, String address, String subject, String text, Action actionAfterMail)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = subject;
            emailComposeTask.Body = text;
            emailComposeTask.To = name + "<" + address + ">";

            emailComposeTask.Show();

            if (actionAfterMail != null)
            {
                actionAfterMail.Invoke();
            }
        }

        public static void OpenRatingPage()
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        public static String CurrentVersion
        {
            get
            {
                //var nameHelper = new AssemblyName(MainAssembly.FullName);
                return "1.0.0.2";//nameHelper.Version.ToString();
            }
        }

        public static void ShowKeyboard(Rectangle rect, Action<Char> onCharacterAction, Action onKeyboardHidden)
        {
        }

        public static void HideKeyboard()
        {
        }
    }
}
