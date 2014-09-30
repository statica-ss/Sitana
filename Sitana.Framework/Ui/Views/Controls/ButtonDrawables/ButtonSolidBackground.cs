// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public class SolidBackground : ButtonDrawable, IDefinitionClass
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
        }

        public override void Draw(AdvancedDrawBatch drawBatch, Rectangle target, float opacity, UiButton.State state, object argument)
        {
            drawBatch.Texture = null;
            drawBatch.DrawRectangle(target, ColorFromState(state) * opacity);
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
