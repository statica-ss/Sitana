using System;
using Ebatianos.Cs;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Ebatianos.Content;
using System.Text;
using Ebatianos.Graphics;
using Microsoft.Xna.Framework;

#if MACOSX
using MonoMac.Foundation;
#elif IOS
using MonoTouch.Foundation;
#else
using System.Diagnostics;
#endif

namespace Ebatianos.Diagnostics
{
    public class PerformanceProfiler: Singleton<PerformanceProfiler>
    {
        class Counter
        {
            public String Name { get; private set;}
            private List<Single> _times = new List<Single>();
            private Int32        _historyCount = 20;

            public Single Value { get; private set; }

            public Boolean Enabled;

            public Int32 MaxFill = 50;

            Double _begin;

            #if IOS || MACOSX

            public void Begin()
            {
                _begin = NSDate.Now.SecondsSinceReferenceDate;
            }

            public void End()
            {
                Single time = (Single)(NSDate.Now.SecondsSinceReferenceDate - _begin);
                AddTime(time);
            }

            #elif ANDROID

            public void Begin()
            {
                
            }


            public void End()
            {
                
            }
            #else

            public void Begin()
            {
                _begin = StopWatch.Elapsed.TotalSeconds;
            }

            public void End()
            {
                Single time = (Single)(StopWatch.Elapsed.TotalSeconds - _begin);
                AddTime(time);
            }
            #endif

            private void AddTime(Single time)
            {
                _times.Add(time);

                while (_times.Count > _historyCount)
                {
                    _times.RemoveAt(0);
                }

                Value = 0;

                for (Int32 idx = 0; idx < _times.Count; ++idx)
                {
                    Value = Math.Max(_times[idx], Value);
                }
            }

            public Counter(Int32 historyCount, String name)
            {
                _historyCount = historyCount;
                Name = name;
                Enabled = true;
            }

            public void Reset()
            {
                Value = Single.NaN;
            }
        }

        private List<Single> _lastFrames = new List<Single>();
        private Single _fps = 0;
        private Single _minFps = 0;
        private Single _maxFrame = 0;

        private Int32 _historyCount = 20;

        private Single _targetFrameTime = 1f / 60f;
        private Single _minFrameTime = 1f / 50f;

        private IFontPresenter _fontPresenter;
        private StringBuilder _stringBuilder = new StringBuilder();

        public Boolean ShowFps { private get; set; }
        public Boolean ShowMinFps { private get; set; }

        public Boolean Enabled = true;

        public Align Position = Align.Top;

        private BasicEffect _effect = null;

        private Single _scale;

    #if !IOS && !MACOSX && !ANDROID
        internal static Stopwatch StopWatch = new Stopwatch();
    #endif

        List<Counter> _counters = new List<Counter>();

        public PerformanceProfiler()
        {
            ShowFps = true;
            ShowMinFps = true;

            #if !IOS && !MACOSX && !ANDROID
                StopWatch.Start();
            #endif
        }

        public Int32 Initialize(String font, Int32 targetFps, Int32 minFps, Single scale)
        {
            _scale = scale;
            _fontPresenter = FontLoader.Load(font);

            _targetFrameTime = 1f / (Single)targetFps;
            _minFrameTime = 1f / (Single)minFps;

            _fontPresenter.PrepareRender("A");
            Vector2 size = _fontPresenter.Size * scale;

            return (Int32)(scale * 8 + size.Y);
        }

        public Int32 AddCounter(String displayName, Int32 historyCount)
        {
            Counter counter = new Counter(historyCount, displayName);
            _counters.Add(counter);

            return _counters.Count - 1;
        }

        public void EnableCounter(Int32 id, Boolean enable)
        {
            Counter counter = _counters[id];
            counter.Enabled = enable;
        }

        public void BeginCounter(Int32 id)
        {
            if (Enabled)
            {
                Counter counter = _counters[id];

                if (counter.Enabled)
                {
                    counter.Begin();
                }
            }
        }

        public void EndCounter(Int32 id)
        {
            if (Enabled)
            {
                Counter counter = _counters[id];

                if (counter.Enabled)
                {
                    counter.End();
                }
            }
        }

        public void Update(TimeSpan timeSpan)
        {
            if (!Enabled)
            {
                return;
            }

            _lastFrames.Add((Single)timeSpan.TotalSeconds);

            while (_lastFrames.Count > _historyCount)
            {
                _lastFrames.RemoveAt(0);
            }

            _fps = 0;
            _minFps = 1000;
            _maxFrame = 0;

            for (Int32 idx = 0; idx < _lastFrames.Count; ++idx)
            {
                Single frameTime = _lastFrames[idx];
                _fps += frameTime;

                if (frameTime > 0)
                {
                    _minFps = Math.Min(_minFps, 1.0f / frameTime);
                    _maxFrame = Math.Max(_maxFrame, frameTime);
                }
            }

            _fps /= (Single)_lastFrames.Count;

            _fps = 1.0f / _fps;

            for ( Int32 idx =0; idx < _counters.Count; ++idx )
            {
                _counters[idx].Reset();
            }

        }

