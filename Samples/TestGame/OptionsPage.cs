using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Views;

namespace TestGame
{
    public class OptionsPage: UiNavigationController
    {
        public void Next()
        {
            Find<UiContentSlider>("TestSlider").ShowNext();
        }

        public void Prev()
        {
            Find<UiContentSlider>("TestSlider").ShowPrev();
        }
    }
}
