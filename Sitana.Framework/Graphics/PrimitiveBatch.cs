// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public class PrimitiveBatch : IDisposable
    {
        // this constant controls how large the vertices buffer is. Larger buffers will
        // require flushing less often, which can increase performance. However, having
        // buffer that is unnecessarily large will waste memory.
        const int DefaultBufferSize = 500;

        public PrimitiveType PrimitiveType
        {
            get
            {
                if (!_hasBegun)
                {
                    throw new Exception("PrimitiveBatch not started.");
                }

                return _primitiveType;
            }
        }

        // A block of vertices that calling AddVertex will fill. Flush will draw using
        // this array, and will determine how many primitives to draw from
        // positionInBuffer.
        VertexPositionColor[] _vertices = new VertexPositionColor[DefaultBufferSize];
        VertexPositionColorTexture[] _verticesTexture = new VertexPositionColorTexture[DefaultBufferSize];

        // Keeps track of how many vertices have been added. this value increases until
        // we run out of space in the buffer, at which time Flush is automatically
        // called.
        int _positionInBuffer = 0;

        // A basic effect, which contains the shaders that we will use to draw our
        // primitives.
        BasicEffect _basicEffect;

        // The device that we will issue draw calls to.
        GraphicsDevice _device;

        // This value is set by Begin, and is the type of primitives that we are
        // drawing.
        PrimitiveType _primitiveType;

        // How many verts does each of these primitives take up? points are 1,
        // lines are 2, and triangles are 3.
        // Strips are always 1.
        int _numVertsPerPrimitive;

        // How many additional vertices is needed to draw primitive. For line strip it's 1,
        // for triangle strip it's 2.
        int _numRestVertsPerPrimitive;

        // HasBegun is flipped to true once Begin is called, and is used to make
        // sure users don't call End before Begin is called.
        Boolean _hasBegun = false;

        // Determines if object was already disposed.
        Boolean _isDisposed = false;

        // Old rasterizer state stored when Begin is called and restored when End is called.
        RasterizerState _oldRasierizerState;

        SamplerState _oldSamplerState;

        public Matrix Transform { get; set; }

        public BasicEffect BasicEffect { get { return _basicEffect; } }

        // the constructor creates a new PrimitiveBatch and sets up all of the internals
        // that PrimitiveBatch will need.
        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }

            _device = graphicsDevice;

            // set up a new basic effect, and enable vertex colors.
            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.VertexColorEnabled = true;

            Transform = Matrix.Identity;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                if (_basicEffect != null)
                    _basicEffect.Dispose();

                _isDisposed = true;
            }
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType)
        {
            Begin(primitiveType, RasterizerState.CullCounterClockwise, SamplerState.LinearClamp, null);
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType, Texture2D texture)
        {
            Begin(primitiveType, RasterizerState.CullCounterClockwise, SamplerState.LinearClamp, texture);
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType, Texture2D texture, SamplerState samplerState)
        {
            Begin(primitiveType, RasterizerState.CullCounterClockwise, samplerState, texture);
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType, RasterizerState rasterizerState)
        {
            Begin(primitiveType, rasterizerState, SamplerState.LinearClamp, null);
        }

        // Begin is called to tell the PrimitiveBatch what kind of primitives will be
        // drawn, and to prepare the graphics card to render those primitives.
        public void Begin(PrimitiveType primitiveType, RasterizerState rasterizerState, SamplerState samplerState, Texture2D texture)
        {
            if (_hasBegun)
            {
                throw new InvalidOperationException
                    ("End must be called before Begin can be called again.");
            }

            // projection uses CreateOrthographicOffCenter to create 2d projection
            // matrix with 0,0 in the upper left.
            _basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                (0, _device.Viewport.Width, _device.Viewport.Height, 0, 0, 1);

            _basicEffect.World = Transform;
            _basicEffect.View = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);

            // Store rasterizer state.
            _oldRasierizerState = _device.RasterizerState;
            _oldSamplerState = _device.SamplerStates[0];

            // Set new rasterizer state.
            _device.RasterizerState = rasterizerState;
            _device.SamplerStates[0] = samplerState;

            _device.BlendState = BlendState.AlphaBlend;

            _primitiveType = primitiveType;

            // How many verts will each of these primitives require?
            SetNumVertsPerPrimitive(primitiveType);

            _basicEffect.TextureEnabled = texture != null;
            _basicEffect.Texture = texture;

            // Tell our basic effect to begin.
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            // Set offset to beginning.
            _positionInBuffer = 0;

            // flip the error checking boolean. It's now ok to call AddVertex, Flush,
            // and End.
            _hasBegun = true;
        }

        // AddVertex is called to add another vertex to be rendered. To draw a point,
        // AddVertex must be called once. for lines, twice, and for triangles 3 times.
        // this function can only be called once begin has been called.
        // if there is not enough room in the vertices buffer, Flush is called
        // automatically.
        public void AddVertex(Vector2 vertex, Color color)
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before AddVertex can be called.");
            }

            if (_basicEffect.TextureEnabled)
            {
                throw new InvalidOperationException
                    ("Texture enabled. Use AddVertex with texture coordinates");
            }

            // are we starting a new primitive? if so, and there will not be enough room
            // for a whole primitive, flush.
            Boolean newPrimitive = false;

            if (_numRestVertsPerPrimitive != 0)
            {
                newPrimitive = (_positionInBuffer >= _numRestVertsPerPrimitive + _numVertsPerPrimitive);
            }
            else
            {
                newPrimitive = ((_positionInBuffer % _numVertsPerPrimitive) == 0);
            }

            if (newPrimitive && (_positionInBuffer + _numVertsPerPrimitive) >= _vertices.Length)
            {
                Flush();
            }

            // once we know there's enough room, set the vertex in the buffer,
            // and increase position.
            _vertices[_positionInBuffer].Position = new Vector3(vertex, 0);
            _vertices[_positionInBuffer].Color = color;

            _positionInBuffer++;
        }

        // AddVertex is called to add another vertex to be rendered. To draw a point,
        // AddVertex must be called once. for lines, twice, and for triangles 3 times.
        // this function can only be called once begin has been called.
        // if there is not enough room in the vertices buffer, Flush is called
        // automatically.
        public void AddVertex(Vector2 vertex, Color color, Vector2 texCoords)
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before AddVertex can be called.");
            }

            if (!_basicEffect.TextureEnabled)
            {
                throw new InvalidOperationException
                    ("Texture is not enabled. Use AddVertex without texture coordinates");
            }

            // are we starting a new primitive? if so, and there will not be enough room
            // for a whole primitive, flush.
            Boolean newPrimitive = false;

            if (_numRestVertsPerPrimitive != 0)
            {
                newPrimitive = (_positionInBuffer >= _numRestVertsPerPrimitive + _numVertsPerPrimitive);
            }
            else
            {
                newPrimitive = ((_positionInBuffer % _numVertsPerPrimitive) == 0);
            }

            if (newPrimitive && (_positionInBuffer + _numVertsPerPrimitive) >= _vertices.Length)
            {
                Flush();
            }

            // once we know there's enough room, set the vertex in the buffer,
            // and increase position.
            _verticesTexture[_positionInBuffer].Position = new Vector3(vertex, 0);
            _verticesTexture[_positionInBuffer].Color = color;
            _verticesTexture[_positionInBuffer].TextureCoordinate = texCoords;

            _positionInBuffer++;
        }

        public void AddVertex(float x, float y, ref Color color, float tx, float ty)
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before AddVertex can be called.");
            }

            if (!_basicEffect.TextureEnabled)
            {
                throw new InvalidOperationException
                    ("Texture is not enabled. Use AddVertex without texture coordinates");
            }

            // are we starting a new primitive? if so, and there will not be enough room
            // for a whole primitive, flush.
            Boolean newPrimitive = false;

            if (_numRestVertsPerPrimitive != 0)
            {
                newPrimitive = (_positionInBuffer >= _numRestVertsPerPrimitive + _numVertsPerPrimitive);
            }
            else
            {
                newPrimitive = ((_positionInBuffer % _numVertsPerPrimitive) == 0);
            }

            if (newPrimitive && (_positionInBuffer + _numVertsPerPrimitive) >= _vertices.Length)
            {
                Flush();
            }

            // once we know there's enough room, set the vertex in the buffer,
            // and increase position.
            _verticesTexture[_positionInBuffer].Position = new Vector3(x, y, 0);
            _verticesTexture[_positionInBuffer].Color = color;
            _verticesTexture[_positionInBuffer].TextureCoordinate = new Vector2(tx,ty);

            _positionInBuffer++;
        }

        // End is called once all the primitives have been drawn using AddVertex.
        // it will call Flush to actually submit the draw call to the graphics card, and
        // then tell the basic effect to end.
        public void End()
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before End can be called.");
            }

            // Draw whatever the user wanted us to draw
            Flush();

            // Restore old rasterizer state;
            _device.RasterizerState = _oldRasierizerState;
            _device.SamplerStates[0] = _oldSamplerState;
            _hasBegun = false;
        }

        // Flush is called to issue the draw call to the graphics card. Once the draw
        // call is made, positionInBuffer is reset, so that AddVertex can start over
        // at the beginning. End will call this to draw the primitives that the user
        // requested, and AddVertex will call this if there is not enough room in the
        // buffer.
        private void Flush()
        {
            if (!_hasBegun)
            {
                throw new InvalidOperationException
                    ("Begin must be called before Flush can be called.");
            }

            // no work to do
            if (_positionInBuffer == 0)
            {
                return;
            }

            // how many primitives will we draw?
            int primitiveCount = _positionInBuffer / _numVertsPerPrimitive - _numRestVertsPerPrimitive;

            if (primitiveCount <= 0)
            {
                throw new InvalidOperationException("Not enough vertices for " + _primitiveType.ToString() + ".");
            }

            if (_basicEffect.TextureEnabled)
            {
                // submit the draw call to the graphics card
                _device.DrawUserPrimitives<VertexPositionColorTexture>(_primitiveType, _verticesTexture, 0, primitiveCount);

                // Copy rest of vertices for next primitives.
                for (int idx = 0; idx < _numRestVertsPerPrimitive; ++idx)
                {
                    _verticesTexture[idx] = _verticesTexture[_positionInBuffer - _numRestVertsPerPrimitive + idx];
                }
            }
            else
            {
                // submit the draw call to the graphics card
                _device.DrawUserPrimitives<VertexPositionColor>(_primitiveType, _vertices, 0, primitiveCount);

                // Copy rest of vertices for next primitives.
                for (int idx = 0; idx < _numRestVertsPerPrimitive; ++idx)
                {
                    _vertices[idx] = _vertices[_positionInBuffer - _numRestVertsPerPrimitive + idx];
                }
            }

            // now that we've drawn, it's ok to reset positionInBuffer back to zero,
            // and write over any vertices that may have been set previously.
            _positionInBuffer = _numRestVertsPerPrimitive;
        }

        // NumVertsPerPrimitive is a boring helper function that tells how many vertices
        // it will take to draw each kind of primitive.
        private void SetNumVertsPerPrimitive(PrimitiveType primitive)
        {
            switch (primitive)
            {
                case PrimitiveType.LineList:
                    _numVertsPerPrimitive = 2;
                    _numRestVertsPerPrimitive = 0;
                    break;

                case PrimitiveType.TriangleList:
                    _numVertsPerPrimitive = 3;
                    _numRestVertsPerPrimitive = 0;
                    break;

                case PrimitiveType.LineStrip:
                    _numVertsPerPrimitive = 1;
                    _numRestVertsPerPrimitive = 1;
                    break;

                case PrimitiveType.TriangleStrip:
                    _numVertsPerPrimitive = 1;
                    _numRestVertsPerPrimitive = 2;
                    break;
            }
        }
    }
}
