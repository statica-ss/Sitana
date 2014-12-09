using System;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Diagnostics;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Input.TouchPad;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Views
{
    public class UiModalLayout : UiContainer
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            foreach(var cn in node.Nodes)
            {
                switch ( cn.Tag )
                {
                case "UiModalLayout.Children":

                    if (cn.Nodes.Count != 1)
                    {
                        string error = node.NodeError("UiModalLayout must have exactly 1 child.");
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

            DefinitionParser parser = new DefinitionParser(node);

            file["TouchOutsideToHide"] = parser.ParseBoolean("TouchOutsideToHide");
        }

        bool _touchOutsideToHide;

        protected override void OnAdded()
        {
            EnabledGestures = (GestureType.Down | GestureType.FreeDrag | GestureType.Tap | GestureType.HoldStart | GestureType.Hold);

            base.OnAdded();
        }

        protected override void OnGesture(Gesture gesture)
        {
            if ( Visible.Value )
            {
                if (_touchOutsideToHide) 
                {
                    if (gesture.GestureType == GestureType.Down) 
                    {
                        if (!ScreenBounds.Contains(gesture.Position.ToPoint()))
                        {
                            Visible.Value = false;
                        }
                    }
                }

                gesture.SkipRest = true;
            }
        }

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiView));

            _touchOutsideToHide = DefinitionResolver.Get<bool>(Controller, Binding, file["TouchOutsideToHide"], true);

            Visible = DefinitionResolver.GetShared<bool>(Controller, binding, file["Visible"], false);

            if (!Visible.Value) 
            {
                DisplayVisibility = 0;
            }

            InitChildren(Controller, binding, definition);
        }

        protected override Rectangle CalculateChildBounds(UiView view)
        {
            return new Rectangle(0, 0, Bounds.Width, Bounds.Height);
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
    }
}