        private Matrix ComputeMatrix(Single height)
        {
            switch (Position)
            {
                case Align.Bottom:
                    return Matrix.CreateTranslation(new Vector3(0,_effect.GraphicsDevice.Viewport.Height - height,0));

                case Align.Left:
                    return Matrix.CreateRotationZ((Single)Math.PI / 2) * Matrix.CreateTranslation(new Vector3(height,0,0));

                case Align.Right:
                    return Matrix.CreateRotationZ((Single)Math.PI / 2) * Matrix.CreateTranslation(new Vector3(_effect.GraphicsDevice.Viewport.Width, 0, 0));
            }

            return Matrix.Identity;
        }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
        {
            if (!Enabled)
            {
                return;
            }

            if ( _effect == null )
            {
                _effect = new BasicEffect(spriteBatch.GraphicsDevice);
            }

            Single scale = _scale;

            Int32 lastMaxFrameFill = (Int32)(100 * _maxFrame / (_targetFrameTime)) - 100;

            _fontPresenter.PrepareRender("A");

            Int32 width = spriteBatch.GraphicsDevice.Viewport.Width;

            Vector2 size = _fontPresenter.Size * scale;

            Matrix matrix = ComputeMatrix(scale * 8 + size.Y);

            primitiveBatch.Transform = matrix;
            primitiveBatch.Begin(PrimitiveType.TriangleStrip, RasterizerState.CullNone);

            primitiveBatch.AddVertex(new Vector2(0, 0), Color.Black);
            primitiveBatch.AddVertex(new Vector2(0, scale*8+size.Y), Color.Black);

            primitiveBatch.AddVertex(new Vector2(width, 0), Color.Black);
            primitiveBatch.AddVertex(new Vector2(width, scale*8+size.Y), Color.Black);

            primitiveBatch.End();

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            _effect.World = matrix;
            _effect.View = Matrix.Identity;
            _effect.Projection = halfPixelOffset * projection;

            _effect.TextureEnabled = true;
            _effect.VertexColorEnabled = true;

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, _effect, matrix);

            Single offset = 0;
            Int32 topLeft = (Int32)(scale*4);

            if (ShowMinFps)
            {
                _stringBuilder.Clear();
                _stringBuilder.AppendFormat("minFPS:{0,-2} ", (Int32)_minFps);
                _fontPresenter.PrepareRender(_stringBuilder);

                _fontPresenter.DrawText(spriteBatch, new Vector2(topLeft + offset, topLeft), _maxFrame > _minFrameTime ? Color.Red : Color.White, scale, Align.Top | Align.Left);

                offset += _fontPresenter.Size.X * scale;
            }

            if (ShowFps)
            {
                _stringBuilder.Clear();
                _stringBuilder.AppendFormat("FPS:{0,-2} ", (Int32)_fps);
                _fontPresenter.PrepareRender(_stringBuilder);

                _fontPresenter.DrawText(spriteBatch, new Vector2(topLeft + offset, topLeft), _fps < (1/_minFrameTime) ? Color.Red : Color.White, scale, Align.Top | Align.Left);

                offset += _fontPresenter.Size.X * scale;
            }
                
            Int32 targetFps = (Int32)Math.Ceiling(1 / _targetFrameTime);

            _stringBuilder.Clear();
            _stringBuilder.AppendFormat("FF{1}:+{0,-4} ", Math.Max(0,lastMaxFrameFill), targetFps);
            _fontPresenter.PrepareRender(_stringBuilder);

            _fontPresenter.DrawText(spriteBatch, new Vector2(topLeft + offset, topLeft), _maxFrame > _minFrameTime ? Color.Red : Color.White, scale, Align.Top | Align.Left);

            offset += _fontPresenter.Size.X * scale;

            for (Int32 idx = 0; idx < _counters.Count; ++idx)
            {
                var counter = _counters[idx];

                Single frameTime = counter.Value;
                Int32 lastUpdatePercent = Single.IsNaN(frameTime) ? -1 : (Int32)(100 * frameTime / (_targetFrameTime));

                _stringBuilder.Clear();
                if (lastUpdatePercent < 0)
                {
                    _stringBuilder.AppendFormat("{1}:{0,-3} ", "-", counter.Name);
                }
                else
                {
                    _stringBuilder.AppendFormat("{1}:{0,-3} ", lastUpdatePercent, counter.Name);
                }

                _fontPresenter.PrepareRender(_stringBuilder);
                
                _fontPresenter.DrawText(spriteBatch, new Vector2(topLeft + offset, topLeft), lastUpdatePercent > counter.MaxFill ? Color.Red : Color.White, scale, Align.Top | Align.Left);

                offset += _fontPresenter.Size.X * scale;
            }

            spriteBatch.End();
        }
    }
}

