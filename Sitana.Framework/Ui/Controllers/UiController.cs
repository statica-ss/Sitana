using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sitana.Framework.Ui.Controllers
{
    public abstract class UiController
    {
        public UiView View { get; private set; }

        Dictionary<string, UiView> _views = new Dictionary<string, UiView>();

        internal void AttachView(UiView view)
        {
            View = view;
            OnViewAttached();
        }

        protected virtual void OnViewAttached()
        {
        }

        public TYPE Find<TYPE>(string id) where TYPE: UiView
        {
            var view = Find(id);

            if ( view is TYPE)
            {
                return (TYPE)view;
            }

            return default(TYPE);
        }

        public UiView Find(string id)
        {
            UiView view;

            _views.TryGetValue(id, out view);
            return view;
        }

        internal void Register(string id, UiView view)
        {
            _views.Add(id, view);
        }

        internal void Unregister(string id, UiView view)
        {
            if (Find(id) == view)
            {
                _views.Remove(id);
            }
        }

        internal void UpdateInternal(float time)
        {
            Update(time);
        }

        protected virtual void Update(float time)
        {
        }

        public void ShowElement(string id)
        {
            Find(id).Visible.Value = true;
        }

        public void HideElement(string id)
        {
            Find(id).Visible.Value = false;
        }
    }
}
