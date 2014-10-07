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
        public void OnNewGame()
        {
            NavigateTo("Ui/MainPage");
        }
    }
}
