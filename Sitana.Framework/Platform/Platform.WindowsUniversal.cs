using Sitana.Framework.Crypto;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Core;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace Sitana.Framework
{
    public static class Platform
    {
        [Obsolete("GetUserStoreForApplication is deprecated, please use IsolatedStorageManager instead.", true)]
        public static StorageFolder GetUserStoreForApplication()
        {
            throw new InvalidOperationException();
        }

        public static Assembly MainAssembly
        {
            get
            {
                return Application.Current.GetType().GetTypeInfo().Assembly;
            }
        }

        public static bool CloseApp()
        {
            return false;
        }

        public static void OpenWebsite(string url)
        {
            Task.Run(async () =>
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            });
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
                PackageVersion pv = Package.Current.Id.Version;
                return $"{pv.Major}.{pv.Minor}.{pv.Build}.{pv.Revision}";
            }
        }

        public static string OsVersion
        {
            get
            {
                string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                ulong v = ulong.Parse(sv);
                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                ulong v4 = (v & 0x000000000000FFFFL);

                return $"{v1}.{v2}.{v3}.{v4}";
            }
        }

        public static string OsName
        {
            get
            {
                return AnalyticsInfo.VersionInfo.DeviceFamily;
            }
        }

        public static string DeviceName
        {
            get
            {
                EasClientDeviceInformation eas = new EasClientDeviceInformation();
                return eas.FriendlyName;
            }
        }

        public static string UniqueDeviceId
        {
            get
            {
                EasClientDeviceInformation eas = new EasClientDeviceInformation();
                return eas.Id.ToString();
            }
        }

        public static void DisableLock(bool disable)
        {
        }

        public static void DownloadAndOpenFile(string url)
		{
            OpenWebsite(url);
        }

        public static int KeyboardHeight(bool landscape)
        {
            return 0;
        }

        public static void PlayVideo(string path, bool local)
        {
        }
		
        public static void PlayYoutubeVideo(string videoId)
        {
        }
    }
}
