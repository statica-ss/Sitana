using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;

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
