using System;
using Ebatianos.Content;
using Ebatianos.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ebatianos.Gui
{
    public class BackgroundFill: GuiElement
    {
        private Vector2 _offset;
        private Texture2D _texture;

        private Vector2 _speed = Vector2.Zero;

        private PrimitiveBatch _primitiveBatch;

        protected override Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;

            if (!base.Initialize(initParams))
            {
                return false;
            }

            UsesSpriteBatch = false;

            String texture = parameters.AsString("Texture");

             _texture = ContentLoader.Current.Load<Texture2D>(texture);

            _offset = GraphicsHelper.Vector2FromPoint(parameters.ParsePoint("Offset")) / new Vector2(_texture.Width, _texture.Height);
            _speed = GraphicsHelper.Vector2FromPoint(parameters.ParsePoint("Speed")) / new Vector2(_texture.Width, _texture.Height);

            ElementRectangle = new Rectangle((Int32)initParams.Offset.X, (Int32)initParams.Offset.Y, (Int32)initParams.AreaSize.X, (Int32)initParams.AreaSize.Y);

            _primitiveBatch = new PrimitiveBatch(Owner.ScreenManager.SpriteBatch.GraphicsDevice);

            return true;
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            base.Update(gameTime, screenState);

            _offset += _speed * (Single)gameTime.TotalSeconds;

            while (_offset.X < 0) _offset.X += 1;
            while (_offset.X > 1) _offset.X -= 1;
            while (_offset.Y < 0) _offset.Y += 1;
            while (_offset.Y > 1) _offset.Y -= 1;

            return true;
        }

        public override void OnAdded()
        {
            base.OnAdded();

            if (SecondInstance != null)
            {
                BackgroundFill first = FirstInstance as BackgroundFill;

                if (first != null)
                {
                    if (first._speed != Vector2.Zero)
                    {
                        _offset = first._offset;

                        while (_offset.X < 0) _offset.X += 1;
                        while (_offset.X > 1) _offset.X -= 1;
                        while (_offset.Y < 0) _offset.Y += 1;
                        while (_offset.Y > 1) _offset.Y -= 1;
                    }
                }
            }
        }

        /// <summary>
        /// Draws label.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch object used to render textures and texts.</param>
        /// <param name="color">Color to multiply all contents by.</param>
        /// <param name="offset">Offset to move bnutton by.</param>
        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            if (Opacity <= 0)
            {
                return;
            }

            Vector2 offset = _offset;

            if (SecondInstance != null)
            {
                if (this == FirstInstance)
                {
                    offset = ((BackgroundFill)SecondInstance)._offset * (1 - transition) + _offset * transition;   
                }
                else
                {
                    return;
                }
            }

            Single width = (Single)ElementRectangle.Width / ((Single)_texture.Width * Scale.X);
            Single height = (Single)ElementRectangle.Height / ((Single)_texture.Height * Scale.Y);

            Single x0 = offset.X;
            Single x1 = offset.X + width;

            Single y0 = offset.Y;
            Single y1 = offset.Y + height;


            _primitiveBatch.Begin(PrimitiveType.TriangleStrip, RasterizerState.CullNone, SamplerState.LinearWrap, _texture);

            _primitiveBatch.AddVertex(new Vector2(ElementRectangle.Left, ElementRectangle.Top), Color.White, new Vector2(x0,y0));
            _primitiveBatch.AddVertex(new Vector2(ElementRectangle.Left, ElementRectangle.Bottom), Color.White, new Vector2(x0, y1));
            _primitiveBatch.AddVertex(new Vector2(ElementRectangle.Right, ElementRectangle.Top), Color.White, new Vector2(x1, y0));
            _primitiveBatch.AddVertex(new Vector2(ElementRectangle.Right, ElementRectangle.Bottom), Color.White, new Vector2(x1, y1));

            _primitiveBatch.End();
        }
    }
}
