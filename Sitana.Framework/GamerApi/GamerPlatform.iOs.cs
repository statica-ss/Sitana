using System;

namespace Sitana.Framework.GamerApi
{
	class GamerPlatform
	{
		public event EventHandler<AchievementInfoEventArgs> AchievementInfo;

        public void EnsureLoggedIn()
        {
        }

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

