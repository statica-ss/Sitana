using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinFormsGraphicsDevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Windows.Forms;
using Sitana.Framework.Graphics.Model;
using Sitana.Framework.Graphics;

namespace ModelConverter
{
    class PreviewControl : GraphicsDeviceControl
    {
        BasicEffect _effect = null;
        Stopwatch _timer;

        ModelX _model;

        VertexPositionColor[] _axes = new VertexPositionColor[6];

        float _horz = 0;
        float _vert = 0;

        float _horzSpeed = 0;
        float _vertSpeed = 0;

        Boolean _invalidate = false;

        System.Drawing.Point? _lastPoint;

        float _zoom = 1;

        Double _lastTimeMouse = 0;
        Double _lastTime = 0;

        Int32 _materialSet = 0;

        Boolean _zisUp = false;

        public Boolean ZisUp
        {
            set
            {
                _zisUp = value;
                _invalidate = true;
            }
        }

        public ModelX Model
        {
            set
            {
                _model = value;
                _model.PrepareForRender(GraphicsDevice);
                _invalidate = true;

                
            }
        }

        public Int32 MaterialSet
        {
            set
            {
                _materialSet = value;
                _invalidate = true;
            }
        }

        /// <summary>
        /// 
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            //// Create our effect.
            _effect = new BasicEffect(GraphicsDevice);


            //// Start the animation timer.
            _timer = Stopwatch.StartNew();

            _axes[0] = new VertexPositionColor(Vector3.Zero, Color.Green);
            _axes[1] = new VertexPositionColor(new Vector3(1,0,0), Color.Green);

            _axes[2] = new VertexPositionColor(Vector3.Zero, Color.Blue);
            _axes[3] = new VertexPositionColor(new Vector3(0, 0, 1), Color.Blue);

            _axes[4] = new VertexPositionColor(Vector3.Zero, Color.Red);
            _axes[5] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);

            //// Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { ComputeDraw(); };

            

