using System;
using Sitana.Framework.Ui.Core;
using System.Threading.Tasks;
using GooglePlay.Services.Helpers;
using Sitana.Framework.Settings;
using Android.Gms.Common.Apis;
using Java.Interop;
using Android.Gms.Games.Achievement;
using Android.Gms.Games.LeaderBoard;


namespace Sitana.Framework.GamerApi
{
	public class GamerPlatform: Java.Lang.Object, IResultCallback
	{
		public class GamerPlatformSettings: SingletonSettings<GamerPlatformSettings>
		{
			public bool ShouldBeSignedIn = false;
		}

		GameHelper _helper;
        bool _enabled = false;

		public bool ShowSignInButton
		{
			get
			{
				return !_enabled;
			}
		}

		AchievementInfoDelegate _achievementInfoDelegate;
		LeaderboardDelegate _leaderboardInfoDelegate;

		public GamerErrorDelegate _errorDelegate;


		internal GameHelper GameHelper
		{
			get
			{
				return _helper;
			}
		}

		public GamerPlatform(GamerErrorDelegate errorDelegate)
		{
			_errorDelegate = errorDelegate;

			_helper = new GameHelper(AppMain.Activity);

			_helper.OnSignInFailed += (object sender, EventArgs e) => 
			{
				_errorDelegate(GamerError.ErrorSignIn);
				_enabled = false;
			};

			_helper.GravityForPopups = Android.Views.GravityFlags.Center;
			_helper.ViewForPopups = AppMain.Current.RootView;
			_helper.Initialize();

			_helper.OnSignedIn += OnSignedIn;
			_helper.OnSignedOut += (sender, e) => _enabled = false;
		}

		void OnSignedIn(object sender, EventArgs args)
		{
			_enabled = true;
			_helper.LoadAchievements(this);
			Console.WriteLine("");
		}

		public void OnActivated(AchievementInfoDelegate achievementInfo, LeaderboardDelegate leaderboardInfo)
		{
		}

        public void Enable(AchievementInfoDelegate achievementInfo, LeaderboardDelegate leaderboardInfo)
        {
			_achievementInfoDelegate = achievementInfo;
			_leaderboardInfoDelegate = leaderboardInfo;
        }

		public void Login()
		{
			_helper.SignIn();
		}

		public void Logout()
		{
			_helper.SignOut();
		}

        public void SendAchievement(string id, int completion)
        {
            if (_enabled)
            {
				if (completion == Achievement.Completed)
				{
					_helper.UnlockAchievement(id);
				} 
				else
				{
					_helper.IncrementAchievement(id, completion);
				}

            }
        }

        public void SendScore(string id, int score)
        {
            if (_enabled)
            {
				_helper.SubmitScore(id, score);
            }
        }

        public void OpenAchievements()
        {
			if (_enabled)
			{
				_helper.ShowAchievements();
			}
        }

        public void OpenLeaderboards()
        {
			if (_enabled)
			{
				_helper.ShowAllLeaderBoardsIntent();
			}
        }

		public void OnResult (Java.Lang.Object result)
		{
			var ar = result.JavaCast<IAchievementsLoadAchievementsResult>();

			if (ar != null) 
			{
				var count = ar.Achievements.Count;

				AchievementInfo[] achievements = new AchievementInfo[count];

				for (int idx = 0; idx < count; idx++)
				{
					var item = ar.Achievements.Get(idx);
					var ach = item.JavaCast<IAchievement>();


					if (ach.Type == Android.Gms.Games.Achievement.Achievement.TypeIncremental)
					{
						achievements[idx] = new AchievementInfo(ach.AchievementId){ Completion = (ach.CurrentSteps * 100 / ach.TotalSteps) };
					}
					else
					{
						achievements[idx] = new AchievementInfo(ach.AchievementId){ Completion = ach.State == Android.Gms.Games.Achievement.Achievement.StateUnlocked ? Achievement.Completed : 0 };
					}
				}

				_achievementInfoDelegate(achievements);
			}

//			ILeaderboardsLoadScoresResult scores = result.JavaCast<ILeaderboardsLoadScoresResult>();
//			if (scores != null)
//			{
//				string id = scores.Leaderboard.LeaderboardId;
//				int count = scores.Scores.Count;
//
//				for (int i = 0; i < count; i++)
//				{
//					var score = scores.Scores.Get(i).JavaCast<ILeaderboardScore>();
//				}
//			}
		}
	}
}

