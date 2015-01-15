using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using Sitana.Framework.Content;
using Sitana.Framework.Text;
using CommonMark;
using System.IO;
using CommonMark.Syntax;
using Sitana.Framework.Ui.RichText;


namespace Sitana.Framework.Ui.Views
{
    public class UiRichView : UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Processor"] = Type.GetType(node.Attribute("Processor"));

            file["Text"] = parser.ParseString("Text");
            file["Font"] = parser.ValueOrNull("Font");
            file["FontSize"] = parser.ParseInt("FontSize");
            file["FontSpacing"] = parser.ParseInt("FontSpacing");

            for (int idx = 0; idx < (int)FontType.Count; ++idx)
            {
                FontType type = (FontType)idx;
                string font = string.Format("{0}.Font", type);
                string spacing = string.Format("{0}.FontSpacing", type);

                file[font] = parser.ValueOrNull(font);
                file[spacing] = parser.ParseInt(spacing);
            }

            for (int idx = 0; idx < (int)SizeType.Count; ++idx)
            {
                SizeType type = (SizeType)idx;
                string spacing = string.Format("{0}.FontSize", type);

                file[spacing] = parser.ParseInt(spacing);
            }

            file["LineHeight"] = parser.ParseInt("LineHeight");
            file["Indent"] = parser.ParseLength("Indent");

            file["Justify"] = parser.ParseBoolean("Justify");

            file["TextColor"] = parser.ParseColor("TextColor");
            file["LinkColor"] = parser.ParseColor("LinkColor");

            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalAlignment>("VerticalContentAlignment");
        }

        float _lineHeight;

        TextAlign _textAlign;

        Point _lastSize = Point.Zero;
        bool _justify;

        FontInfo[] _fonts = new FontInfo[(int)FontType.Count];
        int[] _sizes = new int[(int)SizeType.Count];

        string _text;
        ColorWrapper _colorNormal;
        ColorWrapper _colorClickable;

        IRichProcessor _richProcessor;

        List<Line> _lines = new List<Line>();

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = HtmlSpecialChars.Convert(value);
                _lastSize = Point.Zero;
            }
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiText));



            string defaultFont = file["Font"] as string;
            int defaultFontSize = DefinitionResolver.Get<int>(Controller, Binding, file["FontSize"], 0);
            int defaultFontSpacing = DefinitionResolver.Get<int>(Controller, Binding, file["FontSpacing"], int.MaxValue);

            for (int idx = 0; idx < (int)FontType.Count; ++idx)
            {
                FontType type = (FontType)idx;
                string font = string.Format("{0}.Font", type);
                string spacing = string.Format("{0}.FontSpacing", type);

                string fontName = file[font] as string;
                int fontSpacing = DefinitionResolver.Get<int>(Controller, Binding, file[spacing], defaultFontSpacing == int.MaxValue ? 0 : defaultFontSpacing);

                if (fontName == null)
                {
                    fontName = defaultFont;
                }

                FontFace fontObj = FontManager.Instance.FindFont(fontName);

                if (defaultFont == null)
                {
                    defaultFont = fontName;
                }

                if (defaultFontSpacing == int.MaxValue)
                {
                    defaultFontSpacing = fontSpacing;
                }

                _fonts[idx] = new FontInfo()
                {
                    Font = fontObj,
                    FontSpacing = (float)fontSpacing / 1000.0f
                };
            }

            for (int idx = 0; idx < (int)SizeType.Count; ++idx)
            {
                SizeType type = (SizeType)idx;
                string size = string.Format("{0}.FontSize", type);

                int fontSize = DefinitionResolver.Get<int>(Controller, Binding, file[size], defaultFontSize);

                if (defaultFontSize == 0)
                {
                    defaultFontSize = fontSize;
                }

                _sizes[idx] = fontSize;
            }

            _lineHeight = (float)DefinitionResolver.Get<int>(Controller, Binding, file["LineHeight"], 100) / 100.0f;
            _justify = DefinitionResolver.Get<bool>(Controller, Binding, file["Justify"], false);

            Text = DefinitionResolver.GetString(Controller, Binding, file["Text"]);

            _colorNormal = DefinitionResolver.GetColorWrapper(Controller, Binding, file["TextColor"]) ?? new ColorWrapper(Color.White);
            _colorClickable = DefinitionResolver.GetColorWrapper(Controller, Binding, file["LinkColor"]) ?? new ColorWrapper(Color.White);

            HorizontalAlignment horzAlign = DefinitionResolver.Get<HorizontalAlignment>(Controller, Binding, file["HorizontalContentAlignment"], HorizontalAlignment.Left);
            VerticalAlignment vertAlign = DefinitionResolver.Get<VerticalAlignment>(Controller, Binding, file["VerticalContentAlignment"], VerticalAlignment.Top);

            _textAlign = UiHelper.TextAlignFromAlignment(horzAlign, vertAlign);

            Type processorType = file["Processor"] as Type;

            if (processorType != null)
            {
                _richProcessor = Activator.CreateInstance(processorType) as IRichProcessor;
            }
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if (_lastSize.X != Bounds.Width || _lastSize.Y != Bounds.Height)
            {
                _lastSize.X = Bounds.Width;
                _lastSize.Y = Bounds.Height;
                Process();
                Parent.RecalculateAll();
            }
        }

        void Process()
        {
            _lines.Clear();

            if (Bounds.Width < 2)
            {
                return;
            }

            _richProcessor.Process(_lines, _text);
        }
    }
}
