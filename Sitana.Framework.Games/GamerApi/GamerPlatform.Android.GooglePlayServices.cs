using System;
using Sitana.Framework.Ui.Core;
using System.Threading.Tasks;
using Sitana.Framework.Settings;
using Android.Gms.Common.Apis;
using Java.Interop;
using Android.Gms.Games.Achievement;
using Android.Gms.Games.LeaderBoard;
using Android.Views;
using Android.Gms.Games;
using Android.Gms.Common;
using Android.App;
using Android.Content;
using System.Threading;
using Sitana.Framework.Cs;


namespace Sitana.Framework.GamerApi
{
    public class GamerPlatform_GooglePlayServices: Java.Lang.Object, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener, IGamerPlatform
	{
		public class GamerPlatformSettings: SingletonSettings<GamerPlatformSettings>
		{
			public bool ShouldBeSignedIn = false;
            public string PlayerId = string.Empty;
		}

        internal class AchievementsCallback: Java.Lang.Object, IResultCallback
        {
            AchievementInfoDelegate _achievementInfoDelegate;
            public AchievementsCallback(AchievementInfoDelegate achievementInfoDelegate)
            {
                _achievementInfoDelegate = achievementInfoDelegate;
            }

            public void OnResult(Java.Lang.Object result)
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
            }
        }

        internal class LeaderboardCallback: Java.Lang.Object, IResultCallback
        {
            LeaderboardDelegate _leaderboardDelegate;
            public LeaderboardCallback(LeaderboardDelegate leaderboardDelegate)
            {
                _leaderboardDelegate = leaderboardDelegate;
            }

            public void OnResult(Java.Lang.Object result)
            {
                ILeaderboardsLoadScoresResult scores = result.JavaCast<ILeaderboardsLoadScoresResult>();

                if (scores != null)
                {
                    LeaderboardInfo[] leaderboardInfo = new LeaderboardInfo[1];

                    string id = scores.Leaderboard.LeaderboardId;
    
                    leaderboardInfo[0] = new LeaderboardInfo(id){ Score = 0 };
                    _leaderboardDelegate(leaderboardInfo);
                }
            }
        }
            
            
        bool _enabled = false;
        bool _signingIn = false;
        bool _resolving = false;

        const int RequestLeaderboard = 9002;
        const int RequestAllLeaderboards = 9003;
        const int RequestAchievements = 9004;
        const int RcResolve = 9001;

        public GravityFlags GravityForPopups = GravityFlags.Top | GravityFlags.Center;
        public event GamerErrorDelegate Error;

        IGoogleApiClient _client = null;

		public bool ShowSignInButton
		{
			get
			{
				return !GamerPlatformSettings.Instance.ShouldBeSignedIn && !_signingIn && !_resolving;
			}
		}
            
        AchievementsCallback _achievementsCallback;
        LeaderboardCallback _leaderboardCallback;

        public GamerPlatform_GooglePlayServices()
		{

		}

        void OnSignInFailed(GamerError error)
        {
            _enabled = false;
            if (Error != null)
            {
                Error(error);
            }
        }

        void OnSignedOut()
        {
            _enabled = false;
        }

		void OnSignedIn()
		{
            _enabled = true;

            LoadAchievements(_achievementsCallback);

            foreach(var lb in Gamer.Instance.Leaderboards)
            {
                LoadTopScores(lb, _leaderboardCallback);
            }
		}

		public void OnActivated()
		{

		}

        public void Enable(AchievementInfoDelegate achievementInfo, LeaderboardDelegate leaderboardInfo)
        {
            _achievementsCallback = new AchievementsCallback(achievementInfo);
            _leaderboardCallback = new LeaderboardCallback(leaderboardInfo);

            if(GamerPlatformSettings.Instance.ShouldBeSignedIn)
            {
                CreateClient();
            }
        }

        void CreateClient() 
        {
            string id = GamerPlatformSettings.Instance.PlayerId;

            var builder = new GoogleApiClientBuilder(AppMain.Activity, this, this);
            builder.AddApi(Android.Gms.Games.GamesClass.Api);
            builder.AddScope(Android.Gms.Games.GamesClass.ScopeGames);
            builder.SetGravityForPopups((int)GravityForPopups);

            builder.SetViewForPopups(AppMain.Current.RootView);

            if (!string.IsNullOrEmpty(id)) 
            {
                builder.SetAccountName(id);
            }

            _client = builder.Build();
        }

