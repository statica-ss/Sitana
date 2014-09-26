using Sitana.Framework.Content;
using Sitana.Framework;
using Sitana.Framework.Cs;
using Sitana.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Gui
{
    internal class AccordionGroup
    {
        public enum Colors: int
        {
            ContentBgColor,
            HeaderTextColor,
            HeaderBgColor,
            HeaderCollapsedIconColor,
            HeaderExpandedIconColor,
            Count
        }

        public Int32 DrawHeight
        {
            get
            {
                return HeaderHeight + (Int32)(ContentHeight * _expansion);
            }
        }

        public Int32 HeaderHeight { get; private set; }
        public Int32 ContentHeight { get; private set; }

        public Boolean HasContent
        {
            get
            {
                return _elements.Count > 0;
            }
        }

        public Boolean Selected { private get; set; }

        private IFontPresenter _textPresenter;
        private List<GuiElement> _elements = new List<GuiElement>();
        private List<Int32> _elementsHeights = new List<Int32>();

        private Color[] _colors = new Color[(Int32)Colors.Count];

        private Single _scale;
        private Int32 _textMargin = 0;
        private Pair<Int32, Int32> _mediaMargin;

        public Boolean IsExpanded { get; set; }
        public Boolean IsVisible { get; set; }
        private Single  _expansion = 0;

        private Int32 _width;
        private Texture2D _onePixelWhite;

        private Texture2D _icon;

        private Action _action;
        private Boolean _doAction = false;

        public String Id { get; private set; }

        public AccordionGroup(String id, IFontPresenter font, Texture2D icon, String text, Vector2 scale, Int32 height, Int32 textMargin, Point mediaMargin, Color[] colors, Int32 width, Action action)
        {
            Id = id;

            _textPresenter = font.Clone();
            _textPresenter.PrepareRender(text);

            HeaderHeight = (Int32)(height * scale.Y);
            IsExpanded = false;

            Array.Copy(colors, _colors, (Int32)Colors.Count);

            _textMargin = (Int32)(textMargin * scale.X);
            _mediaMargin = new Pair<Int32, Int32>((Int32)(mediaMargin.X * scale.X), (Int32)(mediaMargin.Y * scale.X));
            _scale = scale.Y;

            ContentHeight = 0;

            _onePixelWhite = ContentLoader.Current.OnePixelWhiteTexture;

            _width = width;
            _icon = icon;

            _action = action;
        }

        public void UpdateText(String text)
        {
            _textPresenter.PrepareRender(text);
        }

        private Color GetColor(Colors id)
        {
            return _colors[(Int32)id];
        }

        public void AddElement(XmlFileNode node, Dictionary<String, String> namespaceAliases, String directory, Vector2 scale, Vector2 areaSize, Screen owner)
        {
            GuiElement element = GuiElement.CreateElement(node, namespaceAliases, directory, scale, areaSize, owner);
            _elements.Add(element);

            _elementsHeights.Add((Int32)areaSize.Y);

            ContentHeight = ContentHeight + (Int32)areaSize.Y;
        }

        public void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 offset, Single transition, Texture2D localSeparator, Texture2D groupArrow, Color arrowColor)
        {
            Vector2 topLeft = offset;
            Vector2 bgScale = new Vector2(_width, HeaderHeight);

            Single selected = Math.Min(1, Math.Max(_expansion*_elements.Count, Selected || IsExpanded ? 1 : 0));

            spriteBatch.Draw(_onePixelWhite, topLeft, null, GraphicsHelper.MixColors(GetColor(Colors.HeaderBgColor), GetColor(Colors.ContentBgColor), selected) * transition, 0, Vector2.Zero, bgScale, SpriteEffects.None, 0);

            Vector2 pos = new Vector2(topLeft.X + _textMargin, topLeft.Y + HeaderHeight / 2);
            _textPresenter.DrawText(spriteBatch, pos, GetColor(Colors.HeaderTextColor) * transition, _scale, Align.Middle);

            if (HasContent && groupArrow != null)
            {
                pos.X = topLeft.X + _width - _mediaMargin.Second - (groupArrow.Width / 2) * _scale;
                Vector2 origin = new Vector2(groupArrow.Width / 2, groupArrow.Height / 2);

                spriteBatch.Draw(groupArrow, pos, null, arrowColor * transition, (Single)(_expansion * Math.PI / 2), origin, _scale, SpriteEffects.None, 0);
            }

            if ( _icon != null )
            {
                pos.X = topLeft.X + _mediaMargin.First;
                Vector2 origin = new Vector2(0, _icon.Height / 2);

                Color color = GraphicsHelper.MixColors(GetColor(Colors.HeaderCollapsedIconColor), GetColor(Colors.HeaderExpandedIconColor), selected);

                spriteBatch.Draw(_icon, pos, null, color * transition, 0, origin, _scale, SpriteEffects.None, 0);
            }

            topLeft.Y += HeaderHeight;

            Vector2 separatorScale = new Vector2((Single)_width / (Single)localSeparator.Width, 1);

            if (_expansion > 0)
            {
                Int32 height = HeaderHeight;

                bgScale.Y = DrawHeight - height + 1;

                spriteBatch.Draw(_onePixelWhite, topLeft, null, GetColor(Colors.ContentBgColor), 0, Vector2.Zero, bgScale, SpriteEffects.None, 0);

                for (Int32 idx = 0; idx < _elements.Count; ++idx)
                {
                    var element = _elements[idx];

                    if (height + element.ElementRectangle.Bottom > DrawHeight)
                    {
                        break;
                    }

                    spriteBatch.Draw(localSeparator, topLeft, null, Color.White * transition, 0, Vector2.Zero, separatorScale, SpriteEffects.None, 0);

                    element.Draw(level, spriteBatch, topLeft, transition);

                    topLeft.Y += _elementsHeights[idx];
                    height += _elementsHeights[idx];
                }
            }
        }

        public Boolean Update(TimeSpan timeSpan, Vector2 topLeft, Screen.ScreenState screenState)
        {
            if ( _doAction && _action != null )
            {
                _doAction = false;
                _action.Invoke();
            }

            Boolean redraw = false;

            Single time = (Single)timeSpan.TotalSeconds;
            Single divider = Math.Max(1, _elements.Count);

            if (IsExpanded && _expansion < 1)
            {

                _expansion += time * 16 / divider;
                _expansion = Math.Min(1, _expansion);
                redraw = true;
            }

            if (!IsExpanded && _expansion > 0)
            {
                _expansion -= time * 16 / divider;
                _expansion = Math.Max(0, _expansion);
                redraw = true;
            }

            if (IsExpanded && _expansion >= 0.99f)
            {
                topLeft.Y += HeaderHeight;

                for (Int32 idx = 0; idx < _elements.Count; ++idx)
                {
                    var element = _elements[idx];

                    InputHandler.Current.OffsetPointers(-topLeft);

                    redraw |= element.Update(timeSpan, screenState);

                    InputHandler.Current.OffsetPointers(topLeft);

                    topLeft.Y += _elementsHeights[idx];
                }
            }

            return redraw;
        }

        public void DoAction()
        {
            _doAction = true;
        }
    }
}
