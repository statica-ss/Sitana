using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Graphics;
using Sitana.Framework.Content;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.Parameters;
using System.Text.RegularExpressions;

namespace Sitana.Framework.Ui.Views
{
    public class UiText: UiView
    {
        struct TextAndSpacing
        {
            public TextAndSpacing(string text, float spacing, int indent)
            {
                Line = text;
                Spacing = spacing;
                Indent = indent;
            }



            public readonly string Line;
            public readonly float Spacing;
            public readonly int Indent;
        }

        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Text"] = parser.ParseString("Text");
            file["Font"] = parser.ValueOrNull("Font");
            file["FontSize"] = parser.ParseInt("FontSize");
            file["FontSpacing"] = parser.ParseInt("FontSpacing");
            file["LineHeight"] = parser.ParseInt("LineHeight");
            file["Indent"] = parser.ParseLength("Indent");

            file["Justify"] = parser.ParseBoolean("Justify");

            file["TextColor"] = parser.ParseColor("TextColor");
            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalAlignment>("VerticalContentAlignment");
        }

        public ColorWrapper TextColor { get; private set; }

        List<TextAndSpacing> _lines = new List<TextAndSpacing>();

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
                SetText(_text);
            }
        }

        public int LineHeight { get; set; }
        public int FontSpacing { get; set; }
        public int FontSize { get; set; }
        public TextAlign TextAlign { get; set; }


        public override Rectangle Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                base.Bounds = value;
            }
        }

        public bool Justify
        {
            get
            {
                return _justify;
            }

            set
            {
                _justify = value;
                SetText(_text);
            }
        }
        
        string _fontName;
        bool _justify;

        FontFace _fontFace = null;

        string _text = string.Empty;

        Point _lastSize = Point.Zero;
        Length _indent;

        public string Text
        {
            set
            {
                SetText(value.Replace("&nbsp;", ((char)0xa0).ToString()));
            }

            get
            {
                return _text;
            }
        }

        void SetText(string value)
        {
            _text = value;

            int width = Bounds.Width;

            if (width < 2)
            {
                return;
            }

            float spacing = (float)FontSpacing / 1000.0f;
            float lineHeight = (float)LineHeight / 100.0f;

            UniversalFont font;
            float scale;

            GetFont(out font, out scale);

            _lines.Clear();
            
            if (value.Length == 0)
            {
                return;
            }

            List<string> lines = value.Split('\n').ToList();
            List<string> newLines = new List<string>();

            for (int idx = 0; idx < lines.Count;)
            {
                string line = lines[idx];

                int indentation = 0;
                int maxWidth = width;

                while (line.StartsWith("\t"))
                {
                    line = line.Substring(1);
                    indentation++;
                    maxWidth -= _indent.Compute(width);
                }

                int index = 0;
                int presize = 0;

                while (index >= 0)
                {
                    presize = 4;
                    int newIndex = line.IndexOf("![](", index);

                    if (newIndex < 0)
                    {
                        presize = 3;
                        newIndex = line.IndexOf("[](", index);
                    }

                    index = newIndex;

                    if (index >= 0)
                    {
                        int index2 = line.IndexOf(')', index);

                        if (index2 > 0)
                        {
                            int lengthBefore = line.Length;

                            string inside = line.Substring(index + presize, index2 - index - presize);
                            line = line.Remove(index, index2 - index + 1);

                            if (presize == 3)
                            {
                                string[] link = inside.Split(' ');
                                if (link.Length > 1)
                                {
                                    line = line.Insert(index, link.Merge(" ", 1, link.Length - 1));
                                }
                                else
                                {
                                    line = line.Insert(index, link[0]);
                                }
                            }

                            int lengthAfter = line.Length;

                            index = Math.Max(0, index2 + lengthAfter - lengthBefore);
                        }
                        else
                        {
                            index++;
                        }
                    }
                }

                Vector2 size = font.MeasureString(line, spacing, lineHeight) * scale;

                if (size.X > maxWidth)
                {
                    lines.RemoveAt(idx);

                    SplitLine(newLines, font, line, scale, maxWidth);

                    if ( indentation > 0 )
                    {
                        string indent = new string('\t', indentation);

                        for (int idx2 = 0; idx2 < newLines.Count; ++idx2)
                        {
                            newLines[idx2] = indent + newLines[idx2];
                        }
                    }
                    lines.InsertRange(idx, newLines);

                    idx += newLines.Count;
                }
                else
                {
                    if (indentation > 0)
                    {
                        line = new string('\t', indentation) + line;
                    }

                    lines[idx] = line + '\r';
                    ++idx;
                }
            }

            int height = font.Height;

            foreach (var line in lines)
            {
                string text = line;
                int maxWidth = width;

                float lineSpacing = spacing;
                int indent = 0;

                while (text.StartsWith("\t"))
                {
                    indent++;
                    maxWidth -= _indent.Compute(width);
                    text = text.Replace("\t", "");
                }

                if (text.EndsWith("\r"))
                {
                    text = text.Replace("\r", "");
                }
                else if (_justify && line.Length > 1)
                {
                    Vector2 size = font.MeasureString(text, spacing) * scale;

                    if (size.X < maxWidth)
                    {
                        float addSpacing = (float)(maxWidth - size.X) / (float)(line.Length - 1);
                        addSpacing /= height;

                        lineSpacing += addSpacing;
                    }
                }

                _lines.Add(new TextAndSpacing(text, lineSpacing, indent));
            }
        }

        void SplitLine(List<string> lines, UniversalFont font, string line, float scale, int maxWidth)
        {
            float spacing = (float)FontSpacing / 1000.0f;
            float lineHeight = (float)LineHeight / 100.0f;

            lines.Clear();

            StringBuilder newLine = new StringBuilder();
            
            for (int idx = 0; idx <= line.Length; ++idx)
            {
                char character = idx == line.Length ? ' ' : line[idx];

                if (Char.IsWhiteSpace(character) && character != 0xa0)
                {
                    Vector2 size = font.MeasureString(newLine, spacing, lineHeight) * scale;

                    if (size.X > maxWidth)
                    {
                        for (int rev = newLine.Length - 1; rev >= 0; --rev)
                        {
                            if (Char.IsWhiteSpace(newLine[rev]) && newLine[rev] != 0xa0)
                            {
                                newLine.Remove(rev, newLine.Length - rev);
                                break;
                            }
                        }

                        if (newLine.Length == 0)
                        {
                            lines.Add(line);
                            return;
                        }

                        line = line.Substring(newLine.Length);
                        idx = 0;

                        if (newLine.Length > 0 && Char.IsWhiteSpace(newLine[0]) && newLine[0] != 0xa0)
                        {
                            newLine.Remove(0, 1);
                        }

                        while (newLine.Length > 0 && Char.IsWhiteSpace(newLine[newLine.Length - 1]))
                        {
                            newLine.Remove(newLine.Length-1, 1);
                        }

                        lines.Add(newLine.ToString());
                        newLine.Clear();
                        continue;
                    }
                }

                newLine.Append(character);
            }

            while (newLine.Length > 0 && Char.IsWhiteSpace(newLine[newLine.Length - 1]))
            {
                newLine.Remove(newLine.Length - 1, 1);
            }

            if (newLine.Length > 0)
            {
                if (Char.IsWhiteSpace(newLine[0]) && newLine[0] != 0xa0)
                {
                    newLine.Remove(0, 1);
                }

                lines.Add(newLine.ToString());
            }

            if (lines.Count > 0)
            {
                lines[lines.Count - 1] = lines.Last() + "\r";
            }
        }

        void GetFont(out UniversalFont font, out float scale)
        {
            if (_fontFace == null)
            {
                _fontFace = FontManager.Instance.FindFont(FontName);
            }

            font = _fontFace.Find(FontSize, out scale);
        }

        public override Point ComputeSize(int width, int height)
        {
            Point size = base.ComputeSize(width, height);

            if (PositionParameters.Height.IsAuto)
            {
                Vector2 sizeInPixels = CalculateSizeInPixels();
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

            if (_lines.Count == 0)
            {
                SetText(_text);
            }

            float scale;
            UniversalFont font = _fontFace.Find(FontSize, out scale);

            Vector2 size = Vector2.Zero;
            int height = (int)(font.Height * scale * LineHeight / 100);

            for (int idx = 0; idx < _lines.Count; ++idx)
            {
                string line = _lines[idx].Line;
                float spacing = _lines[idx].Spacing;

                Vector2 lineSize = font.MeasureString(line, spacing, 0) * scale;

                size.X = Math.Max(size.X, lineSize.X);
                size.Y += height;
            }

            return size;
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiText));

            FontName = file["Font"] as string;
            FontSize = DefinitionResolver.Get<int>(Controller, Binding, file["FontSize"], 0);
            FontSpacing = DefinitionResolver.Get<int>(Controller, Binding, file["FontSpacing"], 0);
            LineHeight = DefinitionResolver.Get<int>(Controller, Binding, file["LineHeight"], 100);
            Justify = DefinitionResolver.Get<bool>(Controller, Binding, file["Justify"], false);

            _indent = DefinitionResolver.Get<Length>(Controller, Binding, file["Indent"], Length.Zero);

            Text = DefinitionResolver.GetString(Controller, Binding, file["Text"]);
            TextColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["TextColor"]) ?? new ColorWrapper(Color.White);

            HorizontalAlignment horzAlign = DefinitionResolver.Get<HorizontalAlignment>(Controller, Binding, file["HorizontalContentAlignment"], HorizontalAlignment.Left);
            VerticalAlignment vertAlign = DefinitionResolver.Get<VerticalAlignment>(Controller, Binding, file["VerticalContentAlignment"], VerticalAlignment.Top);

            TextAlign = UiHelper.TextAlignFromAlignment(horzAlign, vertAlign);
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0 || TextColor.Value.A == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            UniversalFont font;
            float scale;

            GetFont(out font, out scale);

            Rectangle target = ScreenBounds;
            int height = (int)(font.Height * LineHeight * scale / 100);
            int startX = target.X;

            int fullHeight = height * _lines.Count;

            switch (TextAlign & Framework.TextAlign.Vert)
            {
                case Framework.TextAlign.Middle:
                    target.Y = target.Center.Y - fullHeight / 2;
                    break;

                case Framework.TextAlign.Bottom:
                    target.Y = target.Bottom - fullHeight;
                    break;
            }

            int indent = _indent.Compute(Bounds.Width);

            for (int idx = 0; idx < _lines.Count; ++idx)
            {
                string line = _lines[idx].Line;
                float spacing = _lines[idx].Spacing;

                target.X = startX;

                target.X += _lines[idx].Indent * indent;

                parameters.DrawBatch.DrawText(font, line, target, TextAlign & TextAlign.Horz, TextColor.Value * opacity, spacing, 0, scale);

                target.Y += height;
            }
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if (_fontFace == null)
            {
                SetText(_text);
                Parent.RecalculateAll();
            }

            if (_lastSize.X != Bounds.Width || _lastSize.Y != Bounds.Height)
            {
                _lastSize.X = Bounds.Width;
                _lastSize.Y = Bounds.Height;
                SetText(_text);
                Parent.RecalculateAll();
            }
        }
    }
}
