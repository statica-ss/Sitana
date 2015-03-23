// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Xml;
using Sitana.Framework.Content;
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework.Ui.Controllers;

namespace Sitana.Framework.Ui.Views
{
    public class UiNavigationView: UiContainer
    {
        public delegate void OnPageAddedDelegate(UiPage page);

        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            DefinitionParser parser = new DefinitionParser(node);

            file["Page"] = parser.ParseString("Page");
        }

        public event OnPageAddedDelegate OnPageAdded;

        List<Tuple<DefinitionFile, InvokeParameters>> _history = new List<Tuple<DefinitionFile, InvokeParameters>>();

        bool _skipUpdate = false;

        protected override void Update(float time)
        {
            base.Update(time);

            if ( _skipUpdate )
            {
                _skipUpdate = false;
                return;
            }

            for (int idx = 0; idx < _children.Count;)
            {
                var child = _children[idx] as UiPage;

                if (child.DisplayVisibility == 0 && child.Visible == false )
                {
                    _children.RemoveAt(idx);
                    child.ViewRemoved();
                }
                else
                {
                    idx++;
                }
            }
        }

        internal void NavigateTo(DefinitionFile def, InvokeParameters parameters)
        {
            if (def == null)
            {
                AddPage(null);
                return;
            }

            Type controllerType = def["PageController"] as Type;
            
            UiController controller = Controller;
            
            bool attachController = false;

            if (controllerType != null)
            {
                var newController = Activator.CreateInstance(controllerType) as UiController;

                if (newController != null)
                {
                    newController.Parent = Controller;
                    controller = newController;
                    attachController = true;
                }
            }

            IDefinitionClass obj = (IDefinitionClass)Activator.CreateInstance(def.Class);

            if( !(obj is UiPage) )
            {
                throw new Exception("Error while navigating to page. The given file doesn't define UiPage.");
            }

            if (controller is UiNavigationController)
            {
                (controller as UiNavigationController).InitPage(parameters);
            }

            if (attachController)
            {
                controller.AttachView(obj as UiView);
            }

            if(!obj.Init(controller, Binding, def))
            {
                throw new Exception("Error while navigating to page.");
            }

            AddPage(obj as UiPage);
            _history.Add(new Tuple<DefinitionFile, InvokeParameters>(def, parameters));
        }

        internal void NavigateBack()
        {
            NavigateBack(null);
        }

        internal void NavigateBack(string anchor)
        {
            if (_history.Count <= 1)
            {
                return;
            }

            _history.RemoveAt(_history.Count - 1);

            for(int idx = _history.Count-1; idx >= 0; --idx)
            {
                Tuple<DefinitionFile, InvokeParameters> historyItem = _history[idx];
                _history.RemoveAt(idx);

                DefinitionFile newDef = historyItem.Item1;

                if (anchor == null || newDef.Anchor == anchor || idx == 0)
                {
                    NavigateTo(newDef, historyItem.Item2);
                    break;
                }
            }
        }

        internal void ClearNavigation()
        {
            _history.Clear();
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            return new Rectangle(0, 0, Bounds.Width, Bounds.Height);
        }

        private void AddPage(UiPage page)
        {
            UiPage lastVisibleChild = null;

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx] as UiPage;
                child.Hide();
                lastVisibleChild = child;

                if (child.HideTransitionEffect == null)
                {
                    child.HideTransitionEffect = (page != null && page.ShowTransitionEffect != null) ? page.ShowTransitionEffect.Reverse() : null;
                    child.HideSpeed = page != null ? page.ShowSpeed : float.MaxValue;

                    if (child.HideTransitionEffect == null)
                    {
                        child.HideTransitionEffect = child.ShowTransitionEffect != null ? child.ShowTransitionEffect.Reverse() : null;
                        child.HideSpeed = child.ShowSpeed;
                    }
                }
            }

            if (page != null)
            {
                _children.Add(page);
                page.Bounds = CalculateChildBounds(page);
                page.Parent = this;

                page.RegisterView();
                page.ViewAdded();

                if (page.ShowTransitionEffect == null)
                {
                    page.ShowTransitionEffect = (lastVisibleChild != null && lastVisibleChild.HideTransitionEffect != null) ? lastVisibleChild.HideTransitionEffect.Reverse() : null;
                    page.ShowSpeed = lastVisibleChild != null ? lastVisibleChild.HideSpeed : float.MaxValue;
                }

                if (OnPageAdded != null)
                {
                    OnPageAdded(page);
                }

                _skipUpdate = true;
            }
        }

        protected override void OnAdded()
        {
            EnabledGestures = (GestureType.Parent);
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            DrawBackground(ref parameters);

            if (_clipChildren)
            {
                parameters.DrawBatch.PushClip(ScreenBounds);
            }

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                UiPage page = _children[idx] as UiPage;
                page.ViewDraw(ref parameters);
            }

            if (_clipChildren)
            {
                parameters.DrawBatch.PopClip();
            }
        }

        public override void Add(UiView view)
        {
            if (!_children.Contains(view))
            {
                throw new InvalidOperationException("Cannot add views to UiNavigationPage. Use NavigateTo instead.");
            }
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiNavigationView));

            string url = DefinitionResolver.GetString(Controller, binding, file["Page"]);

            if (!string.IsNullOrWhiteSpace(url))
            {
                DefinitionFile pageFile = ContentLoader.Current.Load<DefinitionFile>(url);

                NavigateTo(pageFile, new InvokeParameters());
                UiPage page = _children[0] as UiPage;
                page.InstantShow();
            }

            return true;
        }
    }
}
