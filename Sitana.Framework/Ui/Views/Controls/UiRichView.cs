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
using Sitana.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Views.RichView;
using System.Threading.Tasks;
using System.Net;
using Sitana.Framework.Cs;
using Sitana.Framework.Misc;

namespace Sitana.Framework.Ui.Views
{
    public delegate void LinkResolvedDelegate(string result);

    public interface ILinkResolver
    {    
        void ResolveLink(string link, LinkResolvedDelegate onResolved);
    }

    public class UiRichView : UiView, ICacheClient, ILinkResolver
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Processor"] = Type.GetType(node.Attribute("Processor"));

            file["LinkResolver"] = parser.ParseDelegate("LinkResolver");

            file["EnableBaseLineCorrection"] = parser.ParseBoolean("EnableBaseLineCorrection");

            file["Text"] = parser.ParseString("Text");
            file["Font"] = parser.ValueOrNull("Font");
            file["FontSize"] = parser.ParseInt("FontSize");
            file["FontSpacing"] = parser.ParseInt("FontSpacing");

            for (int idx = 0; idx < (int)FontType.Count; ++idx)
            {
                FontType type = (FontType)idx;
                string font = string.Format("{0}.Font", type);
                string spacing = string.Format("{0}.FontSpacing", type);
                string resize = string.Format("{0}.FontResize", type);

                file[font] = parser.ValueOrNull(font);
                file[spacing] = parser.ParseInt(spacing);
                file[resize] = parser.ParseInt(resize);
            }

            for (int idx = 0; idx < (int)SizeType.Count; ++idx)
            {
                SizeType type = (SizeType)idx;
                string size = string.Format("{0}.FontSize", type);

                file[size] = parser.ParseInt(size);
            }

            file["LineHeight"] = parser.ParseInt("LineHeight");
            file["Indent"] = parser.ParseLength("Indent");
            file["ParagraphSpacing"] = parser.ParseLength("ParagraphSpacing");

            file["Justify"] = parser.ParseBoolean("Justify");

            file["TextColor"] = parser.ParseColor("TextColor");
            file["LinkColor"] = parser.ParseColor("LinkColor");
            file["ActiveLinkColor"] = parser.ParseColor("ActiveLinkColor");
            file["HorizontalRulerColor"] = parser.ParseColor("HorizontalRulerColor");

            file["HorizontalContentAlignment"] = parser.ParseEnum<HorizontalContentAlignment>("HorizontalContentAlignment");
            file["VerticalContentAlignment"] = parser.ParseEnum<VerticalContentAlignment>("VerticalContentAlignment");

            file["BulletText"] = parser.ParseString("BulletText");
            file["HorizontalRulerHeight"] = parser.ParseLength("HorizontalRulerHeight");

            file["ClickMargin"] = parser.ParseLength("ClickMargin");
            file["UrlClick"] = parser.ParseDelegate("UrlClick");
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
        ColorWrapper _colorClickableActive;
        ColorWrapper _colorRuler;

        bool _baseLineCorrection = false;

        IRichProcessor _richProcessor;

        List<RichViewLine> _lines = new List<RichViewLine>();

        List<RichViewLine> _originalRichViewLines = new List<RichViewLine>();

        Length _indentSize;
        Length _paragraphSpacing;
        Length _horizontalRulerHeight;

        string _bulletText = string.Empty;

        RichViewEntity _selected = null;

        int _firstVisibleLine = -1;
        int _lastVisibleLine = -1;

        Length _clickMargin;

        bool _shouldUpdate = true;
        int _height = 0;

        ILinkResolver _linkResolver;
        Dictionary<string, string> _resolvedLinks = new Dictionary<string, string>();

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = HtmlSpecialChars.Convert(value);

