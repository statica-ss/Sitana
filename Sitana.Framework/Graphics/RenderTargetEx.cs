// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public class RenderTargetEx
    {
        GraphicsDevice _device;
        RenderTarget2D _target;

        int _width;
        int _height;

        Viewport _oldViewport;

        Boolean _isBeginned = false;

        public Texture2D Buffer
        {
            get 
            {
                return _target;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public RenderTargetEx(GraphicsDevice device, int width, int height)
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

