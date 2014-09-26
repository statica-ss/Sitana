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
using Ebatianos.Graphics;
using Ebatianos.Ui.Views;
using Ebatianos.Ui.Views.Parameters;
using Microsoft.Xna.Framework;

namespace Ebatianos.Ui.Core
{
    public partial class AppMain
    {
        protected override void BeginRun()
        {
            base.BeginRun();

            Window.ClientSizeChanged += (o, e) =>
            {
                if (Window.ClientBounds.IsEmpty)
                {
                    return;
                }

                if (MainView != null)
                {
                    int width = Math.Max(Window.ClientBounds.Width, MainView.MinSize.X);
                    int height = Math.Max(Window.ClientBounds.Height, MainView.MinSize.Y);

                    MainView.Bounds = new Rectangle(0, 0, width, height);

                    Graphics.PreferredBackBufferWidth = width;
                    Graphics.PreferredBackBufferHeight = height;
                    Graphics.ApplyChanges();
                }

                if (Draw())
                {
                    GraphicsDevice.Present();
                }
            };
        }

        public void ResizeToView()
        {
            Graphics.PreferredBackBufferWidth = MainView.PositionParameters.Width.Compute(100);
            Graphics.PreferredBackBufferHeight = MainView.PositionParameters.Height.Compute(100);

            MainView.Bounds = new Rectangle(0, 0, MainView.PositionParameters.Width.Compute(100), MainView.PositionParameters.Height.Compute(100));

            Graphics.ApplyChanges();
        }
    }
}