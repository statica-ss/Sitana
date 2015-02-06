using System;
using Sitana.Framework.Cs;
using System.IO;
using Sitana.Framework.Ui.Binding;
using System.Xml;
using Sitana.Framework.Content;
using Sitana.Framework;

namespace GameEditor
{
    public class Document: Singleton<Document>
    {
        public static Document Current
        {
            get
            {
                return Instance;
            }
        }

        int _nextIndex = 0;

        public readonly SharedString FileName = new SharedString();

        public string FilePath { get; private set;}

        public ItemsList<DocLayer> Layers { get; private set;}

        private int _layerIndex = 1;

        public bool IsModified { get; private set;}

        public event EmptyArgsVoidDelegate LayerSelectionChanged;

        public DocLayer SelectedLayer
        {
            get
            {
                for (int idx = 0; idx < Layers.Count; ++idx)
                {
                    if (Layers[idx].Selected.Value)
                    {
                        return Layers[idx];
                    }
                }

                return null;
            }
        }

        public void New()
        {
            new Tools.Select();

            _nextIndex++;
            FileName.Format("New {0}", _nextIndex);
            FilePath = null;

            Layers.Clear();

            foreach (var layer in CurrentTemplate.Instance.Layers)
            {
                DocLayer generated = layer.Generate();
                Layers.Add(generated);
                if (layer.Selected)
                {
                    Select(generated);
                }
            }

            _layerIndex = Layers.Count + 1;
            IsModified = false;
        }

        public Document()
        {
            Layers = new ItemsList<DocLayer>();
        }

        public void Save()
        {
            Save(FilePath);
        }

        public void Save(string path)
        {
            FilePath = path;
            FileName.StringValue = Path.GetFileNameWithoutExtension(path);

            IsModified = false;

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(stream);

                writer.Write(1);
                writer.Write(CurrentTemplate.Instance.Guid);

                WriteVersion1(writer);
            }
        }

        public void Open(string path)
        {
            using(Stream stream = new FileStream(path, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(stream);

                int version = reader.ReadInt32();
                string guid = reader.ReadString();

                RegisteredTemplates.Template template = RegisteredTemplates.Instance.FindTemplate(guid);

                if (template == null)
                {
                    MessageBox.Info(string.Format("Template {0} is not registered.", guid));
                    return;
                }

                if (template.Path.IsNullOrEmpty())
                {
                    using (Stream templateStream = ContentLoader.Current.Open("Templates/SampleTemplate.zip"))
                    {
                        CurrentTemplate.Instance.Load(templateStream);
                    }
                }
                else
                {
                    using (Stream templateStream = new FileStream(template.Path, FileMode.Open))
                    {
                        CurrentTemplate.Instance.Load(templateStream);
                    }
                }

                New();
                Layers.Clear();

                ReadVersion1(reader);

                _layerIndex = Layers.Count + 1;
                Layers[Layers.Count - 1].Selected.Value = true;
            }

            FilePath = path;
            FileName.StringValue = Path.GetFileNameWithoutExtension(path);
        }

        void WriteVersion1(BinaryWriter writer)
        {
            writer.Write(Layers.Count);

            for (int idx = 0; idx < Layers.Count; ++idx)
            {
                bool tiled = Layers[idx] is DocTiledLayer;
                writer.Write(tiled);

                Layers[idx].Serialize(writer);
            }
        }

        void ReadVersion1(BinaryReader reader)
        {
            int layersNo = reader.ReadInt32();

            for (int idx = 0; idx < layersNo; ++idx )
            {
                bool tiled = reader.ReadBoolean();

                DocLayer layer = tiled ? (DocLayer)new DocTiledLayer("") : (DocLayer)new DocVectorLayer("");
                layer.Deserialize(reader);

                Layers.Add(layer);
            }
        }

        public void CancelModified()
        {
            IsModified = false;
        }

        public void SetModified()
        {
            IsModified = true;
        }

        public void AddVectorLayer()
        {
            var layer = new DocVectorLayer(String.Format("LAYER {0}", _layerIndex));
            Layers.Add(layer);
            _layerIndex++;

            Select(layer);
            SetModified();
        }
            
        public void AddTilesetLayer()
        {
            var layer = new DocTiledLayer(String.Format("LAYER {0}", _layerIndex));
            layer.Layer.Tileset = CurrentTemplate.Instance.Tileset(null).Item1;
            Layers.Add(layer);
            _layerIndex++;

            Select(layer);
            SetModified();
        }

        public void RemoveSelectedLayer()
        {
            if ( Layers.Count == 1 )
            {
                return;
            }

            for( int idx = 0; idx < Layers.Count; ++idx )
            {
                if(Layers[idx].Selected.Value)
                {
                    Layers.RemoveAt(idx);
                    break;
                }
            }

            Select(Layers[0]);
            SetModified();
        }

        public void Select(DocLayer layer)
        {
            for( int idx = 0; idx < Layers.Count; ++idx )
            {
                Layers[idx].Selected.Value = false;
            }

            layer.Selected.Value = true;

            if (LayerSelectionChanged != null)
            {
                LayerSelectionChanged();
            }
        }
    }
}

