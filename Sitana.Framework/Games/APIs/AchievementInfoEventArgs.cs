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
