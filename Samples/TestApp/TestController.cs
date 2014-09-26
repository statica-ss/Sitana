
using System;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.Views;

namespace TestApp
{

    public class TestController : UiController
    {
        public TestController()
        {
            TextColor = new ColorWrapper();
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

        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddFont("TestFont", "Fonts/TestFont", 16);
        }
    }
}