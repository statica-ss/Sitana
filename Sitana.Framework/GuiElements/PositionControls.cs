using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Gui
{
    public class PositionControls: GuiElement
    {
        enum Mode
        {
            HorizontalTriple
        }

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
            List<GuiElement> elements = new List<GuiElement>();

            String[] ids = initParams.Parameters.AsString("Ids").Split(',');

            String[] scalable = initParams.Parameters.AsString("ScalableIds").Split(',');

            foreach (var id in ids)
            {
                var element = initParams.Owner.Find<GuiElement>(id);
                elements.Add(element);
            }

            Mode mode = initParams.Parameters.AsEnum<Mode>("Mode", Mode.HorizontalTriple);

            Point spacing = initParams.Parameters.ParsePoint("Spacing");

            if (elements.Count > 0)
            {
                Point first = elements.First().ElementRectangle.Location;
                Point last = new Point(elements.Last().ElementRectangle.Right, elements.Last().ElementRectangle.Bottom);

                switch(mode)
                {
                    case Mode.HorizontalTriple:
                        PlaceControlsHorzGrid(elements, scalable, first, last, spacing);
                        break;
                }
            }


            return false;
        }

        private void PlaceControlsHorzGrid(List<GuiElement> elements, String[] scalable, Point first, Point last, Point spacing)
        {
            if (elements.Count != 3)
            {
                throw new InvalidOperationException("HorizontalTriple positioning requires exact 3 elements.");
            }

            Int32 x1 = elements[0].ElementRectangle.Right + spacing.X;
            Int32 x2 = elements[2].ElementRectangle.Left - spacing.X;

            Rectangle rect = elements[1].ElementRectangle;

            rect.X = x1;
            rect.Width = x2 - x1;

            elements[1].UpdatePosition(rect);
        }
    }
}
