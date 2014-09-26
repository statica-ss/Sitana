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
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Sitana.Framework.Gui.List
{
    public abstract class ListItemElement
    {
        public abstract void Init(XmlFileNode elementNode);
        public abstract ListItemElement Clone(Object bind);

        public abstract void Draw(SpriteBatch spriteBatch, Vector2 offset, Single scale, Single itemHeight, Single expanding, Single opacity);

        protected Object Bind { get; set; }

        public Int32 Bottom { get; protected set; }

        public virtual Boolean Update()
        {
            return false;
        }

        public virtual Boolean UpdateUi(Single time)
        {
            return false;
        }

        public virtual Boolean OnGesture(GestureType type, Vector2 position, params Object[] parameters)
        {
            return false;
        }
    }
}
