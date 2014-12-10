using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Ui.Views
{
    public class UiImage : UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Image"] = parser.ParseResource<Texture2D>("Image");
            file["Stretch"] = parser.ParseEnum<Stretch>("Stretch");
            file["Color"] = parser.ParseColor("Color");
        }

        SharedValue<Texture2D> _image = null;

        Stretch _stretch = Stretch.Uniform;
        ColorWrapper _color = null;

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            Rectangle target = ScreenBounds;
            Rectangle source = Rectangle.Empty;

            lock (_image)
            {
                Texture2D image = _image.Value;

                if (image != null)
                {
                    source = new Rectangle(0, 0, image.Width, image.Height);

                    double scaleX = 1;
                    double scaleY = 1;

                    switch (_stretch)
                    {
                        case Stretch.Uniform:
                            scaleX = scaleY = Math.Min((double)target.Width / (double)image.Width, (double)target.Height / (double)image.Height);
                            break;

                        case Stretch.UniformToFill:
                            scaleX = scaleY = Math.Max((double)target.Width / (double)image.Width, (double)target.Height / (double)image.Height);
                            break;

                        case Stretch.Fill:
                            scaleX = (double)target.Width / (double)image.Width;
                            scaleY = (double)target.Height / (double)image.Height;
                            break;
                    }

                    Point pos = target.Center;

                    int width = (int)(Math.Ceiling(image.Width * scaleX));
                    int height = (int)(Math.Ceiling(image.Height * scaleY));

                    Rectangle targetRect = new Rectangle(pos.X - width / 2 - width % 2, pos.Y - height / 2 - height % 2, width, height);

                    target = GraphicsHelper.IntersectRectangle(targetRect, target);

                    int srcWidth = (int)((double)target.Width / scaleX);
                    int srcHeight = (int)((double)target.Height / scaleY);

                    pos = source.Center;
                    source = new Rectangle(pos.X - srcWidth / 2, pos.Y - srcHeight / 2, srcWidth, srcHeight);
                }

                parameters.DrawBatch.DrawImage(image, target, source, _color.Value * opacity);
            }
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiLabel));

            _image = DefinitionResolver.GetShared<Texture2D>(Controller, binding, file["Image"], null);
            _stretch = DefinitionResolver.Get<Stretch>(Controller, binding, file["Stretch"], Stretch.Uniform);
            _color = DefinitionResolver.GetColorWrapper(Controller, binding, file["Color"]);
        }
    }
}
