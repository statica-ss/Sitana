using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoMac.AppKit;

namespace Sitana.Framework
{
    public static class Platform
    {
		public static bool CloseApp()
        {
			return false;
        }
		
        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
        }

        public static void OpenWebsite(String url)
        {
            if (url.StartsWith(Uri.UriSchemeHttp) || url.StartsWith(Uri.UriSchemeHttps))
            {
                Process.Start(url);
            }
        }

        public static void OpenMail(String name, String address, String subject, String text, Action completedAction)
        {
            name = Uri.EscapeDataString(name);
            subject = Uri.EscapeDataString(subject);
            text = Uri.EscapeDataString(text);
            String command = String.Format("mailto:{0}<{1}>?subject={2}&body={3}", name, address, subject, text);

            Process.Start(command);

            if (completedAction != null)
            {
                completedAction.Invoke();
            }
        }

        public static void OpenRatingPage()
        {
            OpenWebsite("http://sitana.net");
        }

        public static String CurrentVersion
        {
            get
            {
                Version version = Assembly.GetEntryAssembly().GetName().Version;
                return version.ToString();
            }
        }

        public static string OpenFileDialog(string promt, string filters)
        {
            var openPanel = new NSOpenPanel();
            openPanel.ReleasedWhenClosed = true;
            openPanel.Prompt = promt;

            var result = openPanel.RunModal();
            if (result == 1)
            {
				var path = openPanel.Url.AbsoluteString;

				if ( path.StartsWith("file:"))
				{
					path = '/' + path.Substring(5).TrimStart('/');
				}

				return path;
            }

            return null;
        }

        public static string SaveFileDialog(string promt, string filters, string defaultExt)
        {
            var savePanel = new NSSavePanel();
            savePanel.ReleasedWhenClosed = true;
            savePanel.Prompt = promt;

            var result = savePanel.RunModal();
            if (result == 1)
            {
				var path = savePanel.Url.AbsoluteString;

				if ( path.StartsWith("file:"))
				{
					path = '/' + path.Substring(5).TrimStart('/');
				}

				return path;
            }

            return null;
        }


        public static int KeyboardHeight(bool landscape)
        {
            return 0;
        }
    }
}
