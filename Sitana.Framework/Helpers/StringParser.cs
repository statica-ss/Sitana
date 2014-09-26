using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sitana.Framework
{
    public static class StringParser
    {
        public static Rectangle ParseRectangle(string value)
        {
            string[] vals = value.Replace(" ", "").Split(',');
            return new Rectangle(int.Parse(vals[0]), int.Parse(vals[1]), int.Parse(vals[2]), int.Parse(vals[3]));
        }
    }
}
