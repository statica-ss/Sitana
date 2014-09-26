using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ebatianos.Platform
{
    public interface IObjFinder
    {
        void Add(CollisionObject obj);
        void Remove(CollisionObject obj);
        void Get(ref Aabb area, CollisionObject[] buffer, ref Int32 startIndex, CollisionObject requestSource);
        void Move(CollisionObject obj);
        void Clear();
        void CleanUp();
    }
}
