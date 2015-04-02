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
            file["FontSpacing"] = parser.ParseInt("FontSpacing");
            file["LineHeight"] = parser.ParseInt("LineHeight");
            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalContentAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalContentAlignment>("VerticalContentAlignment");
            file["Text"] = parser.ParseString("Text");
            file["TextRotation"] = parser.ParseEnum<TextRotation>("TextRotation");
        }

        protected UiFont _font;
        protected TextAlign _textAlign;
        protected SharedString _text;
        protected int _lineHeight;
        protected TextRotation _textRotation;

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(Text));

            string font = DefinitionResolver.GetString(controller, binding, file["Font"]);
            int fontSize = DefinitionResolver.Get<int>(controller, binding, file["FontSize"], 0);
            int fontSpacing = DefinitionResolver.Get<int>(controller, binding, file["FontSpacing"], 0);
            _lineHeight = DefinitionResolver.Get<int>(controller, binding, file["LineHeight"], 0);

            _font = new UiFont(font, fontSize, fontSpacing);

            _textRotation = DefinitionResolver.Get<TextRotation>(controller, binding, file["TextRotation"], TextRotation.None);

            HorizontalContentAlignment horzAlign = DefinitionResolver.Get<HorizontalContentAlignment>(controller, binding, file["HorizontalContentAlignment"], HorizontalContentAlignment.Center);
            VerticalContentAlignment vertAlign = DefinitionResolver.Get<VerticalContentAlignment>(controller, binding, file["VerticalContentAlignment"], VerticalContentAlignment.Center);

            _textAlign = UiHelper.TextAlignFromContentAlignment(horzAlign, vertAlign);
            _text = DefinitionResolver.GetSharedString(controller, binding, file["Text"]);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, DrawButtonInfo info)
        {
            Update(info.EllapsedTime, info.ButtonState);

            SharedString str = _text != null ? _text : info.Text;

            float scale = _font.Scale;
            UniversalFont font = _font.Font;

            Color color = ColorFromState * info.Opacity * Opacity;

            Rectangle target = _margin.ComputeRect(info.Target);

            drawBatch.DrawText(font, str, target, _textAlign, color, _font.Spacing, (float)_lineHeight / 100.0f, scale, _textRotation);
        }

        public override object OnAction(DrawButtonInfo info, params object[] parameters)
        {
            return null;
        }
    }
}
