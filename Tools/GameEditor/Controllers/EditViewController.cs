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

        public int Zoom { get; private set; }

		public EditViewController()
		{
            Zoom = 100;
			UpdateZoomValue();

            LayerSelectionChanged();
            Document.Current.LayerSelectionChanged += LayerSelectionChanged;
		}

		public void ZoomOut()
		{
            Zoom -= Zoom > 200 ? 50 : 25;

            if (Zoom < 25)
			{
                Zoom = 25;
			}
			UpdateZoomValue();
		}

		public void ZoomIn()
		{
            Zoom += Zoom < 200 ? 25 : 50;

            if (Zoom > 400)
			{
                Zoom = 400;
			}
			UpdateZoomValue();
		}

		void UpdateZoomValue()
		{
            ZoomValue.Format("{0}%", Zoom);
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
