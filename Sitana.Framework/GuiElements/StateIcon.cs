using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ebatianos.Content;

namespace Ebatianos.Gui
{
    public class StateIcon: GuiElement
    {
        private List<Texture2D> _stateTextures = new List<Texture2D>();
        private Int32 _currentState = 0;

        private Dictionary<String, Int32> _stateNamesToId = new Dictionary<String, Int32>();

        /// <summary>
        /// Draws button.
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

            if (SecondInstance != null)
            {
                if (this == SecondInstance)
                {
                    transition = 1;
                }
                else
                {
                    return;
                }
            }

            // Position of button (Button's center).
            Vector2 position = GraphicsHelper.Vector2FromPoint(
               new Point(ElementRectangle.X, ElementRectangle.Y)
            );

            Color color = ComputeColorWithTransition(transition, Color.White);
            Vector2 offset = ComputeOffsetWithTransition(transition) + topLeft;

            Vector2 origin = Vector2.Zero;

            spriteBatch.Draw(_stateTextures[_currentState], position + offset, null, color * Opacity, 0, origin, Scale, SpriteEffects.None, 0);
        }

        public String State
        {
            set
            {
                _currentState = _stateNamesToId[value.ToUpperInvariant()];
            }
        }

        /// <summary>
        /// Initializes image from parameters.
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

            String stateTypes = parameters.AsString("States");

            String[] states = stateTypes.Split(',', ';');

            for (Int32 idx = 0; idx < states.Length; ++idx)
            {
                String id = String.Format("{0}Texture", states[idx]);
                String texture = parameters.AsString(id);

                Texture2D textureObj = ContentLoader.Current.Load<Texture2D>(texture);

                _stateTextures.Add(textureObj);
                _stateNamesToId.Add(states[idx].ToUpperInvariant(), idx);

                if (textureObj.Width != _stateTextures[0].Width || textureObj.Height != _stateTextures[0].Height)
                {
                    throw new Exception("Each state icon must be same size.");
                }
            }

            Boolean scaling = parameters.AsBoolean("Scaling", true);

            Int32 width = parameters.AsInt32("Width");
            Int32 height = parameters.AsInt32("Height");

            Align align = parameters.AsAlign("Align", "Valign");

            Point position = FindPosition(parameters, GraphicsHelper.PointFromVector2(areaSize), scale);

            if (!scaling)
            {
                Scale = Vector2.One;
            }

            if (width == 0)
            {
                width = _stateTextures[0].Width;
            }

            if (height == 0)
            {
                height = _stateTextures[0].Height;
            }

            Scale = new Vector2((Single)width / (Single)_stateTextures[0].Width, (Single)height / (Single)_stateTextures[0].Height) * Scale;

            Point size = GraphicsHelper.PointFromVector2(new Vector2(_stateTextures[0].Width, _stateTextures[0].Height) * Scale);

            ElementRectangle = RectangleFromAlignAndSize(position, size, align, offset);

            return true;
        }
    }
}
