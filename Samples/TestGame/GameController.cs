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

namespace SampleGame
{
    public class GameController : UiController
    {
        public GameController()
        {
        }
        
        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddSitanaFont("Font", "Fonts/Font", new int[]{10,14,18,22,28,32,38,44,50,58});
        }
        
        public void OpenLink(UiButton button)
        {
            Platform.OpenWebsite(button.Text.StringValue);
        }
    }
}