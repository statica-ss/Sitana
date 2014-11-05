// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Xml;
using Sitana.Framework.Diagnostics;
using System;
using Microsoft.Xna.Framework;

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

            var parser = new DefinitionParser(node);

            file["ShowTime"] = parser.ParseFloat("ShowTime");
            file["HideTime"] = parser.ParseFloat("HideTime");
        }

        public enum Status
        {
            Show,
            Visible,
            Hide,
            Done
        }

        public Status PageStatus { get; private set; }
        public float Transition { get; internal set; }

        private float _showSpeed = 1;
        private float _hideSpeed = 1;

        internal TransitionEffect ShowTransitionEffect
        {
            get
            {
                return _showTransitionEffect;
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
        }

        internal float HideSpeed
        {
            set
            {
                _hideSpeed = value;
            }
        }

        public UiPage()
        {
            PageStatus = Status.Show;
            Transition = 1;
        }

        internal void Hide()
        {
            switch(PageStatus)
            {
                case Status.Show:
                case Status.Visible:
                    PageStatus = Status.Hide;
                    break;
            }
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);
            InitChildren(Controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiPage));

            double showTime = DefinitionResolver.Get<double>(Controller, Binding, file["ShowTime"], 500) / 1000.0;
            double hideTime = DefinitionResolver.Get<double>(Controller, Binding, file["HideTime"], 500) / 1000.0;

            _showSpeed = (float)(showTime > 0 ? 1 / showTime : 10000000);
            _hideSpeed = (float)(hideTime > 0 ? 1 / hideTime : 10000000);
        }

        protected override void Update(float time)
        {
            switch(PageStatus)
            {
                case Status.Show:
                    Transition -= time * _showSpeed;
                    if ( Transition <= 0 )
                    {
                        Transition = 0;
                        PageStatus = Status.Visible;
                    }
                    break;

                case Status.Hide:
                    Transition += time * _hideSpeed;
                    if (Transition >= 1)
                    {
                        Transition = 1;
                        PageStatus = Status.Done;
                    }
                    break;
            }

            if (PageStatus != Status.Done)
            {
                base.Update(time);
            }
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
            drawParams.Transition = Transition;
            drawParams.TransitionRectangle = ScreenBounds;

            drawParams.TransitionMode = PageStatus == Status.Visible ? TransitionMode.Visible :
                (PageStatus == Status.Show ? TransitionMode.Show : TransitionMode.Hide);

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
            Transition = 0;
            PageStatus = Status.Visible;
        }
    }
}
