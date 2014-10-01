using System;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.Core;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views;
using Sitana.Framework;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Controllers;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.ButtonDrawables;
using System.Threading;
using Sitana.Framework.Essentials.Ui.DefinitionFiles;
using Sitana.Framework.Diagnostics;

namespace TestApp
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var main = new AppMain())
            {
                ContentLoader.Init(main.Content, "TestApp");

                UiUnit.Unit = 1;
                UiUnit.FontUnit = 1;

                StylesManager.Instance.LoadStyles("Ui/AppStyles", true);
                main.LoadView("Ui/MainView");

                main.Window.AllowUserResizing = true;

                main.Graphics.IsFullScreen = false;

                main.IsMouseVisible = true;
                main.OnLoadContent += TestController.OnLoadContent;
                main.OnLoadedView += (a) => a.ResizeToView();

                main.Run();
            }
        }
    }
}
