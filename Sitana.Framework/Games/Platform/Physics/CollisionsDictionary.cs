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

namespace Ebatianos.Platform
{
    public class CollisionsDictionary
    {

        List<KeyValuePair<CollisionObject, Boolean>> _list = new List<KeyValuePair<CollisionObject, Boolean>>();

        public CollisionsDictionary()
        {
            _list.Capacity = 20;
        }

        public void Add(CollisionObject key, Boolean value)
        {
            for (Int32 idx = 0; idx < _list.Count; ++idx)
            {
                if (_list[idx].Key == key)
                {
                    if (_list[idx].Value == true && value == false)
                    {
                        _list[idx] = new KeyValuePair<CollisionObject, Boolean>(key, value);
                    }
                }
            }

            _list.Add(new KeyValuePair<CollisionObject, Boolean>(key, value));
        }

        public void Clear()
        {
            _list.Clear();
        }

        public Boolean FindAt(CollisionObject key)
        {
            for (Int32 idx = 0; idx < _list.Count; ++idx)
            {
                if (_list[idx].Key == key)
                {
                    return _list[idx].Value;
                }
            }

            return false;
        }

        public Int32 Count
        {
            get
            {
                return _list.Count;
            }
        }

        public KeyValuePair<CollisionObject, Boolean> this[Int32 index]
        {
            get
            {
                return _list[index];
            }
        }

        public void At(Int32 index, ref KeyValuePair<CollisionObject, Boolean> val)
        {
            val = _list[index];
        }

        public Boolean ContainsKey(CollisionObject key)
        {
            for (Int32 idx = 0; idx < _list.Count; ++idx)
            {
                if (_list[idx].Key == key)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

