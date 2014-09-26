using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Ebatianos;
using Ebatianos.Graphics;

namespace Editor
{
    class OperationPan: MouseOperation
    {
        public static MouseOperation Pan = new OperationPan();

        Vector2? _lastMouse;

        public override MouseOperationType Type { get { return MouseOperationType.None; } }

        public override bool Mark { get { return false; } }

        public override bool MouseMove(MouseEventArgs args, int height)
        {
            if (_lastMouse.HasValue)
            {
                Vector2 pos = new Vector2(args.X, args.Y);

                Vector2 offset = (_lastMouse.Value - pos) / (float)EditView.Instance.DisplayUnit;
                offset.Y = -offset.Y;

                EditView.Instance.TopLeft += offset;
                _lastMouse = pos;

                return true;
            }

            return false;
        }

        public override bool MouseUp(MouseEventArgs args, int height)
        {
            if (args.Button == MouseButtons.Right)
            {
                _lastMouse = null;
                FixMove();
            }
            return true;
        }

        public override bool MouseDown(MouseEventArgs args, int height)
        {
            if (args.Button == MouseButtons.Right)
            {
                _lastMouse = new Vector2(args.X, args.Y);
            }
            return true;
        }

        public override bool MouseWheel(MouseEventArgs args, int height)
        {
            Vector2 pos = new Vector2(args.X, args.Y);
            Vector2 oldPos = EditView.Instance.PositionFromDisplay(pos, height, false);

            int unit = EditView.Instance.DisplayUnit;
            
            unit += args.Delta;

            unit = Math.Min(EditView.MaxUnit, Math.Max(EditView.MinUnit, unit));

            EditView.Instance.DisplayUnit = unit;

            Vector2 newPos = EditView.Instance.PositionFromDisplay(pos, height, false);

            EditView.Instance.TopLeft += oldPos - newPos;
            return true;
        }

        public override bool MouseLeave(int height)
        {
            _lastMouse = null;
            FixMove();
            return true;
        }

        public void FixMove()
        {
            
        }

        public override void Draw(PrimitiveBatch batch, int height)
        {
        }
    }
}
