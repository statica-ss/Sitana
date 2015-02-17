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
        }

        public SharedString Text { get; private set; }
        public ColorWrapper TextColor { get; private set; }

        public string FontName
        {
            get
            {
                return _fontName;
            }

            set
            {
                _fontName = value;
                _fontFace = null;
            }
        }
        
        public int FontSize {get;set;}
        public int FontSpacing {get; set;}
        public int LineHeight { get; set; }

        string _fontName;

        protected FontFace _fontFace = null;
        public TextAlign TextAlign {get;set;}

        int test = 0;

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0 || TextColor.Value.A == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            if (_fontFace == null)
            {
                _fontFace = FontManager.Instance.FindFont(FontName);
            }

            float scale;
            UniversalFont font = _fontFace.Find(FontSize, out scale);

            parameters.DrawBatch.DrawText(font, Text, ScreenBounds, TextAlign, TextColor.Value * opacity, (float)FontSpacing / 1000.0f, (float)LineHeight / 100.0f, scale);
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

        private Vector2 CalculateSizeInPixels()
        {
            if (_fontFace == null)
            {
                _fontFace = FontManager.Instance.FindFont(FontName);
            }

            float scale;
            UniversalFont font = _fontFace.Find(FontSize, out scale);

            Vector2 size;

            lock (Text)
            {
                size = font.MeasureString(Text.StringBuilder, (float)FontSpacing / 1000.0f, (float)LineHeight / 100.0f);
            }

            return size * scale;
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

            FontName = file["Font"] as string;
            FontSize = DefinitionResolver.Get<int>(Controller, Binding, file["FontSize"], 0);
            FontSpacing = DefinitionResolver.Get<int>(Controller, Binding, file["FontSpacing"], 0);
            LineHeight = DefinitionResolver.Get<int>(Controller, Binding, file["LineHeight"], 100);

            Text = DefinitionResolver.GetSharedString(Controller, Binding, file["Text"]);

            if (Text == null)
            {
                return false;
            }

            TextColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["TextColor"]) ?? new ColorWrapper(Color.White);

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

            return true;
        }
    }
}
