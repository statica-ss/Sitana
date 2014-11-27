using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Content;
using Sitana.Framework.Xml;
using System.IO;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public partial class Sprite
    {
        public Point FrameSize { get; private set; }
        Dictionary<string, Sequence> _sequences = new Dictionary<string, Sequence>();

        public static void LoadSprites(string name)
        {
            XNode node = ContentLoader.Current.Load<XFile>(name);

            if (node.Tag != "Sprites")
            {
                throw new InvalidDataException("Invalid node. Expected: Sprites.");
            }

            foreach (var cn in node.Nodes)
            {
                if (cn.Tag != "Sprite")
                {
                    throw new InvalidDataException("Invalid node. Expected: Sprite.");
                }

                Point frameSize = Point.Zero;

                string spriteName = cn.Attribute("Name");
                Dictionary<string, Sequence> sequences = new Dictionary<string,Sequence>();

                foreach (var seq in cn.Nodes)
                {
                    if (seq.Tag != "Sequence")
                    {
                        throw new InvalidDataException("Invalid node. Expected: Sequence.");
                    }

                    Sequence sequence = new Sequence(seq);

                    int width = sequence.Images[0].Width;
                    int height = sequence.Images[0].Height;

                    if (width != frameSize.X && frameSize.X != 0 || height != frameSize.Y && frameSize.Y != 0)
                    {
                        throw new InvalidDataException("Sprite must have same size frames.");
                    }

                    sequences.Add(sequence.Name, sequence);
                }

                var sprite = new Sprite()
                {
                    _sequences = sequences,
                    FrameSize = frameSize
                };

                ContentLoader.Current.AddContent(spriteName, typeof(Sprite), sprite);
            }
        }

        public Sequence FirstSequence
        {
            get
            {
                return _sequences.First().Value;
            }
        }

        public SpriteInstance CreateInstance()
        {
            return new SpriteInstance(this);
        }

        public Sequence FindSequence(string name)
        {
            Sequence seq;
            _sequences.TryGetValue(name, out seq);
            return seq;
        }
    }
}
