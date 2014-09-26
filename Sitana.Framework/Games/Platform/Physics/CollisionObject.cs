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
using Microsoft.Xna.Framework;

namespace Ebatianos.Platform
{
    public class CollisionObject
    {
        public Aabb Aabb;
        public Single Friction;
        public CollisionSites MovementCollisionSites = CollisionSites.All;
        public CollisionSites ContactCollisionSites = CollisionSites.All;
        public Boolean CausesCorrection = true;
        public Boolean IncludeIntoCollisionObjects = true;

        public Int32 CollisionGroupId = 1;
        public Int32 CollisionGroups = 0;

        public Vector2 CorrectionSiteSizes = Vector2.Zero;

        public Boolean Moved { get; protected set;}

        public virtual Boolean Enabled
        {
            get
            {
                return true;
            }
        }

        internal QuadNode QuadNode {get; set;}

        public CollisionObject()
        {
            MovementCollisionSites = CollisionSites.All;
            ContactCollisionSites = CollisionSites.All;
            Friction = 10;
            Moved = false;
        }

        public virtual void OnCollision(CollisionObject other, Boolean contactCollision)
        {

        }

        public virtual void OnCollisionReflect(CollisionObject other, Boolean contactCollision)
        {

        }

        public virtual Boolean SkipMovementCollision(CollisionObject other)
        {
            return false;
        }

        public Boolean CheckCollisionGroup(Int32 group)
        {
            return group != 0 && (CollisionGroups & group) == group;
        }

        public Boolean CheckCollisionId(Int32 group)
        {
            return group != 0 && (CollisionGroupId & group) == group;
        }
    }
}
