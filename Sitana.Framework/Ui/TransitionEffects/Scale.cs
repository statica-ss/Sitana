using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Controllers;

namespace Sitana.Framework.Ui.Views.TransitionEffects
{
    public class Scale : TransitionEffect
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Horizontal"] = parser.ParseFloat("Horizontal");
            file["Vertical"] = parser.ParseFloat("Vertical");

            file["Power"] = parser.ParseFloat("Power");

            file["Origin"] = parser.ParseEnum<Origin>("Origin");
        }

        enum Origin
        {
            Container,
            Element
        }

        double _horizontal;
        double _vertical;
        double _power;

        Origin _origin = Origin.Container;

        public override void Get(double transition, Rectangle containerRect, Rectangle elementRect, out Matrix transform, out float opacity)
        {
            transition = Math.Max(0, 1 - transition);
            float mul = 1 - (float)Math.Pow(transition, _power);

            float scaleX = (float)(_horizontal * mul + (1-mul));
            float scaleY = (float)(_vertical * mul + (1-mul));

            Vector2 origin = _origin == Origin.Container ? containerRect.Center.ToVector2() : elementRect.Center.ToVector2();

            transform = Matrix.CreateTranslation(-origin.X, -origin.Y, 0) * Matrix.CreateScale(scaleX, scaleY, 0) * Matrix.CreateTranslation(origin.X, origin.Y, 0);

            opacity = 1;
        }

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            _power = DefinitionResolver.Get<double>(controller, binding, definition["Power"], 1);

            _horizontal = DefinitionResolver.Get<double>(controller, binding, definition["Horizontal"], 1);
            _vertical = DefinitionResolver.Get<double>(controller, binding, definition["Vertical"], 1);

            _origin = DefinitionResolver.Get<Origin>(controller, binding, definition["Origin"], Origin.Container);
        }

        public override TransitionEffect Reverse()
        {
            return this;
        }
    }
}
