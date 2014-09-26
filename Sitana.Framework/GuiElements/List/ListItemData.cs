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

namespace Sitana.Framework.Gui.List
{
    public abstract class ListItemData
    {
        private Boolean _update = false;
        public readonly Object Lock = new Object();

        private Boolean _expanded = false;
        private Boolean _showSeparator = true;

        public String Template { get; protected set; }

        public ListItemData()
        {
            Template = String.Empty;
        }

        public ListItemData(String template)
        {
            Template = template;
        }

        public Boolean ShowSeparator
        {
            get
            {
                lock (Lock)
                { 
                    return _showSeparator;
                }
            }

            set
            {
                lock (Lock)
                { 
                    _showSeparator = value;
                }
            }
        }

        public Boolean IsExpanded
        {
            get
            {
                lock (Lock)
                { 
                    return _expanded;
                }
            }

            set
            {
                lock (Lock)
                { 
                    _expanded = value;
                    Update();
                }
            }
        }

        public virtual void Update()
        {
            lock (Lock)
            {
                _update = true;
            }
        }

        public Boolean ShouldUpdate
        {
            get
            {
                lock (Lock)
                {
                    Boolean update = _update;
                    _update = false;
                    return update;
                }
            }
        }

        public virtual void OnRemoved()
        {
        }

        public virtual void OnAdded()
        {
        }
    }
}
