using System;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Binding;

namespace GameEditor
{
    public class LayersListController: UiController
    {
        public IItemsProvider Layers
        {
            get
            {
                return Document.Instance.Layers;
            }
        }
        
        public void AddTiledLayer()
        {
            Document.Current.AddTilesetLayer();
        }

        public void AddVectorLayer()
        {
            Document.Current.AddVectorLayer();
        }

        public void RemoveLayer()
        {
            if ( Document.Current.Layers.Count == 1 )
            {
                MessageBox.Info("Cannot remove last layer!");
                return;
            }

            MessageBox.YesNo(String.Format("Do you want to delete layer \n{0}?", Document.Current.SelectedLayer.Name ), () =>
            {
                Document.Current.RemoveSelectedLayer();
            });
        }

        public void SelectLayer(DocLayer layer)
        {
            Document.Current.Select(layer);
        }

        public void SetDocumentModified()
        {
            Document.Current.SetModified();
        }

        public void MoveLayerUp(DocLayer layer)
        {
            SelectLayer(layer);
            int index = Document.Current.Layers.IndexOf(layer);

            if (index > 0)
            {
                Document.Current.Layers.MoveElementToIndex(layer, index - 1);
                SetDocumentModified();
            }
        }

        public void MoveLayerDown(DocLayer layer)
        {
            SelectLayer(layer);
            int index = Document.Current.Layers.IndexOf(layer);

            if (index < Document.Current.Layers.Count - 1)
            {
                Document.Current.Layers.MoveElementToIndex(layer, index + 1);
                SetDocumentModified();
            }
        }
    }
}

