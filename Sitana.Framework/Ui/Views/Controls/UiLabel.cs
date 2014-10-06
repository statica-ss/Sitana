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

namespace Sitana.Framework.Ui.Views
{
    public class UiLabel: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Text"] = parser.ParseString("Text");
            file["Font"] = parser.Value("Font");
            file["FontSize"] = parser.ParseInt("FontSize");

            file["TextColor"] = parser.ParseColor("TextColor");
            file["TextAlign"] = parser.ParseEnum<TextAlign>("TextAlign");
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

        string _fontName;

        FontFace _fontFace = null;

        public TextAlign TextAlign {get;set;}

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = DisplayOpacity * parameters.Opacity;

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
            Font font = _fontFace.Find(FontSize, out scale);
            
            parameters.DrawBatch.Font = font;
            parameters.DrawBatch.DrawText(Text, ScreenBounds, TextAlign, TextColor.Value * opacity, scale);
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiLabel));

            FontName = file["Font"] as string;
            FontSize = DefinitionResolver.Get<int>(Controller, binding, file["FontSize"], 0);

            Text = DefinitionResolver.GetSharedString(Controller, binding, file["Text"]);
            TextColor = DefinitionResolver.GetColorWrapper(Controller, binding, file["TextColor"]) ?? new ColorWrapper(Color.White);

            TextAlign = DefinitionResolver.Get<TextAlign>(Controller, binding, file["TextAlign"], TextAlign.Middle | TextAlign.Center);
        }
    }
}
