using System;
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
    public class Image : ButtonDrawable
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            ButtonDrawable.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Image"] = parser.ParseResource<Texture2D>("Image");
            file["Scale"] = parser.ParseFloat("Scale");
            file["HorizontalAlignment"] = parser.ParseEnum<HorizontalAlignment>("HorizontalAlignment");
            file["VerticalAlignment"] = parser.ParseEnum<VerticalAlignment>("VerticalAlignment");
        }

        protected Texture2D _image = null;
        protected float _scale = 1;

        protected HorizontalAlignment _horizontalAlignment;
        protected VerticalAlignment _verticalAlignment;

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(NinePatchBackground));

            _image = DefinitionResolver.Get<Texture2D>(controller, binding, file["Image"], null);
            _scale = (float)DefinitionResolver.Get<double>(controller, binding, file["Scale"], 1);
            _horizontalAlignment = DefinitionResolver.Get<HorizontalAlignment>(controller, binding, file["HorizontalAlignment"], HorizontalAlignment.Center);
            _verticalAlignment = DefinitionResolver.Get<VerticalAlignment>(controller, binding, file["VerticalAlignment"], VerticalAlignment.Center);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, DrawButtonInfo info)
        {
            Update(info.EllapsedTime, info.ButtonState);

            Color color = ColorFromState * info.Opacity * Opacity;

            Texture2D image = _image;

            if (image != null)
            {
                float scale = (float)UiUnit.Unit * _scale;

                Rectangle textureSrc = new Rectangle(0, 0, image.Width, image.Height);

                int width = (int)(scale * image.Width);
                int height = (int)(scale * image.Height);

                Rectangle target = _margin.ComputeRect(info.Target);

                switch ( _horizontalAlignment )
                {
                case HorizontalAlignment.Center:
                    target.X = target.Center.X - width / 2;
                    break;

                case HorizontalAlignment.Right:
                    target.X = target.Right - width;
                    break;
                }

                switch ( _verticalAlignment)
                {
                case VerticalAlignment.Center:
                    target.Y = target.Center.Y - height / 2;
                    break;

                case VerticalAlignment.Bottom:
                    target.Y = target.Bottom - height;
                    break;
                }

                target.Width = width;
                target.Height = height;

                drawBatch.DrawImage(image, target, textureSrc, color);
            }
        }
    }
}
