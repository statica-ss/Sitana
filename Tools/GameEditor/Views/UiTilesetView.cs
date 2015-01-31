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

namespace GameEditor.Views
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
        int _tileSize = 32;
        float _scale = 1;

		ColorWrapper _selectionColor;

        IScrolledElement _scrollService;

		float _scrollDirection = 0;

		protected override void OnAdded()
		{
			EnabledGestures = GestureType.Down | GestureType.Move | GestureType.Up;

            _scrollService = Parent as IScrolledElement;

			base.OnAdded();
		}

		protected override bool Init(object controller, object binding, DefinitionFile definition)
		{
            if(!base.Init(controller, binding, definition))
            {
                return false;
            }

			DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiListBox));

			_selectionColor = DefinitionResolver.GetColorWrapper(Controller, Binding, file["SelectionColor"]);

            return true;
		}

        void UpdateSize()
        {
            int size = _currentSize;
            var layer = Document.Instance.SelectedLayer;

            Texture2D oldTileset = _currentTileset;

            if (layer is DocTiledLayer)
            {
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
                _currentTileset = null;
                size = 1;
            }

            if (_currentTileset != oldTileset)
            {
                if (Tools.Tool.Current is Tools.InsertTiles)
                {
                    new Tools.Select();
                }
            }

            if (_currentSize != size)
            {
                _currentSize = size;
                Parent.RecalcLayout();
            }
        }

        protected override void Draw(ref UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            if ( _currentTileset != null )
            {
                Rectangle sb = ScreenBounds;

                int width = _currentTileset.Width / _divideWidth;
                float scale = _scale;

                int ww = (int)(scale * width);

                Point size = new Point((int)ww, (int)(_currentTileset.Height * scale));

                parameters.DrawBatch.DrawRectangle(new Rectangle(sb.X, sb.Y, (int)ww, (int)Math.Ceiling(_currentTileset.Height * scale * _divideWidth)), BackgroundColor);

                SamplerState oldSampler = parameters.DrawBatch.SamplerState;
                parameters.DrawBatch.SamplerState = SamplerState.PointClamp;

                for (int idx = 0; idx < _divideWidth; ++idx)
                {
                    Point target = sb.Location;
                    target.Y += size.Y * idx;

                    Point source = Point.Zero;	
                    source.X += (int)width * idx;

                    parameters.DrawBatch.DrawImage(_currentTileset, target, size, source, scale, Color.White * opacity);
                }
					
                parameters.DrawBatch.SamplerState = oldSampler;

				if ( _selection.HasValue )
				{
                    float tileSize = _tileSize;

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
                int tileSize = CurrentTemplate.Instance.TileSize;
                int texWidth = _currentTileset.Width / _divideWidth;

                _scale = (float)width / (float)texWidth;
                _tileSize = (int)(tileSize * _scale);
                _scale = (float)_tileSize / (float)tileSize;

                width = (int)(texWidth * _scale);

                var size = new Point(width, (int)(_currentSize * _scale + 1));
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

            float texWidth = (float)tileset.Width / (float)_divideWidth;

            _scale = (float)Bounds.Width / texWidth;
            _tileSize = (int)(tileSize * _scale);
            _scale = (float)_tileSize / (float)tileSize;

            return tileset.Height * _divideWidth;
        }

        protected override void Update(float time)
        {
            base.Update(time);
            UpdateSize();

			if (_touchId != 0 && _scrollDirection != 0)
			{
				_scrollService.ScrollingService.ScrollPositionY += time * _scrollDirection * 10;
				_scrollService.ScrollingService.Process();
			}
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

					if (_selection.HasValue)
					{
						_origin = _selection.Value.Location;
					}
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

					if (gesture.GestureType == GestureType.Up && _selection.HasValue )
					{
                        CreateToolFromSelection();
					}

					_touchId = 0;
					_selection = null;
                    _origin = null;
				}
				break;
			}
		}

        void CreateToolFromSelection()
        {
            ushort[,] selection = new ushort[_selection.Value.Width+1, _selection.Value.Height+1];

            Point begin = _selection.Value.Location;

            int height = _currentTileset.Height / CurrentTemplate.Instance.TileSize;
            int width = _currentTileset.Width / CurrentTemplate.Instance.TileSize / _divideWidth;

            for (int idxX = 0; idxX <= _selection.Value.Width; ++idxX)
            {
                for (int idxY = 0; idxY <= _selection.Value.Height; ++idxY)
                {
                    int y = begin.Y + idxY;

                    Point value = new Point(begin.X + idxX + width * (y / height), y % height);

                    selection[idxX, idxY] = (ushort)((value.X & 0xff) | ((value.Y & 0xff) << 8));
                }
            }

            new Tools.InsertTiles(_currentTileset, selection, CurrentTemplate.Instance.TileSize);
        }

        void Select(Gesture gesture)
		{
			_scrollDirection = 0;

            if ( _touchId != 0 )
            {
                Rectangle psb = Parent.ScreenBounds;

                if ( gesture.Position.Y > psb.Bottom )
                {
					_scrollDirection = gesture.Position.Y - psb.Bottom;
					if (_scrollDirection > 32)
					{
						_scrollDirection = 32;
					}
                }
                else if ( gesture.Position.Y < psb.Top )
                {
					_scrollDirection = gesture.Position.Y - psb.Top;

					if (_scrollDirection < -32)
					{
						_scrollDirection = -32;
					}
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
