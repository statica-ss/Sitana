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
            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalAlignment>("VerticalContentAlignment");
        }

        protected string _font;
        protected int _fontSize;
        protected TextAlign _textAlign;

        private FontFace _fontFace;

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(NinePatchBackground));

            _font = DefinitionResolver.GetString(controller, binding, file["Font"]);
            _fontSize = DefinitionResolver.Get<int>(controller, binding, file["FontSize"], 0);

            HorizontalAlignment horzAlign = DefinitionResolver.Get<HorizontalAlignment>(controller, binding, file["HorizontalContentAlignment"], HorizontalAlignment.Center);
            VerticalAlignment vertAlign = DefinitionResolver.Get<VerticalAlignment>(controller, binding, file["VerticalContentAlignment"], VerticalAlignment.Center);

            _textAlign = UiHelper.TextAlignFromAlignment(horzAlign, vertAlign);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, UiButton.DrawButtonInfo info)
        {
            Update(info.EllapsedTime, info.ButtonState);

            SharedString str = info.Text;

            if (_fontFace == null)
            {
                _fontFace = FontManager.Instance.FindFont(_font);
            }

            float scale;
            UniversalFont font = _fontFace.Find(_fontSize, out scale);

            Color color = ColorFromState * info.Opacity;

            Rectangle target = _margin.ComputeRect(info.Target);

            drawBatch.DrawText(font, str, target, _textAlign, color, scale);
        }
    }
}
