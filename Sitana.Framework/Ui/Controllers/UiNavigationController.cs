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
                    else if (View is UiPage)
                    {
                        _navigation = View.Parent as UiNavigationView;
                    }
                }

                return _navigation;
            }
        }

        public UiNavigationController()
        {
            RegisterElementsInParent = false;
        }

        protected override void OnViewAttached()
        {
            if ( !(View is UiPage) && !(View is UiNavigationView))
            {
                throw new InvalidOperationException("Attached view must be either UiNavigationView or UiPage.");
            }
        }

        public void NavigateTo(string uri)
        {
            DefinitionFile def = uri != null ? ContentLoader.Current.Load<DefinitionFile>(uri) : null;
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
