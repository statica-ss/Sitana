using System;

namespace Sitana.Framework.GamerApi
{
    public interface IGamerPlatform
    {
        void OnActivated();
        void Enable(AchievementInfoDelegate achievementInfo, LeaderboardDelegate leaderboardInfo);
        void Login();
        void Logout();
        void SendAchievement(string id, int completion);
        void SendScore(string id, int score);
        void OpenAchievements();
        void OpenLeaderboards();

        bool ShowSignInButton{get;}
    }
}

