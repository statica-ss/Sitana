
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
using Sitana.Framework.Ui.Binding;

namespace TestApp
{
    public class MainController : UiController
    {   
        class Test
        {
            public SharedString Name { get; set; }
            public SharedString Age {get;set;}
        }

        ItemsList<Test> _list = new ItemsList<Test>();

        public MainController()
        {
            Random rand = new Random();

            for (int idx = 0; idx < 100; ++idx)
            {
                _list.Add(new Test()
                {
                    Name = idx.ToString(),
                    Age = String.Format("{0}", rand.Next())
                });
            }
        }

        public IItemsProvider Items
        {
            get
            {
                return _list;
            }
        }

        public void Next()
        {
            Find<UiContentSlider>("TestSlider").ShowNext();
        }

        public void Prev()
        {
            Find<UiContentSlider>("TestSlider").ShowPrev();
        }

        public void OpenLink(UiButton button)
        {
            SystemWrapper.OpenWebsite(button.Text);
        }

        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddSpriteFont("TestFont", "Fonts/Font", new int[]{8,12,16,20,24});
        }
    }
}