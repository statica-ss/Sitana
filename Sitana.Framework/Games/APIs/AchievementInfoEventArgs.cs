// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Sitana.Framework.Games.Apis
{
    public class AchievementInfoEventArgs : EventArgs
    {
        public readonly String Id;
        public readonly Int32 Completion;

        public AchievementInfoEventArgs(String id, Int32 completion)
        {
            Id = id;
            Completion = completion;
        }
    }
}
