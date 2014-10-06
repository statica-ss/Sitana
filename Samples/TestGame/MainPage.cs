using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Binding;
using Sitana.Framework.Cs;

namespace TestGame
{
    public class MainPage : UiNavigationController
    {
        class Test
        {
            public SharedString Name { get; set; }
            public SharedString Age {get;set;}
        }

        ItemsList<Test> _list = new ItemsList<Test>();

        public MainPage()
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

        public void OnNewGame()
        {
            NavigateTo("Ui/MainPage");
        }

        public IItemsProvider Items
        {
            get
            {
                return _list;
            }
        }
    }
}
