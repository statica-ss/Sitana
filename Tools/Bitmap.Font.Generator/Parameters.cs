using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace XnaFontGenerator
{
   class Parameters
   {
      public Color TextColor = Color.White;
      public Color BorderColor = Color.Black;
      public Rectangle BorderSize = new Rectangle(-1, -1, 3, 3);
      public Double BorderOpacity = 0.75;
      public Int32 FirstCharacter = 32;
      public Int32 LastCharacter = 127;
      public String AdditionalCharacters = "";
      public Double BlurSize = 0;
   }
}
