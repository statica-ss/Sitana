using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Content;
using System.Collections.ObjectModel;
using System.IO;

namespace Sitana.Framework.Graphics
{
    public partial class Sprite
    {
        public class Sequence
        {
            public readonly string Name;
            public readonly ReadOnlyCollection<PartialTexture2D> Images;
            public readonly float Fps;
            public readonly bool Loop;

            public Sequence(XNode node)
            {
                XNodeAttributes attr = new XNodeAttributes(node);

                Name = attr.AsString("Name");
                Fps = attr.AsInt32("Fps", 1);
                Loop = attr.AsBoolean("Loop", false);

                string sequence = attr.AsString("ImageSequence");
                int start = attr.AsInt32("Start", 0);
                int end = attr.AsInt32("End", 0);

                List<PartialTexture2D> list = new List<PartialTexture2D>();

                for (int idx = start; idx <= end; ++idx)
                {
                    PartialTexture2D texture = ContentLoader.Current.Load<PartialTexture2D>(sequence.Replace("*", idx.ToString()));

                    if (idx > start)
                    {
                        if (texture.Width != list[0].Width || texture.Height != list[0].Height)
                        {
                            throw new InvalidDataException("Sprite must have same size frames.");
                        }
                    }

                    list.Add(texture);
                }

                Images = new ReadOnlyCollection<PartialTexture2D>(list);
            }
        }
    }
}
