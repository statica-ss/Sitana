using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Games
{
    public class Map
    {
        public string TemplateGuid { get; private set; }

        List<Layer> _layers = new List<Layer>();

        public Map(string guid)
        {

        }
    }
}

