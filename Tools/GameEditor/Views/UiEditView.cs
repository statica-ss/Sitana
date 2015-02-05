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
using Sitana.Framework.Cs;
using Sitana.Framework.Graphics;
using Sitana.Framework.Ui.Core;

namespace GameEditor.Views
{
    public class UiEditView : UiView, IScrolledElement
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

            var parser = new DefinitionParser(node);
            file["Zoom"] = parser.ParseInt("Zoom");
        }

        internal Vector2? MousePosition { get; private set; }
        
        ScrollingService _scrollingService;

        Point _maxScroll = Point.Zero;

        SharedValue<int> _zoom;

        internal Point CurrentPosition
        {
            get
            {
                return new Point((int)_scrollingService.ScrollPositionX, (int)_scrollingService.ScrollPositionY);
            }
        
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if(!base.Init(controller, binding, definition))
            {
                return false;
            }

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiView));
            _zoom = DefinitionResolver.GetShared<int>(Controller, Binding, file["Zoom"], 1);

            return true;
        }

        protected override void OnAdded()
        {
            EnabledGestures = GestureType.Down | GestureType.Up | GestureType.Move | GestureType.MouseMove;
            _scrollingService = new ScrollingService(this, ScrollingService.ExceedRule.Forbid);
        }

        protected override void OnGesture(Gesture gesture)
        {
            Rectangle bounds = ScreenBounds;
            
            switch (gesture.GestureType)
            {
                case GestureType.MouseMove:

                    if (IsPointInsideView(gesture.Position))
                    {
                        float zoom = (float)_zoom.Value / 100f;

                        Point pos = gesture.Position.ToPoint();
                        pos.X -= bounds.X;
                        pos.Y -= bounds.Y;
                        
                        MousePosition = Tools.Tool.PositionToUnit(pos, CurrentPosition, zoom);
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
            float zoom = (float)_zoom.Value / 100f;
            int unitSize = Tools.Tool.UnitToPixels(zoom);

            int startX = CurrentPosition.X / unitSize;
            int startY = CurrentPosition.Y / unitSize;

            Rectangle bounds = ScreenBounds;

            startX = startX * unitSize - CurrentPosition.X + bounds.X;
            startY = startY * unitSize - CurrentPosition.Y + bounds.Y + 1;

            int right = bounds.Right;
            int bottom = bounds.Bottom;

            AdvancedDrawBatch batch = parameters.DrawBatch;

            Color color = Color.White * 0.25f;

            int size = (int)Math.Ceiling(UiUnit.Unit * 3);

            for (int x = startX; x < right; x+=unitSize )
            {
                for (int y = startY; y < bottom; y+=unitSize )
                {
                    batch.DrawLine(new Point(x - size, y), new Point(x + size - 1, y), color);
                    batch.DrawLine(new Point(x, y - size), new Point(x, y + size - 1), color);
                }
            }

            if (MousePosition.HasValue)
            {
                Point pos = CurrentPosition;
                pos.X -= bounds.X;
                pos.Y -= bounds.Y;

                Tools.Tool.Current.Draw(parameters.DrawBatch, pos, MousePosition.Value, zoom);
            }
        }

        protected override void Update(float time)
        {
            base.Update(time);

            float zoom = (float)_zoom.Value / 100f;
            int unitSize = Tools.Tool.UnitToPixels(zoom);

            _maxScroll.X = Math.Max(0, (int)(Document.Current.SelectedLayer.Width * unitSize));
            _maxScroll.Y = Math.Max(0, (int)(Document.Current.SelectedLayer.Height * unitSize));
        }

        Rectangle IScrolledElement.ScreenBounds { get { return ScreenBounds; } }

        int IScrolledElement.MaxScrollX { get { return _maxScroll.X; } }
        int IScrolledElement.MaxScrollY { get { return _maxScroll.Y; } }

        ScrollingService IScrolledElement.ScrollingService { get { return _scrollingService; } }
    }
}
