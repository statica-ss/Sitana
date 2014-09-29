
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

namespace TestApp
{

    public class TestController : UiController
    {
        private bool _continueWork = true;
        private Thread _workingThread;

        public TestController()
        {
            TextColor = new ColorWrapper();

            _workingThread = new Thread(new ThreadStart(DoWork));
            _workingThread.Start();
        }

        void DoWork()
        {
            Random random = new Random();
            while (_continueWork)
            {
                Color color = new Color(random.Next(256), random.Next(256), random.Next(256));
                _text.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
                _color.Value = color;

                Thread.Sleep(500);
            }
        }

        public void OnClick(UiButton button, object binding, int index)
        {
            Console.WriteLine("Test button {0} {1}", index, binding);
        }

        public bool GetVisible(int val)
        {
            return val > 4;
        }

        public string Test(string text)
        {
            return text;
        }

        public SharedString Test2()
        {
            return _text;
        }

        public void OnViewRemoved()
        {
            _continueWork = false;
        }

        public ColorWrapper TextColor { get; private set; }

        ColorWrapper _color = new ColorWrapper(Color.Green);
        SharedString _text = new SharedString();

        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddFont("TestFont", "Fonts/TestFont", 16);
        }
    }
}