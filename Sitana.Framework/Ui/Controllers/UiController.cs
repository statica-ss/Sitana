using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace Sitana.Framework.Ui.Controllers
{
    public abstract class UiController
    {
        public UiView View { get; private set; }

        Dictionary<string, UiView> _views = new Dictionary<string, UiView>();

        public UiController Parent { get; internal set;}

        protected bool RegisterElementsInParent { private get; set; }

        public UiController()
        {
            RegisterElementsInParent = true;
        }

        internal void AttachView(UiView view)
        {
            View = view;
            OnViewAttached();
        }

        protected virtual void OnViewAttached()
        {
        }

        public TYPE FindParent<TYPE>() where TYPE: UiController
        {
            if (Parent == null)
            {
                return null;
            }

            if(Parent is TYPE)
            {
                return Parent as TYPE;
            }

            return Parent.FindParent<TYPE>();
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

            if ( view == null )
            {
                if ( Parent != null )
                {
                    return Parent.Find(id);
                }
            }

            return view;
        }

        internal void Register(string id, UiView view)
        {
            Debug.Assert(!_views.ContainsKey(id), "Cannot duplicate view ids!");

            _views.Add(id, view);

            if (RegisterElementsInParent && Parent != null)
            {
                Parent.Register(id, view);
            }
        }

        internal void Unregister(string id, UiView view)
        {
            if (Find(id) == view)
            {
                _views.Remove(id);
            }

            if ( RegisterElementsInParent && Parent != null )
            {
                Parent.Unregister(id, view);
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
            Find(id).Visible = true;
        }

        public void HideElement(string id)
        {
            Find(id).Visible = false;
        }

        public void OpenWebsite(string url)
        {
            Platform.OpenWebsite(url);
        }

        protected void ForceUpdate(float time)
        {
            View.ViewUpdate(time);
        }
    }
}
