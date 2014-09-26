// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Sitana.Framework.Input;
using System;

namespace Sitana.Framework.Gui
{
    public interface IInfoBarDraw
    {
        Rectangle Draw(SpriteBatch spriteBatch, InfoBarInfo info, InfoBarSettings settings, Single transition, Single scale, Vector2 areaSize);
    }

    public class InfoBarInfo
    {
        public String Title;
        public String Text;
        public Texture2D Icon;
    }

    public class InfoBarSettings
    {
        public Single TransitionInTime = 0.25f;
        public Single TransitionOutTime = 0.5f;
        public Single ShowTime = 2.0f;
        public Single ShowTimeWhenNext = 1.0f;

        public IInfoBarDraw InfoBarDraw = null;
    }

    class InfoBar
    {
        private Single _transition = 0;
        private Single _showTime = 0;

        private List<InfoBarInfo> _infoList = new List<InfoBarInfo>();
        private InfoBarSettings _settings;

        private Rectangle _currentArea = Rectangle.Empty;

        public InfoBar(InfoBarSettings settings)
        {
            Debug.Assert(settings != null);
            _settings = settings;

            Debug.Assert(settings.InfoBarDraw != null);
        }

        public void PushInfo(InfoBarInfo info)
        {
            _infoList.Add(info);
        }

        public void Update(Single time)
        {
            if (_infoList.Count == 0)
            {
                return;
            }

            if (_transition < 1 && _showTime == 0)
            {
                _transition += time / _settings.TransitionInTime;
                _transition = Math.Min(1, _transition);
                return;
            }

            Single desiredShowTime = _infoList.Count > 1 ? _settings.ShowTimeWhenNext : _settings.ShowTime;

            if (_transition == 1 && _showTime < desiredShowTime)
            {
                _showTime += time;
                return;
            }

            if (_transition > 0 && _showTime >= desiredShowTime)
            {
                _transition -= time / _settings.TransitionOutTime;
                _transition = Math.Max(0, _transition);

                if (_transition == 0)
                {
                    _infoList.RemoveAt(0);
                    _showTime = 0;
                }
                return;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Single scale, Vector2 areaSize)
        {
            if (_transition <= 0 || _infoList.Count == 0)
            {
                return;
            }

            InfoBarInfo info = _infoList[0];
            _currentArea = _settings.InfoBarDraw.Draw(spriteBatch, info, _settings, _transition, scale, areaSize);
        }
    }
}
