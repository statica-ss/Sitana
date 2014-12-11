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

namespace Sitana.Framework.Ui.Views
{
    public class UiPage: UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiPage.Content":
                        if (cn.Nodes.Count != 1)
                        {
                            string error = node.NodeError("UiPage must have exactly 1 child.");
                            if (DefinitionParser.EnableCheckMode)
                            {
                                ConsoleEx.WriteLine(error);
                            }
                            else
                            {
                                throw new Exception(error);
                            }
                        }

                        ParseChildren(cn, file);
                        break;
                }
            }
        }  

        internal TransitionEffect ShowTransitionEffect
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

        internal TransitionEffect HideTransitionEffect
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

        internal float ShowSpeed
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

        internal float HideSpeed
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

        public UiPage()
        {
        }

        internal void Hide()
        {
            Visible.Value = false;
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);
            InitChildren(Controller, Binding, definition);
            Visible.Value = true;
            DisplayVisibility = 0;
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            Color backgroundColor = BackgroundColor * opacity;

            if (backgroundColor.A > 0)
            {
                parameters.DrawBatch.DrawRectangle(ScreenBounds, backgroundColor);
            }

            UiViewDrawParameters drawParams = parameters;
            drawParams.Opacity = opacity;
            drawParams.Transition = 1 - DisplayVisibility;
            drawParams.TransitionRectangle = ScreenBounds;

            drawParams.TransitionMode = DisplayVisibility == 1 ? TransitionMode.None : (Visible.Value ? TransitionMode.Show : TransitionMode.Hide);

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
            Visible.Value = true;
        }
    }
}
