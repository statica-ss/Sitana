// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public class ButtonNinePatchBackground : ButtonSolidBackground
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            ButtonSolidBackground.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["ImagePushed"] = parser.ParseNinePatchImage("ImagePushed");
            file["ImageReleased"] = parser.ParseNinePatchImage("ImageReleased");
            file["ImageDisabled"] = parser.ParseNinePatchImage("ImageDisabled");
        }

        private static object ConvertToNinePatchImage(object definition)
        {
            if (definition is string)
            {
                return ContentLoader.Current.Load<NinePatchImage>(definition as string);
            }

            return definition;
        }

        protected NinePatchImage _imagePushed = null;
        protected NinePatchImage _imageReleased = null;
        protected NinePatchImage _imageDisabled = null;

        protected override void Init(UiController controller, object binding, DefinitionFile file)
        {
            base.Init(controller, binding, file);

            _imageDisabled = DefinitionResolver.Get<NinePatchImage>(controller, binding, file["ImageDisabled"]);
            _imageReleased = DefinitionResolver.Get<NinePatchImage>(controller, binding, file["ImageReleased"]);
            _imagePushed = DefinitionResolver.Get<NinePatchImage>(controller, binding, file["ImagePushed"]);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, ref Rectangle target, UiButton.State state)
        {
            Color color = ColorFromState(state);

            NinePatchImage image = _imageReleased;

            switch (state)
            {
                case UiButton.State.Disabled:
                    if (_imageDisabled != null)
                    {
                        image = _imageDisabled;
                    }
                    break;

                case UiButton.State.Pushed:
                    if (_imagePushed != null)
                    {
                        image = _imagePushed;
                    }
                    break;
            }

            drawBatch.NinePatchImage = image;
            drawBatch.DrawNinePatchRect(target, color);
        }
    }
}
