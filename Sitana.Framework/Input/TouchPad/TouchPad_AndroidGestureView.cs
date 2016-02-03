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
using Microsoft.Xna.Framework;
using Android.Content;

namespace Sitana.Framework.Input.TouchPad
{
	public class TouchPad_AndroidGestureView: View
	{
		public TouchPad_AndroidGestureView(Context context): base(context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(-1, -1);
		}

		public override bool OnTouchEvent(MotionEvent motionEvent)
		{
			MotionEventActions maskedAction = motionEvent.ActionMasked;

			switch (maskedAction)
			{
			case MotionEventActions.PointerDown:
			case MotionEventActions.Down:
				{
					Vector2 position = Vector2.Zero;
					position.X = motionEvent.GetX(motionEvent.ActionIndex);
					position.Y = motionEvent.GetY(motionEvent.ActionIndex);

					int pointerId = motionEvent.GetPointerId(motionEvent.ActionIndex);

					TouchPad.Instance.ProcessDown(pointerId, position, DateTime.Now);
				}
				return true;

			case MotionEventActions.PointerUp:
			case MotionEventActions.Up:
			case MotionEventActions.Cancel:
				{
					int pointerId = motionEvent.GetPointerId(motionEvent.ActionIndex);

					TouchPad.Instance.ProcessUp (pointerId, DateTime.Now);
				}
				return true;

			case MotionEventActions.Move:
				{
					int count = motionEvent.PointerCount;

					for (int idx = 0; idx < count; ++idx)
					{
						Vector2 position = Vector2.Zero;

						position.X = motionEvent.GetX(idx);
						position.Y = motionEvent.GetY(idx);

						int pointerId = motionEvent.GetPointerId(idx);
						TouchPad.Instance.ProcessMove(pointerId, position, DateTime.Now);
					}
				}

				return true;
			}

			return false;
		}
	}
}

