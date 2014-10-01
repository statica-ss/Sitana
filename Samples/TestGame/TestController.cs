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

namespace TestGame
{
    public class GameController : UiController
    {
        public GameController()
        {
        }
        
        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddFont("TestFont", "Fonts/TestFont", new int[]{6,8,10,12,14,16,20,24,28,32,38});
        }
    }
}