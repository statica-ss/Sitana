using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Ui.Views.QuickData
{
    public class QuickSeparatorLine : QuickSeparator, IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Color"] = parser.ParseColor("Color");
            file["Left"] = parser.ParseLength("Left");
            file["Right"] = parser.ParseLength("Right");
        }

        ColorWrapper _color;

        Length _left;
        Length _right;

        int _lastWidth = 0;

        int _leftMargin = 0;
        int _rightMargin = 0;

        bool IDefinitionClass.Init(UiController controller, object binding, DefinitionFile definition)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(QuickSeparatorLine));
            _color = DefinitionResolver.GetColorWrapper(controller, binding, file["Color"]);

            _left = DefinitionResolver.Get<Length>(controller, binding, file["Left"], Length.Zero);
            _right = DefinitionResolver.Get<Length>(controller, binding, file["Right"], Length.Zero);

            return true;
        }

        public override void Draw(AdvancedDrawBatch drawBatch, Rectangle target, float opacity)
        {
            if(_lastWidth != target.Width)
            {
                _leftMargin = _left.Compute(target.Width);
                _rightMargin = _right.Compute(target.Width);

                _lastWidth = target.Width;
            }

            target.X += _leftMargin;
            target.Width -= _leftMargin + _rightMargin;

            drawBatch.DrawRectangle(target, _color.Value * opacity);
        }

    }
}
