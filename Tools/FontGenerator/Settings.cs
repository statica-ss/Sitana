using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Settings;
using System.Drawing;

namespace FontGenerator
{
    public class Settings: SingletonSettings<Settings>
    {
        public string Face;
        public int Size;
        public int Style;
        public int BorderColor;
        public int FillColor;
        public int BorderSize;
        public int BorderOpacity;
        public string AdditionalCharacters;
        public string MinChar;
        public string MaxChar;
        public bool RoundBorder;
        public string Serie;
        public bool Kerning;
        public int CutOpacity;

        protected override void Init()
        {
            if (!Loaded)
            {
                Face = "Tahoma";
                Size = 16;
                Style =0;
                BorderColor = Color.Black.ToArgb();
                FillColor = Color.White.ToArgb();
                BorderSize = 10;
                BorderOpacity = 100;
                MinChar = "32";
                MaxChar = "127";
                AdditionalCharacters = "©";
                RoundBorder = true;
                Serie = "10";
                Kerning = false;
                CutOpacity = 128;
            }
        }
    }
}
