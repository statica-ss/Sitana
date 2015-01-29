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
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Ui.Views
{
    public class UiHorizontalGradient : UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["LeftColor"] = parser.ParseColor("LeftColor");
            file["RightColor"] = parser.ParseColor("RightColor");
        }

        protected ColorWrapper _leftColor = null;
        protected ColorWrapper _rightColor = null;

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiHorizontalGradient));

            _leftColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["LeftColor"]);
            _rightColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["RightColor"]);

            return true;
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            Color leftColor = _leftColor != null ? _leftColor.Value : Color.Transparent;
            Color rightColor = _rightColor != null ? _rightColor.Value : Color.Transparent;

            parameters.DrawBatch.BeginPrimitive(PrimitiveType.TriangleStrip, null);

            Rectangle bounds = ScreenBounds;

            parameters.DrawBatch.PushVertex(new Vector2(bounds.Left, bounds.Top), leftColor * opacity);
            parameters.DrawBatch.PushVertex(new Vector2(bounds.Left, bounds.Bottom), leftColor * opacity);

            parameters.DrawBatch.PushVertex(new Vector2(bounds.Right, bounds.Top), rightColor * opacity);
            parameters.DrawBatch.PushVertex(new Vector2(bounds.Right, bounds.Bottom), rightColor * opacity);
        }
    }
}
