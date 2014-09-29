// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework;
using Sitana.Framework.Cs;
using System.Globalization;
using System.Threading;
using System;

namespace Sitana.Framework.Content
{
    /// <summary>
    /// Helper class displaying text using given font.
    /// </summary>
    public class BitmapFontPresenter: IFontPresenter
    {
        #region IFontPresenter

        List<IFontPresenter> IFontPresenter.PrepareMultilineText(StringBuilder text, Int32 width, Boolean justify, Int32 indent)
        {
            return PrepareMultilineText(text, width, justify, indent);
        }

        void IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold)
        {
            DrawText(spriteBatch, position, color, origin, scale, bold);
        }

        void IFontPresenter.DrawText(SpriteBatch spriteBatch, String text, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold)
        {
            DrawText(spriteBatch, text, position, color, origin, scale, bold);
        }

        void IFontPresenter.DrawText(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color, Single scale, Align align, Boolean bold)
        {
            DrawText(spriteBatch, text, position, color, scale, align, bold);
        }

        void IFontPresenter.DrawText(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color, Vector2 scale, Align align, Boolean bold)
        { 
            DrawText(spriteBatch, text, position, color, scale, align, bold);
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Single scale, Align align, Boolean bold)
        {
            return DrawText(spriteBatch, position, color, scale, align, bold);
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Single scale, Align align, Boolean bold)
        {
            return DrawText(spriteBatch, position, colors, scale, align, bold);
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale, Align align, Boolean bold)
        {
            return DrawText(spriteBatch, position, color, scale, align, bold);
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Vector2 scale, Align align, Boolean bold)
        {
            return DrawText(spriteBatch, position, colors, scale, align, bold);
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, String text, Vector2 position, Color color, Single scale, Align align)
        {
            return DrawText(spriteBatch, text, position, color, scale, align);
        }

        Single IFontPresenter.LineHeight
        {
            get
            {
                return BitmapFont.LineHeight;
            }
        }

        Single IFontPresenter.Height
        {
            get
            {
                return BitmapFont.Height;
            }
        }

        Vector2 IFontPresenter.TextSize(String text)
        {
            return TextSize(text);
        }

        Vector2 IFontPresenter.Size
        {
            get
            {
                return Size;
            }
        }

        IFontPresenter IFontPresenter.Clone()
        {
            return Clone();
        }

        void IFontPresenter.PrepareRender(String text)
        {
            PrepareRenderInt(text, true);
        }

        void IFontPresenter.PrepareRender(String text, Boolean trimStartEnd)
        {
            PrepareRenderInt(text, trimStartEnd);
        }

        void IFontPresenter.PrepareRender(StringBuilder text, Boolean trimStartEnd)
        {
            PrepareRenderInt(text, trimStartEnd);
        }

        void IFontPresenter.PrepareRender(StringBuilder text, Int32 start, Int32 length, Int32 lineWidth)
        {
            PrepareRenderInt(text, start, length, lineWidth, true);
        }

        void IFontPresenter.PrepareRender(StringBuilder text, Int32 lineWidth)
        {
            PrepareRenderInt(text, lineWidth);
        }

        void IFontPresenter.PrepareRender(StringBuilder text)
        {
            PrepareRenderInt(text, true);
        }

        #endregion

        public struct CharacterInfo
        {
            public Rectangle Area;
            public Int32 Spacing;
            public Boolean IsSpace;
        }

        // Font object.
        public BitmapFont BitmapFont { get; private set; }

        // Prepared list of characters to render
        private List<CharacterInfo> _renderCharacters = new List<CharacterInfo>();

        private Int32 _biggestWidth = 0;

        private Boolean _fixedWidth = false;
        private Tuple<Single, Single> _underline = null;

        private Texture2D _onePixelWhite = null;
        private Vector2 _size = Vector2.Zero;

        private Single _spacing = 0;
        private Single _startOffset = 0;

