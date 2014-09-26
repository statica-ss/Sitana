// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Cs;
using System;

namespace Sitana.Framework.Platform
{
    public struct Aabb
    {
        private Vector2 _center;
        private Vector2 _halfSize;

        public Vector2 Center
        {
            get
            {
                return _center;
            }

            set
            {
                _center = value;
                UpdateAb();
            }
        }
        public Vector2 HalfSize
        {
            get
            {
                return _halfSize;
            }

            set
            {
                _halfSize = value;
                UpdateAb();
            }
        }

        public Vector2 A;
        public Vector2 B;

        public Single Left
        {
            get
            {
                return A.X;
            }
        }

        public Single Top
        {
            get
            {
                return A.Y;
            }
        }

        public Single Width
        {
            get
            {
                return _halfSize.X * 2;
            }
        }

        public Single Height
        {
            get
            {
                return _halfSize.Y * 2;
            }
        }

        private void UpdateAb()
        {
            A = _center - _halfSize;
            B = _center + _halfSize;
        }

        public Aabb(Vector2 halfSize)
            : this()
        {
            _center = Vector2.Zero;
            _halfSize = new Vector2(Math.Abs(halfSize.X), Math.Abs(halfSize.Y));
            UpdateAb();
        }

        public Aabb(Vector2 center, Vector2 halfSize)
            : this()
        {
            _center = center;
            _halfSize = new Vector2(Math.Abs(halfSize.X), Math.Abs(halfSize.Y));
            UpdateAb();
        }

        public Boolean Intersects(Aabb other)
        {
            return Intersects(ref other);
        }

        public Boolean Intersects(ref Aabb other)
        {
            return !(A.X >= other.B.X || A.Y >= other.B.Y || other.A.X >= B.X || other.A.Y >= B.Y);
        }

        public Boolean Contains(ref Aabb other)
        {
            return (other.A.X >= A.X && other.B.X <= B.X && other.A.Y >= A.Y && other.B.Y <= B.Y);
        }

        public Boolean Contains(Aabb other)
        {
            return Contains(ref other);
        }

        public void Shrink(Vector2 size)
        {
            HalfSize = new Vector2(HalfSize.X - size.X, HalfSize.Y - size.Y);
        }

        public Aabb Sum(ref Aabb aabb)
        {
            Vector2 topLeft = Center - HalfSize;
            Vector2 bottomRight = Center + HalfSize;

            Vector2 topLeft2 = aabb.Center - aabb.HalfSize;
            Vector2 bottomRight2 = aabb.Center + aabb.HalfSize;

            topLeft = new Vector2(Math.Min(topLeft.X, topLeft2.X), Math.Min(topLeft.Y, topLeft2.Y));
            bottomRight = new Vector2(Math.Max(bottomRight.X, bottomRight2.X), Math.Max(bottomRight.Y, bottomRight2.Y));

            return new Aabb((topLeft + bottomRight) / 2, new Vector2(Math.Abs(topLeft.X - bottomRight.X) / 2, Math.Abs(topLeft.Y - bottomRight.Y) / 2));
        }
    }
}
