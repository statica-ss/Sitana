using System;
using Microsoft.Xna.Framework.Audio;
using Sitana.Framework.Content;

namespace Sitana.Framework.Media
{
    public class SoundEffectBuffer
    {
        SoundEffectInstance[] _buffer;
        int _position = 0;

        public SoundEffectBuffer(string path, int capacity)
        {
            SoundEffect sound = ContentLoader.Current.Load<SoundEffect>(path);
            Init(sound, capacity);
        }

        public SoundEffectBuffer(SoundEffect sound, int capacity)
        {
            Init(sound, capacity);
        }

        void Init(SoundEffect sound, int capacity)
        {
            _buffer = new SoundEffectInstance[capacity];

            for(int idx = 0; idx < capacity; ++idx)
            {
                _buffer[idx] = sound.CreateInstance();
            }
        }

        public void Play(double volume = 1, double pan = 0)
        {
            SoundEffectInstance instance = _buffer[_position];
            _position = (_position + 1) % _buffer.Length;

            if ( instance.State != SoundState.Stopped)
            {
                instance.Stop();
            }

            instance.Volume = (float)volume;
            instance.Pan = (float)pan;
            instance.Play();
        }
    }
}

