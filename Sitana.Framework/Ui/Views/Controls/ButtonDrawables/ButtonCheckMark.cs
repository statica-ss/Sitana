﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public class CheckMark : ButtonDrawable
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            ButtonDrawable.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["ImageUnchecked"] = parser.ParseResource<Texture2D>("ImageUnchecked");
            file["ImageChecked"] = parser.ParseResource<Texture2D>("ImageChecked");
            file["Scale"] = parser.ParseFloat("Scale");
        }

        protected Texture2D _imageChecked = null;
        protected Texture2D _imageUnchecked = null;
        protected float _scale = 1;

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(NinePatchBackground));

            _imageChecked = DefinitionResolver.Get<Texture2D>(controller, binding, file["ImageChecked"], null);
            _imageUnchecked = DefinitionResolver.Get<Texture2D>(controller, binding, file["ImageUnchecked"], null);
            _scale = (float)DefinitionResolver.Get<double>(controller, binding, file["Scale"], 1);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, UiButton.DrawButtonInfo info)
        {
            Update(info.EllapsedTime, info.ButtonState);

            UiButton.State state = info.ButtonState;

            Color color = ColorFromState * info.Opacity;

            Texture2D image = _imageUnchecked;

            if ((state & UiButton.State.Checked) != UiButton.State.None)
            {
                image = _imageChecked;
            }

            if (image != null)
            {
                float scale = (float)UiUnit.Unit * _scale;

                Rectangle textureSrc = new Rectangle(0, 0, image.Width, image.Height);

                int width = (int)(scale * image.Width);
                int height = (int)(scale * image.Height);

                Rectangle target = info.Target;

                target.X = target.Center.X - width / 2;
                target.Y = target.Center.Y - height / 2;

                target.Width = width;
                target.Height = height;

                drawBatch.DrawImage(image, target, textureSrc, color);
            }
        }
    }
}