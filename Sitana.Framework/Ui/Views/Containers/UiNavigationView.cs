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

namespace Sitana.Framework.Ui.Views
{
    public class UiNavigationView: UiContainer
    {
        List<DefinitionFile> _history = new List<DefinitionFile>();

        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            DefinitionParser parser = new DefinitionParser(node);

            file["Page"] = parser.ParseString("Page");
        }

        protected override void Update(float time)
        {
            base.Update(time);

            for (int idx = 0; idx < _children.Count;)
            {
                var child = _children[idx] as UiPage;

                if (child.PageStatus == UiPage.Status.Done)
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

        internal void NavigateTo(DefinitionFile def)
        {
            UiPage view = def.CreateInstance(Controller, Binding) as UiPage;

            if (view != null)
            {
                AddPage(view);
                _history.Add(def);
            }
            else
            {
                throw new Exception("Error while navigating to page.");
            }
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
                DefinitionFile newDef = _history[idx];
                _history.RemoveAt(idx);

                if (anchor == null || newDef.Anchor == anchor || idx == 0)
                {
                    NavigateTo(newDef);
                    break;
                }
            }
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            return new Rectangle(0, 0, Bounds.Width, Bounds.Height);
        }

        private void AddPage(UiPage page)
        {
            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx] as UiPage;
                child.Hide();

                if (child.HideTransitionEffect == null)
                {
                    child.HideTransitionEffect = page.ShowTransitionEffect != null ? page.ShowTransitionEffect.Reverse() : null;
                    child.HideSpeed = page.ShowSpeed;
                }
            }

            _children.Add(page);
            page.Bounds = CalculateChildBounds(page);
            page.Parent = this;

            page.ViewAdded();
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            float opacity = DisplayOpacity * parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            Color backgroundColor = BackgroundColor * opacity;

            if (backgroundColor.A > 0)
            {
                parameters.DrawBatch.DrawRectangle(ScreenBounds, backgroundColor);
            }

            parameters.DrawBatch.PushClip(ScreenBounds);

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                UiPage page = _children[idx] as UiPage;

                UiViewDrawParameters drawParams = parameters;
                drawParams.Opacity = opacity;
                drawParams.Transition = page.Transition;
                drawParams.TransitionPageSize = new Point(Bounds.Width, Bounds.Height);
                drawParams.TransitionPageModeHide = page.PageStatus == UiPage.Status.Hide;
                page.ViewDraw(ref drawParams);
            }

            parameters.DrawBatch.PopClip();
        }

        public override void Add(UiView view)
        {
            if (!_children.Contains(view))
            {
                throw new InvalidOperationException("Cannot add views to UiNavigationPage. Use NavigateTo instead.");
            }
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiNavigationView));

            string url = DefinitionResolver.GetString(Controller, binding, file["Page"]);

            DefinitionFile pageFile = ContentLoader.Current.Load<DefinitionFile>(url);

            NavigateTo(pageFile);
            UiPage page = _children[0] as UiPage;
            page.InstantShow();
        }
    }
}
