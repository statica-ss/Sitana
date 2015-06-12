using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Input.TouchPad
{
    public class Gesture
    {
        public GestureType GestureType { internal set; get; }

        public int TouchId { internal set; get; }
        public Vector2 Origin { internal set; get; }
        public Vector2 Position { internal set; get; }

        public Vector2 Offset { internal set; get; }

        public bool Handled {get; private set;}

        public bool SkipRest { get; private set; }
        
        public object PointerCapturedBy { get; internal set; }

        public void CapturePointer(object captureBy)
        {
            if (PointerCapturedBy != null && PointerCapturedBy != captureBy)
            {
                throw new Exception("Pointer already captured by another object.");
            }

            PointerCapturedBy = captureBy;
        }

        public void ResetState()
        {
            Handled = false;
            SkipRest = false;
        }

        public void SetHandled()
        {
            Handled = true;
        }

        public void Skip()
        {
            SkipRest = true;
        }
    }
}
