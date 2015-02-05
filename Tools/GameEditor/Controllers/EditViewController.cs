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

        public int Zoom { get; private set; }

        UiEditView _editView = null;

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

        public void OnViewAdded()
        {
            _editView = Find<UiEditView>("EditView");
        }

        protected override void Update(float time)
        {
            base.Update(time);

            Point? mousePos = _editView.MousePosition;

            if(mousePos.HasValue)
            {
                Vector2 pos = Tools.Tool.PositionToUnit(mousePos.Value, _editView.CurrentPosition, (float)Zoom / 100f);

                WorldCoordinates.Format(CultureInfo.InvariantCulture, "{0:n2}, {1:n2}", pos.X, pos.Y);
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
