// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.using System;
using System.Collections.Generic;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using Sitana.Framework.Gui;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Sitana.Framework.Content.Generators
{
    public class LoadSpriteAtlas
    {
        private String _path;

        public void Generate()
        {
            Texture2D texture = ContentLoader.Current.Load<Texture2D>(_path);

            String directory = Path.GetDirectoryName(_path);

            using (Stream stream = ContentLoader.Current.Open(_path + ".satl"))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    Int32 spritesCount = reader.ReadInt32();

                    for (Int32 spriteIndex = 0; spriteIndex < spritesCount; ++spriteIndex)
                    {
                        String name = reader.ReadString();

                        Int32 spriteSheetsNo = reader.ReadInt32();

                        Point frameSize = new Point(
                           reader.ReadInt32(),
                           reader.ReadInt32()
                           );

                        Int32 sequencesNo = reader.ReadInt32();

                        Sprite.Sequence[] sequences = new Sprite.Sequence[sequencesNo];

                        for (Int32 idx = 0; idx < sequencesNo; ++idx)
                        {
                            String seqName = reader.ReadString();
                            Int32 seqStart = reader.ReadInt32();
                            Int32 seqLength = reader.ReadInt32();
                            Boolean seqLoop = reader.ReadBoolean();
                            Double seqSpeed = reader.ReadDouble();

                            sequences[idx] = new Sprite.Sequence(seqStart, seqLength, seqLoop, seqSpeed, seqName);
                        }

                        List<Point[]> optimizedFrames = new List<Point[]>();

                        Int32 optimizedFramesNo = reader.ReadInt32();

                        for (Int32 idx = 0; idx < optimizedFramesNo; ++idx)
                        {
                            Int32 framesNo = reader.ReadInt32();
                            Point[] frames = new Point[framesNo];

                            for (Int32 frame = 0; frame < framesNo; ++frame)
                            {
                                frames[frame] = new Point(
                                   reader.ReadInt32(),
                                   reader.ReadInt32()
                                   );
                            }

                            optimizedFrames.Add(frames);
                        }

                        Texture2D[] sheet = new Texture2D[spriteSheetsNo];

                        for (Int32 idx = 0; idx < spriteSheetsNo; ++idx)
                        {
                            sheet[idx] = texture;
                        }

                        Sprite sprite = Sprite.FromData(frameSize, optimizedFrames, sequences, sheet);

                        name = Path.Combine(directory, name);

                        ContentLoader.Current.AddContent(name, typeof(Sprite), sprite);
                    }
                }
            }
        }
    }
}
