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
using Sitana.Framework.Diagnostics;

namespace TestGame
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
                Init(main);
                main.Run();
            }
        }

        static void Init(AppMain main)
        {
            ContentLoader.Init(main.Content, "TestGame");

            StylesManager.Instance.LoadStyles("Ui/AppStyles", true);
            main.LoadView("Ui/MainView");

            main.Window.AllowUserResizing = true;
            main.Window.ClientSizeChanged += Window_ClientSizeChanged;
            main.Graphics.IsFullScreen = false;

            main.IsMouseVisible = true;
            main.OnLoadContent += GameController.OnLoadContent;
            main.OnLoadedView += (s)=>s.ResizeToView();
        }

        static void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            UiTask.BeginInvoke(() =>
                {
                    double unit = Math.Min((double)AppMain.Current.GraphicsDevice.Viewport.Height / (double)AppMain.Current.MainView.PositionParameters.Height.Value,
                        (double)AppMain.Current.GraphicsDevice.Viewport.Width / (double)AppMain.Current.MainView.PositionParameters.Width.Value);

                    unit = Math.Round(unit, 1);

                    UiUnit.FontUnit = UiUnit.Unit = unit;
                    AppMain.Current.MainView.RecalculateAll();
                });
        }
    }
}
