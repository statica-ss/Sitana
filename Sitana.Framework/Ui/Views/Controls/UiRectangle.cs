using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views
{
    public class UiRectangle: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Image"] = parser.ParseResource<NinePatchImage>("Image");
            file["ScaleByUnit"] = parser.ParseBoolean("ScaleByUnit");
            file["Scale"] = parser.ParseFloat("Scale");
            file["Color"] = parser.ParseColor("Color");
        }

        protected NinePatchImage _image = null;

        protected bool _scaleByUnit = false;
        protected float _scale = 1;

        protected ColorWrapper _color = null;

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiRectangle));

            _image = DefinitionResolver.Get<NinePatchImage>(Controller, Binding, file["Image"], null);
            _scaleByUnit = DefinitionResolver.Get<bool>(Controller, Binding, file["ScaleByUnit"], false);
            _scale = (float)DefinitionResolver.Get<double>(Controller, Binding, file["Scale"], 1);
            _color = DefinitionResolver.GetColorWrapper(Controller, Binding, file["Color"]);
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = DisplayOpacity * parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            Color color = _color != null ? _color.Value : Color.White;

            float scale = _scaleByUnit ? (float)UiUnit.Unit : 1;

            parameters.DrawBatch.DrawNinePatchRect(_image, ScreenBounds, color * opacity, scale * _scale);
        }
    }
}
