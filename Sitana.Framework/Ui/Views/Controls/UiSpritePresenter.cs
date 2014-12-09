using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.Parameters;

namespace Sitana.Framework.Ui.Views
{
    public class UiSpritePresenter : UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Sprite"] = parser.ParseResource<Sprite>("Sprite");
            file["Sequence"] = parser.ParseString("Sequence");
        }

        SpriteInstance _spriteInstance;

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

            
            source = new Rectangle(0, 0, _spriteInstance.FrameSize.X, _spriteInstance.FrameSize.Y);

            double scaleX = 1;
            double scaleY = 1;

            scaleX = scaleY = Math.Min((double)target.Width / (double)source.Width, (double)target.Height / (double)source.Height);

            Point pos = target.Center;

            int width = (int)(Math.Ceiling(_spriteInstance.FrameSize.X * scaleX));
            int height = (int)(Math.Ceiling(_spriteInstance.FrameSize.Y * scaleY));

            Rectangle targetRect = new Rectangle(pos.X - width / 2 - width % 2, pos.Y - height / 2 - height % 2, width, height);

            target = GraphicsHelper.IntersectRectangle(targetRect, target);

            PartialTexture2D image = _spriteInstance.FrameImage;

            parameters.DrawBatch.DrawImage(image.Texture, target, image.Source, Color.White);
        }

        protected override void Update(float time)
        {
            base.Update(time);

            _spriteInstance.Animate(time);
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiLabel));

            _spriteInstance = DefinitionResolver.Get<Sprite>(Controller, binding, file["Sprite"], null).CreateInstance();
            _spriteInstance.Sequence = DefinitionResolver.GetString(Controller, binding, file["Sequence"]);
        }
    }
}
