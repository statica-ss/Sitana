// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public class NinePatchBackground : ButtonDrawable
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            ButtonDrawable.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["ImagePushed"] = parser.ParseResource<NinePatchImage>("ImagePushed");
            file["ImageReleased"] = parser.ParseResource<NinePatchImage>("Image");
            file["ImageDisabled"] = parser.ParseResource<NinePatchImage>("ImageDisabled");
            file["ScaleByUnit"] = parser.ParseBoolean("ScaleByUnit");
            file["Scale"] = parser.ParseFloat("Scale");
        }

        protected NinePatchImage _imagePushed = null;
        protected NinePatchImage _imageReleased = null;
        protected NinePatchImage _imageDisabled = null;

        protected bool _scaleByUnit = false;
        protected float _scale = 1;

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(NinePatchBackground));

            _imageDisabled = DefinitionResolver.Get<NinePatchImage>(controller, binding, file["ImageDisabled"], null);
            _imageReleased = DefinitionResolver.Get<NinePatchImage>(controller, binding, file["ImageReleased"], null);
            _imagePushed = DefinitionResolver.Get<NinePatchImage>(controller, binding, file["ImagePushed"], null);
            _scaleByUnit = DefinitionResolver.Get<bool>(controller, binding, file["ScaleByUnit"], false);
            _scale = (float)DefinitionResolver.Get<double>(controller, binding, file["Scale"], 1);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, UiButton.DrawButtonInfo info)
        {
            Update(info.EllapsedTime, info.ButtonState);

            UiButton.State state = info.ButtonState;

            Color color = ColorFromState * info.Opacity * Opacity;

            NinePatchImage image = _imageReleased;

            switch (state & UiButton.State.Mask)
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

            float scale = _scaleByUnit ? (float)UiUnit.Unit : 1;
            Rectangle target = _margin.ComputeRect(info.Target);

            drawBatch.DrawNinePatchRect(image, target, color, scale * _scale);
        }
    }
}
