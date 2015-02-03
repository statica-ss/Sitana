using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Binding;
using Sitana.Framework.Cs;

namespace SampleGame
{
    public class MainPage : UiNavigationController
    {
        public void OnNewGame()
        {
            NavigateTo("Ui/MainPage");
        }
    }
}
