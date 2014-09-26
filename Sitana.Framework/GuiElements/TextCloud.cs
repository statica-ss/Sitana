/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos.Content;
using Ebatianos;
using Microsoft.Xna.Framework.Input.Touch;
using Ebatianos.Input;

namespace Ebatianos.Gui
{
    public class TextCloud : GuiElement
    {
        private List<KeyValuePair<IFontPresenter, Vector2>> _texts = new List<KeyValuePair<IFontPresenter, Vector2>>();
        private ColorWrapper _color;
        private Boolean _allowScroll = false;

        private Single _height = 0;
        private VerticalScroller _verticalScroller;

        /// <summary>
        /// Draws label.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch object used to render textures and texts.</param>
        /// <param name="color">Color to multiply all contents by.</param>
        /// <param name="offset">Offset to move bnutton by.</param>
        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            if (Opacity <= 0)
            {
                return;
            }

            Color color = ComputeColorWithTransition(transition, _color.Value);

            Vector2 offset = new Vector2(ElementRectangle.X, ElementRectangle.Y) + ComputeOffsetWithTransition(transition) + topLeft;

            if (_allowScroll)
            {
                offset -= new Vector2(0, _verticalScroller.Position);
            }

            for (Int32 idx = 0; idx < _texts.Count; ++idx)
            {
                var text = _texts[idx];

                Vector2 position = text.Value;
                IFontPresenter presenter = text.Key;

                presenter.DrawText(spriteBatch, position + offset, color, Scale, Align.Left | Align.Top);
            }
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = base.Update(gameTime, screenState);

            Single time = (Single)gameTime.TotalSeconds;

            redraw |= _verticalScroller.Update(time, _height);

            return redraw;
        }

        /// <summary>
        /// Initializes label from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected override Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            String text = parameters.AsString("Text");
            String fonts = parameters.AsString("Fonts");

            if (text.StartsWith(":"))
            {
                text = Owner.ScreenManager.StringsManager[text];
            }

            String[] fontList = fonts.Split(',');

            if (fontList.Length < 1)
            {
                throw new Exception("Too little fonts for text cloud.");
            }

            text = text.Replace("\\n", " \n ");

            String[] texts = text.Split(' ');

            Align textAlign = parameters.AsAlign("TextAlign", "");

            _color = parameters.FindColorWrapper("Color");

            List<IFontPresenter> fontObjs = new List<IFontPresenter>();

            Int32 seed = parameters.AsInt32("RandomSeed");

            Random random;

            if (seed != 0)
            {
                random = new Random(seed);
            }
            else
            {
                random = new Random();
            }


            List<Int32> randomIndicesList = new List<Int32>();

            Int32 fontsHeight = 0;
            Int32 index = 0;

            foreach (var name in fontList)
            {
                IFontPresenter fontObj = FontLoader.Load(name, directory);

                fontObjs.Add(fontObj);

                fontsHeight = Math.Max( (Int32)(fontObj.Height), fontsHeight);

                for (Int32 idx = 0; idx < index + 1; ++idx)
                {
                    randomIndicesList.Add(index);
                }

                ++index;
            }

            Align align = parameters.AsAlign("Align", "Valign");

            Point position1 = ParsePosition(parameters, "X1", "Y1", GraphicsHelper.PointFromVector2(areaSize), scale);
            Point position2 = ParsePosition(parameters, "X2", "Y2", GraphicsHelper.PointFromVector2(areaSize), scale);

            _allowScroll = parameters.AsBoolean("AllowScroll");

            Single maxWidth = position2.X - position1.X - 1;
            Single maxHeight = position2.Y - position1.Y;

            Vector2 currentPosition = Vector2.Zero;
            Vector2 size = Vector2.Zero;

            fontsHeight = (Int32)(fontsHeight * scale.Y + 0.99);

            List<Single> lineOffsets = new List<Single>();
            List<Int32> lineWords = new List<Int32>();
            List<Int32> lines = new List<Int32>();

            Int32 line = 0;

            Int32 lastObjIndex = -1;

