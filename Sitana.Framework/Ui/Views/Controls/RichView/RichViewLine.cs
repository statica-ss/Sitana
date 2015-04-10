using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Views.RichView
{
    class RichViewLine
    {
        public readonly List<RichViewEntity> Entities = new List<RichViewEntity>();
        public int Height = 0;
        public int BaseLine = 0;
        public bool NewParagraph = false;
        public int Width = 0;
    }
}
