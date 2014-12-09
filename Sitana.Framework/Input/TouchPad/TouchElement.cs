using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Input.TouchPad
{
    struct TouchElement
    {
        public static readonly TouchElement Invalid = new TouchElement() { Origin = Vector2.Zero, Position = Vector2.Zero, Valid = false, LockedGesture = GestureType.None };

        public Vector2 Origin;
        public Vector2 Position;

        public bool Valid;

        public GestureType LockedGesture;

        public DateTime DownTime;

        public object LockedListener;
    }
}
