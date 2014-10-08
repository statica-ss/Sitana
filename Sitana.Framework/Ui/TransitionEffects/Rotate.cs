using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.Controllers;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Views.TransitionEffects
{
    public class Rotate : TransitionEffect
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Power"] = parser.ParseFloat("Power");
            file["Angle"] = parser.ParseFloat("Angle");

            file["Origin"] = parser.ParseEnum<Origin>("Origin");
        }

        enum Origin
        {
            Container,
            Element
        }

        Origin _origin = Origin.Container;
        double _power;
        double _angle;

        public override void Get(double transition, Rectangle containerRect, Rectangle elementRect, out Matrix transform, out float opacity)
        {
            transition = Math.Max(0, 1 - transition);
            float mul = 1 - (float)Math.Pow(transition, _power);

            float angle = (float)(_angle * mul);

            Vector2 origin = _origin == Origin.Container ? containerRect.Center.ToVector2() : elementRect.Center.ToVector2();

            transform = Matrix.CreateTranslation(-origin.X, -origin.Y, 0) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(origin.X, origin.Y, 0);

            opacity = 1;
        }

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            _power = DefinitionResolver.Get<double>(controller, binding, definition["Power"], 1);
            _origin = DefinitionResolver.Get<Origin>(controller, binding, definition["Origin"], Origin.Container);
            _angle = DefinitionResolver.Get<double>(controller, binding, definition["Angle"], 0) * Math.PI / 180.0;
        }

        public override TransitionEffect Reverse()
        {
            return new Rotate()
            {
                _power = _power,
                _origin = _origin,
                _angle = -_angle
            };
        }
    }
}
