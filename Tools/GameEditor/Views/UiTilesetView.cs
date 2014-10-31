using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Xml;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Games;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.Parameters;

namespace GameEditor
{
    public class UiTilesetView: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);
        }

        int _currentSize = 1;
        DocLayer _currentLayer = null;
        Texture2D _currentTileset = null;
        int _divideWidth = 1;

        protected override void Init(object controller, object binding, DefinitionFile definition)
        {
            base.Init(controller, binding, definition);

        }

        void UpdateSize()
        {
            int size = _currentSize;
            var layer = Document.Instance.SelectedLayer;

            if (layer is DocTiledLayer)
            {
                Visible.Value = true;

                Texture2D tileset = CurrentTemplate.Instance.Tileset((layer.Layer as TiledLayer).Tileset).Item2;

                if ( tileset != _currentTileset )
                {
                    size = ComputeSize(tileset);
                    _currentTileset = tileset;
                }
            }
            else
            {
                Visible.Value = false;
                _currentTileset = null;
                size = 1;
            }

            if (_currentSize != size)
            {
                _currentSize = size;
                Parent.RecalcLayout();
            }
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = DisplayOpacity * parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            if ( _currentTileset != null )
            {
                

                Rectangle sb = ScreenBounds;

                float width = _currentTileset.Width / _divideWidth;

                float scale = sb.Width / width;

                float ww = (int)(scale * width);
                scale = ww / width;

                Point size = new Point((int)ww, (int)(_currentTileset.Height * scale));

                parameters.DrawBatch.DrawRectangle(new Rectangle(sb.X, sb.Y, (int)ww, sb.Height), BackgroundColor);

                SamplerState oldSampler = parameters.DrawBatch.SamplerState;
                parameters.DrawBatch.SamplerState = SamplerState.PointClamp;

                for (int idx = 0; idx < _divideWidth; ++idx)
                {
                    Point target = sb.Location;
                    target.Y += (int)(scale * _currentTileset.Height) * idx;

                    Point source = Point.Zero;
                    source.X += (int)width * idx;

                    parameters.DrawBatch.DrawImage(_currentTileset, target, size, source, scale, Color.White * opacity);
                }

                parameters.DrawBatch.SamplerState = oldSampler;
            }
        }

        public override Point ComputeSize(int width, int height)
        {
            if (_currentTileset != null)
            {
                float texWidth = _currentTileset.Width / _divideWidth;
                float scale = width / texWidth;

                var size = new Point(width, (int)(_currentSize * scale + 1));
                return size;
            }
            return new Point(width, 1);
        }

        int ComputeSize(Texture2D tileset)
        {
            int tileSize = CurrentTemplate.Instance.TileSize;
            int perLine = CurrentTemplate.Instance.TilesetLineWidth;

            int tilesPerLine = tileset.Width / tileSize;

            _divideWidth = tilesPerLine / perLine;

            if (_divideWidth * perLine != tilesPerLine)
            {
                throw new Exception("Invalid tileset! Cannot divide width properly.");
            }

            return tileset.Height * _divideWidth;
        }

        protected override void Update(float time)
        {
            base.Update(time);
            UpdateSize();
        }
    }
}
