using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Controllers
{
    public class UiPageController: UiController
    {
        UiNavigationView _navigation = null;

        UiNavigationView Navigation
        {
            get
            {
                if ( _navigation == null )
                {
                    _navigation = View.Parent as UiNavigationView;
                }

                return _navigation;
            }
        }

        protected void NavigateTo(string uri)
        {
            DefinitionFile def = null;
            _navigation.NavigateTo(def);
        }

        protected void NavigateBack()
        {
            _navigation.NavigateBack();
        }

        protected void NavigateBack(string anchor)
        {
            _navigation.NavigateBack(anchor);
        }
    }
}
