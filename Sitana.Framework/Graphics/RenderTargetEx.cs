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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public class RenderTargetEx
    {
        GraphicsDevice _device;
        RenderTarget2D _target;

        Int32 _width;
        Int32 _height;

        Viewport _oldViewport;

        Boolean _isBeginned = false;

        public Texture2D Buffer
        {
            get 
            {
                return _target;
            }
        }

        public Int32 Width
        {
            get
            {
                return _width;
            }
        }

        public Int32 Height
        {
            get
            {
                return _height;
            }
        }

        public RenderTargetEx(GraphicsDevice device, Int32 width, Int32 height)
        {
            _device = device;
            _width = width;
            _height = height;

            Restore();
        }

        public void Release()
        {
#if ANDROID || IOS || WINDOWS_PHONE
            if (_target != null)
            {
                if (!_target.IsDisposed)
                {
                    _target.Dispose();
                }
                _target = null;
            }
#endif
        }

        public Boolean Restore()
        {
            if (_target != null && (_target.IsDisposed || _target.IsContentLost))
            {
                Release();
            }

            if (_target == null)
            {
#if ANDROID || IOS || WINDOWS_PHONE
                if (!_device.IsContentLost && !_device.IsDisposed)
#else
                if(!_device.IsDisposed)
#endif
                {
                    _target = new RenderTarget2D(_device, _width, _height);
                    return true;
                }
            }

            return false;
        }

        public void Clear(Color color)
        {
            if (!_isBeginned)
            {
                throw new InvalidOperationException("Call Begin before Clear.");
            }

            _device.Clear(color);
        }

        public Boolean IsOk
        {
            get
            {
                Boolean value = _target != null && !_target.IsDisposed && !_target.IsContentLost;
                return value;
            }
        }
            
        public void Begin()
        {
            if (_isBeginned)
            {
                throw new InvalidOperationException("Call End before next Begin.");
            }

            if (!IsOk )
            {
                Console.WriteLine("Restoring render target.");
            }

            _oldViewport = _device.Viewport;
            _device.SetRenderTarget(_target);

            _isBeginned = true;
        }

        public void End()
        {
            if (!_isBeginned)
            {
                throw new InvalidOperationException("Call Begin before End.");
            }

            _device.SetRenderTarget(null);
            _device.Viewport = _oldViewport;

            _isBeginned = false;
        }
    }
}

