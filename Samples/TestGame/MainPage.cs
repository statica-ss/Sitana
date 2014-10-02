using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;

namespace TestGame
{
    public class MainPage : UiPageController
    {
        public void OnClick()
        {
            NavigateTo("Ui/MainPage");
        }
    }
}
