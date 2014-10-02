using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Content;

namespace Sitana.Framework.Ui.Controllers
{
    public abstract class UiNavigationController: UiController
    {
        UiNavigationView _navigation = null;

        UiNavigationView Navigation
        {
            get
            {
                if ( _navigation == null )
                {
                    if (View is UiNavigationView)
                    {
                        _navigation = View as UiNavigationView;
                    }
                    else
                    {
                        _navigation = View.Parent as UiNavigationView;
                    }
                }

                return _navigation;
            }
        }

        public void NavigateTo(string uri)
        {
            DefinitionFile def = ContentLoader.Current.Load<DefinitionFile>(uri);
            Navigation.NavigateTo(def);
        }

        public void NavigateBack()
        {
            Navigation.NavigateBack();
        }

        public void NavigateBack(string anchor)
        {
            Navigation.NavigateBack(anchor);
        }
    }
}
