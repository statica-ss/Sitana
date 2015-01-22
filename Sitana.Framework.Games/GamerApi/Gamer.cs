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
        private Dictionary<string, List<int>> _localScores = new Dictionary<string, List<int>>();

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

        public void Enable()
        {
            Unserialize();
            _handler.Login(OnAchievementsLoaded);
        }

        public int AchievementCompletion(string name)
        {
            lock (_lock)
            {
                Achievement achievement = FromName(name);
                return achievement != null ? achievement.Completion : 0;
            }
        }

        public string NameToId(string name)
        {
            Achievement achievement;
            _achievementInfos.TryGetValue(name, out achievement);

            return achievement != null ? achievement.Id : null;
        }

        public string IdToName(string id)
        {
            foreach(var achievement in _achievementInfos)
            {
                if ( achievement.Value.Id == id)
                {
                    return achievement.Key;
                }
            }

            return null;
        }

        Achievement FromName(string name)
        {
            lock(_lock)
            {
                Achievement achievement;
                _achievementInfos.TryGetValue(name, out achievement);
                return achievement;
            }
        }

        public bool ReportAchievement(string name, int completion)
        {
            Achievement achievement = FromName(name);
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
                    AchievementCompleted(new AchievementInfo(id){Completion = completion});
                }

                _handler.SendAchievement(name, completion);
                Serialize();
            }

            return isChanged;
        }

        public void LocalScores(string leaderboard, List<int> list)
        {
            list.Clear();

            lock (_lock)
            {
                List<int> local = null;
                if (_localScores.TryGetValue(leaderboard, out list))
                {
                    list.AddRange(local);
                }
            }
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

        public void ReportScore(string leaderboardName, int score)
        {
            lock (_lock)
            {
                List<int> list = null;
                if (!_localScores.TryGetValue(leaderboardName, out list))
                {
                    list = new List<int>();
                    _localScores[leaderboardName] = list;
                }

                list.Add(score);
            }

            Leaderboard leaderboard;
            _leaderboardInfos.TryGetValue(leaderboardName, out leaderboard);

            if (leaderboard != null)
            {
                _handler.SendScore(leaderboard.Id, score);
            }
        }


        private void OnAchievementsLoaded(AchievementInfo[] achievements)
        {
            foreach (var ach in achievements)
            {
                int currentCompletion = AchievementCompletion(ach.Id);

                if (currentCompletion > ach.Completion)
                {
                    _handler.SendAchievement(ach.Id, ach.Completion);
                }

                if (currentCompletion < ach.Completion)
                {
                    lock (_lock)
                    {
                        string name = IdToName(ach.Id);

                        if (name != null)
                        {
                            Achievement achievement = FromName(name);

                            if (achievement != null)
                            {
                                achievement.Completion = ach.Completion;
                            }
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

                            writer.Write(_localScores.Count);
                            foreach (var loc in _localScores)
                            {
                                writer.Write(loc.Key);

                                List<int> list = loc.Value;
                                writer.Write(list.Count);

                                foreach (var val in list)
                                {
                                    writer.Write(val);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Unserialize()
        {
            string path = Serializator.PathFromType(typeof(Gamer));

            _localScores.Clear();

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

                                Achievement achievement = FromName(name);


                                if ( achievement != null)
                                {
                                    achievement.Completion = completion;
                                }
                            }

                            count = reader.ReadInt32();

                            for (int idx = 0; idx < count; ++idx)
                            {
                                string id = reader.ReadString();

                                List<int> list = new List<int>();
                                _localScores.Add(id, list);

                                int count2 = reader.ReadInt32();
                                for (int idx2 = 0; idx2 < count2; ++idx2)
                                {
                                    list.Add(reader.ReadInt32());
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
