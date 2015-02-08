using System;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Cs;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Diagnostics;

namespace SampleGame
{
    public class GameController : UiController
    {
        public GameController()
        {
        }
        
        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddSitanaFont("Font", "Fonts/Font", 10,14,18,22,28,32,38,44,50,58);

            PerformanceProfiler.Instance.Initialize(60, 30);

            UpdateScale(main);
        }

        public static void UpdateScale(AppMain main)
        {
            double scale = Math.Min((double)main.GraphicsDevice.Viewport.Width / 640.0, (double)main.GraphicsDevice.Viewport.Height / 480.0);
            scale = Math.Round(scale, 1);

            UiUnit.FontScaling = UiUnit.ScalingMode.Floating;
            UiUnit.FontUnit = scale;
            UiUnit.Unit = scale;
        }

        public void OpenLink(UiButton button)
        {
            Platform.OpenWebsite(button.Text.StringValue);
        }
    }
}