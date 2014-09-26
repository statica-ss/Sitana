// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Sitana.Framework.Content
{
    public class SpritePresenter : ICustomDraw
    {
        private Sprite _sprite;
        private Int32 _currentSequence = 0;
        private Double[] _currentFrames;

        private ICustomDraw _customDraw;

        public Point FrameSize
        {
            get
            {
                return _sprite.FrameSize;
            }
        }

        public Int32 CurrentSequence
        {
            get
            {
                return _currentSequence;
            }
        }

        public Int32 CurrentFrame
        {
            get
            {
                return (Int32)_currentFrames[_currentSequence];
            }

            set
            {
                _currentFrames[_currentSequence] = value;
            }
        }

        public SpritePresenter(Sprite sprite)
        {
            _sprite = sprite;
            _currentFrames = new Double[sprite.Sequences.Length];
            _customDraw = this;
        }

        void ICustomDraw.Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle source, Color color, Single angle, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(texture, position, source, color, angle, origin, scale, spriteEffects, 0);
        }

        public ICustomDraw CustomDraw
        {
            set
            {
                if (value != null)
                {
                    _customDraw = value;
                }
                else
                {
                    _customDraw = this;
                }
            }
        }

        public void SetSequence(Int32 id)
        {
            SetSequence(id, false);
        }

        public void SetSequence(Int32 id, Boolean reset)
        {
            _currentSequence = id;

            if (reset)
            {
                _currentFrames[_currentSequence] = 0;
            }
        }

        public Int32 FindSequence(String name)
        {
            for (Int32 idx = 0; idx < _sprite.Sequences.Length; ++idx)
            {
                if (_sprite.Sequences[idx].Name == name)
                {
                    return idx;
                }
            }

            throw new Exception("Sprite Animation Sequence Not Found");
        }

        public void Animate(Double units)
        {
            Double currentFrame = _currentFrames[_currentSequence];
            Sprite.Sequence sequence = _sprite.Sequences[_currentSequence];

            currentFrame += units * sequence.Speed;

            if (sequence.Loop && currentFrame >= sequence.Length)
            {
                currentFrame -= sequence.Length;
            }

            _currentFrames[_currentSequence] = currentFrame;
        }

        public void Draw(Int32 sheetPart, SpriteBatch spriteBatch, Vector2 position, Color color, Single angle, Vector2 origin, Single scale, SpriteEffects spriteEffects)
        {
            Draw(sheetPart, spriteBatch, position, color, angle, origin, new Vector2(scale), spriteEffects);
        }

        public void Draw(Int32 sheetPart, SpriteBatch spriteBatch, Vector2 position, Color color, Single angle, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects)
        {
            Double currentFrame = _currentFrames[_currentSequence];
            Sprite.Sequence sequence = _sprite.Sequences[_currentSequence];

            Int32 frame = Math.Min((Int32)currentFrame, sequence.Length - 1) + sequence.Start;

            Int32 offsetX = 0;
            Int32 offsetY = 0;

            if (_sprite.OptimizedFrames != null)
            {
                Point offset = _sprite.OptimizedFrames[sheetPart][frame];

                offsetX = offset.X;
                offsetY = offset.Y;
            }
            else
            {
                offsetX = (frame % _sprite.Columns) * _sprite.FrameSize.X;
                offsetY = (frame / _sprite.Columns) * _sprite.FrameSize.Y;
            }

            _customDraw.Draw(spriteBatch, _sprite.SpriteSheet[sheetPart], position, new Rectangle(offsetX, offsetY, _sprite.FrameSize.X, _sprite.FrameSize.Y), color, angle, origin, scale, spriteEffects);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, Single angle, Vector2 origin, Single scale, SpriteEffects spriteEffects)
        {
            Draw(0, spriteBatch, position, color, angle, origin, scale, spriteEffects);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, Single angle, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects)
        {
            Draw(0, spriteBatch, position, color, angle, origin, scale, spriteEffects);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Single angle, Vector2 origin, Single scale, SpriteEffects spriteEffects)
        {
            Draw(spriteBatch, position, colors, angle, origin, new Vector2(scale), spriteEffects);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Single angle, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects)
        {
            Double currentFrame = _currentFrames[_currentSequence];
            Sprite.Sequence sequence = _sprite.Sequences[_currentSequence];

            Int32 frame = Math.Min((Int32)currentFrame, sequence.Length - 1) + sequence.Start;

            for (Int32 idx = 0; idx < _sprite.SpriteSheet.Length; ++idx)
            {
                Int32 offsetX = 0;
                Int32 offsetY = 0;

                if (_sprite.OptimizedFrames != null)
                {
                    Point offset = _sprite.OptimizedFrames[idx][frame];

                    offsetX = offset.X;
                    offsetY = offset.Y;
                }
                else
                {
                    offsetX = (frame % _sprite.Columns) * _sprite.FrameSize.X;
                    offsetY = (frame / _sprite.Columns) * _sprite.FrameSize.Y;
                }

                _customDraw.Draw(spriteBatch, _sprite.SpriteSheet[idx], position, new Rectangle(offsetX, offsetY, _sprite.FrameSize.X, _sprite.FrameSize.Y), colors.Length > 1 ? colors[idx] : colors[0], angle, origin, scale, spriteEffects);
            }
        }
    }
}
