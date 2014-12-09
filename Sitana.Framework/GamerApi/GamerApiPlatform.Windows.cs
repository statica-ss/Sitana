// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Sitana.Framework.GamerApi
{
    public class GamerApiPlatform
    {
        public event EventHandler<AchievementInfoEventArgs> AchievementInfo;

        public void SendAchievement(string id, int completion)
        {
        }

        public void GetAchievementsList()
        {
        }

        public void SendScore(string id, int score)
        {
        }
    }
}
