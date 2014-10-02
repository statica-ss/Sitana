using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Controllers;

namespace Sitana.Framework.Ui.Views.TransitionEffects
{
    public class Vanish : TransitionEffect
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);
            file["Power"] = parser.ParseFloat("Power");
        }

        double _power = 1;

        public override void Get(double transition, Rectangle containerRect, Rectangle elementRect, out Matrix transform, out float opacity)
        {
            transform = Matrix.Identity;
            opacity = (float)Math.Pow(transition, _power);
        }

        public override TransitionEffect Reverse()
        {
            return this;
        }

        protected override void Init(UiController controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            _power = DefinitionResolver.Get<double>(controller, binding, definition["Power"], 1);
        }

        
    }
}
