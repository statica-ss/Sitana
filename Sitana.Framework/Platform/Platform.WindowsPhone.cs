// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Cs;
using System;
using System.IO;
using System.Net;
using Windows.Storage;


namespace Sitana.Framework
{
    public static class Platform
    {
        public static StorageFolder GetUserStoreForApplication()
        {
            return ApplicationData.Current.LocalFolder;
        }
        
        public static bool CloseApp()
        {
            return false;
        }

        public static void OpenWebsite(string url)
        {
            
        }

        public static void OpenRatingPage()
        {
        }

        public static void OpenMail(string name, string address, string subject, string text, EmptyArgsVoidDelegate completed)
        {
        }

        public static string CurrentVersion
        {
            get
            {
                //Version version = Assembly.GetEntryAssembly().GetName().Version;
                return "1.0";// version.ToString();
            }
        }

        public static string OsVersion
        {
            get
            {
                return "1.0";// System.Environment.OSVersion.ToString();
            }
        }

        public static string OsName
        {
            get
            {
                return "Windows Phone 8.1";
            }
        }

        public static string DeviceName
        {
            get
            {
                return "Test";// System.Environment.MachineName;
            }
        }

        public static string UniqueDeviceId
        {
            get
            {
                return "";
            }
        }

        public static string DeviceId
        {
            get
            {
                return "";
            }
        }

        public static void DisableLock(bool disable)
        {
        }

        public static void DownloadAndOpenFile(string url)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.BeginGetResponse(OnFileDownloaded, request);
		}

        static void OnFileDownloaded(IAsyncResult state)
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

            // TODO: Opening 

            //string contentType = response.ContentType;

            //fileName = Path.Combine(Path.GetTempPath(), fileName);

            //using (var stream = new FileStream(fileName, FileMode.Create))
            //{
            //    Stream responseStream = response.GetResponseStream();
            //    responseStream.CopyTo(stream);
            //}

            //Process.Start(fileName);
        }

        public static string OpenFileDialog(string title, string filter)
        {
            //System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

            //dlg.Filter = filter;

            //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    return dlg.FileName;
            //}

            return null;
        }

        public static string SaveFileDialog(string title, string filter, string defaultExt)
        {
            //System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();

            //dlg.Filter = filter;
            //dlg.DefaultExt = defaultExt;

            //if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    return dlg.FileName;
            //}

            return null;
        }

        public static int KeyboardHeight(bool landscape)
        {
            return 0;
        }

        public static void PlayVideo(string path, bool local)
        {
        }
		
    }
}
