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
            public Int32 NumberOfAppOpenings;
            public String LastOpenedVersion;
            public Boolean RemindLater;
            public Boolean DontRemindUntilNextVersion;
            public Boolean FirstTime;
            public Boolean AppAlreadyRated;
        }

        private Int32 _minDaysFromFirstUse;
        private Int32 _minAppOpenings;
        private Int32 _minDaysToRemind;
        private Action _onShouldRateAction;

        private AppRaterData _data;

        public void Init(Int32 minDaysFromFirstUse, Int32 minAppOpenings, Int32 minDaysToRemind)
        {
            String version = SystemWrapper.CurrentVersion;

            Boolean loaded = true;

            _data = Serializator.Deserialize<AppRaterData>(Serializator.PathFromType(typeof(AppRaterData)));

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

        public void BindAction(Action action)
        {
            _onShouldRateAction = action;
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
            Serializator.Serialize(Serializator.PathFromType(typeof(AppRaterData)), _data);
        }

        public void CheckShowRatingRequest()
        {
            if (CanShowRatingRequest && _onShouldRateAction != null)
            {
                _onShouldRateAction.Invoke();
            }
        }

        public Boolean IsAppRated
        {
            get
            {
                return _data.AppAlreadyRated;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean CanShowRatingRequest
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
