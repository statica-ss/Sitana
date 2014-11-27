using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Graphics
{
    public class Sprite
    {
        Dictionary<string, List<PartialTexture2D>> _sequences = new Dictionary<string, List<PartialTexture2D>>();

        public static void LoadSprites(string name)
        {

        }

        SpriteInstance CreateInstance()
        {
            return new SpriteInstance(this);
        }

        
    }
}