        public void Start()
        {
            if(!GamerPlatformSettings.Instance.ShouldBeSignedIn && !_signingIn)
            {
                return;
            }

            if (_client != null && !_client.IsConnected) 
            {
                _client.Connect();
            }
        }

        public void Stop()
        {
            if (_client != null && _client.IsConnected) 
            {
                _client.Disconnect();
            }
        }

        void Reconnect() 
        {
            if (_client != null)
            {
                _client.Reconnect();
            }
        }

		public void Login()
		{
            _signingIn = true;

            if (_client == null)
            {
                CreateClient();
            }

            if (_client.IsConnected)
            {
                return;
            }

            if (_client.IsConnecting)
            {
                return;
            }

            var result = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(AppMain.Activity);

            if (result != ConnectionResult.Success) 
            {
                return;
            }

            Start();
		}

		public void Logout()
		{
            GamerPlatformSettings.Instance.ShouldBeSignedIn = false;
            GamerPlatformSettings.Instance.PlayerId = string.Empty;
            GamerPlatformSettings.Instance.Serialize();

            if (_client.IsConnected) 
            {
                GamesClass.SignOut(_client);
                Stop();

                _client.Dispose ();
                _client = null;

                OnSignedOut();
            }
		}

        public void SendAchievement(string id, int completion)
        {
			if (_client!=null && _client.IsConnected)
            {
				if (completion == Achievement.Completed)
				{
                    GamesClass.Achievements.Unlock(_client, id);
				} 
				else
				{
                    GamesClass.Achievements.Increment(_client, id, completion);
				}
            }
        }

        public void SendScore(string id, int score)
        {
			if (_client!=null && _client.IsConnected)
            {
                GamesClass.Leaderboards.SubmitScore(_client, id, score);
            }
        }

        public void OpenAchievements()
        {
			if (_enabled && _client != null)
			{
                var intent = GamesClass.Achievements.GetAchievementsIntent(_client);
                AppMain.Activity.StartActivityForResult(intent, RequestAchievements);
			}
        }

        public void OpenLeaderboards()
        {
			if (_enabled && _client != null)
			{
                var intent = GamesClass.Leaderboards.GetAllLeaderboardsIntent(_client);
                AppMain.Activity.StartActivityForResult(intent, RequestAllLeaderboards);
			}
        }

        void LoadAchievements(IResultCallback callback) 
        {
            var pendingResult = GamesClass.Achievements.Load(_client, false);
            pendingResult.SetResultCallback(callback);
        }

        void LoadTopScores(string leaderboardCode, IResultCallback callback) 
        {
            var pendingResult = GamesClass.Leaderboards.LoadTopScores(_client, leaderboardCode, 2, 0, 25);
            pendingResult.SetResultCallback(callback);
        }

        #region IGoogleApiClientConnectionCallbacks implementation

        public void OnConnected(Android.OS.Bundle connectionHint)
        {
            GamerPlatformSettings.Instance.ShouldBeSignedIn = true;
			GamerPlatformSettings.Instance.PlayerId = GamesClass.GetCurrentAccountName(_client);
			GamerPlatformSettings.Instance.Serialize();

			_resolving = false;
            _signingIn = false;


            OnSignedIn();
        }

        public void OnConnectionSuspended (int resultCode)
        {
            GamerPlatformSettings.Instance.ShouldBeSignedIn = true;
			GamerPlatformSettings.Instance.Serialize();

            _resolving = false;
            _signingIn = false;
            _client.Disconnect();

            OnSignInFailed(GamerError.ErrorConnectionSuspended);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            if (_resolving)
            {
                return;
            }

            if (result.HasResolution) 
            {
                _resolving = true;
                result.StartResolutionForResult(AppMain.Activity, RcResolve);
                return;
            }

            GamerPlatformSettings.Instance.ShouldBeSignedIn = true;
			GamerPlatformSettings.Instance.Serialize();

            _resolving = false;
            _signingIn = false;

            OnSignInFailed(GamerError.ErrorSignIn);
        }
        #endregion

        public void OnActivityResult (int requestCode, Result resultCode, Intent data) {

            if (requestCode == RcResolve) 
            {
                if (resultCode == Result.Ok) 
                {
                    Start ();
                } 
                else 
                {
                    OnSignInFailed(GamerError.ErrorResolve);
                }
            }
        }
	}
}

