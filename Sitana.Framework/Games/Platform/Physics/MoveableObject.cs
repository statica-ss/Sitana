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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace Ebatianos.Platform
{
    public class MoveableObject : CollisionObject
    {
        private Vector2 _additionalMovement = Vector2.Zero;

        // Objects which collide with current object. Boolean determines if its a contact collision.
        private CollisionsDictionary _collisions = new CollisionsDictionary();

        private List<Sensor> _sensors = new List<Sensor>();

        private Vector2 _frictionVector = Vector2.Zero;

        private Vector2 _velocity = Vector2.Zero;
        private Vector2 _force = Vector2.Zero;
        public Vector2 Bounce { get; protected set; }

        public UInt64 PositionXFlag { get; private set; }
        public UInt64 PositionYFlag { get; private set; }

        protected Vector2 MaxVelocity;

        protected CollisionSites LastMovementCollision { get; private set; }

        public World World { get; internal set; }

        public Single Mass { get; private set; }

        public Boolean Active { get; protected set; }
        public Boolean UpdateAlways { get; protected set; }
        protected Boolean _smallObject = false;

        protected Single AirFloat;

        private Vector2 _lastPosition = Vector2.Zero;

        protected Boolean UsesCorrection = false;

        public override Boolean Enabled
        {
            get
            {
                return Active;
            }
        }

        public Vector2 LastPosition
        {
            get
            {
                return _lastPosition;
            }
        }

        public virtual Boolean Updatable
        {
            get
            {
                return Active;
            }
        }

        public virtual Aabb UpdateAabb
        {
            get
            {
                return Aabb;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
        }

        public MoveableObject()
        {
            Mass = 1;
            Aabb = new Aabb(Vector2.Zero, Vector2.Zero);

            MaxVelocity = new Vector2(Single.MaxValue, Single.MaxValue);
            Active = true;

            LastMovementCollision = CollisionSites.None;

            CausesCorrection = false;
            UpdateAlways = false;

            AirFloat = 0;
            Bounce = Vector2.Zero;
        }

        [Conditional("ETRACE")]
        private void TestNanVector(ref Vector2 vector)
        {
            if (Single.IsNaN(vector.X))
            {
                if (Single.IsNaN(vector.Y))
                {
                    throw new Exception("Vector's X,Y is NaN");
                }

                throw new Exception("Vector's X is NaN");
            }

            if (Single.IsNaN(vector.Y))
            {
                throw new Exception("Vector's Y is NaN");
            }
        }

        public void Reset()
        {
            _velocity = Vector2.Zero;
            _force = Vector2.Zero;
            _frictionVector = Vector2.Zero;
            _additionalMovement = Vector2.Zero;
            _collisions.Clear();

            LastMovementCollision = CollisionSites.None;
        }

        public void UpdateVelocity(Vector2 velocity)
        {
            _velocity = velocity;
        }

        public MoveableObject(Single mass, Vector2 center, Vector2 halfSize)
        {
            LastMovementCollision = CollisionSites.None;

            Mass = mass;
            Aabb = new Aabb(center, halfSize);

            MaxVelocity = new Vector2(Single.MaxValue, Single.MaxValue);

            CausesCorrection = false;
        }


        protected Int32 AddSensor(Vector2 center, Vector2 halfSize, CollisionSites collisionSites)
        {
            Int32 returnValue = _sensors.Count;
            _sensors.Add(new Sensor(center, halfSize, collisionSites));
            return returnValue;
        }

        protected Boolean SensorValue(Int32 id)
        {
            return _sensors[id].Value;
        }

        public void Remove()
        {
            World.Remove(this);
        }

        public void AddForce(Vector2 force)
        {
            _force += force;
        }

        public void MoveBy(Vector2 offset)
        {
            _additionalMovement += offset;
        }

        public virtual void Update(Single time)
        {
            Vector2 movementVector = Vector2.Zero;

            LastMovementCollision = CollisionSites.None;
            _frictionVector = Vector2.Zero;

            _collisions.Clear();

            Int32 steps = (Int32)Math.Round(time / 0.016666666666f);
            steps = Math.Max(steps, 1);

            time /= (Single)steps;

            Vector2 impactForce = World.GravityVector * time * Mass * (1.0f - AirFloat) + _force;

            for (Int32 idx = 0; idx < steps; ++idx)
            {
                _velocity += (World.GravityVector * time * Mass * (1.0f - AirFloat) + _force) / Mass;

                TestNanVector(ref _velocity);

                _velocity = new Vector2(
                   Math.Sign(_velocity.X) * Math.Min(MaxVelocity.X, Math.Abs(_velocity.X)),
                   Math.Sign(_velocity.Y) * Math.Min(MaxVelocity.Y, Math.Abs(_velocity.Y))
                   );

                TestNanVector(ref _velocity);

                _force = Vector2.Zero;

                movementVector += _velocity * time + _additionalMovement;
                _additionalMovement = Vector2.Zero;
            }

            CollisionObject[] obstacles = null;

            if (CollisionGroups != 0)
            {
                Aabb movementAabb = GetMovementAabb(ref movementVector);
                obstacles = World.Obstacles(this, ref movementAabb);

                if (obstacles[0] == null)
                {
                    obstacles = null;
                }
            }

            if (obstacles != null)
            {
                Move(movementVector, obstacles);

                if (UsesCorrection)
                {
                    Correct(obstacles);
                }

                KeyValuePair<CollisionObject, Boolean> collisionObj = new KeyValuePair<CollisionObject, Boolean>();

                for (Int32 idx = 0; idx < _collisions.Count; ++idx)
                {
                    _collisions.At(idx, ref collisionObj);

                    OnCollision(collisionObj.Key, collisionObj.Value);
                    collisionObj.Key.OnCollisionReflect(this, collisionObj.Value);
                }

                if (_frictionVector != Vector2.Zero || Bounce != Vector2.Zero)
                {
                    if (LastMovementCollision != CollisionSites.None)
                    {
                        Console.WriteLine("Collision: {2} Vector:({0},{1})", _velocity.X, _velocity.Y, LastMovementCollision.ToString());
                    }

                    for (Int32 idx = 0; idx < steps; ++idx)
                    {
                        if (CheckSite(LastMovementCollision, CollisionSites.Right) && _velocity.X > 0)
                        {
                            if (Bounce.X != 0)
                            {
                                if (idx == 0)
                                {
                                    _velocity.X = -_velocity.X * Bounce.X;
                                }
                            }
                            else
                            {
                                _velocity.X = 0;
                            }

                            if (impactForce.X > 0)
                            {
                                Single frictionForce = impactForce.X * _frictionVector.X;
                                frictionForce = Math.Min(frictionForce, Math.Abs(_velocity.Y) * Mass / time);

                                _velocity.Y -= Math.Sign(_velocity.Y) * frictionForce / Mass * time;
                            }
                            TestNanVector(ref _velocity);
                        }

                        if (CheckSite(LastMovementCollision, CollisionSites.Left) && _velocity.X < 0)
                        {
                            if (Bounce.X != 0)
                            {
                                if (idx == 0)
                                {
                                    _velocity.X = -_velocity.X * Bounce.X;
                                }
                            }
                            else
                            {
                                _velocity.X = 0;
                            }

                            if (impactForce.X < 0)
                            {
                                Single frictionForce = -impactForce.X * _frictionVector.X;
                                frictionForce = Math.Min(frictionForce, Math.Abs(_velocity.Y) * Mass / time);

                                _velocity.Y -= Math.Sign(_velocity.Y) * frictionForce / Mass * time;
                            }
                            TestNanVector(ref _velocity);
                        }

                        if (CheckSite(LastMovementCollision, CollisionSites.Top) && _velocity.Y < 0)
                        {
                            if (Bounce.Y != 0)
                            {
                                if (idx == 0)
                                {
                                    _velocity.Y = -_velocity.Y * Bounce.Y;
                                }
                            }
                            else
                            {
                                _velocity.Y = 0;
                            }

                            if (impactForce.Y < 0)
                            {
                                Single frictionForce = -impactForce.Y * _frictionVector.Y;
                                frictionForce = Math.Min(frictionForce, Math.Abs(_velocity.X) * Mass / time);

                                _velocity.X -= Math.Sign(_velocity.X) * frictionForce / Mass * time;
                            }
                            TestNanVector(ref _velocity);
                        }

                        if (CheckSite(LastMovementCollision, CollisionSites.Bottom) && _velocity.Y > 0)
                        {
                            if (Bounce.Y != 0)
                            {
                                if (idx == 0)
                                {
                                    _velocity.Y = -_velocity.Y * Bounce.Y;
                                }
                            }
                            else
                            {
                                _velocity.Y = 0;
                            }

                            if (impactForce.Y > 0)
                            {
                                Single frictionForce = impactForce.Y * _frictionVector.Y;
                                frictionForce = Math.Min(frictionForce, Math.Abs(_velocity.X) * Mass / time);

                                _velocity.X -= Math.Sign(_velocity.X) * frictionForce / Mass * time;
                            }
                            TestNanVector(ref _velocity);
                        }
                    }
                }
            }
            else
            {
                Aabb.Center += movementVector;
            }

            CalculateSensors(obstacles);
        }

        private void AddCollision(CollisionObject obj, Boolean contactCollision)
        {
            _collisions.Add(obj, contactCollision);
        }

        private void Move(Vector2 moveBy, CollisionObject[] obstacles)
        {
            for (Int32 idx = 0; obstacles[idx] != null; ++idx)
            {
                CollisionObject simObj = obstacles[idx];

                if (simObj.MovementCollisionSites != CollisionSites.None)
                {
                    CollisionSites collisionSites = CheckCollision(ref moveBy, simObj, 1, false);

                    if (collisionSites != CollisionSites.None)
                    {
                        AddCollision(simObj, false);
                    }

                    LastMovementCollision |= collisionSites;
                }
            }

            for (Int32 idx = 0; obstacles[idx] != null; ++idx)
            {
                CollisionObject simObj = obstacles[idx];

                Vector2 move = moveBy;

                if (simObj.Aabb.Intersects(ref Aabb) || CheckCollision(ref move, simObj, 1, true) != CollisionSites.None)
                {
                    AddCollision(simObj, true);
                }
            }

            Aabb.Center += moveBy;
        }

        private void Correct(CollisionObject[] obstacles)
        {
            Vector2 move = Vector2.Zero;

            for (Int32 idx = 0; obstacles[idx] != null; ++idx)
            {
                CollisionObject simObj = obstacles[idx];

                if (simObj.CausesCorrection)
                {
                    if (_collisions.FindAt(simObj))
                    {
                        move = CorrectPosition(simObj, move);
                    }
                }
            }

            if (move != Vector2.Zero)
            {
                OnCorrection(move);
                Aabb.Center += move;
            }
        }

        private void CalculateSensors(CollisionObject[] obstacles)
        {
            for (Int32 sensorIdx = 0; sensorIdx < _sensors.Count; ++sensorIdx)
            {
                Sensor sensor = _sensors[sensorIdx];

                sensor.Value = false;
                sensor.Update(Aabb.Center);

                if (obstacles != null)
                {
                    for (Int32 idx = 0; obstacles[idx] != null; ++idx)
                    {
                        CollisionObject simObj = obstacles[idx];

                        if (simObj.CausesCorrection)
                        {
                            if (CheckSite(sensor.CollisionSites, simObj.MovementCollisionSites) && sensor.Aabb.Intersects(ref simObj.Aabb))
                            {
                                sensor.Value = true;
                                OnSensedObstacle(sensorIdx, simObj);
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnSensedObstacle(Int32 sensorId, CollisionObject other)
        {

        }

        private Aabb GetMovementAabb(ref Vector2 velocity)
        {
            Vector2 center2 = Aabb.Center + velocity;
            return new Aabb((center2 + Aabb.Center) * 0.5f, new Vector2(Math.Abs(Aabb.Center.X - center2.X), Math.Abs(Aabb.Center.Y - center2.Y)) * 0.5f + Aabb.HalfSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obstacle"></param>
        private Vector2 CorrectPosition(CollisionObject obstacle, Vector2 currentMove)
        {
            Vector2 normal = Vector2.Zero;
            Single distance = Math.Max(obstacle.Aabb.HalfSize.X, obstacle.Aabb.HalfSize.Y) * 2;

            Aabb aabbObstacle = new Aabb(obstacle.Aabb.Center - Aabb.Center, obstacle.Aabb.HalfSize + Aabb.HalfSize);

            CollisionSites objectCollisionSites = obstacle.MovementCollisionSites;

            if (aabbObstacle.A.X < 0 && CheckSite(objectCollisionSites, CollisionSites.Right))
            {
                Single dist = Math.Abs(aabbObstacle.B.X);

                if (obstacle.Aabb.A.X - Aabb.A.X - World.Epsilon < 0
                    && dist <= distance + World.Epsilon)
                {
                    distance = dist;
                    normal = new Vector2(-1, 0);
                }

                distance = dist;
                normal = new Vector2(1, 0);
            }

            if (aabbObstacle.B.X > 0 && CheckSite(objectCollisionSites, CollisionSites.Left))
            {
                Single dist = Math.Abs(aabbObstacle.A.X);

                if ( obstacle.Aabb.B.X - Aabb.B.X + World.Epsilon > 0
                    && dist <= distance + World.Epsilon)
                {
                    distance = dist;
                    normal = new Vector2(-1, 0);
                }
            }

            if (aabbObstacle.A.Y < 0 && CheckSite(objectCollisionSites, CollisionSites.Bottom))
            {
                Single dist = Math.Abs(aabbObstacle.B.Y);

                if ( obstacle.Aabb.A.Y - Aabb.A.Y - World.Epsilon < 0
                    && dist <= distance + World.Epsilon)
                {
                    distance = dist;
                    normal = new Vector2(0, 1);
                }
            }

            if (aabbObstacle.B.Y > 0 && CheckSite(objectCollisionSites, CollisionSites.Top))
            {
                Single dist = Math.Abs(aabbObstacle.A.Y);

                if ( obstacle.Aabb.B.Y - Aabb.B.Y + World.Epsilon > 0
                     && dist <= distance + World.Epsilon)
                {
                    distance = dist;
                    normal = new Vector2(0, -1);
                }
            }

            Vector2 factor = Vector2.One;

            if (normal.X != 0)
            {
                Single diff = 1;

                if (Velocity.Y > 0)
                {
                    diff = Aabb.B.Y - obstacle.Aabb.A.Y;
                }
                else
                {
                    diff = obstacle.Aabb.B.Y - Aabb.A.Y;
                }

                diff = obstacle.CorrectionSiteSizes.Y > 0 ? diff / obstacle.CorrectionSiteSizes.Y : 1;

                diff = Math.Min(1, Math.Max(0, diff));
                factor.X = diff;
            }
            else if (normal.Y != 0)
            {
                Single diff = 1;

                if (Velocity.X > 0)
                {
                    diff = Aabb.B.X - obstacle.Aabb.A.X;
                }
                else
                {
                    diff = obstacle.Aabb.B.X - Aabb.A.X;
                }

                diff = obstacle.CorrectionSiteSizes.X > 0 ? diff / obstacle.CorrectionSiteSizes.X : 1;

                diff = Math.Min(1, Math.Max(0, diff));
                factor.Y = diff;
            }

            Vector2 correctionMove = (normal * distance) * factor;

            if (Math.Abs(currentMove.X) > Math.Abs(correctionMove.X))
            {
                correctionMove.X = currentMove.X;
            }

            if (Math.Abs(currentMove.Y) > Math.Abs(correctionMove.Y))
            {
                correctionMove.Y = currentMove.Y;
            }

            return correctionMove;
        }

        /// <summary>
        /// Check collision with obstacle.
        /// </summary>
        /// <param name="velocity">Velocity of object.</param>
        /// <param name="obstacle">Obstacle AABB.</param>
        /// /// <param name="collisionSites">Sites of object to check collision.</param>
        /// <returns></returns>
        private CollisionSites CheckCollision(ref Vector2 velocity, CollisionObject obstacle, Single slide, Boolean contactCollision)
        {
            CollisionSites collisionOccured = CollisionSites.None;

            Aabb aabbObstacle = new Aabb(obstacle.Aabb.Center - Aabb.Center, obstacle.Aabb.HalfSize + Aabb.HalfSize);

            CollisionSites obstacleCollisionSites = contactCollision ? obstacle.ContactCollisionSites : obstacle.MovementCollisionSites;

            Single multiply = 1;
            //Boolean moveHorizontal = Math.Abs(velocity.X) > Math.Abs(velocity.Y);

            Vector2 specVelocity = Vector2.Zero;

            if (velocity.Y < 0 && aabbObstacle.B.Y <= World.Epsilon && CheckSite(obstacleCollisionSites, CollisionSites.Bottom))
            {
                Single by = Math.Min(0, aabbObstacle.B.Y);

                if (velocity.Y <= by)
                {
                    multiply = by / velocity.Y;

                    specVelocity = velocity * multiply + new Vector2((1 - multiply) * velocity.X * slide, 0);

                    if (IsBetween(specVelocity.X, aabbObstacle.A.X, aabbObstacle.B.X))
                    {
                        if (!SkipMovementCollision(obstacle))
                        {
                            velocity = specVelocity;
                            collisionOccured |= CollisionSites.Top;
                        }
                    }
                }
            }
            else if (velocity.Y > 0 && aabbObstacle.A.Y >= -World.Epsilon && CheckSite(obstacleCollisionSites, CollisionSites.Top))
            {
                Single ay = Math.Max(0, aabbObstacle.A.Y);

                if (velocity.Y >= ay)
                {
                    multiply = ay / velocity.Y;

                    specVelocity = velocity * multiply + new Vector2((1 - multiply) * velocity.X * slide, 0);

                    if (IsBetween(specVelocity.X, aabbObstacle.A.X, aabbObstacle.B.X))
                    {
                        if (!SkipMovementCollision(obstacle))
                        {
                            velocity = specVelocity;
                            collisionOccured |= CollisionSites.Bottom;
                        }
                    }
                }
            }

            if (velocity.X < 0 && aabbObstacle.B.X <= World.Epsilon && CheckSite(obstacleCollisionSites, CollisionSites.Right))
            {
                Single bx = Math.Min(0, aabbObstacle.B.X);

                if (velocity.X <= bx)
                {
                    multiply = bx / velocity.X;

                    specVelocity = velocity * multiply + new Vector2(0, (1 - multiply) * velocity.Y * slide);

                    if (IsBetween(specVelocity.Y, aabbObstacle.A.Y, aabbObstacle.B.Y))
                    {
                        if (!SkipMovementCollision(obstacle))
                        {
                            velocity = specVelocity;
                            collisionOccured |= CollisionSites.Left;
                        }
                    }
                }
            }
            else if (velocity.X > 0 && aabbObstacle.A.X >= -World.Epsilon && CheckSite(obstacleCollisionSites, CollisionSites.Left))
            {
                Single ax = Math.Max(0, aabbObstacle.A.X);

                if (velocity.X >= ax)
                {
                    multiply = ax / velocity.X;

                    specVelocity = velocity * multiply + new Vector2(0, (1 - multiply) * velocity.Y * slide);

                    if (IsBetween(specVelocity.Y, aabbObstacle.A.Y, aabbObstacle.B.Y))
                    {
                        if (!SkipMovementCollision(obstacle))
                        {
                            velocity = specVelocity;
                            collisionOccured |= CollisionSites.Right;
                        }
                    }
                }
            }

            if ((collisionOccured & CollisionSites.Horz) != CollisionSites.None)
            {
                _frictionVector.X = Math.Max(_frictionVector.X, Friction * obstacle.Friction);
            }
            

            if ((collisionOccured&CollisionSites.Vert) != CollisionSites.None)
            {
                _frictionVector.Y = Math.Max(_frictionVector.Y, Friction * obstacle.Friction);
            }

            return collisionOccured;
        }

        private Boolean IsBetween(Single value, Single a, Single b)
        {
            return value > a && value < b;
        }

        protected Boolean CheckSite(CollisionSites value, CollisionSites site)
        {
            return ((value & site) != CollisionSites.None);
        }

        protected virtual void OnCorrection(Vector2 move)
        {

        }
    }
}
