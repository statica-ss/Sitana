using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.Controllers;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using Sitana.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public class Text : ButtonDrawable
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            ButtonDrawable.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Font"] = parser.Value("Font");
            file["FontSize"] = parser.ParseInt("FontSize");
            file["TextAlign"] = parser.ParseEnum<TextAlign>("TextAlign");
            file["Padding"] = parser.ParseInt("Padding");
        }

        protected string _font;
        protected int _fontSize;
        protected TextAlign _textAlign;
        protected int _padding;

        private FontFace _fontFace;

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(NinePatchBackground));

            _padding = DefinitionResolver.Get<int>(controller, binding, file["Padding"], 0);

            _font = DefinitionResolver.GetString(controller, binding, file["Font"]);
            _fontSize = DefinitionResolver.Get<int>(controller, binding, file["FontSize"], 0);
            _textAlign = DefinitionResolver.Get<TextAlign>(controller, binding, file["TextAlign"], TextAlign.Center | TextAlign.Middle);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, UiButton.DrawButtonInfo info)
        {
            Update(info.EllapsedTime, info.ButtonState);

            SharedString str = info.Text;

            if (_fontFace == null)
            {
                _fontFace = FontManager.Instance.FindFont(_font);
            }

            UiButton.State state = info.ButtonState;

            float scale;
            UniversalFont font = _fontFace.Find(_fontSize, out scale);

            Color color = ColorFromState * info.Opacity;

            Rectangle rect = info.Target;
            rect.Inflate(-_padding, -_padding);

            drawBatch.DrawText(font, str, rect, _textAlign, color, scale);
        }
    }
}
