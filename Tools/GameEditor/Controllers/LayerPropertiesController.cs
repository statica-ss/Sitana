using System;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Games;
using Sitana.Framework.Cs;

namespace GameEditor
{
    public class LayerPropertiesController: UiController
    {
        DocLayer _layer;

        public SharedString LayerName { get; private set; }

        public SharedString LayerWidth { get; private set;}
        public SharedString LayerHeight { get; private set;}

        public SharedString LayerSpeedX{ get; private set;}
        public SharedString LayerSpeedY { get; private set;}

        public SharedValue<bool> ShowTiled { get; private set;}
        public SharedValue<bool> ShowVector { get; private set;}

        public SharedValue<bool> TiledWidth { get; private set;}
        public SharedValue<bool> TiledHeight { get; private set;}

        public LayerPropertiesController()
        {
            LayerName = new SharedString();
            LayerWidth = new SharedString();
            LayerHeight = new SharedString();
            LayerSpeedX = new SharedString();
            LayerSpeedY = new SharedString();
            ShowTiled = new SharedValue<bool>();
            ShowVector = new SharedValue<bool>();

            TiledWidth = new SharedValue<bool>();
            TiledHeight = new SharedValue<bool>();

            TiledWidth.ValueChanged += (newValue) => 
            {
                TiledLayer tiled = (_layer as DocTiledLayer).Layer;
                tiled.TiledWidth = newValue;
            };

            TiledHeight.ValueChanged += (newValue) => 
            {
                TiledLayer tiled = (_layer as DocTiledLayer).Layer;
                tiled.TiledHeight = newValue;
            };
        }

        protected override void Update(float time)
        {
            if ( Document.Instance.SelectedLayer != _layer )
            {
                _layer = Document.Instance.SelectedLayer;
                UpdateProperties();
            }
        }

        public string OnApplyLayerName(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return _layer.Name.StringValue;
            }

            _layer.Name.StringValue = text;


            Document.Current.SetModified();
            return text;
        }

        public string OnApplySpeedX(string text)
        {
            int value;
            int.TryParse(text, out value);

            int newValue = Math.Max(10, Math.Min(200, value));

            if ( newValue != value )
            {
                text = string.Format("{0}", newValue);
            }

            _layer.Layer.ScrollSpeed.X = (float)value / 100f;

            return text;
        }

        public string OnApplySpeedY(string text)
        {
            int value;
            int.TryParse(text, out value);

            int newValue = Math.Max(10, Math.Min(200, value));

            if ( newValue != value )
            {
                text = string.Format("{0}", newValue);
            }

            _layer.Layer.ScrollSpeed.Y = (float)value / 100f;
            Document.Current.SetModified();
            return text;
        }

        public string OnApplyWidth(string text)
        {
            TiledLayer tiled = (_layer as DocTiledLayer).Layer;

            int value;
            int.TryParse(text, out value);

            int newValue = Math.Max(1, Math.Min(1024, value));

            if ( newValue != value )
            {
                text = string.Format("{0}", newValue);
            }

            tiled.Resize(value, tiled.Height);
            Document.Current.SetModified();
            return text;
        }

        public string OnApplyHeight(string text)
        {
            TiledLayer tiled = (_layer as DocTiledLayer).Layer;

            int value;
            int.TryParse(text, out value);

            int newValue = Math.Max(1, Math.Min(1024, value));

            if ( newValue != value )
            {
                text = string.Format("{0}", newValue);
            }

            tiled.Resize(tiled.Width, value);
            Document.Current.SetModified();
            return text;
        }

        void UpdateProperties()
        {
            Layer layer = _layer.Layer;

            LayerName.Format("{0}", _layer.Name);
            LayerSpeedX.Format("{0}", (int)(layer.ScrollSpeed.X * 100));
            LayerSpeedY.Format("{0}", (int)(layer.ScrollSpeed.Y * 100));

            if (_layer is DocTiledLayer)
            {
                TiledLayer tiled = (_layer as DocTiledLayer).Layer;
                LayerWidth.Format("{0}", tiled.Width);
                LayerHeight.Format("{0}", tiled.Height);
                ShowVector.Value = false;
                ShowTiled.Value = true;
                TiledWidth.Value = tiled.TiledWidth;
                TiledHeight.Value = tiled.TiledHeight;
            }
            else
            {
                ShowVector.Value = true;
                ShowTiled.Value = false;
            }
        }
    }
}

