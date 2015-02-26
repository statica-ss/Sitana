using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;

namespace Sitana.Framework.Ui.Views.BackgroundDrawables
{
    public class NinePatchBackground : IBackgroundDrawable, IDefinitionClass
    {
        public static void Parse(XNode node, DefinitionFile file)
        {
            var parser = new DefinitionParser(node);

            file["Color"] = parser.ParseColor("Color");

            file["Image"] = parser.ParseResource<NinePatchImage>("Image");
            file["ScaleByUnit"] = parser.ParseBoolean("ScaleByUnit");
            file["Scale"] = parser.ParseDouble("Scale");

            file["Margin"] = parser.ParseMargin("Margin");
        }

        protected NinePatchImage _image = null;
        protected bool _scaleByUnit = false;
        protected float _scale = 1;
        protected ColorWrapper _color;
        protected Margin _margin;

        void IBackgroundDrawable.Draw(AdvancedDrawBatch drawBatch, Rectangle target, Color color)
        {
            float scale = _scaleByUnit ? (float)UiUnit.Unit : 1;

            color = GraphicsHelper.MultiplyColors(color, _color.Value);

            target = _margin.ComputeRect(target);
            drawBatch.DrawNinePatchRect(_image, target, color, scale*_scale);
        }

        bool IDefinitionClass.Init(UiController controller, object binding, DefinitionFile definition)
        {
            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(NinePatchBackground));

            _image = DefinitionResolver.Get<NinePatchImage>(controller, binding, file["Image"], null);
            _scaleByUnit = DefinitionResolver.Get<bool>(controller, binding, file["ScaleByUnit"], false);
            _scale = (float)DefinitionResolver.Get<double>(controller, binding, file["Scale"], 1);
            _color = DefinitionResolver.GetColorWrapper(controller, binding, file["Color"]) ?? new ColorWrapper();

            _margin = DefinitionResolver.Get<Margin>(controller, binding, file["Margin"], Margin.None);

            return true;
        }
    }
}
