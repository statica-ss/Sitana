// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
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

