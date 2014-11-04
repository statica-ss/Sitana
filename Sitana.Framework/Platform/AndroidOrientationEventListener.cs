// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Android.Views;
using Android.Content;
using Microsoft.Xna.Framework;

namespace Ebatianos
{
    public class AndroidOrientationEventListener: OrientationEventListener
    {
        Orientation _current = Orientation.Portrait;

        public AndroidOrientationEventListener(Context context): base(context, Android.Hardware.SensorDelay.Ui)
        {
        }

        public override void OnOrientationChanged(Int32 orientation)
        {
            if (!CanDetectOrientation() || orientation < 0)
            {
                return;
            }
                
            Vector2 up = new Vector2(0, 1);           
            up = Vector2.Transform(up, Matrix.CreateRotationZ((Single)orientation / 180.0f * (Single)Math.PI));

            Orientation newValue = _current;

            if (Math.Abs(up.X) > Math.Abs(up.Y) * 4 )
            {
                if (up.X > 0)
                {
                    newValue = Orientation.LandscapeLeft;
                } 
                else
                {
                    newValue = Orientation.LandscapeRight;
                }
            }

            if ( Math.Abs(up.Y) > Math.Abs(up.X) * 4 && up.Y > 0 )
            {
                newValue = Orientation.Portrait;
            }


            if (newValue != _current)
            {
                _current = newValue;
                SystemWrapper.OnOrientationChanged(_current);
            }
        }
    }
}

