using Ebatianos.Cs;
using Ebatianos.Ui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ebatianos.Ui.Controllers
{
    public abstract class UiController
    {
        public UiView View { get; private set; }

        internal void AttachView(UiView view)
        {
            View = view;
            OnViewAttached();
        }

        protected virtual void OnViewAttached()
        {
        }

        protected TYPE Find<TYPE>(string id) where TYPE: UiView
        {
            var view = Find(id);

            if ( view is TYPE)
            {
                return (TYPE)view;
            }

            return default(TYPE);
        }

        protected UiView Find(string id)
        {
            UiContainer container = View as UiContainer;

            if (container != null)
            {
                return container.ViewFromId(id);
            }

            return null;
        }
    }
}
