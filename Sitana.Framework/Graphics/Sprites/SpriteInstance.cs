using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public class SpriteInstance
    {
        Sprite _sprite;
        Sprite.Sequence _sequence = null;
        float _frame = 0;
        bool _animate = true;

        public event EmptyArgsVoidDelegate AnimationFinished;
        public event EmptyArgsVoidDelegate AnimationReplay;

        public Point FrameSize
        {
            get
            {
                return new Point(FrameImage.Width, FrameImage.Height);
            }
        }

        public Sprite Sprite
        {
            get
            {
                return _sprite;
            }
        }

        public string Sequence
        {
            set
            {
                _sequence = _sprite.FindSequence(value);
                _frame = 0;
                _animate = true;
            }

            get
            {
                return _sequence.Name;
            }
        }

        public PartialTexture2D FrameImage
        {
            get
            {
                return _sequence.Images[(int)_frame];
            }
        }

        public int CurrentFrame
        {
            get
            {
                return (int)_frame;
            }

            set
            {
                _frame = value;
            }
        }

        internal SpriteInstance(Sprite sprite)
        {
            _sprite = sprite;
            _sequence = _sprite.FirstSequence;
        }

        public void Animate(float time)
        {
            if (_animate)
            {
                float max = _sequence.Images.Count;
                _frame += _sequence.Fps * time;

                if (_frame >= max)
                {
                    if (_sequence.Loop)
                    {
                        _frame -= max;
                        if (AnimationReplay != null)
                        {
                            AnimationReplay();
                        }
                    }
                    else
                    {
                        _frame = max - 0.0001f;
                        _animate = false;

                        if (AnimationFinished != null)
                        {
                            AnimationFinished();
                        }
                    }
                }
            }
        }
    }
}
