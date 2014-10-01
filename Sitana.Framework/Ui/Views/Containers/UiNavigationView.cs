// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;

namespace Sitana.Framework.Ui.Views
{
    public class UiNavigationView: UiContainer
    {
        List<DefinitionFile> _history = new List<DefinitionFile>();

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = DisplayOpacity * parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            parameters.DrawBatch.DrawRectangle(ScreenBounds, BackgroundColor * opacity);

            UiViewDrawParameters drawParams = parameters;
            drawParams.Opacity = opacity;

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ViewDraw(ref drawParams);
            }
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
            UiPage view = UiPage.Load(def);
            AddPage(view);

            _history.Add(def);
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

        private void AddPage(UiPage page)
        {
            for (int idx = 0; idx < _children.Count; ++idx)
            {
                var child = _children[idx] as UiPage;
                child.Hide();
            }

            _children.Add(page);
            page.Bounds = CalculateChildBounds(page);
            page.Parent = this;

            page.ViewAdded();
        }

        public override void Add(UiView view)
        {
            throw new InvalidOperationException("Cannot add views to UiNavigationPage. Use NavigateTo instead.");
        }
    }
}
