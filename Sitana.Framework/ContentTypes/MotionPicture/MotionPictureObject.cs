/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace Sitana.Framework.Content.MotionPictureCore
{
    public abstract class MotionPictureObject
    {
        public String Name { get; protected set; }
        public Boolean PointSamplerState { get; protected set; }
        public Boolean AdditiveBlendState { get; protected set; }

        public class MotionPictureAction
        {
            public Single Time { get; set; }
            public Action Action { get; set; }
        }

        protected List<MotionPictureAction> _actions = new List<MotionPictureAction>();
        private Int32 _currentIndex = 0;

        public Int32 SortOrder { get; protected set; }

        public Boolean Visible { get; protected set; }

        public MotionPictureObject()
        {
            SortOrder = 0;
            PointSamplerState = false;
            AdditiveBlendState = false;
            Visible = true;
        }

        private void Mode(String blend, String sampler)
        {
            PointSamplerState = sampler.ToLowerInvariant() != "0";
            AdditiveBlendState = blend.ToLowerInvariant() != "0";
        }

        public virtual void Draw(SpriteBatch spriteBatch, Single scale, Vector2 offset, Single transition)
        {

        }

        public virtual void Process(Single totalTime, Single time)
        {
            for (Int32 idx = _currentIndex; idx < _actions.Count; ++idx)
            {
                if (_actions[idx].Time > totalTime)
                {
                    return;
                }
                _actions[idx].Action.Invoke();
                _currentIndex = idx + 1;
            }
        }

        public virtual void Reset()
        {
            _currentIndex = 0;
            Visible = true;
        }

        public void AddAction(Single time, String name, String[] parameters)
        {
            Action action = CreateAction(name, parameters, time);

            if (action != null)
            {
                _actions.Add(
                   new MotionPictureAction()
                   {
                       Time = time,
                       Action = action
                   }
                );
            }
        }

        private void Zorder(Int32 zorder)
        {
            SortOrder = zorder;
        }

        private void Show(Int32 show)
        {
            Visible = show != 0;
        }

        protected virtual Action CreateAction(String name, String[] parameters, Single time)
        {
            switch (name.ToLowerInvariant())
            {
                case "mode":
                    return new Action(() => Mode(parameters[0], parameters[1]));

                case "show":
                    return new Action(() => Show(Int32.Parse(parameters[0])));

                case "zorder":
                    return new Action(() => Zorder(Int32.Parse(parameters[0])));
            }

            return null;
        }
    }

    public class MotionPicturePlayback : MotionPictureObject
    {
        public Vector2 View { get; private set; }
        public Color BgColor { get; private set; }

        private MotionPicture _parent;

        public MotionPicturePlayback(MotionPicture parent)
        {
            Name = ".";
            SortOrder = 100000;
            _parent = parent;
        }

        protected override Action CreateAction(String name, String[] parameters, Single time)
        {
            switch (name.ToLowerInvariant())
            {
                case "view":
                    return new Action(() => View = new Vector2(Single.Parse(parameters[0], CultureInfo.InvariantCulture), Single.Parse(parameters[1], CultureInfo.InvariantCulture)));

                case "bgcolor":
                    return new Action(() =>
                       BgColor = Color.FromNonPremultiplied(Int32.Parse(parameters[0]), Int32.Parse(parameters[1]), Int32.Parse(parameters[2]), 255));

                case "sprite":
                    _parent.AddObject(new MotionPictureSprite(parameters[0]));
                    return null;
            }

            return base.CreateAction(name, parameters, time);
        }
    }

    public class MotionPictureSprite : MotionPictureObject
    {
        class Movement
        {
            public Single Begin;
            public Single End;
            public Vector2 Vector = Vector2.Zero;
        }

        private SpritePresenter _spritePresenter;
        private Vector2 _position;
        private Vector2 _scale;
        private Vector2 _origin;
        private Color[] _colors;
        private Single _speed;
        private Vector2 _displayPosition;
        private List<Movement> _movements = new List<Movement>();
        private Color[] _tempColors;

        public MotionPictureSprite(String name)
        {
            Name = name;
        }

        public override void Draw(SpriteBatch spriteBatch, Single scale, Vector2 offset, Single transition)
        {
            Vector2 position = _displayPosition * scale + offset;
            Vector2 rescale = _scale * scale;

            for (Int32 idx = 0; idx < _tempColors.Length; ++idx)
            {
                _tempColors[idx] = _colors[idx] * transition;
            }

            _spritePresenter.Draw(spriteBatch, position, _tempColors, 0, _origin, rescale, SpriteEffects.None);
        }

        public override void Reset()
        {
            _position = Vector2.Zero;
            _scale = Vector2.One;
            _origin = Vector2.Zero;
            _speed = 1;

            base.Reset();
        }

        private void Load(String path)
        {
            Sprite sprite = ContentLoader.Current.Load<Sprite>(path);
            _spritePresenter = new SpritePresenter(sprite);

            _colors = new Color[sprite.SpriteSheet.Length];
            _tempColors = new Color[sprite.SpriteSheet.Length];
        }

        private void Scale(Single scaleX, Single scaleY)
        {
            _scale = new Vector2(scaleX, scaleY);
        }

        private void Origin(Single originX, Single originY)
        {
            _origin = new Vector2(originY, originY);
        }

        private void Position(Single posX, Single posY)
        {
            _position = new Vector2(posX, posY);
            _movements.Clear();
            _displayPosition = _position;
        }

        private void Position(Single totalTime)
        {
            _displayPosition = _position;

            for (Int32 idx = 0; idx < _movements.Count; ++idx)
            {
                _displayPosition += GetMovement(_movements[idx], totalTime);
            }

            _movements.Clear();
            _position = _displayPosition;
        }

        private void Color(Int32 index, Int32 a, Int32 r, Int32 g, Int32 b)
        {
            if (index < 0)
            {
                for (Int32 idx = 0; idx < _colors.Length; ++idx)
                {
                    _colors[idx] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(r, g, b, a);
                }
                return;
            }
            _colors[index] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(r, g, b, a);
        }

        private void Move(Single x, Single y, Single time, Single duration)
        {
            Movement movement = new Movement()
            {
                Begin = time,
                End = time + duration,
                Vector = new Vector2(x, y)
            };

            _movements.Add(movement);
        }

        private void Sequence(String name, Single speed)
        {
            _speed = speed;

            Int32 seqId = _spritePresenter.FindSequence(name);
            _spritePresenter.SetSequence(seqId, true);
        }



        protected override Action CreateAction(String name, String[] parameters, Single time)
        {
            switch (name.ToLowerInvariant())
            {
                case "load":
                    return new Action(() => Load(parameters[0]));

                case "scale":
                    return new Action(() => Scale(Single.Parse(parameters[0], CultureInfo.InvariantCulture), Single.Parse(parameters[1], CultureInfo.InvariantCulture)));

                case "origin":
                    return new Action(() => Origin(Single.Parse(parameters[0], CultureInfo.InvariantCulture), Single.Parse(parameters[1], CultureInfo.InvariantCulture)));

                case "position":

                    if (parameters.Length == 0)
                    {
                        return new Action(() => Position(time));
                    }
                    else
                    {
                        return new Action(() => Position(Single.Parse(parameters[0], CultureInfo.InvariantCulture), Single.Parse(parameters[1], CultureInfo.InvariantCulture)));
                    }


                case "move":
                    return new Action(() => Move(Single.Parse(parameters[0], CultureInfo.InvariantCulture), Single.Parse(parameters[1], CultureInfo.InvariantCulture), time, Single.Parse(parameters[2], CultureInfo.InvariantCulture)));

                case "color":
                    if (parameters.Length == 4)
                    {
                        return new Action(() => Color(-1, Int32.Parse(parameters[0]), Int32.Parse(parameters[1]), Int32.Parse(parameters[2]), Int32.Parse(parameters[3])));
                    }
                    else if (parameters.Length == 3)
                    {
                        return new Action(() => Color(-1, 255, Int32.Parse(parameters[0]), Int32.Parse(parameters[1]), Int32.Parse(parameters[2])));
                    }

                    break;

                case "color_0":
                case "color_1":
                case "color_2":
                case "color_3":
                case "color_4":
                case "color_5":
                case "color_6":
                case "color_7":
                case "color_8":
                    {
                        Int32 index = (Int32)name[6] - 48;

                        if (parameters.Length == 4)
                        {
                            return new Action(() => Color(index, Int32.Parse(parameters[0]), Int32.Parse(parameters[1]), Int32.Parse(parameters[2]), Int32.Parse(parameters[3])));
                        }
                        else if (parameters.Length == 3)
                        {
                            return new Action(() => Color(index, 255, Int32.Parse(parameters[0]), Int32.Parse(parameters[1]), Int32.Parse(parameters[2])));
                        }
                    }

                    break;

                case "sequence":
                    return new Action(() => Sequence(parameters[0], Single.Parse(parameters[1], CultureInfo.InvariantCulture)));

            }

            return base.CreateAction(name, parameters, time);
        }

        public override void Process(Single totalTime, Single time)
        {
            base.Process(totalTime, time);

            _displayPosition = _position;

            for (Int32 idx = 0; idx < _movements.Count; ++idx)
            {
                _displayPosition += GetMovement(_movements[idx], totalTime);
            }

            if (Visible)
            {
                _spritePresenter.Animate(time * _speed);
            }
        }

        private Vector2 GetMovement(Movement movement, Single totalTime)
        {
            if (movement.Begin > totalTime)
            {
                return Vector2.Zero;
            }

            if (movement.End <= totalTime)
            {
                return movement.Vector;
            }

            Single factor = (totalTime - movement.Begin) / (movement.End - movement.Begin);
            return movement.Vector * factor;
        }
    }
}
