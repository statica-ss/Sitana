using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Gui
{
    public class ControlsContainer: GuiElement
    {
        public override void Draw(int level, SpriteBatch spriteBatch, Vector2 topLeft, float transition)
        {
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            return false;
        }

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
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            ElementRectangle = FindElementRectangle(parameters, GraphicsHelper.PointFromVector2(areaSize), scale, offset);

            return true;
        }
    }
}
