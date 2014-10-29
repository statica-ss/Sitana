// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Sitana.Framework.Cs;
using System.IO;
using Sitana.Framework.Settings;

namespace Sitana.Framework.GamerApi
{
    public class AchievementsManager: Singleton<AchievementsManager>
    {
        private AchievementsHandler _handler;
        private String _domain;
        private Dictionary<String, Int32> _achievements = new Dictionary<String, Int32>();

        private Object _lock = new Object();

        public event EventHandler<AchievementInfoEventArgs> AchievementCompleted;

        public void Initialize(AchievementsHandler handler, String domain)
        {
            _handler = handler;
            _domain = domain;

            _handler.AchievementInfo += OnAchievementInfo;
            _handler.GetAchievementsList();

            Unserialize();
        }

        void OnAchievementInfo(object sender, AchievementInfoEventArgs args)
        {
            SetAchievement(args.Id, args.Completion);
        }

        public Int32 AchievementCompletion(String id)
        {
            lock (_lock)
            {
                Int32 completion = 0;
                _achievements.TryGetValue(id, out completion);
                return completion;
            }
        }

        public void SetAchievement(String id, Int32 completion)
        {
            Int32 currentCompletion = AchievementCompletion(id);

            Boolean isChanged = false;

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

                    _handler.SendAchievement(_domain + "." + id, completion);
                    Serialize();
                }
            }
        }

        public Int32 this[String id]
        {
            get
            {
                return AchievementCompletion(id);
            }

            set
            {
                SetAchievement(id, value);
            }
        }

        private void Serialize()
        {
            String path = Serializator.PathFromType(typeof(AchievementsManager));

            lock (_lock)
            {
                // Open isolated storage.
                using (var isolatedStorageFile = Platform.GetUserStoreForApplication())
                {
                    // Open file from storage.
                    using (Stream stream = isolatedStorageFile.OpenFile(path, FileMode.Create))
                    {
                        using (BinaryWriter writter = new BinaryWriter(stream))
                        {
                            writter.Write(_achievements.Count);
                            foreach (var ach in _achievements)
                            {
                                writter.Write(ach.Key);
                                writter.Write(ach.Value);
                            }
                        }
                    }
                }
            }
        }

        private void Unserialize()
        {
            String path = Serializator.PathFromType(typeof(AchievementsManager));

            _achievements.Clear();

            // Open isolated storage.
            using (var isolatedStorageFile = Platform.GetUserStoreForApplication())
            {
                // Open file from storage.
                using (var stream = isolatedStorageFile.OpenFile(path, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        Int32 count = reader.ReadInt32();

                        for (Int32 idx = 0; idx < count; ++idx)
                        {
                            String id = reader.ReadString();
                            Int32 completion = reader.ReadInt32();

                            _achievements[id] = completion;
                        }
                    }
                }
            }
        }
    }
}
