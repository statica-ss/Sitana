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
using Sitana.Framework.Ui.Interfaces;

namespace GameEditor.Views
{
    public class UiEditView : UiView, IScrolledElement
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);
        }

        internal Point? MousePosition { get; private set; }
        
        ScrollingService _scrollingService;

        Point _maxScroll = new Point(10000,10000);

        internal Point CurrentPosition
        {
            get
            {
                return new Point((int)_scrollingService.ScrollPositionX, (int)_scrollingService.ScrollPositionY);
            }
        }

        protected override void OnAdded()
        {
            EnabledGestures = GestureType.Down | GestureType.Up | GestureType.Move | GestureType.MouseMove;
            _scrollingService = new ScrollingService(this, ScrollingService.ExceedRule.Forbid);
        }

        protected override void OnGesture(Gesture gesture)
        {
            switch (gesture.GestureType)
            {
                case GestureType.MouseMove:

                    if (IsPointInsideView(gesture.Position))
                    {
                        float zoom = (Controller as EditViewController).Zoom / 100.0f;
                        int unitSize = Tools.Tool.UnitToPixels(zoom);

                        MousePosition = gesture.Position.ToPoint();
                    }
                    else
                    {
                        MousePosition = null;
                    }
                    break;
            }
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            if (MousePosition.HasValue)
            {
                float zoom = (Controller as EditViewController).Zoom / 100.0f;
                Tools.Tool.Current.Draw(parameters.DrawBatch, MousePosition.Value, zoom);
            }
        }

        protected override void Update(float time)
        {
            base.Update(time);

            float zoom = (Controller as EditViewController).Zoom / 100.0f;
            int unitSize = Tools.Tool.UnitToPixels(zoom);

            _maxScroll.X = Math.Max(0, (int)(Document.Current.SelectedLayer.Width * unitSize) - 0);
            _maxScroll.Y = Math.Max(0, (int)(Document.Current.SelectedLayer.Height * unitSize) - 0);
        }

        Rectangle IScrolledElement.ScreenBounds { get { return ScreenBounds; } }

        int IScrolledElement.MaxScrollX { get { return _maxScroll.X; } }
        int IScrolledElement.MaxScrollY { get { return _maxScroll.Y; } }

        ScrollingService IScrolledElement.ScrollingService { get { return _scrollingService; } }
    }
}
