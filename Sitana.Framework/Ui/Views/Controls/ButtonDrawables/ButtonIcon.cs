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
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public class Icon : ButtonDrawable
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            ButtonDrawable.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalContentAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalContentAlignment>("VerticalContentAlignment");
            file["Scale"] = parser.ParseDouble("Scale");
            file["Stretch"] = parser.ParseEnum<Stretch>("Stretch");
        }

        protected string _font;
        protected int _fontSize;
        protected HorizontalContentAlignment _horzAlign;
        protected VerticalContentAlignment _vertAlign;
        protected float _scale;
        protected Stretch _stretch;

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(Icon));

            _horzAlign = DefinitionResolver.Get<HorizontalContentAlignment>(controller, binding, file["HorizontalContentAlignment"], HorizontalContentAlignment.Center);
            _vertAlign = DefinitionResolver.Get<VerticalContentAlignment>(controller, binding, file["VerticalContentAlignment"], VerticalContentAlignment.Center);
            _stretch = DefinitionResolver.Get<Stretch>(controller, binding, file["Stretch"], Stretch.None);
            _scale = (float)DefinitionResolver.Get<double>(controller, binding, file["Scale"], 1);
        }

        public override void Draw(AdvancedDrawBatch drawBatch, DrawButtonInfo info)
        {
            Update(info.EllapsedTime, info.ButtonState);

            if (info.Icon == null)
            {
                return;
            }

            float scale = (float)UiUnit.Unit * _scale;

            float scaleX = scale;
            float scaleY = scale;

            Rectangle target = info.Target;
            Texture2D image = info.Icon;
            
            Rectangle bounds = _margin.ComputeRect(target);

            switch (_stretch)
            {
                case Stretch.Uniform:
                    scaleX = scaleY = Math.Min((float)target.Width / (float)image.Width, (float)target.Height / (float)image.Height);
                    break;

                case Stretch.UniformToFill:
                    scaleX = scaleY = Math.Max((float)target.Width / (float)image.Width, (float)target.Height / (float)image.Height);
                    break;

                case Stretch.Fill:
                    scaleX = (float)target.Width / (float)image.Width;
                    scaleY = (float)target.Height / (float)image.Height;
                    break;
            }

            Color color = ColorFromState * info.Opacity * Opacity;

            Rectangle source = new Rectangle(0, 0, info.Icon.Width, info.Icon.Height);
            Vector2 size = new Vector2(source.Width * scaleX, source.Height * scaleY);

            

            target.Width = (int)size.X;
            target.Height = (int)size.Y;

            switch (_horzAlign)
            {
                case HorizontalContentAlignment.Center:
                case HorizontalContentAlignment.Auto:
                    target.X = bounds.Center.X - target.Width / 2;
                    break;

                case HorizontalContentAlignment.Left:
                    target.X = bounds.X;
                    break;

                case HorizontalContentAlignment.Right:
                    target.X = bounds.Right - target.Width;
                    break;
            }

            switch (_vertAlign)
            {
                case VerticalContentAlignment.Center:
                case VerticalContentAlignment.Auto:
                    target.Y = bounds.Center.Y - target.Height / 2;
                    break;

                case VerticalContentAlignment.Top:
                    target.Y = bounds.Y;
                    break;

                case VerticalContentAlignment.Bottom:
                    target.Y = bounds.Bottom - target.Height;
                    break;
            }

            

            drawBatch.DrawImage(info.Icon, target, source, color);
        }
    }
}
