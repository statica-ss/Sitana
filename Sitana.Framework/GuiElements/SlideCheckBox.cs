using Ebatianos.Content;
using Ebatianos;
using Ebatianos.Cs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebatianos.Gui
{
    public class SlideCheckBox: CheckBox
    {
        private Texture2D _thumbTexture;
        private Pair<Single, Single> _thumbRange;

        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            if (SecondInstance != null)
            {
                if (this == FirstInstance)
                {
                    transition = 1;
                }
                else
                {
                    return;
                }
            }

            Vector2 offset = ComputeOffsetWithTransition(transition);
            Color color = ComputeColorWithTransition(transition, _color) * Opacity;
            Color thumbColor = ComputeColorWithTransition(transition, _markColor) * Opacity;

            Vector2 position = new Vector2(ElementRectangle.X, ElementRectangle.Y) + offset + topLeft;

            Single thumbStart = _thumbRange.First;
            Single thumbEnd = _thumbRange.Second;

            Single thumbPosF = thumbStart + (thumbEnd - thumbStart) * _checkedAlpha;

            Vector2 thumbPos = position + new Vector2((Int32)(thumbPosF * ElementRectangle.Width), 0);

            Rectangle uncheckedSource = new Rectangle((Int32)(thumbPosF * _uncheckedTexture.Width), 0, _uncheckedTexture.Width, _uncheckedTexture.Height);
            uncheckedSource.Width -= uncheckedSource.X;

            Rectangle checkedSource = new Rectangle(0, 0, (Int32)(thumbPosF * _checkedTexture.Width)+1, _checkedTexture.Height);

            spriteBatch.Draw(_checkedTexture, position, checkedSource, color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);

            spriteBatch.Draw(_uncheckedTexture, position + new Vector2( ElementRectangle.Width, 0 ), uncheckedSource, color, 0, new Vector2(uncheckedSource.Width,0), Scale, SpriteEffects.None, 0);

            spriteBatch.Draw(_thumbTexture, thumbPos, null, thumbColor, 0, new Vector2(_thumbTexture.Width / 2, 0), Scale, SpriteEffects.None, 0);
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

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            _thumbTexture = ContentLoader.Current.Load<Texture2D>(parameters.AsString("ThumbImage"));

            if (_thumbTexture.Height != _checkedTexture.Height)
            {
                throw new InvalidOperationException("Thumb texture must be equal height as checked and unchecked textures.");
            }

            Single thumbStart = parameters.AsSingle("ThumbStart");
            Single thumbEnd = parameters.AsSingle("ThumbEnd");

            if ( thumbStart == 0 && thumbEnd == 0 )
            {
                thumbStart = (Single)(_thumbTexture.Width / 2) / (Single)_checkedTexture.Width;
                thumbEnd = (Single)(_checkedTexture.Width - _thumbTexture.Width / 2) / (Single)_checkedTexture.Width;
            }

            _thumbRange = new Pair<Single, Single>(thumbStart, thumbEnd);

            return true;
        }
    }
}
