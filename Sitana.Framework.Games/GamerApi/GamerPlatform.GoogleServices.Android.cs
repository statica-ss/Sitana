using System;
using Sitana.Framework.Ui.Core;
using System.Threading.Tasks;


namespace Sitana.Framework.GamerApi
{
	class GamerPlatform
	{
        bool _enabled = false;

		public void OnActivated(AchievementInfoDelegate achievementInfo, LeaderboardDelegate leaderboardInfo)
		{
			
		}

        public void Login(AchievementInfoDelegate achievementInfo, LeaderboardDelegate leaderboardInfo)
        {

        }

        public void SendAchievement(string id, int completion)
        {
            if (_enabled)
            {

            }
        }

        public void SendScore(string id, int score)
        {
            if (_enabled)
            {

            }
        }

        public void OpenAchievements()
        {

        }

        public void OpenLeaderboards()
        {

        }
	}
}

