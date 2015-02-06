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
using Sitana.Framework.Games;
using Microsoft.Xna.Framework.Graphics;

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

        Vector2 PositionFromGesture(Gesture gesture)
        {
            Rectangle bounds = ScreenBounds;

            float zoom = (float)_zoom.Value / 100f;

            Point pos = gesture.Position.ToPoint();

            pos.X -= bounds.X;
            pos.Y -= bounds.Y;

            return Tools.Tool.PositionToUnit(pos, CurrentPosition, zoom);
        }

        protected override void OnGesture(Gesture gesture)
        {
            Rectangle bounds = ScreenBounds;
            
            switch (gesture.GestureType)
            {
                case GestureType.MouseMove:

                    if (IsPointInsideView(gesture.Position))
                    {
                        MousePosition = PositionFromGesture(gesture);
                    }
                    else
                    {
                        MousePosition = null;
                    }
                    break;

                case GestureType.Move:
                    if (IsPointInsideView(gesture.Position))
                    {
                        MousePosition = PositionFromGesture(gesture);
                        Tools.Tool.Current.OnMove(MousePosition.Value);
                    }
                    break;

                case GestureType.Down:
                    if (IsPointInsideView(gesture.Position))
                    {
                        MousePosition = PositionFromGesture(gesture);
                        Tools.Tool.Current.OnDown(MousePosition.Value);
                    }
                    else
                    {
                        MousePosition = null;
                    }
                    break;

                case GestureType.Up:
                    Tools.Tool.Current.OnUp();
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

            int size = (int)Math.Ceiling(UiUnit.Unit * 3);

            int right = bounds.Right + size;
            int bottom = bounds.Bottom + size;

            AdvancedDrawBatch batch = parameters.DrawBatch;

            batch.PushClip(bounds);

            Color color = Color.White * 0.25f;

            for (int x = startX; x < right; x+=unitSize )
            {
                for (int y = startY; y < bottom; y+=unitSize )
                {
                    batch.DrawLine(new Point(x - size, y), new Point(x + size - 1, y), color);
                    batch.DrawLine(new Point(x, y - size), new Point(x, y + size - 1), color);
                }
            }

            DrawCurrentLayer(ref parameters);

            if (MousePosition.HasValue)
            {
                Point pos = CurrentPosition;
                pos.X -= bounds.X;
                pos.Y -= bounds.Y;

                Tools.Tool.Current.Draw(parameters.DrawBatch, pos, MousePosition.Value, zoom);
            }

            batch.PopClip();
        }

        protected override void Update(float time)
        {
            base.Update(time);

            float zoom = (float)_zoom.Value / 100f;
            int unitSize = Tools.Tool.UnitToPixels(zoom);

            _maxScroll.X = Math.Max(0, (int)(Document.Current.SelectedLayer.Width * unitSize));
            _maxScroll.Y = Math.Max(0, (int)(Document.Current.SelectedLayer.Height * unitSize));
        }

        void DrawCurrentLayer(ref UiViewDrawParameters parameters)
        {
            if(Document.Current.SelectedLayer is DocTiledLayer)
            {
                TiledLayer layer = Document.Current.SelectedLayer.Layer as TiledLayer;
                DrawTiles(ref parameters, layer);
            }
        }

        void DrawTiles(ref UiViewDrawParameters parameters, TiledLayer layer)
        {
            Rectangle bounds = ScreenBounds;

            AdvancedDrawBatch batch = parameters.DrawBatch;

            SamplerState oldSamplerState = batch.SamplerState;
            batch.SamplerState = SamplerState.PointClamp;

            int width = layer.Width;
            int height = layer.Height;

            float zoom = (float)_zoom.Value / 100f;
            int unitSize = Tools.Tool.UnitToPixels(zoom);

            int tileSize = Tools.Tool.UnitToPixels(1);

            Rectangle target = new Rectangle(bounds.X-CurrentPosition.X, bounds.Y-CurrentPosition.Y, unitSize, unitSize);
            Rectangle source = new Rectangle(0, 0, tileSize - 1, tileSize - 1);

            Texture2D tileset = CurrentTemplate.Instance.Tileset(layer.Tileset).Item2;

            int startY = target.Y;

            ushort[,] tiles = layer.Content;

            for(int idxX = 0; idxX < width; ++idxX)
            {
                target.Y = startY;

                if (target.Right >= bounds.X && target.X <= bounds.Right)
                {
                    for (int idxY = 0; idxY < height; ++idxY)
                    {
                        if (target.Bottom >= bounds.Y && target.Y <= bounds.Bottom)
                        {
                            ushort tile = tiles[idxX, idxY];

                            if (tile != 0xffff)
                            {
                                source.X = (tile & 0xff) * tileSize;
                                source.Y = ((tile >> 8) & 0xff) * tileSize;

                                batch.DrawImage(tileset, target, source, Color.White * 0.5f);
                            }
                        }

                        target.Y += unitSize;
                    }
                }

                target.X += unitSize;
            }

            Color color = Color.White * 0.25f;

            batch.DrawLine(new Point(target.X, bounds.Top), new Point(target.X, target.Y), color);
            batch.DrawLine(new Point(bounds.Left, target.Y), new Point(target.X, target.Y), color);

            batch.SamplerState = oldSamplerState;
        }

        Rectangle IScrolledElement.ScreenBounds { get { return ScreenBounds; } }

        int IScrolledElement.MaxScrollX { get { return _maxScroll.X; } }
        int IScrolledElement.MaxScrollY { get { return _maxScroll.Y; } }

        ScrollingService IScrolledElement.ScrollingService { get { return _scrollingService; } }
    }
}
