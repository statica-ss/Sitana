using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Content;
using Sitana.Framework.Input.GamePad;
using Microsoft.Xna.Framework.Input;

namespace Sitana.Framework.Ui.Controllers
{
    public abstract class UiNavigationController: UiController
    {
        UiNavigationView _navigation = null;

        protected UiNavigationView Navigation
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

        public virtual void InitPage(UiPage page, InvokeParameters parameters)
        {

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
            Navigation.NavigateTo(def, new InvokeParameters());
        }

        public void NavigateTo(string uri, params InvokeParam[] arguments)
        {
            InvokeParameters invokeParameters = new InvokeParameters();

            foreach (var param in arguments)
            {
                invokeParameters.Set(param);
            }

            DefinitionFile def = uri != null ? ContentLoader.Current.Load<DefinitionFile>(uri) : null;
            Navigation.NavigateTo(def, invokeParameters);
        }

        public void NavigateBack()
        {
            Navigation.NavigateBack();
        }

        public void NavigateBack(string anchor)
        {
            Navigation.NavigateBack(anchor);
        }

        public void ClearNavigation()
        {
            Navigation.ClearNavigation();
        }

        protected override void Update(float time)
        {
			if ( View.DisplayVisibility == 1 && GamePads.Instance[0].ButtonState(Buttons.Back) == GamePadButtonState.Pressed)
            {
                OnBack();
            }

            base.Update(time);
        }

        protected virtual void OnBack()
        {
            NavigateBack();
        }
    }
}
