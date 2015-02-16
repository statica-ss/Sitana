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
    public class UiVerticalGradient : UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["TopColor"] = parser.ParseColor("TopColor");
            file["BottomColor"] = parser.ParseColor("BottomColor");
        }

        protected ColorWrapper _topColor = null;
        protected ColorWrapper _bottomColor = null;

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiVerticalGradient));

            _topColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["TopColor"]);
            _bottomColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["BottomColor"]);

            return true;
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            Color topColor = _topColor != null ? _topColor.Value : Color.Transparent;
            Color bottomColor = _bottomColor != null ? _bottomColor.Value : Color.Transparent;

            parameters.DrawBatch.BeginPrimitive(PrimitiveType.TriangleStrip, null);
            
            Rectangle bounds = ScreenBounds;

            parameters.DrawBatch.PushVertex(new Vector2(bounds.Left, bounds.Top), topColor * opacity);
            parameters.DrawBatch.PushVertex(new Vector2(bounds.Left, bounds.Bottom), bottomColor * opacity);

            parameters.DrawBatch.PushVertex(new Vector2(bounds.Right, bounds.Top), topColor * opacity);
            parameters.DrawBatch.PushVertex(new Vector2(bounds.Right, bounds.Bottom), bottomColor * opacity);

            parameters.DrawBatch.Flush();
        }
    }
}
