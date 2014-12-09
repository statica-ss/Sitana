using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Input.TouchPad;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;

namespace GameEditor.Views
{
    public class UiEditView: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);
        }

        Point? _mousePosition;

        protected override void OnAdded()
        {
            EnabledGestures = GestureType.Down | GestureType.Up | GestureType.Move | GestureType.MouseMove;
        }

        protected override void OnGesture(Gesture gesture)
        {
            switch (gesture.GestureType)
            {
                case GestureType.MouseMove:

                    if (IsPointInsideView(gesture.Position))
                    {
                        _mousePosition = gesture.Position.ToPoint();
                    }
                    else
                    {
                        _mousePosition = null;
                    }
                    break;
            }
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            if (_mousePosition.HasValue)
            {
                float zoom = (Controller as EditViewController).Zoom / 100.0f;
                Tools.Tool.Current.Draw(parameters.DrawBatch, _mousePosition.Value, zoom);
            }
        }
    }
}