            foreach (var phrase in texts)
            {
                Int32 objIndex = lastObjIndex;

                while (objIndex == lastObjIndex)
                {
                    objIndex = randomIndicesList[random.Next(randomIndicesList.Count)];

                    if (fontObjs.Count == 1)
                    {
                        break;
                    }
                }

                lastObjIndex = objIndex;

                IFontPresenter presenter = fontObjs[objIndex].Clone();

                if (currentPosition.X > 0)
                {
                    presenter.PrepareRender(" " + phrase);
                }
                else
                {
                    presenter.PrepareRender(phrase);
                }

                Vector2 phraseSize = presenter.Size * scale;
                phraseSize.Y = presenter.Height * scale.Y;

                phraseSize.X = (Single)Math.Ceiling(phraseSize.X);
                phraseSize.Y = (Single)Math.Ceiling(phraseSize.Y);

                Single theOffset = 0;
                
                switch (random.Next(2))
                {
                    case 0:
                        theOffset = fontsHeight - phraseSize.Y;
                        break;

                    case 1:
                        theOffset = (fontsHeight - phraseSize.Y) / 2;
                        break;
                }

                if (currentPosition.X + phraseSize.X > maxWidth || phrase == "\n")
                {
                    currentPosition.Y += fontsHeight;

                    if (!_allowScroll && currentPosition.Y + fontsHeight > maxHeight)
                    {
                        break;
                    }

                    if (phrase == "\n" && textAlign == Align.Justify)
                    {
                        lineOffsets.Add(0);
                    }
                    else
                    { 
                        lineOffsets.Add(currentPosition.X);
                    }
                    line++;

                    currentPosition.X = 0;
                    
                    presenter.PrepareRender(phrase);
                    phraseSize = presenter.Size * scale;

                    if (phrase == "\n")
                    {
                        continue;
                    }
                }

                _texts.Add(new KeyValuePair<IFontPresenter, Vector2>(presenter, currentPosition + new Vector2(0, theOffset)));

                currentPosition.X += phraseSize.X;

                size.Y = Math.Max(size.Y, currentPosition.Y + fontsHeight);
                size.X = Math.Max(size.X, currentPosition.X);

                lines.Add(line);

                if (lineWords.Count < line + 1)
                {
                    lineWords.Add(1);
                }
                else
                {
                    lineWords[line] = lineWords[line] + 1;
                }
            }

            lineOffsets.Add(currentPosition.X);

            Int32 lineSize = position2.X - position1.X;

            for (Int32 idx = 0; idx < lineOffsets.Count; ++idx)
            {
                if (IsAlign(textAlign, Align.Right))
                {
                    lineOffsets[idx] = size.X - lineOffsets[idx];
                }
                else if (IsAlign(textAlign, Align.Center))
                {
                    lineOffsets[idx] = (size.X - lineOffsets[idx]) / 2;
                }
                else if ( IsAlign(textAlign, Align.Justify))
                {
                    if ( lineOffsets[idx] > lineSize * 0.5f )
                    {
                        lineOffsets[idx] = (lineSize - lineOffsets[idx]);
                    }
                    else
                    {
                        lineOffsets[idx] = 0;
                    }
                }
                else
                {
                    lineOffsets[idx] = 0;
                }
            }

            if (position1 != position2)
            {
                if (IsAlign(align, Align.Right))
                {
                    position1.X = position2.X;
                }

                if (IsAlign(align, Align.Bottom))
                {
                    position1.Y = position2.Y;
                }

                if (IsAlign(align, Align.Center))
                {
                    position1.X = (position1.X + position2.X - (Int32)size.X) / 2;
                }

                if (IsAlign(align, Align.Middle))
                {
                    position1.Y = (position1.Y + position2.Y - (Int32)size.Y) / 2;
                }
            }

            Int32 word = 0;
            line = -1;

            for (Int32 idx = 0; idx < _texts.Count; ++idx)
            {
                Vector2 theOffset = new Vector2(lineOffsets[lines[idx]], 0);

                if (IsAlign(textAlign, Align.Justify))
                {
                    if ( lines[idx] != line )
                    {
                        word = 0;
                        line = lines[idx];
                    }
                    Int32 words = lineWords[lines[idx]];
                    Single add = words > 0 ? theOffset.X / (Single)(words - 1) : 0;

                    theOffset.X = (Int32)(add * word - 1);
                    _texts[idx] = new KeyValuePair<IFontPresenter, Vector2>(_texts[idx].Key, _texts[idx].Value + theOffset);

                    word++;
                }
                else
                {
                    _texts[idx] = new KeyValuePair<IFontPresenter, Vector2>(_texts[idx].Key, _texts[idx].Value + theOffset);
                }
            }

            _height = size.Y;
            size.Y = Math.Min(position2.Y - position1.Y, size.Y);

            ElementRectangle = RectangleFromAlignAndSize(position1, GraphicsHelper.PointFromVector2(size), Align.Left | Align.Top, offset);
            ClipToElement = _allowScroll;

            _verticalScroller = new VerticalScroller(ElementRectangle, Scale.Y * 100);

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.VerticalDrag, _verticalScroller.HandleVerticalDragGesture);

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Flick, _verticalScroller.HandleFlickGesture);

            InstallGestureHandler(GestureAdditionalType.TouchDown, GestureType.None, _verticalScroller.HandleTouchDownGesture);
            InstallGestureHandler(GestureAdditionalType.TouchUp, GestureType.None, _verticalScroller.HandleTouchUpGesture);

            return true;
        }
    }
}
