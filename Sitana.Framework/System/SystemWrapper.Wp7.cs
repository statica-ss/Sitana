/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
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
