// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Sitana.Framework.GamerApi
{
    public abstract class AchievementsHandler
    {
        public event EventHandler<AchievementInfoEventArgs> AchievementInfo;

        public abstract void SendAchievement(String id, Int32 completion);

        public abstract void GetAchievementsList();
    }
}