                if (_richProcessor != null)
                {
                    _richProcessor.Process(value);
                }
                _shouldUpdate = true;
            }
        }

        void ILinkResolver.ResolveLink(string link, LinkResolvedDelegate onResolved)
        {
            onResolved(link);
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiRichView));

            string defaultFont = file["Font"] as string;
            int defaultFontSize = DefinitionResolver.Get<int>(Controller, Binding, file["FontSize"], 0);
            int defaultFontSpacing = DefinitionResolver.Get<int>(Controller, Binding, file["FontSpacing"], int.MaxValue);

            for (int idx = 0; idx < (int)FontType.Count; ++idx)
            {
                FontType type = (FontType)idx;
                string font = string.Format("{0}.Font", type);
                string spacing = string.Format("{0}.FontSpacing", type);
                string resize = string.Format("{0}.FontResize", type);

                string fontName = file[font] as string;
                int fontSpacing = DefinitionResolver.Get<int>(Controller, Binding, file[spacing], defaultFontSpacing == int.MaxValue ? 0 : defaultFontSpacing);
                int fontResize = DefinitionResolver.Get<int>(Controller, Binding, file[resize], 0);

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
                    FontSpacing = (float)fontSpacing / 1000.0f,
                    FontResize = fontResize
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

            _bulletText = DefinitionResolver.GetString(Controller, Binding, file["BulletText"]) ?? "* ";
            //_bulletText = _bulletText.Replace(" ", ((char)0xa0).ToString());

            _horizontalRulerHeight = DefinitionResolver.Get<Length>(Controller, Binding, file["HorizontalRulerHeight"], new Length(0, 0, 1));
            _indentSize = DefinitionResolver.Get<Length>(Controller, Binding, file["Indent"], Length.Zero);
            _paragraphSpacing = DefinitionResolver.Get<Length>(Controller, Binding, file["ParagraphSpacing"], Length.Zero);

            _lineHeight = (float)DefinitionResolver.Get<int>(Controller, Binding, file["LineHeight"], 100) / 100.0f;
            _justify = DefinitionResolver.Get<bool>(Controller, Binding, file["Justify"], false);

            _linkResolver = DefinitionResolver.Get<ILinkResolver>(Controller, Binding, file["LinkResolver"], this);

            _baseLineCorrection = DefinitionResolver.Get<bool>(Controller, Binding, file["EnableBaseLineCorrection"], false);

            Type processorType = file["Processor"] as Type;

            if (processorType != null)
            {
                _richProcessor = Activator.CreateInstance(processorType) as IRichProcessor;
            }

            Text = DefinitionResolver.GetString(Controller, Binding, file["Text"]);

            _colorNormal = DefinitionResolver.GetColorWrapper(Controller, Binding, file["TextColor"]) ?? UiLabel.DefaultTextColor;
            _colorClickable = DefinitionResolver.GetColorWrapper(Controller, Binding, file["LinkColor"]) ?? UiLabel.DefaultTextColor;
            _colorClickableActive = DefinitionResolver.GetColorWrapper(Controller, Binding, file["ActiveLinkColor"]) ?? _colorClickable;
            _colorRuler = DefinitionResolver.GetColorWrapper(Controller, Binding, file["HorizontalRulerColor"]) ?? UiLabel.DefaultTextColor;

            HorizontalContentAlignment horzAlign = DefinitionResolver.Get<HorizontalContentAlignment>(Controller, Binding, file["HorizontalContentAlignment"], HorizontalContentAlignment.Left);
            VerticalContentAlignment vertAlign = DefinitionResolver.Get<VerticalContentAlignment>(Controller, Binding, file["VerticalContentAlignment"], VerticalContentAlignment.Top);

            _textAlign = UiHelper.TextAlignFromContentAlignment(horzAlign, vertAlign);

            _clickMargin = DefinitionResolver.Get<Length>(Controller, Binding, file["ClickMargin"], Length.Zero);

            RegisterDelegate("UrlClick", file["UrlClick"]);

            EnabledGestures = (GestureType.Down | GestureType.Up | GestureType.Move | GestureType.Tap);

            return true;
        }

        protected override void OnGesture(Gesture gesture)
        {
            switch (gesture.GestureType)
            {
                case GestureType.CapturedByOther:
                    _selected = null;
                    break;

                case GestureType.Down:
                    _selected = EntityFromPoint(gesture.Position.ToPoint());
                    break;

                case GestureType.Move:
                    {
                        var entity = EntityFromPoint(gesture.Position.ToPoint());

                        if (_selected != entity)
                        {
                            _selected = null;
                        }
                    }
                    break;

                case GestureType.Up:
                    ProcessUrl(_selected);
                    _selected = null;
                    break;

                case GestureType.Tap:
                    ProcessUrl(_selected);
                    _selected = null;
                    break;
            }
        }

        private RichViewEntity EntityFromPoint(Point point)
        {
            int margin = _clickMargin.Compute(Bounds.Width);

            Rectangle entityRect = ScreenBounds;
            int startX = entityRect.X;

            for (int idx = 0; idx < _firstVisibleLine; ++idx)
            {
                entityRect.Y += _lines[idx].Height;
            }

            if (_firstVisibleLine < 0)
            {
                return null;
            }

            _lastVisibleLine = Math.Min(_lines.Count - 1, _lastVisibleLine);

            for (int idx = _firstVisibleLine; idx <= _lastVisibleLine; ++idx)
            {
                RichViewLine line = _lines[idx];

                entityRect.Y -= margin;
                entityRect.Height = line.Height + margin * 2;

                int lineOffset = OffsetFromWidthAndAlign(line.Width);

                for (int entIdx = 0; entIdx < line.Entities.Count; ++entIdx)
                {
                    RichViewEntity entity = line.Entities[entIdx];

                    if (!string.IsNullOrWhiteSpace(entity.Url))
                    {
                        entityRect.X = entity.Offset + startX - margin + lineOffset;
                        entityRect.Width = entity.Width + margin * 2;

                        if (entityRect.Contains(point))
                        {
                            return entity;
                        }
                    }
                }

                entityRect.Y += line.Height + margin;
            }

            return null;
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if (IsViewDisplayed || _shouldUpdate)
            {
                if (_lastSize.X != Bounds.Width || _shouldUpdate)
                {
                    _lastSize.X = Bounds.Width;
                    _lastSize.Y = Bounds.Height;
                    Process();
                    Parent.RecalculateAll();
                    _shouldUpdate = false;
                }
            }
        }

        public override Point ComputeSize(int width, int height)
        {
            Point size = base.ComputeSize(width, height);

            if (PositionParameters.Height.IsAuto)
            {
                size.Y = 0;

                foreach (var line in _lines)
                {
                    size.Y += line.Height;
                }
            }

            return size;
        }

        void Process()
        {
            _lines.Clear();

            if (Bounds.Width < 2)
            {
                return;
            }

            GenerateRichViewLines();
            ProcessLines();
        }

        void ProcessLines(bool internalProcess = false)
        {
            int width = Bounds.Width;
            int maxWidth = width;

            List<string> tempLines = new List<string>();
            List<RichViewEntity> restOfLine = new List<RichViewEntity>();
            int indent = 0;

            for (int idx = 0; idx < _lines.Count; ++idx)
            {
                RichViewLine line = _lines[idx];

                char _lastChar = '\0';
                Font _lastFont = null; 
                float position = line.Entities.Count > 0 ? line.Entities[0].Offset : 0;

                if (line.NewParagraph)
                {
                    indent = 0;
                }

                for (int ent = 0; ent < line.Entities.Count; )
                {
                    RichViewEntity entity = line.Entities[ent];

                    bool process = true;
                    int lineHeight = 0;

                    switch (entity.Type)
                    {
                        case EntityType.HorizontalLine:
                            _lastChar = '\0';
                            line.Height = _horizontalRulerHeight.Compute(0);
                            break;

                        case EntityType.ListIndent:
                        case EntityType.Indent:

                            _lastChar = '\0';
                            position += _indentSize.Compute(width);
                            process = false;

                            line.Entities.RemoveAt(ent);

                            if (ent == 0)
                            {
                                indent = (int)position;
                            }
                            break;

                        case EntityType.Image:

                            _lastChar = '\0';
                            entity.Offset = (int)position;

                            int imageWidth = Math.Min((int)(entity.Image.Width * UiUnit.Unit), maxWidth-indent);
                            
                            if (position + imageWidth > maxWidth)
                            {
                                entity.Offset = indent;
                                RichViewLine newLine = new RichViewLine();

                                for (int ent2 = ent; ent2 < line.Entities.Count; ++ent2)
                                {
                                    newLine.Entities.Add(line.Entities[ent2]);
                                }

                                line.Entities.RemoveRange(ent, line.Entities.Count - ent);
                                _lines.Insert(idx + 1, newLine);
                            }
                            else
                            {
                                float newHeight = imageWidth * entity.Image.Height / entity.Image.Width;
                                
                                lineHeight = Math.Max(lineHeight, (int)newHeight);
                                position += imageWidth;

                                line.Height = lineHeight;
                                line.Width = (int)position;
                            }

                            break;

                        case EntityType.String:
                            {
                                entity.Offset = (int)position;

                                Vector2 kerning = Vector2.Zero;

                                //if (_lastChar != '\0' && entity.Font.SitanaFont != null)
                                //{
                                //    Glyph glyph = entity.Font.SitanaFont.Find(entity.Text[0]);

                                //    if (glyph != null)
                                //    {
                                //        kerning.X = (float)glyph.Kerning(_lastChar) / 10f * entity.FontScale;
                                //    }
                                //}

                                Point size = (entity.Font.MeasureString(entity.Text, entity.FontSpacing, 0) * entity.FontScale + kerning).ToPoint();

                                entity.Offset += (int)kerning.X;

                                size.Y = (int)(entity.Font.Height * entity.FontScale * _lineHeight);

                                lineHeight = Math.Max(lineHeight, size.Y);

                                line.Height = lineHeight;

                                if (position + size.X > maxWidth)
                                {
                                    lineHeight = 0;
                                    restOfLine.Clear();

                                    for (int ent2 = ent + 1; ent2 < line.Entities.Count; ++ent2)
                                    {
                                        restOfLine.Add(line.Entities[ent2]);
                                    }

                                    if (line.Entities.Count - ent - 1 > 0)
                                    {
                                        line.Entities.RemoveRange(ent + 1, line.Entities.Count - ent - 1);
                                    }

                                    SplitLine(tempLines, entity, position, indent, maxWidth);

                                    if (tempLines.Count > 1)
                                    {
                                        RichViewEntity newEntity = entity.Clone();
                                        newEntity.Text = tempLines[0];

                                        line.Entities[ent] = newEntity;

                                        position += ComputeWidth(newEntity);
                                        line.Width = (int)position;

                                        for (int strIdx = 1; strIdx < tempLines.Count; ++strIdx)
                                        {
                                            ++idx;
                                            RichViewLine newLine = new RichViewLine();
                                            _lines.Insert(idx, newLine);

                                            newLine.Height = size.Y;

                                            newEntity = entity.Clone();
                                            newEntity.Offset = indent;
                                            newEntity.Text = tempLines[strIdx];

                                            newLine.Entities.Add(newEntity);
                                            newLine.Width = indent + ComputeWidth(newEntity);

                                            if (strIdx == tempLines.Count - 1)
                                            {
                                                foreach (var en in restOfLine)
                                                {
                                                    newLine.Entities.Add(en);
                                                }
                                            }
                                        }
                                        --idx;
                                    }
                                    else
                                    {
                                        position += ComputeWidth(entity);
                                        line.Width = (int)position;

                                        RichViewLine newLine = new RichViewLine();
                                        _lines.Insert(idx + 1, newLine);

                                        foreach (var en in restOfLine)
                                        {
                                            newLine.Entities.Add(en);
                                        }
                                    }
                                }
                                else
                                {
                                    _lastChar = entity.Text.Length > 0 ? entity.Text.Last() : _lastChar;
                                    position += size.X;
                                    line.Width = (int)position;
                                }
                            }
                            break;
                    }

                    if (process)
                    {
                        ++ent;
                    }
                }
            }

            int space = _paragraphSpacing.Compute(0);

            for (int idx = 0; idx < _lines.Count; ++idx)
            {
                RichViewLine line = _lines[idx];
                bool last = idx == _lines.Count - 1 ? true : _lines[idx + 1].NewParagraph;

                if (last)
                {
                    line.Height += space;
                }

                int lineBase = 0;

                foreach (var entity in line.Entities)
                {
                    if (entity.Font != null)
                    {
                        int baseLine = (int)((entity.Font.BaseLine - 1) * entity.FontScale);

                        lineBase = Math.Max(lineBase, baseLine);
                        line.BaseLine = lineBase;
                    }
                }
            }

            if (_justify && !internalProcess)
            {
                for (int idx = 0; idx < _lines.Count; ++idx)
                {
                    RichViewLine line = _lines[idx];

                    bool last = idx == _lines.Count - 1 ? true : _lines[idx + 1].NewParagraph;

                    if (!last)
                    {
                        int wholeWidth = width;
                        int tries = 0;

                        do
                        {
                            int currentWidth = 0;

                            foreach (var entity in line.Entities)
                            {
                                if (entity.Type == EntityType.String)
                                {
                                    if (wholeWidth < width)
                                    {
                                        entity.FontSpacing += 0.001f;
                                    }
                                    else if (wholeWidth > width)
                                    {
                                        entity.FontSpacing -= 0.001f;
                                    }
                                }

                                currentWidth += ComputeWidth(entity);
                            }
                            
                            wholeWidth = currentWidth + (line.Entities.Count > 0 ? line.Entities[0].Offset : 0);
                            tries++;
                        }
                        while (Math.Abs(wholeWidth - width) > 5 && tries < 50);
                    }
                }
            }

            _height = 0;

            foreach (var line in _lines)
            {
                int lineWidth = 0;
                foreach (var entity in line.Entities)
                {
                    entity.Width = ComputeWidth(entity);
                    lineWidth += entity.Width;
                }

                line.Width = lineWidth;
                _height += line.Height;
            }
        }

        int ComputeWidth(RichViewEntity entity)
        {
            switch (entity.Type)
            {
                case EntityType.String:
                    return (int)(entity.Font.MeasureString(entity.Text, entity.FontSpacing, 0).X * entity.FontScale);

                case EntityType.Image:
                    return (int)(entity.Image.Width * UiUnit.Unit);
            }
            return 0;
        }

        void SplitLine(List<string> lines, RichViewEntity entity, float position, int indent, int width)
        {
            float spacing = entity.FontSpacing;

            lines.Clear();

            int maxWidth = (int)(width - position);
            float scale = entity.FontScale;

            UniversalFont font = entity.Font;

            StringBuilder newLine = new StringBuilder();

            string line = entity.Text;

            for (int idx = 0; idx <= line.Length; ++idx)
            {
                char character = idx == line.Length ? ' ' : line[idx];

                if (Char.IsWhiteSpace(character) && character != 0xa0)
                {
                    Vector2 size = font.MeasureString(newLine, spacing, 0) * scale;

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
                            maxWidth = width - indent;
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
                            newLine.Remove(newLine.Length - 1, 1);
                        }

                        lines.Add(newLine.ToString());
                        maxWidth = width - indent;
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


            lines[lines.Count - 1] = lines.Last() + " ";

        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            RemoteImageCache.Instance.RemoveClient(this);
        }

        void GenerateRichViewLines()
        {
            if (_richProcessor == null)
            {
                return;
            }

            foreach (var line in _richProcessor.Lines)
            {
                RichViewLine richLine = new RichViewLine() { NewParagraph = true };
                _lines.Add(richLine);

                foreach (var entity in line.Entities)
                {
                    RichViewEntity richEntity = GenerateEntity(entity);

                    if (entity.Type == EntityType.ListBullet || entity.Type == EntityType.ListNumber)
                    {
                        richLine.Entities.Add(new RichViewEntity()
                            {
                                Type = EntityType.ListIndent,
                                Url = richEntity.Url
                            });
                    }
                    richLine.Entities.Add(richEntity);
                }
            }
        }

        RichViewEntity GenerateEntity(Entity entity)
        {
            RichViewEntity richEntity = new RichViewEntity();

            richEntity.Url = entity.Url;
            richEntity.Type = entity.Type;

            switch (entity.Type)
            {
                case EntityType.String:
                    {
                        FillEntityData(richEntity, entity);
                        richEntity.Text = entity.Data as string;
                    }
                    break;

                case EntityType.Image:
                    {
                        richEntity.Text = entity.Data as string;
                        richEntity.Image = AdvancedDrawBatch.OnePixelWhiteTexture;

                        LinkResolvedDelegate onLinkResolved = (string result) =>
                            {
                                Uri uriResult;
                                bool isUrl = Uri.TryCreate(result, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                                if (isUrl)
                                {
                                    richEntity.Image = RemoteImageCache.Instance.RegisterClient(uriResult, this);
                                }
                                else
                                {
                                    richEntity.Image = ContentLoader.Current.Load<Texture2D>(richEntity.Text);
                                    _shouldUpdate = true;
                                }
                            };

                        _linkResolver.ResolveLink(richEntity.Text, onLinkResolved);
                    }
                    break;

                case EntityType.ListNumber:
                    {
                        richEntity.Type = EntityType.String;
                        FillEntityData(richEntity, entity);
                        richEntity.Text = string.Format("{0}. ", entity.Data);
                    }
                    break;

                case EntityType.ListBullet:
                    {
                        richEntity.Type = EntityType.String;
                        FillEntityData(richEntity, entity);
                        richEntity.Text = _bulletText;
                    }
                    break;
            }

            return richEntity;
        }

        void ICacheClient.ImageUpdated()
        {
            _shouldUpdate = true;
        }

        void FillEntityData(RichViewEntity richEntity, Entity entity)
        {
            FontFace fontFace = _fonts[(int)entity.Font].Font;
            float spacing = _fonts[(int)entity.Font].FontSpacing;
            int size = _sizes[(int)entity.Size] + _fonts[(int)entity.Font].FontResize;
            float scale;

            UniversalFont font = fontFace.Find(size, out scale);

            richEntity.Font = font;
            richEntity.FontScale = scale;
            richEntity.FontSpacing = spacing;
        }

        private int OffsetFromWidthAndAlign(int width)
        {
            switch (_textAlign & TextAlign.Horz)
            {
                case TextAlign.Right:
                    return Bounds.Width - width;

                case TextAlign.Center:
                    return (Bounds.Width - width) / 2;
            }

            return 0;
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            Rectangle target = ScreenBounds;

            if (_height < target.Height)
            {
                switch (_textAlign & TextAlign.Vert)
                {
                    case TextAlign.Middle:
                        target.Y = target.Center.Y - _height / 2;
                        break;

                    case TextAlign.Bottom:
                        target.Y = target.Bottom - _height;
                        break;
                }
            }

            int startX = target.X;

            int endY = target.Bottom;

            int top = parameters.DrawBatch.ClipRect.Top;
            int bottom = parameters.DrawBatch.ClipRect.Bottom;

            _firstVisibleLine = -1;

            for (int idx = 0; idx < _lines.Count; ++idx)
            {
                RichViewLine line = _lines[idx];

                if (target.Y > bottom)
                {
                    break;
                }

                target.Height = line.Height;

                if (target.Y + line.Height >= top)
                {
                    if (_firstVisibleLine < 0)
                    {
                        _firstVisibleLine = idx;
                    }

                    _lastVisibleLine = idx;

                    int lineOffset = OffsetFromWidthAndAlign(line.Width);

                    for (int ent = 0; ent < line.Entities.Count; ++ent)
                    {
                        RichViewEntity entity = line.Entities[ent];

                        float spacing = entity.FontSpacing;
                        float scale = entity.FontScale;
                        UniversalFont font = entity.Font;

                        int offset = 0;

                        if (font != null)
                        {
                            offset = line.BaseLine - (int)((font.BaseLine - 1) * scale);
                        }

                        target.X = startX + entity.Offset + lineOffset;

                        switch (entity.Type)
                        {
                            case EntityType.HorizontalLine:
                                {
                                    Rectangle rect = new Rectangle(target.X, target.Y, Bounds.Width, _horizontalRulerHeight.Compute(0));
                                    parameters.DrawBatch.DrawRectangle(rect, _colorRuler.Value * opacity);
                                }
                                break;

                            case EntityType.String:
                                {
                                    Color textColor = _colorNormal.Value;

                                    if (entity.Url != null)
                                    {
                                        textColor = _selected == entity ? _colorClickableActive.Value : _colorClickable.Value;
                                    }

                                    target.Y += offset;
                                    parameters.DrawBatch.DrawText(font, entity.Text, target, TextAlign.Left, textColor * opacity, spacing, 0, scale);
                                    target.Y -= offset;
                                }
                                break;

                            case EntityType.Image:
                                {
                                    Point size = new Point((int)(entity.Image.Width * UiUnit.Unit), (int)(entity.Image.Height * UiUnit.Unit));

                                    int maxWidth = Bounds.Width - entity.Offset;

                                    if (size.X > maxWidth)
                                    {
                                        size.Y = maxWidth * size.Y / size.X;
                                        size.X = maxWidth;
                                    }

                                    parameters.DrawBatch.DrawImage(entity.Image, target.Location, size, Point.Zero, (float)size.X / (float)entity.Image.Width, Color.White * opacity);
                                }
                                break;
                        }
                    }
                }

                target.Y += line.Height;
            }
        }

        void ProcessUrl(RichViewEntity entity)
        {
            if (entity != null && !String.IsNullOrWhiteSpace(entity.Url))
            {
                object processed = CallDelegate("UrlClick", new InvokeParam("sender", this), new InvokeParam("url", entity.Url));

                if (processed == null || !processed.Equals(true))
                {
                    Platform.OpenWebsite(entity.Url);
                }
            }
        }
    }
}
