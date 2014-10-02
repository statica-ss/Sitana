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
    public class Swype : TransitionEffect
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Horizontal"] = parser.ParseLength("Horizontal");
            file["Vertical"] = parser.ParseLength("Vertical");

            file["Power"] = parser.ParseFloat("Power");
        }

        Length _horizontal;
        Length _vertical;
        double _power;
        bool _reverse = false;

        public override void Get(double transition, Point size, out Matrix transform, out float opacity)
        {
            int moveX = _horizontal.Compute(size.X);
            int moveY = _vertical.Compute(size.Y);

            float mul = 1 - (float)Math.Pow(transition, _power);

            if (_reverse)
            {
                mul = -mul;
            }

            transform = Matrix.CreateTranslation(moveX * mul, moveY * mul, 0);
            opacity = 1;
        }

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            _power = DefinitionResolver.Get<double>(controller, binding, definition["Power"], 1);

            _horizontal = DefinitionResolver.Get<Length>(controller, binding, definition["Horizontal"], Length.Zero);
            _vertical = DefinitionResolver.Get<Length>(controller, binding, definition["Vertical"], Length.Zero);
        }

        public override TransitionEffect Reverse()
        {
            return new Swype()
            {
                _power = _power,
                _horizontal = _horizontal,
                _vertical = _vertical,
                _reverse = true
            };
        }
    }
}
