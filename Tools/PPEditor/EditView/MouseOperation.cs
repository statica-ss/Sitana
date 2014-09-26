using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ebatianos.Graphics;
using Microsoft.Xna.Framework;

namespace Editor
{
    public enum MouseOperationType
    {
        None,
        Selection,
        Polygon,
        Rectangle,
        Circle
    }

    public abstract class MouseOperation
    {
        public MouseOperation()
        {
            EditView.Instance.Selection = null;
        }

        public abstract MouseOperationType Type { get; }

        public virtual bool MouseMove(MouseEventArgs args, int height) { return false; }
        public virtual bool MouseUp(MouseEventArgs args, int height) { return false; }
        public virtual bool MouseDown(MouseEventArgs args, int height) { return false; }
        public virtual bool MouseWheel(MouseEventArgs args, int height) { return false; }
        public virtual bool MouseDblClick(MouseEventArgs args, int height) { return false; }

        public virtual bool MouseLeave(int height) { return false; }

        public virtual bool Mark { get { return false; } }

        public virtual void Draw(PrimitiveBatch batch, int height){}

        public virtual bool KeyDown(Keys key, bool control, bool shift)
        {
            if (key == Keys.Escape)
            {
                EditView.Instance.Operation = new OperationSelection();
                return true;
            }

            return false;
        }
    }
}
