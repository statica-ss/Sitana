using System;
using Ebatianos.Content;
using Ebatianos.Ui.Core;
using Microsoft.Xna.Framework;
using Ebatianos.Ui.Views;
using Ebatianos;
using Ebatianos.Input.TouchPad;
using Ebatianos.Ui.Controllers;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Ebatianos.Ui.DefinitionFiles;
using Ebatianos.Ui.Views.ButtonDrawables;
using System.Threading;
using Ebatianos.Essentials.Ui.DefinitionFiles;
using Ebatianos.Diagnostics;

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
            _text.Append("tteteywteyuwq euyqwt ");
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
                ContentLoader.Init(main.Content, "Assets");

                main.LoadView("Ui/MainView");

                main.Window.AllowUserResizing = true;

                main.Graphics.IsFullScreen = false;

                main.IsMouseVisible = true;
                main.OnLoadContent += OnLoadContent;
                main.OnLoadedView += (a) => a.ResizeToView();

                main.Graphics.PreferredBackBufferWidth = 1280;
                main.Graphics.PreferredBackBufferHeight = 720;

                main.Run();
            }
        }

        static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddFont("TestFont", "Fonts/TestFont", 22);
        }
    }
}
