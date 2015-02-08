using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Cs;
using Sitana.Framework;
using GameEditor.Views;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace GameEditor
{
    public class EditViewController: UiController
    {
		public readonly SharedString ZoomValue = new SharedString();

        public readonly SharedString WorldCoordinates = new SharedString();

        public readonly SharedValue<int> Zoom = new SharedValue<int>(100);

        UiEditView _editView = null;

		public EditViewController()
		{
            LayerSelectionChanged();
            Document.Current.LayerSelectionChanged += LayerSelectionChanged;

            UpdateZoomValue(100);
            Zoom.ValueChanged += UpdateZoomValue;
		}

		public void ZoomOut()
		{
            Zoom.Value -= Zoom.Value > 200 ? 50 : 25;

            if (Zoom.Value < 25)
			{
                Zoom.Value = 25;
			}
		}

		public void ZoomIn()
		{
            Zoom.Value += Zoom.Value < 200 ? 25 : 50;

            if (Zoom.Value > 400)
			{
                Zoom.Value = 400;
			}
		}

		void UpdateZoomValue(int zoom)
		{
            ZoomValue.Format("{0}%", zoom);
		}

        public void OnViewAdded()
        {
            _editView = Find<UiEditView>("EditView");
        }

        protected override void Update(float time)
        {
            base.Update(time);

            Vector2? mousePos = _editView.MousePosition;

            if(mousePos.HasValue)
            {
                if (Document.Current.SelectedLayer is DocTiledLayer)
                {
                    WorldCoordinates.Format(CultureInfo.InvariantCulture, "{0:n0}, {1:n0}", mousePos.Value.X, mousePos.Value.Y);
                }
                else
                {
                    WorldCoordinates.Format(CultureInfo.InvariantCulture, "{0:n2}, {1:n2}", mousePos.Value.X, mousePos.Value.Y);
                }
            }
            else
            {
                WorldCoordinates.Clear();
            }
        }

        void LayerSelectionChanged()
        {
            WorldCoordinates.Clear();
        }
    }
}
