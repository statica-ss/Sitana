using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.Views;
using Sitana.Framework;
using Sitana.Framework.Ui.Controllers;

namespace GameEditor
{
    public class MainController: UiController
    {
        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddSpriteFont("Font", "Fonts/Font", new int[] { 8, 12, 16, 20, 24 });
        }

        public void OpenLink(UiButton sender)
        {
            SystemWrapper.OpenWebsite(sender.Text.StringValue);
        }

        public void Open()
        {
            string path = SystemWrapper.OpenFileDialog("Open file");

            if ( path != null )
            {
                Console.WriteLine(path);
            }
        }
    }
}
