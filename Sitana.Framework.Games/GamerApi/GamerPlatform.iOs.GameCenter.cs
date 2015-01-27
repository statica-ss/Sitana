using System;
using Sitana.Framework.Ui.Core;
using System.Threading.Tasks;
using GameKit;
using UIKit;
using Foundation;

namespace Sitana.Framework.GamerApi
{
    public class GamerPlatform_GameCenter: IGamerPlatform
	{
        bool _enabled = false;

        AchievementInfoDelegate _achievementInfo;
        LeaderboardDelegate _leaderboardInfo;

        public bool ShowSignInButton
        {
            get
            {
                return false;
            }
        }

        public void Enable(AchievementInfoDelegate achievementInfo, LeaderboardDelegate leaderboardInfo)
        {
            _achievementInfo = achievementInfo;
            _leaderboardInfo = leaderboardInfo;
            Login();
        }

		public void OnActivated()
		{
			if (_enabled)
			{
				Login();
			}
		}

        public void Login()
        {
            GKLocalPlayer player = GKLocalPlayer.LocalPlayer;

            if (!player.Authenticated)
            {
                player.AuthenticateHandler = (UIViewController controller, NSError error) =>
                {
                    if (controller != null)
                    {
                        UIViewController parent = AppMain.Current.Services.GetService(typeof(UIViewController)) as UIViewController;
                        parent.PresentViewController(controller, true, null);
                    }
                    else
                    {
                        if (GKLocalPlayer.LocalPlayer.Authenticated)
                        {
                            _enabled = true;

                            GKLeaderboard.LoadLeaderboards((GKLeaderboard[] leaderboards, NSError error3)=>
                            {
                                if (leaderboards != null && leaderboards.Length > 0 && _leaderboardInfo != null)
                                {
                                    LeaderboardInfo[] info = new LeaderboardInfo[leaderboards.Length];

                                    for (int idx = 0; idx < leaderboards.Length; ++idx)
                                    {
                                        GKLeaderboard lb = leaderboards[idx];

                                        int score = 0;

                                        if(lb.LocalPlayerScore!=null)
                                        {
                                            score = (int)lb.LocalPlayerScore.Value;
                                        }

                                        info[idx] = new LeaderboardInfo(lb.Identifier){Score = score};
                                    }

                                    _leaderboardInfo(info);
                                }
                            });

                            GKAchievement.LoadAchievements((GKAchievement[] achievements, NSError error2)=>
                            {
                                if (achievements != null && achievements.Length > 0 && _achievementInfo != null)
                                {
                                    AchievementInfo[] info = new AchievementInfo[achievements.Length];

                                    for (int idx = 0; idx < achievements.Length; ++idx)
                                    {
                                        GKAchievement ach = achievements[idx];

                                        info[idx] = new AchievementInfo(ach.Identifier){Completion = ach.Completed ? Achievement.Completed : (int)ach.PercentComplete};
                                    }

                                    _achievementInfo(info);
                                }
                            });
                        }
                    }
                };
            }
        }

        public void Logout()
        {

        }

        public void SendAchievement(string id, int completion)
        {
            if (_enabled)
            {
                GKAchievement[] achievements = new GKAchievement[1];
                achievements[0] = new GKAchievement(id, GKLocalPlayer.LocalPlayer);
                achievements[0].PercentComplete = completion;

                GKAchievement.ReportAchievements(achievements, null, (error)=>{});
            }
        }

        public void SendScore(string id, int score)
        {
            if (_enabled)
            {
                GKScore gkScore = new GKScore(id, GKLocalPlayer.LocalPlayer);
                gkScore.Value = score;

                GKScore.ReportScores(new GKScore[1]{gkScore}, (error)=>{});
            }
        }

        public void OpenAchievements()
        {
            UIViewController controller = AppMain.Current.Services.GetService(typeof(UIViewController)) as UIViewController;

            GKAchievementViewController achievementViewController = new GKAchievementViewController ();
            achievementViewController.DidFinish += 
                (object sender, EventArgs e) => 
            {
                achievementViewController.DismissViewController(true, null);
            };

            controller.PresentViewController(achievementViewController, true, null);
        }

        public void OpenLeaderboards()
        {
            UIViewController controller = AppMain.Current.Services.GetService(typeof(UIViewController)) as UIViewController;

            GKLeaderboardViewController leaderboardViewController = new GKLeaderboardViewController ();
            leaderboardViewController.DidFinish += 
                (object sender, EventArgs e) => 
            {
                leaderboardViewController.DismissViewController(true, null);
            };

            controller.PresentViewController(leaderboardViewController, true, null);
        }
	}
}

