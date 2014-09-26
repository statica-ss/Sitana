using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebatianos.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Ebatianos.Gui
{
    public class ScrollingSelector: GuiElement
    {
        private IFontPresenter _caption;
        private IFontPresenter _annotation;

        private ISelectorContext _context;

        private StringBuilder _captionText = new StringBuilder();
        private StringBuilder _annotationText = new StringBuilder();

        private Vector2 _elementsSize;

        private Int32 _spacing = 0;

        private Int32 _selectedPosition;

        private Single _scroll = 0;

        private Boolean _canHandleDrag = false;

        private Boolean _isScrolling = false;

        private Single _scrollTo = Single.NaN;

        private Single _textMargin;

        private Color _textColor;
        private Color _backgroundColor;

        private Color _textColorDisabled;
        private Color _backgroundColorDisabled;

        private Color _textColorSelected;
        private Color _backgroundColorSelected;

        private Single _speed = 0;

        protected override Boolean Initialize(InitializeParams initParams)
        {
            if (!base.Initialize(initParams))
            {
                return false;
            }

            ParametersCollection parameters = initParams.Parameters;
            Point areaSize = GraphicsHelper.PointFromVector2(initParams.AreaSize);
            String directory = initParams.Directory;

            String fontPath = parameters.AsString("CaptionFont");
            _caption = FontLoader.Load(fontPath);

            fontPath = parameters.AsString("AnnotationFont");
            _annotation = FontLoader.Load(fontPath);

            _elementsSize = Vector2.Zero;

            _elementsSize.Y = parameters.AsSingle("ElementHeight") * Scale.Y;
            _elementsSize = _elementsSize.TrimToIntValues();

            _spacing = (Int32)(parameters.AsSingle("Spacing") * Scale.Y);

            _selectedPosition = (Int32)(parameters.AsSingle("SelectedPositionOffset") * Scale.Y);

            _context = parameters.AsObject<ISelectorContext>("Context");

            _textMargin = parameters.AsSingle("TextMargin") * Scale.Y;

            ElementRectangle = FindElementRectangle(parameters, areaSize, Scale, initParams.Offset);

            _elementsSize.X = ElementRectangle.Width;

            _selectedPosition += ElementRectangle.Center.Y;

            ClipToElement = true;

            _textColor = parameters.AsColor("TextColor");
            _backgroundColor = parameters.AsColor("BackgroundColor");

            _textColorDisabled = parameters.AsColor("DisabledTextColor");
            _backgroundColorDisabled = parameters.AsColor("DisabledBackgroundColor");

            _textColorSelected = parameters.AsColor("SelectedTextColor");
            _backgroundColorSelected = parameters.AsColor("SelectedBackgroundColor");

            //_textColorScrolling = parameters.AsColorIfExists("ScrollingTextColor");
            //_backgroundColorScrolling = parameters.AsColorIfExists("ScrollingBackgroundColor");

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.VerticalDrag, OnVerticalDrag);

            InstallGestureHandler(GestureAdditionalType.TouchDown, GestureType.None, OnTouchDown);
            InstallGestureHandler(GestureAdditionalType.TouchUp, GestureType.None, OnTouchUp);

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Tap, OnTap);

            return true;
        }

        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            Color bgColor = ComputeColorWithTransition(transition, _backgroundColor);
            Color fgColor = ComputeColorWithTransition(transition, _textColor);
            Color bgColorDis = ComputeColorWithTransition(transition, _backgroundColorDisabled);
            Color fgColorDis = ComputeColorWithTransition(transition, _textColorDisabled);
            Color bgColorSel = ComputeColorWithTransition(transition, _backgroundColorSelected);
            Color fgColorSel = ComputeColorWithTransition(transition, _textColorSelected);

            Texture2D white = ContentLoader.Current.OnePixelWhiteTexture;

            Vector2 origin = new Vector2(0.5f,0.5f);
            Vector2 scale = _elementsSize;

            Single size = _elementsSize.Y + _spacing;

            Int32 start = 0;
            Int32 center = _selectedPosition + (Int32)(_scroll * size);

            Vector2 offset = topLeft + ComputeOffsetWithTransition(transition);

            while (center > ElementRectangle.Top-_elementsSize.Y / 2)
            {
                center -= (Int32)(_elementsSize.Y + _spacing);
                start--;
            }

            while (center < ElementRectangle.Bottom + _elementsSize.Y / 2)
            {
                if (center > ElementRectangle.Top - _elementsSize.Y / 2)
                {
                    Vector2 pos = new Vector2(ElementRectangle.Center.X, center) + offset;

                    Color color = bgColor;
                    Color textColor = fgColor;

                    if (center == _selectedPosition && !_isScrolling && Single.IsNaN(_scrollTo))
                    {
                        color = bgColorSel;
                        textColor = fgColorSel;
                    }

                    Boolean enabled = false;

                    _context.GetData(start, _captionText, _annotationText, out enabled);

                    if (!enabled)
                    {
                        color = bgColorDis;
                        textColor = fgColorDis;
                    }

                    _annotation.PrepareRender(_annotationText);
                    _caption.PrepareRender(_captionText);

                    spriteBatch.Draw(white, pos, null, color, 0, origin, scale, SpriteEffects.None, 0);

                    Int32 height = (Int32)(_caption.Size.Y * Scale.Y);

                    if (_annotationText.Length > 0)
                    {
                        height += (Int32)(_annotation.Size.Y * Scale.Y + _textMargin);
                    }

                    pos.Y -= height / 2;

                    _caption.DrawText(spriteBatch, pos, textColor, Scale.X, Align.Center | Align.Top);

                    if (_annotationText.Length > 0)
                    {
                        pos.Y += _textMargin + _caption.Size.Y * Scale.Y;
                        _annotation.DrawText(spriteBatch, pos, textColor, Scale.X, Align.Center | Align.Top);
                    }
                }

                center += (Int32)(_elementsSize.Y + _spacing);
                start++;
            }
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean ret = base.Update(gameTime, screenState);
            ret |= _isScrolling;

            Single time = (Single)gameTime.TotalSeconds;
            Single factor = 0;

            if (_context.ShouldUpdateSelection)
            {
                ComputeScrollTo();
            }

            if (!Single.IsNaN(_scrollTo))
            {
                factor = Math.Min(8, 1.0f / time);

                _scroll = (1 - time * factor) * _scroll + time * factor * _scrollTo;

                if (Math.Abs(_scroll - _scrollTo) < 0.02)
                {
                    _context.SetCurrent(-(Int32)_scrollTo);

                    _scroll = 0;
                    _scrollTo = Single.NaN;
                }

                ret = true;
            }

            return ret;
        }

        private void OnTap(Object sender, GestureEventArgs args)
        {
            if (!ElementRectangle.Contains(GraphicsHelper.PointFromVector2(args.Sample.Position)))
            {
                return;
            }

            if (Single.IsNaN(_scrollTo))
            {
                Point pos = GraphicsHelper.PointFromVector2(args.Sample.Position);

                Single size = _elementsSize.Y + _spacing;

                _canHandleDrag = false;
                _isScrolling = false;

                Int32 center = _selectedPosition + (Int32)(_scroll * size);
                Int32 beginCenter = center;

                while (center > ElementRectangle.Top - _elementsSize.Y / 2)
                {
                    center -= (Int32)(_elementsSize.Y + _spacing);
                }

                while (center < ElementRectangle.Bottom + _elementsSize.Y / 2)
                {
                    center += (Int32)(_elementsSize.Y + _spacing);

                    if (center > ElementRectangle.Top - _elementsSize.Y / 2)
                    {
                        Rectangle rect = new Rectangle(ElementRectangle.X, center - (Int32)_elementsSize.Y / 2, ElementRectangle.Width, (Int32)_elementsSize.Y);

                        if (rect.Contains(pos))
                        {
                            _scrollTo = (Int32)((beginCenter - center) / (_elementsSize.Y + _spacing));
                        }
                    }
                }
            }
        }

        private void OnTouchUp(Object sender, GestureEventArgs args)
        {
            if (_isScrolling)
            {
                ComputeScrollTo();
            }

            _canHandleDrag = false;
            _isScrolling = false;
        }

        private void ComputeScrollTo()
        {
            Single internalFactor = _scroll - (Int32)_scroll;

            Boolean enabled = false;

            Single newScrollTo = (Int32)(_scroll + Math.Sign(_scroll) * 0.5f);

            Int32 scrollTo = (Int32)newScrollTo;

            Int32 sign = internalFactor < 0.5f ? -1 : 1;
            Single add = 1;

            while (!enabled)
            {
                _context.GetData(-scrollTo, _captionText, _annotationText, out enabled);

                if (!enabled)
                {
                    scrollTo = (Int32)newScrollTo + (Int32)add * sign;

                    add += 0.5f;
                    sign *= -1;
                }
            }

            _scrollTo = scrollTo;
        }

        private void OnTouchDown(Object sender, GestureEventArgs args)
        {
            if (ElementRectangle.Contains(GraphicsHelper.PointFromVector2(args.Sample.Position)))
            {
                _scrollTo = Single.NaN;
                _canHandleDrag = true;
                args.Handled = true;
            }
        }

        private void OnVerticalDrag(Object sender, GestureEventArgs args)
        {
            if (_canHandleDrag)
            {
                _speed += args.Sample.Delta.Y / (_elementsSize.Y + _spacing);

                _isScrolling = true;
                _scroll += args.Sample.Delta.Y / (_elementsSize.Y + _spacing);
                args.Handled = true;
            }
        }
    }
}
