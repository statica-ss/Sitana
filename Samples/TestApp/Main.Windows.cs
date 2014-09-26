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
    public class TestController: UiController
    {
        public TestController()
        {
            TextColor = new ColorWrapper();
        }

        void ChangeColors()
        {
            Random random = new Random();
            while(true)
            {
                _color.Value = new Color(random.Next(255), random.Next(255), random.Next(255));
                Thread.Sleep(500);
            }
        }

        protected void OnClick(UiButton button)
        {
            Console.WriteLine("Test button");
        }

        public bool GetVisible(int val)
        {
            return val > 4;
        }

        public string Test(string text)
        {
            return text;
        }

        public StringBuilder Test2()
        {
            _text.Append("Another text got from controller.");
            return _text;
        }

        public ColorWrapper TextColor { get; private set; }

        ColorWrapper _color = new ColorWrapper(Color.Green);
        StringBuilder _text = new StringBuilder();
    }

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            using (var main = new AppMain())
            {
                ContentLoader.Init(main.Content, "TestApp");

                main.LoadView("Ui/MainView");

                main.Window.AllowUserResizing = true;

                main.Graphics.IsFullScreen = false;

                main.IsMouseVisible = true;
                main.OnLoadContent += OnLoadContent;
                main.OnLoadedView += (a) => a.ResizeToView();

                main.Run();
            }
        }

        static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddFont("TestFont", "Fonts/TestFont", 16);
        }
    }
}
