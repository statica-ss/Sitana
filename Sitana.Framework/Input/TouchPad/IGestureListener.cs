using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Input.TouchPad
{
    public interface IGestureListener
    {
        void OnGesture(Gesture gesture);
    }
}