        private Color[] _colors = new Color[1];

        internal BitmapFontPresenter()
        {
        }

        internal void SetFont(BitmapFont font)
        {
            Init(font, false, 0);
        }

        public BitmapFontPresenter Clone()
        {
            BitmapFontPresenter newPresenter = new BitmapFontPresenter(BitmapFont, null);
            newPresenter._spacing = _spacing;
            newPresenter._fixedWidth = _fixedWidth;
            newPresenter._biggestWidth = _biggestWidth;
            newPresenter._underline = _underline;

            return newPresenter;
        }

        public static BitmapFontPresenter New(String pathString)
        {
            String[] names = pathString.Split(';');
            BitmapFont font = ContentLoader.Current.Load<BitmapFont>(names[0]);

            return new BitmapFontPresenter(font, names);
        }

        public BitmapFontPresenter(BitmapFont bitmapFont, String[] additionalParams)
        {
            Single spacing = 0;
            Boolean monospace = false;

            if (additionalParams != null)
            {
                for (Int32 idx = 0; idx < additionalParams.Length; ++idx)
                {
                    switch (additionalParams[idx].ToLowerInvariant())
                    {
                        case "mono":
                            monospace = true;
                            break;

                        default:

                            if (additionalParams[idx].ToLowerInvariant().StartsWith("u"))
                            {
                                String[] underlineParams = additionalParams[idx].Split(':');

                                Single thickness;
                                Single offset;

                                if (Single.TryParse(underlineParams[0].Substring(1), NumberStyles.Any, CultureInfo.InvariantCulture, out thickness))
                                {
                                    if (Single.TryParse(underlineParams[1], NumberStyles.Any, CultureInfo.InvariantCulture, out offset))
                                    {
                                        _underline = new Tuple<Single, Single>(thickness, offset);
                                    }
                                }
                            }
                            else
                            {
                                Single.TryParse(additionalParams[idx], NumberStyles.Any, CultureInfo.InvariantCulture, out spacing);
                            }
                            break;
                    }
                }
            }

            Init(bitmapFont, monospace, spacing);
        }

        /// <summary>
        /// Initializes new font presenter.
        /// </summary>
        /// <param name="bitmapFont">BitmapFont object.</param>
        public BitmapFontPresenter(BitmapFont bitmapFont, Boolean monospace, Single spacing)
        {
            Init(bitmapFont, monospace, spacing);
        }

        private void Init(BitmapFont bitmapFont, Boolean monospace, Single spacing)
        {
            BitmapFont = bitmapFont;

            _fixedWidth = monospace;

            _spacing = spacing * BitmapFont.Height;

            if ( _underline != null )
            {
                _onePixelWhite = ContentLoader.Current.OnePixelWhiteTexture;
            }

            foreach (var info in bitmapFont.CharactersMap)
            {
                _biggestWidth = Math.Max(_biggestWidth, (Int32)(info.Value.Spacing + _spacing));
            }
        }
        
        /// <summary>
        /// Prepares list of characters to render.
        /// </summary>
        /// <param name="text">Text to display.</param>
        private void PrepareRenderInt(String text, Boolean trimStartEnd = true)
        {
            // Clear list.
            _renderCharacters.Clear();

            if (!String.IsNullOrEmpty(text))
            {
                // Default start offset is 0.
                _startOffset = 0;

                BitmapFont.CharacterInfo info = null;

                // Search for character info in map and fint start offset kernin for pair: '\0' and first character.
                if (trimStartEnd && BitmapFont.CharactersMap.TryGetValue('\0', out info))
                {
                   _startOffset = info.CalculateSpacing(text.ElementAt(0));
                }

                // Iterate thru text characters
                for (Int32 idx = 0; idx < text.Length; ++idx)
                {
                    // Get character id.
                    Char character = text.ElementAt(idx);

                    // Search for character info in map and add it to the list.
                    if (BitmapFont.CharactersMap.TryGetValue(character, out info))
                    {
                        Char nextChar = idx == text.Length-1 ? (trimStartEnd ? '\0' : '\n') : text[idx+1];
                        Int32 spacing = info.Spacing + info.CalculateSpacing(nextChar);

                        _renderCharacters.Add(
                            new CharacterInfo()
                            {
                                Area = info.Area,
                                Spacing = spacing,
                                IsSpace = character == ' '
                            });
                    }
                }
            }

            _size = CalculateSize();
        }

