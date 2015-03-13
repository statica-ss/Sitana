// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Xml;
using Sitana.Framework.Diagnostics;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Sitana.Framework.Ui.Controllers;

namespace Sitana.Framework.Ui.Views
{
    public class UiPage: UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

			var parser = new DefinitionParser(node);

			file["PageShown"] = parser.ParseBoolean("PageShown");
        }

        public TransitionEffect ShowTransitionEffect
        {
            get
            {
                return _showTransitionEffect;
            }

            set
            {
                _showTransitionEffect = value;
            }
        }

        public TransitionEffect HideTransitionEffect
        {
            get
            {
                return _hideTransitionEffect;
            }

            set
            {
                _hideTransitionEffect = value;
            }
        }

        public float ShowSpeed
        {
            get
            {
                return _showSpeed;
            }

            set
            {
                _showSpeed = value;
            }
        }

        public float HideSpeed
        {
            get
            {
                return _hideSpeed;
            }

            set
            {
                _hideSpeed = value;
            }
        }

		bool _alreadyFullyVisible = false;

        public UiPage()
        {
        }

        internal void Hide()
        {
            Visible = false;
        }

        public void Init(InvokeParameters parameters)
        {
            UiNavigationController controller = (Controller as UiNavigationController);
            
            if(controller != null)
            {
                controller.InitPage(this, parameters);
            }
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            InitChildren(Controller, Binding, definition);
            Visible = true;
            DisplayVisibility = 0;

			RegisterDelegate("PageShown", definition["PageShown"]);

            return true;
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            DrawBackground(ref parameters);

            UiViewDrawParameters drawParams = parameters;
            drawParams.Opacity = opacity;
            drawParams.Transition = 1 - DisplayVisibility;
            drawParams.TransitionRectangle = ScreenBounds;

            drawParams.TransitionMode = DisplayVisibility == 1 ? TransitionMode.None : (Visible ? TransitionMode.Show : TransitionMode.Hide);

			if (DisplayVisibility == 1 && !_alreadyFullyVisible)
			{
				_alreadyFullyVisible = true;

				UiTask.BeginInvoke(() => CallDelegate("PageShown"));
			}

            for (int idx = 0; idx < _children.Count; ++idx)
            {
                _children[idx].ViewDraw(ref drawParams);
            }
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            return new Rectangle(0, 0, Bounds.Width, Bounds.Height);
        }

        internal void InstantShow()
        {
            DisplayVisibility = 1;
            Visible = true;
        }
    }
}
