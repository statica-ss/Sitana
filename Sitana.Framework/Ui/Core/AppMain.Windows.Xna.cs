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
using System;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using Sitana.Framework.Diagnostics;

namespace Sitana.Framework.Ui.Core
{
    public partial class AppMain
    {
        public void ResizeToView()
        {
            if (!Graphics.IsFullScreen)
            {
                int width = MainView.PositionParameters.Width.Compute();
                int height = MainView.PositionParameters.Height.Compute();

                if (width > 0 && height > 0)
                {
                    Graphics.PreferredBackBufferWidth = width;
                    Graphics.PreferredBackBufferHeight = height;

                    MainView.Bounds = new Rectangle(0, 0, width, height);
                    Graphics.ApplyChanges();
                }
            }
        }

        public void OnSize(int width, int height)
        {
            if (MainView != null)
            {
                int newWidth = Math.Max(width, MainView.MinSize.X);
                int newHeight = Math.Max(height, MainView.MinSize.Y);

                var rect = new Rectangle(0, 0, width, height);
                PerformanceProfiler.Instance.ComputeContentRect(ref rect);

                MainView.Bounds = rect;

                if (Window.AllowUserResizing)
                {
                    Form gameForm = (Form)Form.FromHandle(Window.Handle);
                    gameForm.MinimumSize = new System.Drawing.Size(MainView.MinSize.X, MainView.MinSize.Y);
                }
            }
        }
    }
}