            MouseDown += new MouseEventHandler(PreviewControl_MouseDown);
            MouseUp += new MouseEventHandler(PreviewControl_MouseUp);
            MouseMove += new MouseEventHandler(PreviewControl_MouseMove);
        }

        void PreviewControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_lastTimeMouse > 0)
            {
                Double time = _timer.Elapsed.TotalSeconds;
                Single ellapsed = (Single)(time - _lastTimeMouse);

                if (ellapsed > 0.1f)
                {
                    _horzSpeed = 0;
                    _vertSpeed = 0;
                }
            }
            
            _lastPoint = null;
        }

        void PreviewControl_MouseDown(object sender, MouseEventArgs e)
        {
            Focus();
            var point = PointToClient(MousePosition);
            _lastPoint = point;
            _lastTimeMouse = _timer.Elapsed.TotalSeconds;
        }

        void ComputeSpeed(System.Drawing.Point point)
        {
            if (_lastPoint.HasValue)
            {
                _horzSpeed = (Single)(point.X - _lastPoint.Value.X) / (Single)Width * (Single)Math.PI * 2;
                _vertSpeed = (Single)(point.Y - _lastPoint.Value.Y) / (Single)Width * (Single)Math.PI * 2;
            }
        }

        void PreviewControl_MouseMove(object sender, MouseEventArgs e)
        {
            var point = PointToClient(MousePosition);

            Double time = _timer.Elapsed.TotalSeconds;

            if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left) && _lastPoint.HasValue)
            {
                Double oldHorz = _horz;
                Double oldVert = _vert;

                ComputeSpeed(point);

                _horz += _horzSpeed;
                _vert += _vertSpeed;

                if (_lastTimeMouse > 0)
                {
                    Single ellapsed = (Single)(time - _lastTimeMouse);

                    _horzSpeed /= ellapsed;
                    _vertSpeed /= ellapsed;
                }
                else
                {
                    _horzSpeed = 0;
                    _vertSpeed = 0;
                }

                _lastPoint = point;

                if (oldHorz != _horz || oldVert != _vert)
                {
                    _invalidate = true;
                }
            }
            else if (e.Button.HasFlag(System.Windows.Forms.MouseButtons.Right) && _lastPoint.HasValue)
            {
                Single zoom = _zoom;

                _zoom += _zoom * (Single)(point.Y - _lastPoint.Value.Y) / (Single)Width;
                _zoom = Math.Min(1, Math.Max(0.1f, _zoom));

                if (zoom != _zoom)
                {
                    _invalidate = true;
                }
            }

            _lastTimeMouse = time;
        }


        private Matrix ComputeCameraRotation(Vector3 cameraDir, Single horz, Single vert)
        {
            Matrix matrix = _zisUp ? Matrix.CreateRotationZ(horz) : Matrix.CreateRotationY(horz);

            cameraDir = Vector3.Transform(cameraDir, matrix);

            Single x = cameraDir.X;

            if (_zisUp)
            {
                cameraDir.X = -cameraDir.Y;
                cameraDir.Y = -x;
            }
            else
            {
                cameraDir.X = cameraDir.Z;
                cameraDir.Z = x;
            }

            matrix = Matrix.Multiply(Matrix.CreateFromAxisAngle(cameraDir, -vert), matrix);

            return matrix;
        }

        private void ComputeDraw()
        {
            if (_model == null || _effect == null)
            {
                return;
            }

            Double oldHorz = _horz;
            Double oldVert = _vert;

            //// Spin the triangle according to how much time has passed.
            Double time = _timer.Elapsed.TotalSeconds;

            if (!_lastPoint.HasValue && _lastTime > 0)
            {
                Single ellapsed = (Single)(time - _lastTime);

                Single signHorz = Math.Sign(_horzSpeed);
                Single signVert = Math.Sign(_vertSpeed);

                Single valHorz = Math.Abs(_horzSpeed);
                Single valVert = Math.Abs(_vertSpeed);

                valHorz = Math.Min(10, valHorz);
                valHorz -= ellapsed * valHorz * 2;

                if (valHorz < 0.1)
                {
                    valHorz = 0;
                }

                valVert = Math.Min(10, valVert);
                valVert -= ellapsed * valVert * 2;

                if (valVert < 0.1)
                {
                    valVert = 0;
                }

                _horzSpeed = valHorz * signHorz;
                _vertSpeed = valVert * signVert;

                _horz += _horzSpeed * ellapsed;
                _vert += _vertSpeed * ellapsed;
            }

            _lastTime = time;

            if (_invalidate || oldHorz != _horz || oldVert != _vert)
            {
                
                Invalidate();
                _invalidate = false;
            }
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if ( _model == null || _effect == null)
            {
                return;
            }

            Vector3 center = (_model.BoundA + _model.BoundB) / 2;
            Vector3 camera = center + new Vector3(1, 0, 0) * _model.Size.Length() * 1.5f * _zoom;

            Vector3 cameraDir = camera - center;
            cameraDir.Normalize();

            //// Set transform matrices.
            float aspect = GraphicsDevice.Viewport.AspectRatio;

            _effect.World = ComputeCameraRotation(cameraDir, _horz, _vert);
            
            _effect.View = Matrix.CreateLookAt(camera,
                center, _zisUp ? new Vector3(0,0,1): new Vector3(0,1,0));

            _effect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, (camera - center).Length() / 4, (camera-center).Length() * 3f / _zoom);
            
            //// Set renderstates.
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            GraphicsDevice.SetVertexBuffer(_model.VertexBuffer);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _effect.VertexColorEnabled = false;
            
            _effect.LightingEnabled = true;

            _effect.EnableDefaultLighting();
            _effect.PreferPerPixelLighting = true;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            Color ambientColor = new Color(32,24,48);

            Material[] materials = _model.Materials(_materialSet);

            for (Int32 steps = 0; steps < 2; ++steps)
            {
                for (Int32 idx = 0; idx < _model.Subsets.Length; ++idx)
                {
                    var subset = _model.Subsets[idx];

                    Material material = materials[subset.Material];

                    if (steps == 0)
                    {
                        if (material.Opacity < 1)
                        {
                            continue;
                        }
                    }

                    if (steps == 1)
                    {
                        if (material.Opacity == 1)
                        {
                            continue;
                        }
                    }

                    GraphicsDevice.Indices = subset.IndexBuffer;

                    //// Draw the triangle.
                    _effect.DiffuseColor = material.Diffuse;
                    _effect.AmbientLightColor = ambientColor.ToVector3() * material.Ambient;
                    _effect.SpecularColor = material.Specular;
                    _effect.EmissiveColor = material.Emissive;
                    _effect.Alpha = material.Opacity;

                    MaterialTextures textures = material.Textures;

                    _effect.SpecularPower = material.SpecularExponent;
                    _effect.Texture = textures != null ? textures.Diffuse : null;
                    _effect.TextureEnabled = _effect.Texture != null;

                    _effect.CurrentTechnique.Passes[0].Apply();

                    Int32 startIndex = 0;

                    while (startIndex < subset.Indices.Length)
                    {
                        Int32 primitiveCount = Math.Min((subset.Indices.Length - startIndex) / 3, 30000);

                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _model.Vertices.Length, startIndex, primitiveCount);
                        startIndex += primitiveCount * 3;
                    }
                }
            }

            _effect.TextureEnabled = false;
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;

            Single length = _model.Size.Length() * 1.5f;

            _effect.World = _effect.World * Matrix.CreateScale(length);

            _effect.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, _axes, 0, 3);
        }
    }
}
