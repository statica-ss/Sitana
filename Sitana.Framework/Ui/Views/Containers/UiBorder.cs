/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (C)2013-2014 Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF SEBASTIAN SEJUD AND IS NOT TO BE 
/// RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT THE EXPRESSED WRITTEN 
/// CONSENT OF SEBASTIAN SEJUD.
/// 
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. SEBASTIAN SEJUD GRANTS
/// TO YOU (ONE SOFTWARE DEVELOPER) THE LIMITED RIGHT TO USE THIS SOFTWARE 
/// ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// essentials@sejud.com
/// sejud.com/essentials-library
/// 
///---------------------------------------------------------------------------
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Core;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Ui.DefinitionFiles;
using System;

namespace Sitana.Framework.Ui.Views
{
    public class UiBorder : UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            foreach(var cn in node.Nodes)
            {
                switch ( cn.Tag )
                {
                    case "UiBorder.Children":
                        ParseChildren(cn, file);
                        break;
                }
            }
        }
    }
}
