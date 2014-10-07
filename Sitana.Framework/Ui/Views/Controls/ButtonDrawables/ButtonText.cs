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
    public class Text : ButtonDrawable, IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["ColorPushed"] = parser.ParseColor("ColorPushed");
            file["ColorReleased"] = parser.ParseColor("ColorReleased");
            file["ColorDisabled"] = parser.ParseColor("ColorDisabled");

            file["Font"] = parser.Value("Font");
            file["FontSize"] = parser.ParseInt("FontSize");
            file["TextAlign"] = parser.ParseEnum<TextAlign>("TextAlign");
            file["Padding"] = parser.ParseInt("Padding");
        }

        protected ColorWrapper _colorPushed;
        protected ColorWrapper _colorReleased;
        protected ColorWrapper _colorDisabled;

        protected string _font;
        protected int _fontSize;
        protected TextAlign _textAlign;
        protected int _padding;

        private FontFace _fontFace;

        void IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            Init(controller, binding, file);
        }

        protected virtual void Init(UiController controller, object binding, DefinitionFile definition)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(NinePatchBackground));

            _colorDisabled = DefinitionResolver.GetColorWrapper(controller, binding, file["ColorDisabled"]);
            _colorReleased = DefinitionResolver.GetColorWrapper(controller, binding, file["ColorReleased"]);
            _colorPushed = DefinitionResolver.GetColorWrapper(controller, binding, file["ColorPushed"]);
            _padding = DefinitionResolver.Get<int>(controller, binding, file["Padding"], 0);

            _font = DefinitionResolver.GetString(controller, binding, file["Font"]);
            _fontSize = DefinitionResolver.Get<int>(controller, binding, file["FontSize"], 0);
            _textAlign = DefinitionResolver.Get<TextAlign>(controller, binding, file["TextAlign"], TextAlign.Center | TextAlign.Middle);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, Rectangle target, float opacity, UiButton.DrawButtonInfo info)
        {
            SharedString str = info.Text;

            if (_fontFace == null)
            {
                _fontFace = FontManager.Instance.FindFont(_font);
            }

            UiButton.State state = info.ButtonState;

            float scale;
            UniversalFont font = _fontFace.Find(_fontSize, out scale);

            Color color = ColorFromState(state) * opacity;

            Rectangle rect = target;
            rect.Inflate(-_padding, -_padding);

            drawBatch.DrawText(font, str, rect, _textAlign, color, scale);
        }

        protected Color ColorFromState(UiButton.State state)
        {
            Color color = Color.Transparent;

            switch (state & UiButton.State.Mask)
            {
                case UiButton.State.Disabled:
                    color = _colorDisabled.Value;
                    break;

                case UiButton.State.Pushed:
                    color = _colorPushed.Value;
                    break;

                case UiButton.State.None:
                    color = _colorReleased.Value;
                    break;
            }

            return color;
        }
    }
}
