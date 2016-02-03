using System;
using UIKit;
using Foundation;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;

namespace Sitana.Framework.Input.TouchPad
{
	public class TouchPad_iOSGestureRecognizer : UIGestureRecognizer
	{
		delegate void OnTouchDelegate(Vector2 position, int id, IntPtr handle);

		Dictionary<IntPtr, int> _touchIds = new Dictionary<IntPtr, int>();
		int _nextTouchId = 2;

		UIView _view;
		public TouchPad_iOSGestureRecognizer(UIView view)
		{
			_view = view;
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			OnTouch (touches, (position, id, handle) => 
				{
					UiTask.BeginInvoke( ()=> TouchPad.Instance.ProcessDown(id, position, DateTime.Now));
				});
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			OnTouch (touches, (position, id, handle) => 
				{
					UiTask.BeginInvoke( ()=> TouchPad.Instance.ProcessUp(id, position, DateTime.Now));
				});
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			OnTouch (touches, (position, id, handle) => 
				{
					UiTask.BeginInvoke( ()=> TouchPad.Instance.ProcessUp(id, position, DateTime.Now));
					_touchIds.Remove(handle);
				});
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			OnTouch(touches, (position, id, handle) => 
				{
					UiTask.BeginInvoke( ()=> TouchPad.Instance.ProcessMove(id, position, DateTime.Now));
				});
			
		}

		void OnTouch(NSSet touches, OnTouchDelegate onTouch)
		{
			foreach (var touch in touches) 
			{
				var uitouch = (touch as UITouch);
				var location = uitouch.LocationInView(_view);

				float posx = Platform.PointsToPixels((float)location.X);
				float posy = Platform.PointsToPixels((float)location.Y);
					
				Vector2 position = new Vector2(posx, posy);

				int id;

				if (!_touchIds.TryGetValue(uitouch.Handle, out id)) 
				{
					id = _nextTouchId;
					_nextTouchId++;

					_touchIds.Add(uitouch.Handle, id);
				}

				onTouch(position, id, uitouch.Handle);
			}
		}
	}
}

