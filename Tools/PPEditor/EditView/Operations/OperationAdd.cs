using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using System.Windows.Forms;
using Sitana.Framework.Games.Elements;

namespace Editor
{
    public abstract class OperationAdd: MouseOperation
    {
        public static Color DefaultColor = Color.Green;

        Vector2? _mouseDownPos = Vector2.Zero;
        protected PpElement _element;

        public override bool Mark { get { return true; } }

        protected int _index = 0;

        public OperationAdd()
        {
            (OperationPan.Pan as OperationPan).FixMove();
        }

        protected void Apply()
        {
            Document.Instance.Scene.Add(_element);
            _element.Cleanup();

            EditView.Instance.Operation = new OperationSelection();
            EditView.Instance.Selection = _element;
        }

        public override bool MouseDown(MouseEventArgs args, int height)
        {
            _mouseDownPos = EditView.Instance.PositionFromDisplay(new Vector2(args.X, args.Y), height);
            return false;
        }

        public override void Draw(PrimitiveBatch batch, int height)
        {
            if (_element != null)
            {
                EditView.Instance.DrawElement(batch, height, _element, true);
                EditView.Instance.DrawSelection(batch, height, _element, -1);
            }
        }

        public override bool MouseLeave(int height)
        {
            _mouseDownPos = null;
            return true;
        }

        public override bool MouseDblClick(MouseEventArgs args, int height)
        {
            if (_element != null && _element.CanAddVertices)
            {
                _mouseDownPos = null;
                if (_element.Valid)
                {
                    Apply();
                }

                return true;
            }

            return false;
        }

        public override bool MouseWheel(MouseEventArgs args, int height)
        {
            return OperationPan.Pan.MouseWheel(args, height);
        }

        public override bool MouseMove(MouseEventArgs args, int height)
        {
            Vector2 pos = EditView.Instance.PositionFromDisplay(new Vector2(args.X, args.Y), height);

            if (_mouseDownPos.HasValue && _mouseDownPos.Value != pos)
            {
                _mouseDownPos = null;
            }

            if (_element != null)
            {
                _element[_index] = pos;
            }

            return _element != null;
        }

        public override bool MouseUp(MouseEventArgs args, int height)
        {
            Vector2 pos = EditView.Instance.PositionFromDisplay(new Vector2(args.X, args.Y), height);

            if (_mouseDownPos.HasValue && _mouseDownPos.Value == pos)
            {
                if (_element != null)
                {
                    _element[_index] = pos;
                    ++_index;

                    if (!_element.CanAddVertices && _index >= _element.Count)
                    {
                        if (_element.Valid)
                        {
                            Apply();
                        }
                        else
                        {
                            _index--;
                            _index = Math.Min(_element.Count - 1, _index);
                        }
                    }
                }
                else
                {
                    _index = 2;

                    CreateElement(pos);
                    _element.Color = DefaultColor;

                    if (!_element.CanAddVertices)
                    {
                        _index = Math.Min(_element.Count - 1, _index);
                    }
                }
            }

            return true;
        }

        protected abstract void CreateElement(Vector2 pos);
    }
}
