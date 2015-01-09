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

            file["TextColor"] = parser.ParseColor("TextColor");
            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalAlignment>("VerticalContentAlignment");
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

        string _fontName;

        FontFace _fontFace = null;


        public TextAlign TextAlign {get;set;}

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
            
            parameters.DrawBatch.DrawText(font, Text, ScreenBounds, TextAlign, TextColor.Value * opacity, (float)FontSpacing / 1000.0f, scale);
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
                    size.Y = (int)Math.Ceiling(sizeInPixels.Y);
                }
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
                size = font.MeasureString(Text.StringBuilder, (float)FontSpacing / 1000.0f);
            }

            return size * scale;
        }

        public Point CalculateSize()
        {
            Vector2 size = CalculateSizeInPixels();
            return (size / (float)UiUnit.Unit).ToPoint();
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiLabel));

            FontName = file["Font"] as string;
            FontSize = DefinitionResolver.Get<int>(Controller, Binding, file["FontSize"], 0);
            FontSpacing = DefinitionResolver.Get<int>(Controller, Binding, file["FontSpacing"], 0);

            Text = DefinitionResolver.GetSharedString(Controller, Binding, file["Text"]);
            TextColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["TextColor"]) ?? new ColorWrapper(Color.White);

            HorizontalAlignment horzAlign = DefinitionResolver.Get<HorizontalAlignment>(Controller, Binding, file["HorizontalContentAlignment"], HorizontalAlignment.Center);
            VerticalAlignment vertAlign = DefinitionResolver.Get<VerticalAlignment>(Controller, Binding, file["VerticalContentAlignment"], VerticalAlignment.Center);

            TextAlign = UiHelper.TextAlignFromAlignment(horzAlign, vertAlign);
        }
    }
}
