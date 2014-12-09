// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Cs;
using System.Collections.Generic;
using System;
using System.IO;
using Sitana.Framework.Settings;

namespace Sitana.Framework.GamerApi
{
    public class Gamer: Singleton<Gamer>
    {
        private GamerPlatform _handler = new GamerPlatform();
        private Dictionary<string, int> _achievements = new Dictionary<string, int>();
        private Dictionary<string, List<int>> _localScores = new Dictionary<string, List<int>>();

        private object _lock = new object();

        public event EventHandler<AchievementInfoEventArgs> AchievementCompleted;

        public Gamer()
        {
        }

        public void Enable()
        {
            Unserialize();

            _handler.AchievementInfo += OnAchievementInfo;
            _handler.Login();
        }

        public int AchievementCompletion(string id)
        {
            lock (_lock)
            {
                int completion = 0;
                _achievements.TryGetValue(id, out completion);
                return completion;
            }
        }

        public void ReportAchievement(string id, int completion)
        {
            int currentCompletion = AchievementCompletion(id);

            bool isChanged = false;

            if (currentCompletion < completion)
            {
                lock (_lock)
                {
                    currentCompletion = AchievementCompletion(id);

                    if (currentCompletion < completion)
                    {
                        _achievements[id] = completion;
                        isChanged = true;
                    }
                }

                if (isChanged)
                {
                    if (AchievementCompleted != null)
                    {
                        AchievementCompleted(this, new AchievementInfoEventArgs(id, completion));
                    }

                    _handler.SendAchievement(id, completion);
                    Serialize();
                }
            }
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

        public void LocalAchievements(List<KeyValuePair<string, int>> list)
        {
            list.Clear();

            lock (_lock)
            {
                foreach (var keyValue in _achievements)
                {
                    list.Add(keyValue);
                }
            }
        }

        public void ReportScore(string leaderboard, int score)
        {
            lock (_lock)
            {
                List<int> list = null;
                if (!_localScores.TryGetValue(leaderboard, out list))
                {
                    list = new List<int>();
                    _localScores[leaderboard] = list;
                }

                list.Add(score);
            }

            _handler.SendScore(leaderboard, score);
        }


        private void OnAchievementInfo(object sender, AchievementInfoEventArgs args)
        {
            int currentCompletion = AchievementCompletion(args.Id);

            if (currentCompletion > args.Completion)
            {
                _handler.SendAchievement(args.Id, args.Completion);
            }

            if (currentCompletion < args.Completion)
            {
                lock (_lock)
                {
                    _achievements[args.Id] = args.Completion;
                }

                Serialize();
            }
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
                            writer.Write(_achievements.Count);
                            foreach (var ach in _achievements)
                            {
                                writer.Write(ach.Key);
                                writer.Write(ach.Value);
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

            _achievements.Clear();
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
                                string id = reader.ReadString();
                                int completion = reader.ReadInt32();

                                _achievements[id] = completion;
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
