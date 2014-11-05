using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Cs;

namespace GameEditor
{
    public class EditViewController: UiController
    {
		public readonly SharedString ZoomValue = new SharedString();

        public readonly SharedString WorldCoordinates = new SharedString();
        public readonly SharedString TileCoordinates = new SharedString();

		private int _zoom = 100;

		public EditViewController()
		{
			UpdateZoomValue();

            LayerSelectionChanged();
            Document.Current.LayerSelectionChanged += LayerSelectionChanged;
		}

		public void ZoomOut()
		{
			_zoom -= _zoom > 200 ? 50 : 25;

			if (_zoom < 25)
			{
				_zoom = 25;
			}
			UpdateZoomValue();
		}

		public void ZoomIn()
		{
			_zoom += _zoom < 200 ? 25 : 50;

			if (_zoom > 400)
			{
				_zoom = 400;
			}
			UpdateZoomValue();
		}

		void UpdateZoomValue()
		{
			ZoomValue.Format("{0}%", _zoom);
		}

        void LayerSelectionChanged()
        {
            DocLayer layer = Document.Current.SelectedLayer;

            WorldCoordinates.Format("24.5, 25.8");
            
            if (layer is DocTiledLayer)
            {
                TileCoordinates.Format("12, 15");
            }
            else
            {
                TileCoordinates.Clear();
            }
        }
    }
}
