// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Cs;
using System.Collections.Generic;
using System;
using System.IO;
using Sitana.Framework.Settings;
using Sitana.Framework.Xml;

namespace Sitana.Framework.GamerApi
{
    public class Gamer: Singleton<Gamer>
    {
        private GamerPlatform _handler = new GamerPlatform();

        private Dictionary<string, Achievement> _achievementInfos = new Dictionary<string, Achievement>();
        private Dictionary<string, Leaderboard> _leaderboardInfos = new Dictionary<string, Leaderboard>();

        private object _lock = new object();

        public event AchievementCompletedDelegate AchievementCompleted;

#if __ANDROID__
        private string _appId = "";
#endif

        public Gamer()
        {
        }

        public void Import(XFile file)
        {
            XNode node = (XNode)file;

            if ( node.Tag != "Gamer")
            {
                throw new Exception("Invalid tag. Expected: Gamer.");
            }

            foreach(var cn in node.Nodes)
            {
                switch(cn.Tag)
                {
#if __ANDROID__
                case "App":
                    _appId = cn.Attribute("Id");
                    break;
#endif

                case "Achievement":
                    {
                        var achievement = new Achievement(cn);
                        _achievementInfos.Add(cn.Attribute("Name"), achievement);
                    }
                    break;

                case "Leaderboard":
                    {
                        var leaderboard = new Leaderboard(cn);
                        _leaderboardInfos.Add(cn.Attribute("Name"), leaderboard);
                    }
                    break;
                }
            }
        }

        public void OpenAchievements()
        {
            _handler.OpenAchievements();
        }

        public void OpenLeaderboards()
        {
            _handler.OpenLeaderboards();
        }

		public void OnActivated()
		{
			_handler.OnActivated(OnAchievementsLoaded, OnLeaderboardsLoaded);
		}

        public void Enable()
        {
            Unserialize();
            _handler.Login(OnAchievementsLoaded, OnLeaderboardsLoaded);
        }

        public int AchievementCompletion(string name)
        {
            lock (_lock)
            {
                Achievement achievement = AchievementFromName(name);
                return achievement != null ? achievement.Completion : 0;
            }
        }

        public string NameToId(string name)
        {
            Leaderboard leaderboard = LeaderboardFromName(name);

            if(leaderboard != null)
            {
                return leaderboard.Id;
            }

            Achievement achievement = AchievementFromName(name);

            if(achievement != null)
            {
                return achievement.Id;
            }

            return null;
        }

        public string IdToName(string id)
        {
            foreach(var leaderboard in _leaderboardInfos)
            {
                if ( leaderboard.Value.Id == id)
                {
                    return leaderboard.Key;
                }
            }

            foreach(var achievement in _achievementInfos)
            {
                if ( achievement.Value.Id == id)
                {
                    return achievement.Key;
                }
            }

            return null;
        }

        Achievement AchievementFromName(string name)
        {
            lock(_lock)
            {
                Achievement achievement;
                _achievementInfos.TryGetValue(name, out achievement);
                return achievement;
            }
        }

        Leaderboard LeaderboardFromName(string name)
        {
            lock(_lock)
            {
                Leaderboard leaderboard;
                _leaderboardInfos.TryGetValue(name, out leaderboard);
                return leaderboard;
            }
        }

        public bool ReportAchievement(string name, int completion)
        {
            Achievement achievement = AchievementFromName(name);
            string id = achievement.Id;

            if ( id == null )
            {
                return false;
            }

            int currentCompletion = AchievementCompletion(name);

            bool isChanged = false;

            if (currentCompletion < completion)
            {
                achievement.Completion = completion;
                isChanged = true;
            }

            if (isChanged)
            {
                if (AchievementCompleted != null)
                {
                    AchievementCompleted(achievement);
                }

                _handler.SendAchievement(id, completion);
                Serialize();
            }

            return isChanged;
        }

        public int LocalScore(string name)
        {
            Leaderboard leaderboard = LeaderboardFromName(name);
            if(leaderboard != null)
            {
                return leaderboard.Score;
            }
            return 0;
        }

