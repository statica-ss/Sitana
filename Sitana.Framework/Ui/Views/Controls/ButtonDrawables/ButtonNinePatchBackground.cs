using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebatianos.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos.Content;
using Ebatianos.Ui.DefinitionFiles;
using Ebatianos.Essentials.Ui.DefinitionFiles;

namespace Ebatianos.Ui.Views.ButtonDrawables
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

        protected override void Init(object context, DefinitionFile file)
        {
            base.Init(context, file);

            _imageDisabled = DefinitionResolver.Get<NinePatchImage>(context, file["ImageDisabled"]);
            _imageReleased = DefinitionResolver.Get< NinePatchImage>(context, file["ImageReleased"]);
            _imagePushed = DefinitionResolver.Get<NinePatchImage>(context, file["ImagePushed"]);
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
