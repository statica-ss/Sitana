// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.using Sitana.Framework.Content;

using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Reflection;
using Sitana.Framework.Cs;

namespace Sitana.Framework
{
    public static class Platform
    {
        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
        }
        
        public static bool CloseApp()
        {
            return false;
        }

        public static void OpenWebsite(string url)
        {
            if (url.StartsWith(Uri.UriSchemeHttp) || url.StartsWith(Uri.UriSchemeHttps))
            {
                Process.Start(url);
            }
        }

        public static void OpenRatingPage()
        {
        }

        public static void OpenMail(string name, string address, string subject, string text, EmptyArgsVoidDelegate completed)
        {
            name = Uri.EscapeDataString(name);
            subject = Uri.EscapeDataString(subject);
            text = Uri.EscapeDataString(text);
            String command = String.Format("mailto:{0}<{1}>?subject={2}&body={3}", name, address, subject, text);

            Process.Start(command);

            if (completed != null)
            {
                completed.Invoke();
            }
        }

        public static string CurrentVersion
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

        public static string OpenFileDialog(string title, string filter)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

            dlg.Filter = filter;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dlg.FileName;
            }

            return null;
        }

        public static string SaveFileDialog(string title, string filter, string defaultExt)
        {
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();

            dlg.Filter = filter;
            dlg.DefaultExt = defaultExt;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dlg.FileName;
            }

            return null;
        }

        public static int KeyboardHeight(bool landscape)
        {
            return 0;
        }
    }
}
