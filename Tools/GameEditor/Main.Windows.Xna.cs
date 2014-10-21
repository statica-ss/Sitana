using System;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.Core;
using Sitana.Framework;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;

namespace GameEditor
{
    public static class Program
    {
        static AppMain _appMain;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (_appMain = new AppMain())
            {

                ContentLoader.Init(_appMain.Services, Assembly.GetExecutingAssembly(), "GameEditor.Assets");

                StylesManager.Instance.LoadStyles("AppStyles", true);
                _appMain.LoadView("MainView");

                _appMain.Window.AllowUserResizing = true;
                _appMain.Window.ClientSizeChanged += Window_ClientSizeChanged;

                _appMain.Graphics.IsFullScreen = false;
                _appMain.CanClose = (a) =>
                    {
                        if (!MainController.Current.CanClose)
                        {
                            // This makes the game remain active after close button is pressed.
                            Form form = (Form)Form.FromHandle(_appMain.Window.Handle);
                            form.Hide();
                            form.Show();

                            MainController.Current.Exit();
                        }

                        return MainController.Current.CanClose;
                    };

                _appMain.IsMouseVisible = true;
                _appMain.OnLoadContent += MainController.OnLoadContent;
                _appMain.OnLoadedView += (a) => a.ResizeToView();

                _appMain.InactiveSleepTime = TimeSpan.FromSeconds(1);

                _appMain.Run();
            }
        }

        static void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            UiTask.BeginInvoke(() =>
            {
                double unit = Math.Min((double)AppMain.Current.GraphicsDevice.Viewport.Width / 640.0,
                                        (double)AppMain.Current.GraphicsDevice.Viewport.Height / 480.0);

                unit = Math.Min(1, unit);

                UiUnit.FontUnit = UiUnit.Unit = unit;
                AppMain.Current.SizeChanged();
            });
        }

    }
}
