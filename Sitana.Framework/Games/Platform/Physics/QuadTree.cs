using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ebatianos.Platform
{
    class QuadNode
    {
        private QuadTree _tree;
        //private QuadNode _parent;

        private const Int32 _initialBufferSize = 64;

        private CollisionObject[] _elementsToCheck = new CollisionObject[_initialBufferSize];
        private List<CollisionObject> _elements = new List<CollisionObject>();

        private QuadNode _nodeTopLeft = null;
        private QuadNode _nodeTopRight = null;
        private QuadNode _nodeBottomLeft = null;
        private QuadNode _nodeBottomRight = null;

        private Boolean _nodesHaveData = false;

        public Aabb Area { get; private set; }

        private Int32 _collisionGroups = 0;

        internal Int32 CollisionGroupsAll
        {
            get
            {
                Int32 groups = _collisionGroups;

                if ( _nodesHaveData )
                {
                    groups |= _nodeTopLeft.CollisionGroupsAll;
                    groups |= _nodeTopRight.CollisionGroupsAll;
                    groups |= _nodeBottomLeft.CollisionGroupsAll;
                    groups |= _nodeBottomRight.CollisionGroupsAll;
                }

                return groups;
            }
        }

        public Int32 CollisionGroupsToCheck {get; private set;}

        private void Init(QuadTree tree, QuadNode parent)
        {
            _tree = tree;
            //_parent = parent;

            CollisionGroupsToCheck = 0;
        }

        public QuadNode(QuadTree tree, QuadNode parent, Single width, Single height)
        {
            Area = new Aabb(new Vector2(width / 2, height / 2), new Vector2(width / 2, height / 2));
            Init(tree, parent);
        }

        public QuadNode(QuadTree tree, QuadNode parent, Single left, Single top, Single width, Single height)
        {
            Area = new Aabb(new Vector2(left + width / 2, top + height / 2), new Vector2(width / 2, height / 2));
            Init(tree, parent);
        }

        public QuadNode(QuadTree tree, QuadNode parent, Aabb area)
        {
            Area = area;
            Init(tree, parent);
        }

        public void Add(CollisionObject obj, Boolean createNewNodes = true)
        {
            if (createNewNodes && !_nodesHaveData && _elements.Count >= _tree.MaxObjectsPerNode)
            {
                MoveElementsToSubnodes();
            }

            if (!AddToSubNodes(obj, createNewNodes))
            {
                _elements.Add(obj);
                obj.QuadNode = this;

                _collisionGroups |= obj.CollisionGroupId;

                UpdateBuffer();
            }
        }

        private void UpdateBuffer()
        {
            if ( _elementsToCheck.Length < _elements.Count + 1 )
            {
                _elementsToCheck = new CollisionObject[_elements.Count + _initialBufferSize];
            }

            Int32 idx = 0;
            for ( idx = 0; idx < _elements.Count; ++idx )
            {
                _elementsToCheck[idx] = _elements[idx];
            }

            _elementsToCheck[idx] = null;
        }

        private void MoveElementsToSubnodes()
        {
            if (_nodeTopLeft == null)
            {
                if (Area.HalfSize.X > Area.HalfSize.Y * 3)
                {
                    Single quadWidth = Area.Width / 4;

                    _nodeTopLeft = new QuadNode(_tree, this, Area.Left + quadWidth * 0, Area.Top, quadWidth, Area.Height);
                    _nodeTopRight = new QuadNode(_tree, this, Area.Left + quadWidth * 1, Area.Top, quadWidth, Area.Height);

                    _nodeBottomLeft = new QuadNode(_tree, this, Area.Left + quadWidth * 2, Area.Center.Y, quadWidth, Area.Height);
                    _nodeBottomRight = new QuadNode(_tree, this, Area.Left * quadWidth * 3, Area.Top, quadWidth, Area.Height);
                }
                else if (Area.HalfSize.Y > Area.HalfSize.X * 3)
                {
                    Single quadHeight = Area.Height / 4;

                    _nodeTopLeft = new QuadNode(_tree, this, Area.Left, Area.Top + quadHeight * 0, Area.Width, quadHeight);
                    _nodeTopRight = new QuadNode(_tree, this, Area.Left, Area.Top + quadHeight * 1, Area.Width, quadHeight);

                    _nodeBottomLeft = new QuadNode(_tree, this, Area.Left, Area.Top + quadHeight * 2, Area.Width, quadHeight);
                    _nodeBottomRight = new QuadNode(_tree, this, Area.Left, Area.Top + quadHeight * 3, Area.Width, quadHeight);
                }
                else
                {
                    _nodeTopLeft = new QuadNode(_tree, this, Area.Left, Area.Top, Area.Width / 2, Area.Height / 2);
                    _nodeTopRight = new QuadNode(_tree, this, Area.Center.X, Area.Top, Area.Width / 2, Area.Height / 2);

                    _nodeBottomLeft = new QuadNode(_tree, this, Area.Left, Area.Center.Y, Area.Width / 2, Area.Height / 2);
                    _nodeBottomRight = new QuadNode(_tree, this, Area.Center.X, Area.Center.Y, Area.Width / 2, Area.Height / 2);
                }
            }

            _nodesHaveData = true;

            for (Int32 idx = 0; idx < _elements.Count;)
            {
                var element = _elements[idx];

                if (AddToSubNodes(element, true))
                {
                    _elements.RemoveAt(idx);
                }
                else
                {
                    ++idx;
                }
            }
                
            UpdateBuffer();
        }

        private Boolean AddToNode(QuadNode node, CollisionObject obj, Boolean createNewNodes)
        {
            if (node.Area.Contains(ref obj.Aabb))
            {
                node.Add(obj, createNewNodes);
                return true;
            }

            return false;
        }

        private Boolean AddToSubNodes(CollisionObject obj, Boolean createNewNodes)
        {
            if (!_nodesHaveData)
            {
                return false;
            }

            if (AddToNode(_nodeTopLeft, obj, createNewNodes))
            {
                return true;
            }

            if (AddToNode(_nodeBottomLeft, obj, createNewNodes))
            {
                return true;
            }

            if (AddToNode(_nodeBottomRight, obj, createNewNodes))
            {
                return true;
            }

            if (AddToNode(_nodeBottomRight, obj, createNewNodes))
            {
                return true;
            }

            return false;
        }

        public void Remove(CollisionObject obj)
        {
            _elements.Remove(obj);
            obj.QuadNode = null;

            _collisionGroups = 0;

            for ( Int32 idx = 0; idx < _elements.Count; ++idx )
            {
                _collisionGroups |= _elements[idx].CollisionGroupId;
            }

            UpdateBuffer();
        }

        private void GetAll(CollisionObject[] buffer, ref Int32 startIndex, CollisionObject requestSource)
        {
            if ((CollisionGroupsToCheck & requestSource.CollisionGroups) == 0)
            {
                return;
            }

            for (Int32 idx = 0; _elementsToCheck[idx] != null; ++idx)
            {
                var element = _elementsToCheck[idx];

                if (element.Enabled && element != requestSource && requestSource.CheckCollisionGroup(element.CollisionGroupId))
                {
                    buffer[startIndex] = element;
                    ++startIndex;
                }
            }

            if (_nodesHaveData)
            {
                _nodeBottomLeft.GetAll(buffer, ref startIndex, requestSource);
                _nodeBottomRight.GetAll(buffer, ref startIndex, requestSource);
                _nodeTopLeft.GetAll(buffer, ref startIndex, requestSource);
                _nodeTopRight.GetAll(buffer, ref startIndex, requestSource);
            }
        }

        public void Get(ref Aabb area, CollisionObject[] buffer, ref Int32 startIndex, CollisionObject requestSource)
        {
            if ((CollisionGroupsToCheck & requestSource.CollisionGroups) == 0)
            {
                return;
            }

            if (area.Contains(Area))
            {
                GetAll(buffer, ref startIndex, requestSource);
            }
            else if (area.Intersects(Area))
            {
                for (Int32 idx = 0; _elementsToCheck[idx] != null; ++idx)
                {
                    var element = _elementsToCheck[idx];

                    if (element.Enabled && element != requestSource && requestSource.CheckCollisionGroup(element.CollisionGroupId) && area.Intersects(ref element.Aabb))
                    {
                        buffer[startIndex] = element;
                        ++startIndex;
                    }
                }

                if (_nodesHaveData)
                {
                    _nodeBottomLeft.Get(ref area, buffer, ref startIndex, requestSource);
                    _nodeBottomRight.Get(ref area, buffer, ref startIndex, requestSource);
                    _nodeTopLeft.Get(ref area, buffer, ref startIndex, requestSource);
                    _nodeTopRight.Get(ref area, buffer, ref startIndex, requestSource);
                }
            }
        }

        public void CleanUp()
        {
            if (_nodesHaveData)
            {
                _nodeTopLeft.CleanUp();
                _nodeBottomLeft.CleanUp();

                _nodeTopRight.CleanUp();
                _nodeBottomRight.CleanUp();

                if (_nodeBottomLeft.IsEmpty && _nodeTopLeft.IsEmpty && _nodeBottomRight.IsEmpty && _nodeTopRight.IsEmpty)
                {
                    _nodesHaveData = false;
                }
            }
        }

        public Boolean IsEmpty
        {
            get
            {
                if (_elements.Count > 0)
                {
                    return false;
                }

                if (_nodeBottomLeft != null)
                {
                    return _nodeBottomLeft.IsEmpty && _nodeTopLeft.IsEmpty && _nodeBottomRight.IsEmpty && _nodeTopRight.IsEmpty;
                }

                return true;
            }
        }

        internal void UpdateCollisionGroups()
        {
            CollisionGroupsToCheck = CollisionGroupsAll;

            if ( _nodesHaveData )
            {
                _nodeBottomLeft.UpdateCollisionGroups();
                _nodeTopLeft.UpdateCollisionGroups();
                _nodeBottomRight.UpdateCollisionGroups();
                _nodeTopRight.UpdateCollisionGroups();
            }
        }
    }

    public class QuadTree : IObjFinder
    {
        private QuadNode _root;

        internal Int32 MaxObjectsPerNode = 2;

        public QuadTree(Single x, Single y, Single width, Single height)
        {
            _root = new QuadNode(this, null, x, y, width, height);
        }

        public void Add(CollisionObject obj)
        {
            _root.Add(obj);
            _root.UpdateCollisionGroups();
        }

        public void Remove(CollisionObject obj)
        {
            if (obj.QuadNode != null)
            {
                obj.QuadNode.Remove(obj);
            }

            _root.UpdateCollisionGroups();
        }

        public void Get(ref Aabb area, CollisionObject[] buffer, ref Int32 startIndex, CollisionObject requestSource)
        {
            _root.Get(ref area, buffer, ref startIndex, requestSource);
        }

        public void Move(CollisionObject obj)
        {
            if (obj.QuadNode == null)
            {
                return;
            }

            if (!obj.QuadNode.Area.Contains(obj.Aabb))
            {   
                Remove(obj);
                _root.Add(obj, true);
                _root.UpdateCollisionGroups();
            }
        }

        public void Clear()
        {
            _root = new QuadNode(this, null, _root.Area);
        }

        public void CleanUp()
        {
            _root.CleanUp();
        }
    }
}
