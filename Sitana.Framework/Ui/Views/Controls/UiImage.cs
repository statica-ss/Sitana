﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Ui.Views
{
    public class UiImage : UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);

            file["Image"] = parser.ParseResource<Texture2D>("Image");
            file["Stretch"] = parser.ParseEnum<Stretch>("Stretch");
            file["Color"] = parser.ParseColor("Color");
            file["RotationSpeed"] = parser.ParseDouble("RotationSpeed");
            file["ScaleByUnit"] = parser.ParseBoolean("ScaleByUnit");
            file["Scale"] = parser.ParseScale("Scale");

			file["ResampleFilter"] = parser.ParseEnum<ResampleFilter>("ResampleFilter");
        }

		enum ResampleFilter
		{
			Default,
			Point,
			Linear,
			Anisotropic
		}

        SharedValue<Texture2D> _image = null;

        Stretch _stretch = Stretch.Uniform;
        ColorWrapper _color = null;
        float _rotationSpeed = 0;
        float _rotation = 0;
        bool _scaleByUnit = false;

        Scale _scale;

		SamplerState _samplerState = null;

        float ImageScale
        {
            get
            {
                return _scale.Value(_scaleByUnit);
            }
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            base.Draw(ref parameters);

            Rectangle target = ScreenBounds;
            Rectangle source = Rectangle.Empty;

            float scale = ImageScale;

            lock (_image)
            {
                Texture2D image = _image.Value;

                if (image != null)
                {
                    source = new Rectangle(0, 0, image.Width, image.Height);

                    double scaleX = scale;
                    double scaleY = scale;

                    switch (_stretch)
                    {
                        case Stretch.Uniform:
                            scaleX = scaleY = Math.Min(target.Width / (double)image.Width, target.Height / (double)image.Height) * _scale.Value(false);
                            scale = (float)scaleX;
                            break;

                        case Stretch.UniformToFill:
                            scaleX = scaleY = Math.Max(target.Width / (double)image.Width, target.Height / (double)image.Height) * _scale.Value(false);
                            scale = (float)scaleX;
                            break;

                        case Stretch.Fill:
                            scaleX = ImageScale * target.Width / (double)image.Width;
                            scaleY = ImageScale * target.Height / (double)image.Height;
                            scale = (float)Math.Min(scaleX, scaleY);
                            break;
                    }

                    Point pos = target.Center;

                    int width = (int)(Math.Ceiling(image.Width * scaleX));
                    int height = (int)(Math.Ceiling(image.Height * scaleY));

                    Rectangle targetRect = new Rectangle(pos.X - width / 2 - width % 2, pos.Y - height / 2 - height % 2, width, height);
                    target = GraphicsHelper.IntersectRectangle(targetRect, target);

                    int srcWidth = (int)((double)target.Width / scaleX);
                    int srcHeight = (int)((double)target.Height / scaleY);

                    pos = source.Center;
                    source = new Rectangle(pos.X - srcWidth / 2, pos.Y - srcHeight / 2, srcWidth, srcHeight);
                }

                Color color = _color != null ? _color.Value * opacity : Color.White;

				SamplerState oldState = parameters.DrawBatch.SamplerState;

				if (_samplerState != null)
				{
					parameters.DrawBatch.SamplerState = _samplerState;
				}

                if (_rotationSpeed == 0)
                {
                    parameters.DrawBatch.DrawImage(image, target, source, color);
                }
                else
                {
                    parameters.DrawBatch.DrawImage(image, target.Center.ToVector2(), null, color, _rotation, new Vector2(image.Width / 2, image.Height / 2), scale);
                }

				parameters.DrawBatch.SamplerState = oldState;
            }
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();

            _image.ValueChanged -= _image_ValueChanged;
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiImage));

            _image = DefinitionResolver.GetShared<Texture2D>(Controller, Binding, file["Image"], null);
            _stretch = DefinitionResolver.Get<Stretch>(Controller, Binding, file["Stretch"], Stretch.Uniform);
            _color = DefinitionResolver.GetColorWrapper(Controller, Binding, file["Color"]) ?? new ColorWrapper();
            _rotationSpeed = (float)DefinitionResolver.Get<double>(Controller, Binding, file["RotationSpeed"], 0);
            _scaleByUnit = DefinitionResolver.Get<bool>(Controller, Binding, file["ScaleByUnit"], true);
            _scale = DefinitionResolver.Get(Controller, Binding, file["Scale"], Scale.One);

            _image.ValueChanged += _image_ValueChanged;

			switch(DefinitionResolver.Get<ResampleFilter>(Controller, Binding, file["ResampleFilter"], ResampleFilter.Default))
			{
			case ResampleFilter.Point:
				_samplerState = SamplerState.PointClamp;
				break;

			case ResampleFilter.Linear:
				_samplerState = SamplerState.LinearClamp;
				break;

			case ResampleFilter.Anisotropic:
				_samplerState = SamplerState.AnisotropicClamp;
				break;

			}

            return true;
        }

        void _image_ValueChanged(Texture2D newValue)
        {
            UiTask.BeginInvoke(() =>
                {
                    SetForceRecalcFlag();
                    Parent.RecalcLayout();
                });
        }

        public override Point ComputeSize(int width, int height)
        {
            Point size = base.ComputeSize(width, height);

            Vector2 sizeInPixels = new Vector2(-1, -1);

            if (PositionParameters.Width.IsAuto)
            {
                sizeInPixels = CalculateSizeInPixels();
                size.X = (int)Math.Ceiling(sizeInPixels.X);
            }

            if (PositionParameters.Height.IsAuto)
            {
                if (sizeInPixels.Y < 0)
                {
                    sizeInPixels = CalculateSizeInPixels();
                }
                size.Y = (int)Math.Ceiling(sizeInPixels.Y);
            }

            return size;
        }

        private Vector2 CalculateSizeInPixels()
        {
            lock (_image)
            {
                if (_image.Value != null)
                {
                    Texture2D image = _image.Value;
                    return new Vector2(image.Width, image.Height) * ImageScale;
                }
            }
            return Vector2.Zero;
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if (_rotationSpeed != 0)
            {
                _rotation += MathHelper.TwoPi * time * _rotationSpeed;

                if (_rotation > MathHelper.TwoPi)
                {
                    _rotation -= MathHelper.TwoPi;
                }

                if (_rotation < 0)
                {
                    _rotation += MathHelper.TwoPi;
                }

                if(DisplayVisibility > 0)
                {
                    AppMain.Redraw(this);
                }
            }
        }
    }
}
