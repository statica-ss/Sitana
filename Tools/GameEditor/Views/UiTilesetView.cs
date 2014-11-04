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
using Sitana.Framework.Input.TouchPad;
using Sitana.Framework;
using Sitana.Framework.Ui.Interfaces;

namespace GameEditor
{
    public class UiTilesetView: UiView
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiView.Parse(node, file);

			DefinitionParser parser = new DefinitionParser(node);

			file["SelectionColor"] = parser.ParseColor("SelectionColor");
        }

        int _currentSize = 1;
        Texture2D _currentTileset = null;
        int _divideWidth = 1;

		Rectangle? _selection;
		int _touchId = 0;

        Point? _origin;

		ColorWrapper _selectionColor;

        IScrolledElement _scrollService;

		protected override void OnAdded()
		{
			TouchPad.Instance.AddListener(GestureType.Down | GestureType.Move | GestureType.Up, this);

            _scrollService = Parent as IScrolledElement;

			base.OnAdded();
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();

			TouchPad.Instance.RemoveListener(this);
		}

		protected override void Init(object controller, object binding, DefinitionFile definition)
		{
			base.Init(controller, binding, definition);

			DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiListBox));

			_selectionColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["SelectionColor"]);
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
					_selection = null;
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

				if ( _selection.HasValue )
				{
					float tileSize = CurrentTemplate.Instance.TileSize * scale;

					Rectangle selection = sb;

					selection.X += (int)(_selection.Value.X * tileSize);
					selection.Y += (int)(_selection.Value.Y * tileSize);

					selection.Width = (int)Math.Ceiling((_selection.Value.Width+1)*tileSize);
					selection.Height = (int)Math.Ceiling((_selection.Value.Height+1)*tileSize);

					parameters.DrawBatch.DrawRectangle(selection, _selectionColor.Value * opacity);
				}
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

		protected override void OnGesture(Gesture gesture)
		{
			if (_currentTileset == null)
			{
				return;
			}

			switch (gesture.GestureType)
			{
			case GestureType.Down:

				if (_touchId ==0 &&  ScreenBounds.Contains(gesture.Origin))
				{
					_touchId = gesture.TouchId;
					_selection = null;
                    _origin = null;

					Select(gesture);
                    _origin = _selection.Value.Location;
				}

				break;

			case GestureType.Move:

				if (_touchId == gesture.TouchId)
				{
					Select(gesture);
				}

				break;

			case GestureType.CapturedByOther:
			case GestureType.Up:
				if (_touchId == gesture.TouchId)
				{

					if (gesture.GestureType == GestureType.Up)
					{

					}

					_touchId = 0;
					_selection = null;
                    _origin = null;
				}
				break;
			}
		}

		void Select(Gesture gesture)
		{
            if ( _touchId != 0 )
            {
                Rectangle psb = Parent.ScreenBounds;

                if ( gesture.Position.Y > psb.Bottom )
                {
                    _scrollService.ScrollingService.ScrollPositionY += gesture.Position.Y - psb.Bottom;
                    _scrollService.ScrollingService.Process();
                }
                else if ( gesture.Position.Y < psb.Top )
                {
                    _scrollService.ScrollingService.ScrollPositionY -= psb.Top - gesture.Position.Y;
                    _scrollService.ScrollingService.Process();
                }
            }

            if (!IsPointInsideView(gesture.Position))
			{
				return;
			}
			
            Rectangle sb = ScreenBounds;

			float width = _currentTileset.Width / _divideWidth;
			float scale = sb.Width / width;

			int maxHeightInTiles = _currentSize / CurrentTemplate.Instance.TileSize;

			float tileSize = CurrentTemplate.Instance.TileSize * scale;

			Point pos2 = new Vector2( (gesture.Position.X - sb.X) / tileSize, (gesture.Position.Y - sb.Y) / tileSize).ToPoint();

            Point pos1 = _origin.HasValue ? _origin.Value : pos2;

			pos1.X = Math.Max(0, Math.Min(CurrentTemplate.Instance.TilesetLineWidth - 1, pos1.X));
			pos2.X = Math.Max(0, Math.Min(CurrentTemplate.Instance.TilesetLineWidth - 1, pos2.X));

			pos1.Y = Math.Max(0, Math.Min(maxHeightInTiles - 1, pos1.Y));
			pos2.Y = Math.Max(0, Math.Min(maxHeightInTiles - 1, pos2.Y));

			if (pos1.X > pos2.X)
			{
				int pos = pos1.X;
				pos1.X = pos2.X;
				pos2.X = pos;
			}

			if (pos1.Y > pos2.Y)
			{
				int pos = pos1.Y;
				pos1.Y = pos2.Y;
				pos2.Y = pos;
			}

			_selection = new Rectangle(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y);
		}
    }
}