        private void PrepareRenderInt(StringBuilder text, Int32 start, Int32 length, Int32 lineWidth, Boolean trimStartEnd = true)
        {
            PrepareRenderInt(text, start, length, lineWidth, trimStartEnd, true);
        }

        /// <summary>
        /// Prepares list of characters to render.
        /// </summary>
        /// <param name="text">Text to display.</param>
        private void PrepareRenderInt(StringBuilder text, Int32 start, Int32 length, Int32 lineWidth, Boolean trimStartEnd, Boolean tryAgain)
        {
            try
            {
                // Clear list.
                _renderCharacters.Clear();
                Int32 spaces = 0;

                if (text != null && length > 0)
                {
                    // Default start offset is 0.
                    _startOffset = 0;

                    BitmapFont.CharacterInfo info = null;

                    // Search for character info in map and fint start offset kernin for pair: '\0' and first character.
                    if (trimStartEnd && BitmapFont.CharactersMap.TryGetValue('\0', out info))
                    {
                        _startOffset = info.CalculateSpacing(text[start]);
                    }

                    for (Int32 idx = 0; idx < length; ++idx)
                    {
                        // Get character id.
                        Char character = text[idx + start];

                        if (character == ' ')
                        {
                            spaces++;
                        }

                        // Search for character info in map and add it to the list.
                        if (BitmapFont.CharactersMap.TryGetValue(character, out info))
                        {
                            Char nextChar = idx == text.Length - 1 ? (trimStartEnd ? '\0' : '\n') : text[idx + 1];

                            Int32 spacing = info.Spacing + info.CalculateSpacing(nextChar);

                            _renderCharacters.Add(
                                new CharacterInfo()
                                {
                                    Area = info.Area,
                                    Spacing = spacing,
                                    IsSpace = character == ' '
                                });
                        }
                    }
                }

                _size = CalculateSize();

                spaces = Math.Max(1, spaces);

                if (_size.X < lineWidth)
                {
                    Int32 diff = lineWidth - (Int32)_size.X;

                    Single spacingAdd = (Single)diff / _renderCharacters.Count;

                    Int32 spacingNormal = (Int32)spacingAdd;

                    diff = diff - spacingNormal * _renderCharacters.Count;

                    Int32 whiteAdd = (Int32)(diff / spaces);

                    for (Int32 idx = 0; idx < _renderCharacters.Count; ++idx)
                    {
                        var character = _renderCharacters[idx];
                        character.Spacing += spacingNormal;

                        if (character.IsSpace)
                        {
                            character.Spacing += whiteAdd;
                        }

                        _renderCharacters[idx] = character;
                    }
                }
            }
            catch(IndexOutOfRangeException)
            {
                if (tryAgain)
                {
                    Thread.Sleep(1);
                    PrepareRenderInt(text, start, length, lineWidth, trimStartEnd, false);
                }
                else
                {
                    throw new PrepareRenderException("StringBuilder data has changed during preparing characters.");
                }
            }
        }

        private void PrepareRenderInt(StringBuilder text, Int32 lineWidth)
        {
            // Clear list.
            _renderCharacters.Clear();

            if (text != null)
            {
                PrepareRenderInt(text, 0, text.Length, lineWidth, true);
            }
        }

        private void PrepareRenderInt(StringBuilder text, Boolean trimStartEnd = true)
        {
            // Clear list.
            _renderCharacters.Clear();

            if (text != null)
            {
                PrepareRenderInt(text, 0, text.Length, 0, trimStartEnd);
            }
        }

