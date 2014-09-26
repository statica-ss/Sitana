using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebatianos.Graphics;
using Microsoft.Xna.Framework;
using Ebatianos.Ui.Controllers;
using Ebatianos.Ui.DefinitionFiles;
using Ebatianos.Essentials.Ui.DefinitionFiles;
using System.Xml;

namespace Ebatianos.Ui.Views.ButtonDrawables
{
    public class ButtonSolidBackground : StateDrawable<UiButton.State>, IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["ColorPushed"] = parser.ParseColor("ColorPushed");
            file["ColorReleased"] = parser.ParseColor("ColorReleased");
            file["ColorDisabled"] = parser.ParseColor("ColorDisabled");
        }

        protected ColorWrapper _colorPushed;
        protected ColorWrapper _colorReleased;
        protected ColorWrapper _colorDisabled;

        void IDefinitionClass.Init(object context, DefinitionFile file)
        {
            Init(context, file);
        }

        protected virtual void Init(object context, DefinitionFile file)
        {
            _colorDisabled = DefinitionResolver.GetColorWrapper(context, file["ColorDisabled"]);
            _colorReleased = DefinitionResolver.GetColorWrapper(context, file["ColorReleased"]);
            _colorPushed = DefinitionResolver.GetColorWrapper(context, file["ColorPushed"]);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, ref Rectangle target, UiButton.State state)
        {
            drawBatch.DrawRectangle(target, ColorFromState(state));
        }

        protected Color ColorFromState(UiButton.State state)
        {
            Color color = Color.Transparent;

            switch (state)
            {
                case UiButton.State.Disabled:
                    color = _colorDisabled.Value;
                    break;

                case UiButton.State.Pushed:
                    color = _colorPushed.Value;
                    break;

                case UiButton.State.Released:
                    color = _colorReleased.Value;
                    break;
            }

            return color;
        }
    }
}
