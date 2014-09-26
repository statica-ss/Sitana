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

using Ebatianos.Content;
using Ebatianos.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace Ebatianos.Gui
{
    public class 
        Button : GuiElement
    {

        // Possible button states.
        enum ButtonState
        {
            None,
            Pressed,
            Hold,
            Outside,
            Released
        }

        // Action call mode
        enum Mode
        {
            Push,
            Delayed,
            Release,
            Game
        }

        enum Sound : int
        {
            Pushed = 0,
            Released = 1,
            Action = 2,
            Count
        }

        protected Vector2 _iconPosition = Vector2.Zero;
        protected Vector2 _textPosition = Vector2.Zero;

        protected Texture2D _iconTexture;
        protected NinePatchImage _backgroundTexture;
        protected NinePatchImage _backgroundPushedTexture;

        protected Vector2 _backgroundScale = Vector2.One;

        protected IFontPresenter _textPresenter;
        protected IFontPresenter _smallTextPresenter;

        protected ColorWrapper _textColor;
        protected ColorWrapper _textPushedColor;
        protected ColorWrapper _textDisabledColor;

        private ColorWrapper _iconColor;
        private ColorWrapper _iconDisabledColor;
        private ColorWrapper _iconPushedColor;

        protected ColorWrapper _backgroundColor;
        protected ColorWrapper _backgroundPushedColor;

        protected ColorWrapper _backgroundDisabledColor;

        protected Action ButtonAction { get; set; }
        private Mode _mode = Mode.Release;

        private Double _actionDelay = 0.1;

        protected Int32 _pointerId = 0;
        private Double _wait = 0;

        private Int32 _touchMargin = 0;

        protected Vector2 _smallTextOffset = Vector2.Zero;

        private ButtonState _state = ButtonState.None;

        protected Single _margin = 0;

        protected String _text = null;
        String _smallText = null;

        private SoundEffectContainer<Sound> _sounds;

        protected Rectangle _touchRectangle;

        protected Single _additionalRightMargin = 0;

        protected Vector2 _iconAndTextSize;

        private Align _contentAlign;

        private String _iconPositionType;

        private Single _iconMargin;

        private Single _repeatPeriod;
        private Single _repeatAcceleration;

        private Single _currentRepeatPeriod;
        private Single _repeatPeriodMin;

        private Single _waitToRepeat;

        private Boolean _repeatActionInvoked = false;

        

        public virtual Boolean IsPushed
        {
            get
            {
                return _state == ButtonState.Pressed || _state == ButtonState.Hold;
            }
        }

        public Button()
        {
            
        }

        public void UpdateIcon(Texture2D icon)
        {
            if ( icon.Width != _iconTexture.Width || icon.Height != _iconTexture.Height )
            {
                throw new Exception("Cannot update icon because size of new texture is different.");
            }

            _iconTexture = icon;
        }

        public override Boolean Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = InputHandler.Current.PointersState.Count > 0;

            redraw |= base.Update(gameTime, screenState);

            ButtonState state = _state;

            _touchRectangle = ElementRectangle;
            _touchRectangle.X -= _touchMargin;
            _touchRectangle.Y -= _touchMargin;

            _touchRectangle.Width += _touchMargin * 2;
            _touchRectangle.Height += _touchMargin * 2;


            if (Enabled && Visible && !InputHandler.Current.DisableButtons)
            {
                if (_mode == Mode.Delayed && _wait > 0)
                {
                    _wait -= gameTime.TotalSeconds;

                    if (_wait <= 0)
                    {
                        DoAction(Vector2.Zero);
                        _sounds.Play(Sound.Action);
                    }

                    return true;
                }

                if (screenState == Screen.ScreenState.Visible)
                {
                    switch (_state)
                    {
                        case ButtonState.None:
                            ProcessNoneOrReleasedState();
                            break;

                        case ButtonState.Pressed:
                            ProcessPressedOrHoldOrOutsideState();
                            break;

                        case ButtonState.Hold:
                            ProcessPressedOrHoldOrOutsideState();
                        break;

                        case ButtonState.Outside:
                            ProcessPressedOrHoldOrOutsideState();
                            break;

                        case ButtonState.Released:
                            ProcessNoneOrReleasedState();
                            break;
                    }
                }

                if (_state == ButtonState.Hold)
                {
                    if (_waitToRepeat > 0)
                    {
                        _waitToRepeat -= (Single)gameTime.TotalSeconds;

                        if (_waitToRepeat <= 0)
                        {
                            DoAction(GraphicsHelper.Vector2FromPoint(ElementRectangle.Center));
                            _sounds.Play(Sound.Action);

                            _currentRepeatPeriod -= _repeatAcceleration;
                            _currentRepeatPeriod = Math.Max(_currentRepeatPeriod, _repeatPeriodMin);

                            _waitToRepeat = _currentRepeatPeriod;
                            _repeatActionInvoked = true;
                        }
                    }
                }
            }
            else
            {
                _state = ButtonState.None;
            }

            return redraw || _state != state;
        }

        private void DoAction(Vector2 position)
        {
            if (ButtonAction != null)
            {
                ButtonAction.Invoke();
            }

            OnAction(new Vector2(position.X - ElementRectangle.Left, position.Y - ElementRectangle.Top));
        }

        protected virtual void OnAction(Vector2 position)
        {

        }

        // Processes non-pressed button state.
        private void ProcessNoneOrReleasedState()
        {
            _repeatActionInvoked = false;

            // Get input.
            InputHandler input = InputHandler.Current;

            // Iterate through pointer states.
            for (Int32 index = 0; index < input.PointersState.Count; ++index)
            {
                var state = input.PointersState[index];

                // Pointer is suitable to process in non-pressed state when it's been just pressed
                // or when button is a game button and pointer has moved.
                Boolean canProcess = (state.State == PointerInfo.PressState.Pressed);
                canProcess |= (_mode == Mode.Game && (state.State == PointerInfo.PressState.Moved));

                // Check if pointer is inside button's area.
                if (canProcess && _touchRectangle.Contains(GraphicsHelper.PointFromVector2(state.Position)))
                {
                    // Store pointer id.
                    _pointerId = state.PointerId;

                    // Set button state to pressed.
                    _state = ButtonState.Pressed;
                    input.PointersState.RemoveAt(index);

                    _currentRepeatPeriod = _repeatPeriod;
                    _waitToRepeat = _repeatPeriod * 2;

                    if (_mode == Mode.Push)
                    {
                        DoAction(state.Position);
                        _sounds.Play(Sound.Action);
                    }
                    else
                    {
                        _wait = _actionDelay;
                        _sounds.Play(Sound.Pushed);
                    }

                    return;
                }
            }

            // If none pointers matched, set state to none (This changes state of button from released to none).
            _state = ButtonState.None;
        }

        // Processes button when it has assigned pointer id.
        private void ProcessPressedOrHoldOrOutsideState()
        {
            // Get input handler.
            InputHandler input = InputHandler.Current;

            // Iterate through pointer states.
            for (Int32 index = 0; index < input.PointersState.Count; ++index)
            {
                var state = input.PointersState[index];

                // Check if pointer matches stored pointerId.
                Boolean canProcess = (_pointerId == state.PointerId);

                if (canProcess)
                {
                    // Switch button's state and process properly.
                    switch (state.State)
                    {
                        // If pointer is pressed or moved.
                        case PointerInfo.PressState.Pressed:
                        case PointerInfo.PressState.Moved:

                            // Check if pointer is inside button area.
                            if (_touchRectangle.Contains(GraphicsHelper.PointFromVector2(state.Position)))
                            {
                                if (_state != ButtonState.Hold)
                                {
                                    _waitToRepeat = _repeatPeriod * 2;
                                    _currentRepeatPeriod = _repeatPeriod;
                                }

                                // Set state to hold.
                                _state = ButtonState.Hold;
                            }
                            else // if outside button area.
                            {
                                // If it's game button, release it.
                                if (_mode == Mode.Game || _mode == Mode.Delayed)
                                {
                                    _state = ButtonState.Released;
                                    _pointerId = PointerInfo.InvalidPointerId;
                                }
                                // If menu button, set outside state (where button may be pressed again by moving pointer over it).
                                else
                                {
                                    _state = ButtonState.Outside;
                                }
                            }

                            input.PointersState.RemoveAt(index);

                            // Return to prevent button from changing state to released.
                            return;

                        // If pointer is released or have invalid state.
                        default:

                            // If pointer was released inside button, and action is set, perform action.
                            if (_touchRectangle.Contains(GraphicsHelper.PointFromVector2(state.Position)))
                            {
                                input.PointersState.RemoveAt(index);

                                if (_mode == Mode.Release)
                                {
                                    if (!_repeatActionInvoked)
                                    {
                                        DoAction(state.Position);
                                        _sounds.Play(Sound.Action);
                                    }
                                }
                            }
                            break;
                    }
                    break;
                }
            }

            // Set button to released state and reset pointer id.
            _state = ButtonState.Released;
            _pointerId = PointerInfo.InvalidPointerId;

            _sounds.Play(Sound.Released);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="transition"></param>
        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            Boolean drawBackground = true;
            Single textOpacity = 1;

            Single opacity = Opacity;

            Color backgroundColor = IsPushed ? _backgroundPushedColor.Value : _backgroundColor.Value;
            Color textColor = IsPushed ? _textPushedColor.Value : _textColor.Value;
            Color iconColor = IsPushed ? _iconPushedColor.Value : _iconColor.Value;

            if (!Enabled)
            {
                textColor = _textDisabledColor.Value;
                iconColor = _iconDisabledColor.Value;
                backgroundColor = _backgroundDisabledColor.Value;
            }

            if (SecondInstance != null)
            {
                if ((SecondInstance as Button)._text == (FirstInstance as Button)._text)
                {
                    if (this == FirstInstance)
                    {
                        return;
                    }

                    GuiElement first = FirstInstance;

                    Vector2 pos1 = new Vector2(first.ElementRectangle.X, first.ElementRectangle.Y);
                    Vector2 pos2 = new Vector2(ElementRectangle.X, ElementRectangle.Y);

                    Vector2 newPos = (1 - transition) * pos1 + transition * pos2;

                    topLeft += newPos - pos2;

                    transition = transition * transition * transition;

                    opacity = first.Opacity * (1 - transition) + Opacity * transition;
                    

                    transition = 1;
                }
                else
                {
                    drawBackground = (this == FirstInstance);

                    textOpacity = transition;
                    transition = 1;

                    backgroundColor = _backgroundColor.Value;
                    textColor = _textColor.Value;
                    iconColor = _iconColor.Value;
                }
            }

            NinePatchImage background = IsPushed ? _backgroundPushedTexture : _backgroundTexture;
            Texture2D icon = _iconTexture;
            IFontPresenter text = _textPresenter;
            IFontPresenter smallText = _smallTextPresenter;

            if (!Enabled)
            {
                textColor = _textDisabledColor.Value;
                iconColor = _iconDisabledColor.Value;
            }

            textColor = ComputeColorWithTransition(transition, textColor) * textOpacity;
            iconColor = ComputeColorWithTransition(transition, iconColor) * textOpacity;
            backgroundColor = ComputeColorWithTransition(transition, backgroundColor);

            Vector2 offset = ComputeOffsetWithTransition(transition) + topLeft;

            Int32 top = 0;
            Int32 bottom = ElementRectangle.Height;
            Int32 left = 0;
            Int32 right = ElementRectangle.Width;

            if (drawBackground && background != null)
            {
                Vector2 pos = new Vector2(ElementRectangle.X, ElementRectangle.Y) + offset;

                Rectangle destination = new Rectangle((Int32)pos.X, (Int32)pos.Y, ElementRectangle.Width, ElementRectangle.Height);

                Rectangle contentRect = background.Draw(spriteBatch, destination , Scale, backgroundColor * opacity);

                top = contentRect.Top;
                bottom = contentRect.Bottom;
                left = contentRect.Left;
                right = contentRect.Right;
            }

            Vector2 contentOffset = Vector2.Zero;

            if (IsAlign(_contentAlign, Align.Right))
            {
                contentOffset.X = right - _margin - _iconAndTextSize.X;
            }
            else if (IsAlign(_contentAlign, Align.Center))
            {
                contentOffset.X = (right + left) / 2 - _iconAndTextSize.X / 2;
            }
            else
            {
                contentOffset.X = left + _margin;
            }

            if (IsAlign(_contentAlign, Align.Bottom))
            {
                contentOffset.X = bottom - _margin - _iconAndTextSize.Y;
            }
            if (IsAlign(_contentAlign, Align.Middle))
            {
                contentOffset.Y = (bottom + top) / 2 - _iconAndTextSize.Y / 2;
            }
            else
            {
                contentOffset.Y = top + _margin;
            }

            contentOffset += new Vector2(ElementRectangle.X, ElementRectangle.Y);

            if (icon != null)
            {
                spriteBatch.Draw(icon, _iconPosition + offset + contentOffset, null, iconColor * opacity, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            }

            Vector2 textPosition = _textPosition + contentOffset;

            Single over = 0;

            if (text != null && smallText != null)
            {
                over = (text.Size.X + _smallTextOffset.X) * Scale.X - ( right - left - _margin * Scale.X * 2.25f);
                over += _additionalRightMargin;
            }

            if (over > 0)
            {
                contentOffset.X = ElementRectangle.X + left + _margin;
                textPosition = _textPosition + contentOffset;

                textPosition.X -= over;

                Rectangle clipRect = new Rectangle((Int32)(ElementRectangle.Left + left), ElementRectangle.Top, right-left, ElementRectangle.Height);

                ScreenManager.Current.AdditionalDraw(() =>
                {
                    if (text != null)
                    {
                        text.DrawText(spriteBatch, textPosition + offset, textColor * opacity, Scale, Align.Top | Align.Left);
                    }

                    if (smallText != null)
                    {
                        Vector2 add = new Vector2(text != null ? text.Size.X * Scale.X : 0, 0) + _smallTextOffset * Scale;
                        smallText.DrawText(spriteBatch, textPosition + offset + add, textColor * opacity, Scale, Align.Top | Align.Left);
                    }
                }, clipRect);
            }
            else
            {
                if (text != null)
                {
                    text.DrawText(spriteBatch, textPosition + offset, textColor * opacity, Scale, Align.Top | Align.Left);
                }

                if (smallText != null)
                {
                    Vector2 add = new Vector2(text != null ? text.Size.X * Scale.X : 0, 0) + _smallTextOffset * Scale;
                    smallText.DrawText(spriteBatch, textPosition + offset + add, textColor * opacity, Scale, Align.Top | Align.Left);
                }
            }
        }

        public void UpdateText(String text)
        {
            _textPresenter.PrepareRender(text);
            _text = text;

            ComputeIconAndText();
        }

        public void UpdateSmallText(String text)
        {
            _smallTextPresenter.PrepareRender(text);
            _smallText = text;
        }

        public override GuiElement Clone()
        {
            Button button = new Button();

            button._iconPosition = _iconPosition;
            button._textPosition = _textPosition;

            button._iconTexture = _iconTexture;
            button._backgroundTexture = _backgroundTexture;
            button._backgroundPushedTexture = _backgroundPushedTexture;

            button._backgroundScale = _backgroundScale;

            button._contentAlign = _contentAlign;
            button._iconAndTextSize = _iconAndTextSize;

            button._textPresenter = _textPresenter.Clone();
            button._textPresenter.PrepareRender(_text);

            if (_smallTextPresenter != null)
            {
                button._smallTextPresenter = _smallTextPresenter.Clone();
                button._smallTextPresenter.PrepareRender(_smallText);
            }

            button._textColor = _textColor;
            button._textPushedColor = _textPushedColor;
            button._textDisabledColor = _textDisabledColor;

            button._iconColor = _iconColor;
            button._iconPushedColor = _iconPushedColor;
            button._iconDisabledColor = _iconDisabledColor;

            button._backgroundColor = _backgroundColor;
            button._backgroundPushedColor = _backgroundPushedColor;
            button._backgroundDisabledColor = _backgroundDisabledColor;

            button.ButtonAction = ButtonAction;
            button._mode = _mode;

            button._actionDelay = _actionDelay;
            button.Enabled = Enabled;

            button._text = _text;
            button._smallText = _smallText;

            button._sounds = _sounds;

            button._touchMargin = _touchMargin;
            button._margin = _margin;

            button.OnCloned(this);


            return button;
        }

        protected Boolean InitializeOnlyBase(InitializeParams initParams)
        {
            return base.Initialize(initParams);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="directory"></param>
        /// <param name="contentLoader"></param>
        /// <param name="scale"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        /// <summary>
        /// Initializes accordion from parameters.
        /// </summary>
        /// <param name="node">XML node entity.</param>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="scale">Current screen scale.</param>
        /// <param name="areaSize">Size of the area.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected override Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;
            Screen owner = initParams.Owner;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            Vector2 size = Vector2.Zero;
            Single unifiedScale = Math.Min(scale.X, scale.Y);

            String background = parameters.AsString("Background");
            String backgroundPushed = parameters.AsString("BackgroundPushed");
            String icon = parameters.AsString("Icon");
            String text = owner.ScreenManager.StringsManager[parameters.AsString("Text")];
            String smallText = owner.ScreenManager.StringsManager[parameters.AsString("SmallText")];
            String font = parameters.AsString("Font");
            String smallFont = parameters.AsString("SmallFont");

            Int32 actionDelay = parameters.AsInt32("ActionDelay");

            _actionDelay = actionDelay > 0 ? (Double)actionDelay / 1000.0 : 0.1;

            _smallTextOffset = GraphicsHelper.Vector2FromPoint(parameters.ParsePoint("SmallTextOffset"));

            Single width = parameters.AsInt32("Width") * unifiedScale;
            Single height = parameters.AsInt32("Height") * unifiedScale;

            Single padding = parameters.AsInt32("Padding") * unifiedScale;
            _iconMargin = parameters.AsInt32("IconMargin") * unifiedScale;

            Align align = parameters.AsAlign("Align", "Valign");

            Point position = FindPosition(parameters, GraphicsHelper.PointFromVector2(areaSize), scale);

            if ( parameters.HasKey("X2"))
            {
                Point position1 = ParsePosition(parameters, "X1", "Y", GraphicsHelper.PointFromVector2(areaSize), scale);
                Point position2 = ParsePosition(parameters, "X2", "Y", GraphicsHelper.PointFromVector2(areaSize), scale);

                if (width == 0)
                {
                    width = position2.X - position1.X;
                    position.X = position1.X;
                    align &= Align.Vert;
                }
            }

            if (parameters.HasKey("Y2"))
            {
                Point position1 = ParsePosition(parameters, "X", "Y1", GraphicsHelper.PointFromVector2(areaSize), scale);
                Point position2 = ParsePosition(parameters, "X", "Y2", GraphicsHelper.PointFromVector2(areaSize), scale);

                if (height == 0)
                {
                    height = position2.Y - position1.Y;
                    position.Y = position1.Y;
                    align &= Align.Horz;
                }
            }

            Align contentAlign = parameters.AsAlign("ContentAlign", "ContentValign", Align.Center | Align.Middle);
            _iconPositionType = parameters.AsString("IconPosition");

            _touchMargin = (Int32)(parameters.AsSingle("TouchMargin") * unifiedScale);

            _iconColor = parameters.FindColorWrapper("IconColor");
            _textColor = parameters.FindColorWrapper("TextColor");
            _backgroundColor = parameters.FindColorWrapper("BackgroundColor");

            _iconPushedColor = parameters.FindColorWrapper("IconPushedColor", false) ?? _iconColor;
            _textPushedColor = parameters.FindColorWrapper("TextPushedColor", false) ?? _textColor;

            _iconDisabledColor = parameters.FindColorWrapper("IconDisabledColor", false) ?? _iconColor;
            _textDisabledColor = parameters.FindColorWrapper("TextDisabledColor", false) ?? _textColor;

            _backgroundPushedColor = parameters.FindColorWrapper("BackgroundPushedColor", false) ?? _backgroundColor;

            _iconDisabledColor = parameters.FindColorWrapper("IconDisabledColor", false) ?? _iconColor;
            _textDisabledColor = parameters.FindColorWrapper("TextDisabledColor", false) ?? _textColor;
            _backgroundDisabledColor = parameters.FindColorWrapper("BackgroundDisabledColor", false) ?? _backgroundColor;


            Vector2 backgroundSize = Vector2.Zero;

            if (!String.IsNullOrEmpty(font))
            {
                _textPresenter = FontLoader.Load(font, directory);
                _textPresenter.PrepareRender(text);
            }

            if ( !String.IsNullOrEmpty(smallFont))
            {
                if (!String.IsNullOrEmpty(smallText))
                {
                    _smallTextPresenter = FontLoader.Load(smallFont, directory);
                    _smallTextPresenter.PrepareRender(smallText);
                }
            }

            _iconTexture = String.IsNullOrEmpty(icon) ? null : ContentLoader.Current.Load<Texture2D>(icon);
            _backgroundTexture = String.IsNullOrEmpty(background) ? null : ContentLoader.Current.Load<NinePatchImage>(background);

            _backgroundPushedTexture = String.IsNullOrEmpty(backgroundPushed) ? _backgroundTexture : ContentLoader.Current.Load<NinePatchImage>(backgroundPushed);

            if (_backgroundTexture != null)
            {
                if (_backgroundTexture.Width != _backgroundPushedTexture.Width || _backgroundTexture.Height != _backgroundPushedTexture.Height)
                {
                    throw new InvalidOperationException("Background textures must be equal size");
                }

                if (width == 0)
                {
                    width = _backgroundTexture.Width * Scale.X;
                }

                if (height == 0)
                {
                    height = _backgroundTexture.Height * Scale.Y;
                }

                backgroundSize = new Vector2(width, height);

                _backgroundScale = new Vector2(width / (Single)_backgroundTexture.Width, height / (Single)_backgroundTexture.Height);
            }

            ComputeIconAndText();

            ElementRectangle = RectangleFromAlignAndSize(position, new Point((Int32)width, (Int32)height), align, offset);

            _margin = padding;
            _contentAlign = contentAlign;

            _mode = parameters.AsEnum<Mode>("ActionMode", Mode.Release);
            _repeatPeriod = parameters.AsSingle("RepeatPeriod");

            _repeatPeriodMin = parameters.AsSingle("RepeatPeriodMin");
            _repeatAcceleration = parameters.AsSingle("RepeatAcceleration");


            ButtonAction = parameters.AsAction("OnClick", this);

            _sounds = new SoundEffectContainer<Sound>();

            String soundPath = parameters.AsString("SoundPushed");

            if (!String.IsNullOrWhiteSpace(soundPath))
            {
                _sounds.Add(Sound.Pushed, soundPath);
            }

            soundPath = parameters.AsString("SoundReleased");

            if (!String.IsNullOrWhiteSpace(soundPath))
            {
                _sounds.Add(Sound.Released, soundPath);
            }

            soundPath = parameters.AsString("SoundAction");

            if (!String.IsNullOrWhiteSpace(soundPath))
            {
                _sounds.Add(Sound.Action, soundPath);
            }

            _text = text;

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Tap, OnTap);

            return true;
        }

        private void ComputeIconAndText()
        {
            Vector2 iconSize = Vector2.Zero;
            Vector2 textSize = Vector2.Zero;

            Vector2 iconAndTextSize = Vector2.Zero;

            Single iconMargin = _iconMargin;

            if (_textPresenter != null)
            {
                textSize = _textPresenter.Size * Scale;
            }

            if (_smallTextPresenter != null)
            {
                Vector2 tsize = _smallTextPresenter.Size * Scale;

                textSize.X += tsize.X;
                textSize.Y = Math.Max(tsize.Y, textSize.Y);
            }

            if (_iconTexture != null)
            {
                iconSize = new Vector2(_iconTexture.Width, _iconTexture.Height) * Scale;
            }

            switch (_iconPositionType.ToUpperInvariant())
            {
                case "TOP":
                    _iconPosition.X = (textSize.X - iconSize.X) / 2;
                    _textPosition.X = 0;

                    _iconPosition.Y = 0;
                    _textPosition.Y = iconSize.Y + iconMargin;

                    iconAndTextSize = new Vector2(Math.Max(textSize.X, iconSize.X), iconSize.Y + textSize.Y + iconMargin);

                    break;

                case "BOTTOM":

                    _iconPosition.X = (textSize.X - iconSize.X) / 2;
                    _textPosition.X = 0;

                    _iconPosition.Y = textSize.Y + iconMargin;
                    _textPosition.Y = 0;

                    iconAndTextSize = new Vector2(Math.Max(textSize.X, iconSize.X), iconSize.Y + textSize.Y + iconMargin);

                    break;

                case "RIGHT":

                    _iconPosition.X = textSize.X + iconMargin;
                    _textPosition.X = 0;

                    _iconPosition.Y = (textSize.Y - iconSize.Y) / 2;
                    _textPosition.Y = 0;

                    iconAndTextSize = new Vector2(textSize.X + iconSize.X + iconMargin, Math.Max(iconSize.Y, textSize.Y));

                    break;

                default:

                    _iconPosition.X = 0;
                    _textPosition.X = iconSize.X + iconMargin;

                    _iconPosition.Y = (textSize.Y - iconSize.Y) / 2;
                    _textPosition.Y = 0;

                    iconAndTextSize = new Vector2(textSize.X + iconSize.X + iconMargin, Math.Max(iconSize.Y, textSize.Y));

                    break;
            }


            if (_iconPosition.X < 0)
            {
                _textPosition.X -= _iconPosition.X;
                _iconPosition.X = 0;
            }

            if (_iconPosition.Y < 0)
            {
                _textPosition.Y -= _iconPosition.Y;
                _iconPosition.Y = 0;
            }

            if (_textPosition.X < 0)
            {
                _iconPosition.X -= _textPosition.X;
                _textPosition.X = 0;
            }

            if (_textPosition.Y < 0)
            {
                _iconPosition.Y -= _textPosition.Y;
                _textPosition.Y = 0;
            }

            _iconAndTextSize = iconAndTextSize;
        }

        private void OnTap(Object sender, GestureEventArgs eventArgs)
        {
            if (Visible && Enabled)
            {
                if (_touchRectangle.Contains(GraphicsHelper.PointFromVector2(eventArgs.Sample.Position)))
                {
                    eventArgs.Handled = true;
                }
            }
        }

        protected void ResetState()
        {
            _state = ButtonState.Released;
            _pointerId = PointerInfo.InvalidPointerId;
        }
    }
}
