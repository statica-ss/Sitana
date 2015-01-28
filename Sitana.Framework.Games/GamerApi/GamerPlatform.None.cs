using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.GamerApi
{
	public class GamerPlatform_None: IGamerPlatform
	{
        public void OnActivated(){ }
        public void Enable(AchievementInfoDelegate achievementInfo, LeaderboardDelegate leaderboardInfo){ }
        public void Login() { }
        public void Logout() { }
        public void SendAchievement(string id, int completion) { }
        public void SendScore(string id, int score) { }
        public void OpenAchievements() { }
        public void OpenLeaderboards() { }
        public bool ShowSignInButton { get { return false; } }
	}
}
