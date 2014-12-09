using System;
using MonoTouch.GameKit;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Sitana.Framework.Ui.Core;
using System.Threading.Tasks;

namespace Sitana.Framework.GamerApi
{
	class GamerPlatform
	{
		public event EventHandler<AchievementInfoEventArgs> AchievementInfo;

        bool _enabled = false;

        public void Login()
        {
            GKLocalPlayer player = GKLocalPlayer.LocalPlayer;

            if (!player.Authenticated)
            {
                player.AuthenticateHandler = Authentication;
            }
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

        private void Authentication(UIViewController controller, NSError error)
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

                    GKAchievement.LoadAchievements(AchievementList);
                }
            }
        }

        private void AchievementList(GKAchievement[] achievements, NSError error)
        {
            if (achievements != null && AchievementInfo != null)
            {
                foreach (var ach in achievements)
                {
                    AchievementInfo(this, new AchievementInfoEventArgs(ach.Identifier, ach.Completed ? 100 : (int)ach.PercentComplete));
                }
            }
        }
	}
}

