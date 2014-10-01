using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using Sitana.Framework.Content;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework;
using System.Globalization;

namespace Editor
{
    class EditControl: WinFormsGraphicsDevice.GraphicsDeviceControl
    {
        static readonly Color MarkColor = Color.Yellow;

        PrimitiveBatch _primitiveBatch;
        SpriteBatch _spriteBatch;
        //IFontPresenter _fontPresenter;
        Vector2? _lastPosition = null;

        DateTime? _timeToFix = null;

        /// <summary>
        /// Derived classes override this to initialize their drawing code.
        /// </summary>
        protected override void Initialize()
        {
            EditView.Instance.Operation = new OperationSelection();

            _primitiveBatch = new PrimitiveBatch(GraphicsDevice);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var names = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();

            //using (Stream textureStream = thisExe.GetManifestResourceStream("Editor.Resources.UiFont.png"))
            //{
            //    using (Stream infoStream = thisExe.GetManifestResourceStream("Editor.Resources.UiFont.ff0"))
            //    {
            //        _fontPresenter = new BitmapFontPresenter(new BitmapFont(GraphicsDevice, textureStream, infoStream), null);
            //    }
            //}

            MouseDown += new MouseEventHandler(EditControl_MouseDown);
            MouseUp += new MouseEventHandler(EditControl_MouseUp);
            MouseMove += new MouseEventHandler(EditControl_MouseMove);
            MouseLeave += new EventHandler(EditControl_MouseLeave);
            MouseWheel += new MouseEventHandler(EditControl_MouseWheel);
            MouseDoubleClick += new MouseEventHandler(EditControl_MouseDoubleClick);

            Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            if (_timeToFix != null)
            {
                if (DateTime.Now > _timeToFix.Value)
                {
                    (OperationPan.Pan as OperationPan).FixMove();
                    _timeToFix = null;
                }
            }
        }

        void EditControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (EditView.Instance.Operation.MouseDblClick(e, Height))
            {
                Invalidate();
            }
        }

        void EditControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (EditView.Instance.Operation.MouseWheel(e, Height))
            {
                Invalidate();
                _timeToFix = DateTime.Now.AddSeconds(0.25);
            }
        }

        void EditControl_MouseLeave(object sender, EventArgs e)
        {
            _lastPosition = null;

            OperationPan.Pan.MouseLeave(Height);

            if (EditView.Instance.Operation.MouseLeave(Height))
            {
                Invalidate();
            }
        }

        void EditControl_MouseMove(object sender, MouseEventArgs e)
        {
            _lastPosition = new Vector2(e.X, e.Y);

            if (e.Button.HasFlag(MouseButtons.Right))
            {
                if (OperationPan.Pan.MouseMove(e, Height))
                {
                    Invalidate();
                }
            }
            else if (EditView.Instance.Operation.MouseMove(e, Height) || EditView.Instance.Operation.Mark)
            {
                Invalidate();
            }
        }

        void EditControl_MouseUp(object sender, MouseEventArgs e)
        {
            _lastPosition = new Vector2(e.X, e.Y);

            if (e.Button.HasFlag(MouseButtons.Right))
            {
                if (OperationPan.Pan.MouseUp(e, Height))
                {
                    Invalidate();
                }
            }
            else if (EditView.Instance.Operation.MouseUp(e, Height))
            {
                Invalidate();
            }
        }

        void EditControl_MouseDown(object sender, MouseEventArgs e)
        {
            _lastPosition = new Vector2(e.X, e.Y);
            Focus();

            if (e.Button.HasFlag(MouseButtons.Right))
            {
                if (OperationPan.Pan.MouseDown(e, Height))
                {
                    Invalidate();
                }
            }
            else if (EditView.Instance.Operation.MouseDown(e, Height))
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Derived classes override this to draw themselves using the GraphicsDevice.
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(new Color(120, 136, 153));

            DrawGrid();
            DrawElements();
            DrawOperation();
        }

        void DrawElements()
        {
            foreach (var el in Document.Instance.Scene)
            {
                EditView.Instance.DrawElement(_primitiveBatch, Height, el, el == EditView.Instance.Selection);
            }
        }

        void DrawOperation()
        {
            EditView.Instance.Operation.Draw(_primitiveBatch, Height);

            if (EditView.Instance.Operation.Mark && _lastPosition.HasValue)
            {
                _primitiveBatch.Begin(Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList);

                Vector2 vert = EditView.Instance.PositionFromDisplay(_lastPosition.Value, Height);
                vert = EditView.Instance.DisplayFromPosition(vert, Height);

                _primitiveBatch.AddVertex(vert + new Vector2(-4, -4), MarkColor);
                _primitiveBatch.AddVertex(vert + new Vector2(4, -4), MarkColor);

                _primitiveBatch.AddVertex(vert + new Vector2(4, -4), MarkColor);
                _primitiveBatch.AddVertex(vert + new Vector2(4, 4), MarkColor);

                _primitiveBatch.AddVertex(vert + new Vector2(4, 4), MarkColor);
                _primitiveBatch.AddVertex(vert + new Vector2(-4, 4), MarkColor);

                _primitiveBatch.AddVertex(vert + new Vector2(-4, 4), MarkColor);
                _primitiveBatch.AddVertex(vert + new Vector2(-4, -4), MarkColor);
                _primitiveBatch.End();
            }

            _spriteBatch.Begin();

            if (_lastPosition.HasValue && EditView.Instance.Operation.Mark)
            {
                Vector2 vert = EditView.Instance.PositionFromDisplay(_lastPosition.Value, Height);
                //_fontPresenter.DrawText(_spriteBatch, String.Format(CultureInfo.InvariantCulture, "{0},{1}", vert.X, vert.Y), new Vector2(20, Height - 24), Color.White, 1, Align.Bottom | Align.Left);
            }

            _spriteBatch.End();
        }

        private void DrawGrid()
        {
            float multiplier = EditView.Instance.GridMultiplier;
            Vector2 pos = EditView.Instance.TopLeft;

            pos = (pos / multiplier).TrimToIntValues() * multiplier;
            pos -= new Vector2(multiplier*4, multiplier*4);

            float gridSize = multiplier * EditView.Instance.DisplayUnit;

            _primitiveBatch.Begin(Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList);

            Vector2 origPos = EditView.Instance.DisplayFromPosition(pos, Height);

            pos.X = origPos.X;

            while(true)
            {
                pos.Y = origPos.Y;
                while(true)
                {
                    _primitiveBatch.AddVertex(pos, Color.White);
                    _primitiveBatch.AddVertex(pos + new Vector2(1, 0), Color.White);
                    pos.Y -= gridSize;

                    if (pos.Y < 0 )
                    {
                        break;
                    }
                }

                pos.X += gridSize;

                if (pos.X > Width)
                {
                    break;
                }
            }

            _primitiveBatch.End();
        }
    }
}