        public void LocalAchievements(List<Achievement> list)
        {
            list.Clear();

            lock (_lock)
            {
                foreach (var keyValue in _achievementInfos)
                {
                    Achievement achievement = keyValue.Value;

                    if (achievement != null)
                    {
                        list.Add(achievement);
                    }
                }
            }
        }

		public void ReportScore(string leaderboardName, int score, bool? report = null)
        {
            Leaderboard leaderboard;
            _leaderboardInfos.TryGetValue(leaderboardName, out leaderboard);

            if (leaderboard != null)
            {
                if (leaderboard.Score < score)
                {
                    leaderboard.Score = score;

					if (!report.HasValue)
					{
						report = true;
					}

					if (report.HasValue && report.Value == true)
					{
						Serialize();
					}
                }

				if (report.HasValue && report.Value == true)
				{
					_handler.SendScore(leaderboard.Id, score);
				}
            }
        }

        private void OnLeaderboardsLoaded(LeaderboardInfo[] leaderboards)
        {
            foreach (var lb in leaderboards)
            {
                string name = IdToName(lb.Id);

                if (name != null)
                {
                    Leaderboard leaderboard = LeaderboardFromName(name);

                    if (leaderboard != null)
                    {
                        int currentScore = leaderboard.Score;

                        if (currentScore > lb.Score)
                        {
                            _handler.SendScore(lb.Id, currentScore);
                        }

                        if (currentScore < lb.Score)
                        {
                            leaderboard.Score = lb.Score;
                        }
                    }
                }
            }

            Serialize();
        }

        private void OnAchievementsLoaded(AchievementInfo[] achievements)
        {
            foreach (var ach in achievements)
            {
                string name = IdToName(ach.Id);

                if (name != null)
                {
                    Achievement achievement = AchievementFromName(name);

                    if (achievement != null)
                    {
                        int currentCompletion = achievement.Completion;

                        if (currentCompletion > ach.Completion)
                        {
                            _handler.SendAchievement(ach.Id, currentCompletion);
                        }

                        if (currentCompletion < ach.Completion)
                        {
                            achievement.Completion = ach.Completion;
                        }
                    }
                }
            }
                
            Serialize();
        }

        private void Serialize()
        {
            string path = Serializator.PathFromType(typeof(Gamer));

            lock (_lock)
            {
                // Open isolated storage.
                using (var isolatedStorageFile = Platform.GetUserStoreForApplication())
                {
                    // Open file from storage.
                    using (Stream stream = isolatedStorageFile.OpenFile(path, FileMode.Create))
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Write(_achievementInfos.Count);

                            foreach (var ach in _achievementInfos)
                            {
                                writer.Write(ach.Key);
                                writer.Write(ach.Value.Completion);
                            }

                            writer.Write(_leaderboardInfos.Count);

                            foreach (var lb in _leaderboardInfos)
                            {
                                writer.Write(lb.Key);
                                writer.Write(lb.Value.Score);
                            }
                        }
                    }
                }
            }
        }

        private void Unserialize()
        {
            string path = Serializator.PathFromType(typeof(Gamer));

            // Open isolated storage.
            using (var isolatedStorageFile = Platform.GetUserStoreForApplication())
            {
                try
                {
                    // Open file from storage.
                    using (var stream = isolatedStorageFile.OpenFile(path, FileMode.Open))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            int count = reader.ReadInt32();

                            for (int idx = 0; idx < count; ++idx)
                            {
                                string name = reader.ReadString();
                                int completion = reader.ReadInt32();

                                Achievement achievement = AchievementFromName(name);

                                if ( achievement != null)
                                {
                                    achievement.Completion = completion;
                                }
                            }

                            count = reader.ReadInt32();

                            for (int idx = 0; idx < count; ++idx)
                            {
                                string id = reader.ReadString();

                                Leaderboard leaderboard;
                                _leaderboardInfos.TryGetValue(id, out leaderboard);

                                if(leaderboard != null)
                                {
                                    leaderboard.Score = reader.ReadInt32();
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }
    }
}
