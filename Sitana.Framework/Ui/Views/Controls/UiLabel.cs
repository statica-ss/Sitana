// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.DefinitionFiles;
using System;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Cs;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views
{
    public class UiLabel: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Text"] = parser.ParseString("Text");
            file["Font"] = parser.ValueOrNull("Font");
            file["FontSize"] = parser.ParseInt("FontSize");
            file["FontSpacing"] = parser.ParseInt("FontSpacing");
            file["LineHeight"] = parser.ParseInt("LineHeight");

            file["TextColor"] = parser.ParseColor("TextColor");
            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalContentAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalContentAlignment>("VerticalContentAlignment");

            file["AutoSizeUpdate"] = parser.ParseBoolean("AutoSizeUpdate");
            file["TextRotation"] = parser.ParseEnum<TextRotation>("TextRotation");

            file["TextMargin"] = parser.ParseMargin("TextMargin");
            file["MaxWidth"] = parser.ParseLength("MaxWidth");
        }

        public static ColorWrapper DefaultTextColor = new ColorWrapper();

        public SharedString Text{ get; private set; }
        public ColorWrapper TextColor { get; protected set; }

        protected TextRotation _rotation;
        
        int FontSize {get; set;}
        int FontSpacing {get;  set;}

        protected int _lineHeight;

        protected Margin _textMargin;

        public TextAlign TextAlign {get;set;}

        protected UiFont _font;


        protected virtual UiFont Font
        {
            get
            {
                return _font;
            }
        }

        Length _maxWidth;
        protected float _rescale = 1;

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0 || TextColor.Value.A == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            UiFont font = Font;

            float scale = font.Scale;
            scale *= _rescale;

            Rectangle bounds = ScreenBounds;

            if(Text.Length > 0)
            {
                bounds =  _textMargin.ComputeRect(bounds);
            }

            parameters.DrawBatch.DrawText(font.Font, Text, bounds, TextAlign, TextColor.Value * opacity, font.Spacing, (float)_lineHeight / 100.0f, scale, _rotation);
        }

        public override Point ComputeSize(int width, int height)
        {
            Point size = base.ComputeSize(width, height);

            Vector2 sizeInPixels = new Vector2(-1,-1);

            if (PositionParameters.Width.IsAuto)
            {
                sizeInPixels = CalculateSizeInPixels();
                size.X = (int)Math.Ceiling(sizeInPixels.X);
            }

            if (PositionParameters.Height.IsAuto)
            {
                if (sizeInPixels.Y < 0)
                {
                    sizeInPixels = CalculateSizeInPixels();
                }
                size.Y = (int)Math.Ceiling(sizeInPixels.Y);
            }

            return size;
        }

        protected Vector2 CalculateSizeInPixels()
        {
            Vector2 size;

            UiFont font = Font;

            lock (Text)
            {
                size = font.Font.MeasureString(Text.StringBuilder, font.Spacing, (float)_lineHeight / 100.0f);
            }

            switch(_rotation)
            {
                case TextRotation.Rotate270:
                case TextRotation.Rotate90:
                    size = new Vector2(size.Y, size.X);
                    break;
            }

            Vector2 marginIfText = Vector2.Zero;

            if(Text.Length > 0)
            {
                marginIfText = new Vector2(_textMargin.Width, _textMargin.Height);
            }

            float scale = font.Scale;
            size = size * scale + marginIfText;
            
            int maxWidth = _maxWidth.Compute(Parent.Bounds.Width);

            if(size.X > maxWidth)
            {
                _rescale = (float)maxWidth / size.X;
                size = size * _rescale; 
            }

            return size;
        }

        public Point CalculateSize()
        {
            Vector2 size = CalculateSizeInPixels();
            return (size / (float)UiUnit.Unit).ToPoint();
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiLabel));

            string fontName = file["Font"] as string;
            int fontSize = DefinitionResolver.Get<int>(Controller, Binding, file["FontSize"], 0);
            int fontSpacing = DefinitionResolver.Get<int>(Controller, Binding, file["FontSpacing"], 0);

            _font = new UiFont(fontName, fontSize, fontSpacing);

            _lineHeight = DefinitionResolver.Get<int>(Controller, Binding, file["LineHeight"], 100);

            _textMargin = DefinitionResolver.Get<Margin>(Controller, Binding, file["TextMargin"], Margin.None);

            _rotation = DefinitionResolver.Get<TextRotation>(Controller, Binding, file["TextRotation"], TextRotation.None);

            _maxWidth = DefinitionResolver.Get<Length>(Controller, Binding, file["MaxWidth"], new Length(pixels: int.MaxValue));

            Text = DefinitionResolver.GetSharedString(Controller, Binding, file["Text"]);

            if (Text == null)
            {
                return false;
            }

            TextColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["TextColor"]) ?? DefaultTextColor;

            HorizontalContentAlignment horzAlign = DefinitionResolver.Get<HorizontalContentAlignment>(Controller, Binding, file["HorizontalContentAlignment"], HorizontalContentAlignment.Auto);
            VerticalContentAlignment vertAlign = DefinitionResolver.Get<VerticalContentAlignment>(Controller, Binding, file["VerticalContentAlignment"], VerticalContentAlignment.Auto);

            if (horzAlign == HorizontalContentAlignment.Auto)
            {
                horzAlign = UiHelper.ContentAlignFromAlignment(PositionParameters.HorizontalAlignment);
            }

            if (vertAlign == VerticalContentAlignment.Auto)
            {
                vertAlign = UiHelper.ContentAlignFromAlignment(PositionParameters.VerticalAlignment);
            }

            TextAlign = UiHelper.TextAlignFromContentAlignment(horzAlign, vertAlign);

            if(DefinitionResolver.Get<bool>(Controller, Binding, file["AutoSizeUpdate"], false))
            {
                Text.ValueChanged += Text_ValueChanged;
            }

            return true;
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            Text.ValueChanged -= Text_ValueChanged;
        }

        protected void Text_ValueChanged()
        {
            if(Parent!=null)
            {
                UiTask.BeginInvoke(() =>
                    {
                        SetForceRecalcFlag();
                        Parent.RecalcLayout();
                    });
            }
        }
    }
}
