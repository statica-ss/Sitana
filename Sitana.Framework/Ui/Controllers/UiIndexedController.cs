using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Controllers
{
    public class UiIndexedController: UiController
    {
        IIndexedElement _indexedElement;

        public UiIndexedController(IIndexedElement indexedElement)
        {
            _indexedElement = indexedElement;
        }

        public void Select(int index)
        {
            _indexedElement.SelectedIndex = index;
        }
    }
}
