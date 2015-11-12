// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.using System.Reflection;
using System;
using Sitana.Framework.Cs;
using Sitana.Framework.Settings;

namespace Sitana.Framework
{
    /// <summary>
    /// Manages time, when user rating request should be popuped.
    /// </summary>
    public sealed class AppRater : Singleton<AppRater>
    {
        public class AppRaterData
        {
            public DateTime FirstUse;
            public DateTime LastTimeAsked;
            public int NumberOfAppOpenings;
            public string LastOpenedVersion;
            public bool RemindLater;
			public bool DontRemindUntilNextVersion;
			public bool FirstTime;
			public bool AppAlreadyRated;
        }

        private int _minDaysFromFirstUse;
        private int _minAppOpenings;
        private int _minDaysToRemind;

        private AppRaterData _data;

        public void Init(int minDaysFromFirstUse, int minAppOpenings, int minDaysToRemind)
        {
			string version = Platform.CurrentVersion;

            bool loaded = true;

            var task = Serializator.Deserialize<AppRaterData>(Serializator.PathFromType(typeof(AppRaterData)));
            task.Wait();
            _data = task.Result;

            if (_data == null)
            {
                loaded = false;
                _data = new AppRaterData();
            }

            _minDaysFromFirstUse = minDaysFromFirstUse;
            _minAppOpenings = minAppOpenings;
            _minDaysToRemind = minDaysToRemind;

            if (!loaded || (_data.DontRemindUntilNextVersion && version != _data.LastOpenedVersion))
            {
                _data.FirstUse = DateTime.Today;
                _data.LastTimeAsked = DateTime.Today;
                _data.RemindLater = false;
                _data.AppAlreadyRated = false;
                _data.NumberOfAppOpenings = 0;
                _data.LastOpenedVersion = version;
                _data.FirstTime = true;
                _data.DontRemindUntilNextVersion = false;
                Serialize();
            }
        }

        public void OnAppActivated()
        {
            _data.NumberOfAppOpenings++;
            Serialize();
        }

        public void OnRemindLater()
        {
            _data.FirstTime = false;
            _data.RemindLater = true;
            _data.NumberOfAppOpenings = 0;
            _data.LastTimeAsked = DateTime.Today;
            Serialize();
        }

        public void OnRemindNever()
        {
            _data.FirstTime = false;
            _data.RemindLater = false;
            Serialize();
        }

        public void OnRemindOnNewVersion()
        {
            _data.FirstTime = false;
            _data.RemindLater = false;
            _data.DontRemindUntilNextVersion = true;

            Serialize();
        }

        public void OnRateApp()
        {
            _data.AppAlreadyRated = true;
            Serialize();
        }

        private void Serialize()
        {
            var task = Serializator.Serialize(Serializator.PathFromType(typeof(AppRaterData)), _data);
            task.Wait();
        }

		public bool IsAppRated
        {
            get
            {
                return _data.AppAlreadyRated;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanShowRatingRequest
        {
            get
            {
                if (_data.AppAlreadyRated)
                {
                    return false;
                }

                if (_data.FirstTime)
                {
                    if ((DateTime.Today - _data.FirstUse).TotalDays < _minDaysFromFirstUse)
                    {
                        return false;
                    }
                }
                else if (!_data.RemindLater || (DateTime.Today - _data.LastTimeAsked).TotalDays < _minDaysToRemind)
                {
                    return false;
                }

                if (_data.NumberOfAppOpenings < _minAppOpenings)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
