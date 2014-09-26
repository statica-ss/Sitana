using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ebatianos.Graphics;
using Microsoft.Xna.Framework;
using Ebatianos.PP.Elements;

namespace Editor
{
    class OperationSelection: MouseOperation
    {
        Vector2? _mouseDownPos = null;
        Vector2 _lastMousePos = Vector2.Zero;

        int? _selectedIndex;

        public override MouseOperationType Type { get { return MouseOperationType.Selection; } }

        public override bool Mark { get { return false; } }

        public override bool MouseMove(MouseEventArgs args, int height)
        {
            Vector2 mousePos = new Vector2(args.X, args.Y);
            Vector2 pos = EditView.Instance.PositionFromDisplay(mousePos, height);

            if (_selectedIndex.HasValue)
            {
                PpElement element = EditView.Instance.Selection;

                if (element != null)
                {
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        element.Rotate(_selectedIndex.Value, (float)(mousePos.X - _lastMousePos.X)/64.0f);

                        _lastMousePos = mousePos;
                    }
                    else
                    {
                        element[_selectedIndex.Value] = pos;
                    }
                    return true;
                }
            }

            if (_mouseDownPos.HasValue && _mouseDownPos.Value != pos)
            {
                _mouseDownPos = null;
            }

            return false;
        }

        public override bool MouseUp(MouseEventArgs args, int height)
        {
            Vector2 pos = EditView.Instance.PositionFromDisplay(new Vector2(args.X, args.Y), height, false);

            if (_mouseDownPos.HasValue && _mouseDownPos.Value == pos)
            {
                if (_selectedIndex == null)
                {
                    PpElement element = Document.Instance.FindTop(pos);
                    EditView.Instance.Selection = element;
                }
            }

            _selectedIndex = null;
            return true;
        }

        public override bool MouseDown(MouseEventArgs args, int height)
        {
            _mouseDownPos = EditView.Instance.PositionFromDisplay(new Vector2(args.X, args.Y), height, false);
            _lastMousePos = new Vector2(args.X, args.Y);

            PpElement element = EditView.Instance.Selection;

            if (element != null)
            {
                float minDist = float.MaxValue;

                Vector2 cursorPos = new Vector2(args.X, args.Y);
                _selectedIndex = null;

                for (int idx = 0; idx < element.Count; ++idx)
                {
                    Vector2 vpos = EditView.Instance.DisplayFromPosition(element[idx], height);
                    float dist = (vpos - cursorPos).Length();

                    if (dist < 16)
                    {
                        if (dist < minDist)
                        {
                            minDist = dist;
                            _selectedIndex = idx;
                        }
                    }
                }
            }

            return true;
        }

        public override bool MouseWheel(MouseEventArgs args, int height)
        {
            return OperationPan.Pan.MouseWheel(args, height);
        }

        public override bool MouseLeave(int height)
        {
            _selectedIndex = null;
            return true;
        }

        public override void Draw(PrimitiveBatch batch, int height)
        {
            PpElement element = EditView.Instance.Selection;

            if ( element != null )
            {
                EditView.Instance.DrawSelection(batch, height, element, _selectedIndex ?? -1);
            }
        }

        public override bool KeyDown(Keys key, bool control, bool shift)
        {
            PpElement element = EditView.Instance.Selection;

            if (element != null)
            {
                if (_selectedIndex.HasValue && _selectedIndex.Value > 0)
                {
                    if (element.CanAddVertices)
                    {
                        return ProcessVerticesOperation(key);
                    }
                }

                switch (key)
                {
                    case Keys.Escape:
                        EditView.Instance.Selection = null;
                        return true;

                    case Keys.H:

                        if (EditView.Instance.Selection != null && !control)
                        {
                            EditView.Instance.Selection.FlipHorizontal();
                            return true;
                        }
                        break;

                    case Keys.V:

                        if (EditView.Instance.Selection != null && !control)
                        {
                            EditView.Instance.Selection.FlipVertical();
                            return true;
                        }
                        break;

                    case Keys.D:

                        if (EditView.Instance.Selection != null && control)
                        {
                            var clone = EditView.Instance.Selection.Clone();
                            Document.Instance.Scene.Add(clone);

                            EditView.Instance.Selection = clone;
                            return true;
                        }
                        break;

                    case Keys.C:

                        if (EditView.Instance.Selection != null && control && shift)
                        {
                            var clone = EditView.Instance.Selection.ToPolygon();
                            Document.Instance.Scene.Overwrite(EditView.Instance.Selection, clone);

                            EditView.Instance.Selection = clone;
                            return true;
                        }
                        break;

                    case Keys.Delete:

                        if (EditView.Instance.Selection != null && !control)
                        {
                            Document.Instance.Scene.Remove(EditView.Instance.Selection);
                            EditView.Instance.Selection = null;
                            return true;
                        }
                        break;

                    case Keys.PageUp:
                        if (EditView.Instance.Selection != null && !control)
                        {
                            Document.Instance.Scene.MoveToFront(EditView.Instance.Selection);
                            return true;
                        }
                        break;

                    case Keys.PageDown:
                        if (EditView.Instance.Selection != null && !control)
                        {
                            Document.Instance.Scene.MoveToBack(EditView.Instance.Selection);
                            return true;
                        }
                        break;

                    case Keys.Home:
                        if (EditView.Instance.Selection != null && !control)
                        {
                            Document.Instance.Scene.BringToFront(EditView.Instance.Selection);
                            return true;
                        }
                        break;

                    case Keys.End:
                        if (EditView.Instance.Selection != null && !control)
                        {
                            Document.Instance.Scene.SendToBack(EditView.Instance.Selection);
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        bool ProcessVerticesOperation(Keys key)
        {
            PpElement element = EditView.Instance.Selection;

            switch (key)
            {
                case Keys.Delete:

                    if (element.Count > 3)
                    {
                        element.Remove(_selectedIndex.Value);
                        return true;
                    }
                    break;

                case Keys.Insert:

                    element.Insert(_selectedIndex.Value, element[_selectedIndex.Value]);

                    return true;
            }

            return false;
        }
    }
}
