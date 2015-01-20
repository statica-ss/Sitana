using Sitana.Framework.Ui.RichText;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Graphics;

namespace Sitana.Framework.Ui.Views.RichView
{
    class RichViewEntity
    {
        public EntityType Type = EntityType.String;
        public string Text = null;
        public Texture2D Image = null;

        public int Offset = 0;

        public string Url = null;

        public UniversalFont Font = null;
        public float FontScale = 1;
        public float FontSpacing = 0;

        public int Width = 0;

        public RichViewEntity Clone()
        {
            return new RichViewEntity()
            {
                Type = Type,
                Text = Text,
                Image = Image,
                Offset = Offset,
                Url = Url,
                Font = Font,
                FontScale = FontScale,
                FontSpacing = FontSpacing,
                Width = Width
            };
        }
    }
}