        public Vector2 Size
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Returns size of prepared text.
        /// </summary>
        /// <returns></returns>
        private Vector2 CalculateSize()
        {
            Single width = _startOffset;
            Int32 height = 0;

            // Iterate thru prepared list
            for (Int32 idx = 0; idx < _renderCharacters.Count; ++idx)
            {
                var info = _renderCharacters[idx];

                // Add character spacing to width and take maximum of characters height.
                width += (_fixedWidth ? _biggestWidth : info.Spacing) + _spacing;
                height = Math.Max(height, info.Area.Height);
            }

            if (_renderCharacters.Count > 0)
            {
                width -= _spacing;
            }

            return new Vector2(width, BitmapFont.LineHeight);
        }

        /// <summary>
        /// Resturns size of given text.
        /// </summary>
        /// <param name="text">Text to calculate size of.</param>
        /// <returns></returns>
        public Vector2 TextSize(String text)
        {
            PrepareRenderInt(text);
            return Size;
        }

        public Vector2 TextSize(StringBuilder text)
        {
            PrepareRenderInt(text);
            return Size;
        }

        private void DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Vector2 origin, Vector2 scale, Boolean bold = false)
        {
            Vector2 currentPosition = position - origin * scale;
            Single spacing = _spacing;
            
            if (_underline != null)
            {
                Vector2 underlinePos = currentPosition + new Vector2(-_startOffset*scale.X, (origin.Y + _size.Y * (1+_underline.Item2)) * scale.Y );
                Vector2 lineScale = scale * new Vector2(_size.X, _underline.Item1);

                spriteBatch.Draw(_onePixelWhite, underlinePos, null, _colors[0], 0, new Vector2(0, 0), lineScale, SpriteEffects.None, 0);
            }

            Int32 numPasses = bold ? 2 : 1;

            for (Int32 idx2 = 0; idx2 < numPasses; ++idx2)
            {
                currentPosition = position - origin * scale + new Vector2((Single)idx2 * scale.X,0);

                // Iterate thru prepared list
                for (Int32 idx = 0; idx < _renderCharacters.Count; ++idx)
                {
                    var info = _renderCharacters[idx];

                    Vector2 offset = Vector2.Zero;

                    if (_fixedWidth)
                    {
                        offset.X += (_biggestWidth - info.Spacing) / 2;
                    }

                    spriteBatch.Draw(BitmapFont.FontTexture, currentPosition + offset, info.Area, colors[idx % colors.Length], 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
                    currentPosition.X += (Single)((_fixedWidth ? _biggestWidth : info.Spacing) + spacing) * scale.X;
                }
            }
        }

        public void DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold = false)
        {
            _colors[0] = color;
            DrawText(spriteBatch, position, _colors, origin, new Vector2(scale), bold);
        }

        public void DrawText(SpriteBatch spriteBatch, String text, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold = false)
        {
            PrepareRenderInt(text);
            DrawText(spriteBatch, position, color, origin, scale, bold);
        }

        public void DrawText(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color, Single scale, Align align, Boolean bold = false)
        {
            PrepareRenderInt(text);
            DrawText(spriteBatch, position, color, scale, align, bold);
        }

        public void DrawText(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color, Vector2 scale, Align align, Boolean bold = false)
        {
            PrepareRenderInt(text);
            _colors[0] = color;
            DrawText(spriteBatch, position, _colors, scale, align, bold);
        }

        public Vector2 DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Single scale, Align align, Boolean bold = false)
        {
            _colors[0] = color;
            return DrawText(spriteBatch, position, _colors, new Vector2(scale), align, bold);
        }

        public Vector2 DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Single scale, Align align, Boolean bold = false)
        {
            return DrawText(spriteBatch, position, colors, new Vector2(scale), align, bold);
        }

        public Vector2 DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale, Align align, Boolean bold = false)
        {
            _colors[0] = color;
            return DrawText(spriteBatch, position, _colors, scale, align, bold);
        }

        public Vector2 DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Vector2 scale, Align align, Boolean bold = false)
        {
            Vector2 origin = new Vector2(-_startOffset, BitmapFont.CapLine);
            Vector2 size = Size;

            Align center = align & Align.Center;
            Align right = align & Align.Right;
            Align vcenter = align & Align.Middle;
            Align bottom = align & Align.Bottom;

            if (center == Align.Center)
            {
                origin.X = size.X / 2 - _startOffset;
            }
            else if (right == Align.Right)
            {
                origin.X = size.X - _startOffset;
            }

            if (vcenter == Align.Middle)
            {
                origin.Y = (BitmapFont.BaseLine - BitmapFont.CapLine) / 2 + BitmapFont.CapLine;
            }
            else if (bottom == Align.Bottom)
            {
                origin.Y = BitmapFont.BaseLine;
            }

            DrawText(spriteBatch, position, colors, origin, scale, bold);

            return size;
        }

        public Vector2 DrawText(SpriteBatch spriteBatch, String text, Vector2 position, Color color, Single scale, Align align)
        {
            PrepareRenderInt(text);
            return DrawText(spriteBatch, position, color, scale, align);
        }

        public List<IFontPresenter> PrepareMultilineText(StringBuilder text, Int32 width, Boolean justify = true, Int32 indent = 0)
        {
            var list = new List<IFontPresenter>();

            StringBuilder line = new StringBuilder();
            Int32 wordStart = 0;
            Int32 wordStartInText = 0;

            Int32 justWidth = justify ? width : 0;

            Int32 indentLine = indent;

            for (Int32 idx = 0; idx <= text.Length;)
            {
                Char ch = idx == text.Length ? ' ' : text[idx];

                if (Char.IsWhiteSpace(ch))
                {
                    PrepareRenderInt(line);

                    if (Size.X >= width - indentLine)
                    {
                        if (wordStart == 0)
                        {
                            Single divide = Size.X / (Single)width / 2;
                            divide = Math.Max(1.25f, divide);

                            Int32 divideArea = (Int32)(line.Length / divide);
                            line.Remove(divideArea, line.Length - divideArea);
                            idx = wordStartInText + divideArea;

                            PrepareRenderInt(line);

                            while (Size.X >= width && divideArea > 0)
                            {
                                divideArea--;
                                idx--;

                                line.Remove(divideArea, line.Length - divideArea);
                                PrepareRenderInt(line);
                            }

                            wordStartInText = idx;
                            wordStart = 0;

                            AddLine(list, line, justWidth - indentLine);
                            indentLine = 0;
                            line.Clear();
                            continue;
                        }

                        line.Remove(wordStart, line.Length - wordStart);
                        AddLine(list, line, justWidth - indentLine);
                        indentLine = 0;
                        line.Clear();

                        wordStart = 0;
                        idx = wordStartInText;
                        continue;
                    }
                }

                if (idx == text.Length)
                {
                    break;
                }

                switch(ch)
                {
                    case ' ':
                        wordStart = line.Length;
                        line.Append(text[idx]);
                        wordStartInText = idx + 1;
                        ++idx;
                        continue;

                    case '\n':
                        AddLine(list, line, 0);
                        indentLine = indent;
                        wordStart = 0;
                        wordStartInText = idx + 1;
                        line.Clear();
                        ++idx;
                        continue;

                    default:
                        line.Append(text[idx]);
                        ++idx;
                        break;
                }
            }

            if (line.Length > 0)
            {
                AddLine(list, line, 0);
            }

            return list;
        }

        private void AddLine(List<IFontPresenter> list, StringBuilder builder, Int32 width)
        {
            var presenter = Clone();
            presenter.PrepareRenderInt(builder, width);
            list.Add(presenter);
        }
    }
}
