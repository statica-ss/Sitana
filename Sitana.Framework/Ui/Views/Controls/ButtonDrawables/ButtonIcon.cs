﻿using System;
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

            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalAlignment>("VerticalContentAlignment");
            file["Scale"] = parser.ParseFloat("Scale");
        }

        protected string _font;
        protected int _fontSize;
        protected HorizontalAlignment _horzAlign;
        protected VerticalAlignment _vertAlign;
        protected float _scale;

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(Icon));

            _horzAlign = DefinitionResolver.Get<HorizontalAlignment>(controller, binding, file["HorizontalContentAlignment"], HorizontalAlignment.Center);
            _vertAlign = DefinitionResolver.Get<VerticalAlignment>(controller, binding, file["VerticalContentAlignment"], VerticalAlignment.Center);
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

            Color color = ColorFromState * info.Opacity * Opacity;

            Rectangle target = info.Target;
            Rectangle source = new Rectangle(0, 0, info.Icon.Width, info.Icon.Height);

            Vector2 size = new Vector2(source.Width, source.Height) * scale;

            target.Width = (int)size.X;
            target.Height = (int)size.Y;

            switch (_horzAlign)
            {
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    target.X = info.Target.Center.X - target.Width / 2;
                    break;

                case HorizontalAlignment.Left:
                    target.X = info.Target.X;
                    break;

                case HorizontalAlignment.Right:
                    target.X = info.Target.Right - target.Width;
                    break;
            }

            switch (_vertAlign)
            {
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    target.Y = info.Target.Center.Y - target.Height / 2;
                    break;

                case VerticalAlignment.Top:
                    target.Y = info.Target.Y;
                    break;

                case VerticalAlignment.Bottom:
                    target.Y = info.Target.Bottom - target.Height;
                    break;
            }

            target = _margin.ComputeRect(target);

            drawBatch.DrawImage(info.Icon, target, source, color);
        }
    }
}
