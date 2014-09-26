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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ebatianos.Platform
{
    public class World
    {
        public Vector2 GravityVector = Vector2.Zero;
        public Int32 Iterations = 1;

        public Single Epsilon = 0.001f;

        //private Int32 _worldDivisionAreaSize = 16;

        private List<MoveableObject> _moveableObjects = new List<MoveableObject>();

        private CollisionObject[] _obstacles = new CollisionObject[129];
        private CollisionObject[] _mapCollisionBuffer = new CollisionObject[64];

        private IMapCollisionFeeder _mapCollisionFeeder = null;

        IObjFinder _collisionObjects;

        private List<MoveableObject> _temporaryObjects = new List<MoveableObject>();

        //public Int32 WorldDivisionAreaSize
        //{
        //    get
        //    {
        //        return _worldDivisionAreaSize;
        //    }
        //}

        public List<MoveableObject> MoveableObjects
        {
            get
            {
                return _moveableObjects;
            }
        }

        public World(IObjFinder objFinder)
        {
            _collisionObjects = objFinder;
            UpdatePositions();
        }

        public World(Point size)
            : this(new QuadTree(-10, -10, size.X + 20, size.Y + 20))
        {
        }

        public World(IMapCollisionFeeder mapCollisionFeeder)
            : this(mapCollisionFeeder,
                new QuadTree(-10, -10, mapCollisionFeeder.Width + 20, mapCollisionFeeder.Height + 20))
        {
        }

        public World(IMapCollisionFeeder mapCollisionFeeder, IObjFinder objFinder)
        {
            for (Int32 idx = 0; idx < _mapCollisionBuffer.Length; ++idx)
            {
                _mapCollisionBuffer[idx] = new CollisionObject();
            }

            _mapCollisionFeeder = mapCollisionFeeder;

            _collisionObjects = objFinder;
            UpdatePositions();
        }

        public void UpdatePositions()
        {
            _collisionObjects.Clear();

            for (Int32 idx = 0; idx < _moveableObjects.Count; ++idx)
            {
                var moveableObject = _moveableObjects[idx];

                if (moveableObject.IncludeIntoCollisionObjects)
                {
                    _collisionObjects.Add(moveableObject);
                }
            }
        }

        public void Add(MoveableObject moveableObject)
        {
            _moveableObjects.Add(moveableObject);

            if (moveableObject.IncludeIntoCollisionObjects)
            {
                _collisionObjects.Add(moveableObject);
            }
                
            moveableObject.World = this;
        }

        public void AddTemporary(MoveableObject moveableObject)
        {
            _temporaryObjects.Add(moveableObject);
            _moveableObjects.Add(moveableObject);

            moveableObject.World = this;
        }

        internal void Remove(MoveableObject moveableObject)
        {
            _moveableObjects.Remove(moveableObject);
            _temporaryObjects.Remove(moveableObject);

            if (moveableObject.IncludeIntoCollisionObjects)
            {
                _collisionObjects.Remove(moveableObject);
            }

            moveableObject.World = null;
        }

        public CollisionObject[] Obstacles(MoveableObject requestSource, ref Aabb area)
        {
            Int32 index = 0;

            if (_mapCollisionFeeder != null && (requestSource.CollisionGroups & 1) != 0)
            {
                _mapCollisionFeeder.GetCollisions(ref area, _obstacles, _mapCollisionBuffer, ref index);
            }

            if (requestSource.CollisionGroups > 1)
            {
                _collisionObjects.Get(ref area, _obstacles, ref index, requestSource);

                for ( Int32 idx = 0; idx < _temporaryObjects.Count; ++idx )
                {
                    var obj = _temporaryObjects[idx];

                    if ( obj.CheckCollisionId(requestSource.CollisionGroups) && obj.Aabb.Intersects(area))
                    {
                        _obstacles[index] = obj;
                        index++;
                    }
                }
            }

            _obstacles[index] = null;
            return _obstacles;
        }

        public void GetVisibleObjects<T>(Aabb area, List<T> list) where T: MoveableObject
        {
            list.Clear();

            for (Int32 idx = 0; idx < _moveableObjects.Count; ++idx)
            {
                MoveableObject obj = _moveableObjects[idx];

                if (obj == null)
                {
                    break;
                }

                T objectVal = obj as T;

                if (objectVal != null)
                {
                    if (obj.Aabb.Intersects(area))
                    {
                        list.Add(objectVal);
                    }
                }
            }
        }

        public void Update(Single time, Aabb? area)
        {
            time /= (Single)Iterations;

            for (Int32 iteration = 0; iteration < Iterations; ++iteration)
            {
                for (Int32 idx = 0; idx < _moveableObjects.Count; ++idx)
                {
                    MoveableObject obj = _moveableObjects[idx];

                    if (obj == null)
                    {
                        break;
                    }

                    if (obj.Updatable)
                    {
                        if (obj.UpdateAlways || !area.HasValue || obj.Aabb.Intersects(area.Value))
                        {

                            Vector2 position = obj.Aabb.Center;
                            obj.Update(time);

                            if ( obj.Aabb.Center != position && obj.IncludeIntoCollisionObjects)
                            {
                                _collisionObjects.Move(obj);
                            }
                        }
                    }
                }
            }

            _collisionObjects.CleanUp();
        }
    }
}
