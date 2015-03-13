using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Xml;
using System;

namespace Sitana.Framework.Ui.Views
{
    public class UiSeparatorLine: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Scale"] = parser.ParseDouble("Scale");
            file["Mode"] = parser.ParseEnum<Mode>("Mode");
            file["Image"] = parser.ParseResource<Texture2D>("Image");
            file["ScaleByUnit"] = parser.ParseBoolean("ScaleByUnit");
            file["Color"] = parser.ParseColor("Color");
        }

        enum Mode
        {
            Horizontal,
            Vertical
        }

        SharedValue<Texture2D> _image = null;
        float _scale;
        bool _scaleByUnit = false;
        ColorWrapper _color = null;
        bool _vertical;

        float Scale
        {
            get
            {
                return _scale * (_scaleByUnit ? (float)UiUnit.Unit : 1);
            }
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiSeparatorLine));

            _image = DefinitionResolver.GetShared<Texture2D>(Controller, Binding, file["Image"], null);
            _color = DefinitionResolver.GetColorWrapper(Controller, Binding, file["Color"]) ?? new ColorWrapper();
            _scaleByUnit = DefinitionResolver.Get<bool>(Controller, Binding, file["ScaleByUnit"], true);
            _scale = (float)DefinitionResolver.Get<double>(Controller, Binding, file["Scale"], 1);
            _vertical = DefinitionResolver.Get<Mode>(Controller, Binding, file["Mode"], Mode.Horizontal) == Mode.Vertical;

            return true;
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            Texture2D image = _image.Value;
            float scale = Scale;

            Vector2 textureCoord = Vector2.One;

            Vector2 size = Vector2.Zero;

            Color color = _color.Value * parameters.Opacity;

            if(_vertical)
            {
                size.X = (float)Math.Ceiling(scale * image.Width);
                size.Y = scale * image.Height;

                textureCoord.Y = Bounds.Height / size.Y;
                size.Y = Bounds.Height;
            }
            else
            {
                size.Y = (float)Math.Ceiling(scale * image.Height);
                size.X = scale * image.Width;

                textureCoord.X = Bounds.Width / size.X;
                size.X = Bounds.Width;
            }

            Vector2 p1 = new Vector2(ScreenBounds.X, (int)(ScreenBounds.Center.Y - size.Y / 2));
            Vector2 p2 = new Vector2(ScreenBounds.Right, p1.Y + size.Y);

            parameters.DrawBatch.BeginPrimitive(PrimitiveType.TriangleStrip, image);
            SamplerState oldState = parameters.DrawBatch.SamplerState;

            parameters.DrawBatch.SamplerState = SamplerState.LinearWrap;

            parameters.DrawBatch.PushVertex(new Vector2(p1.X, p1.Y), color, new Vector2(0,0));
            parameters.DrawBatch.PushVertex(new Vector2(p2.X, p1.Y), color, new Vector2(textureCoord.X, 0));
            parameters.DrawBatch.PushVertex(new Vector2(p1.X, p2.Y), color, new Vector2(0, textureCoord.Y));
            parameters.DrawBatch.PushVertex(new Vector2(p2.X, p2.Y), color, new Vector2(textureCoord.X, textureCoord.Y));

            parameters.DrawBatch.Flush();

            parameters.DrawBatch.SamplerState = oldState;
        }

    }
}
