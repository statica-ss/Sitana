using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Xml;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameEditor
{
    public class CurrentTemplate : Singleton<CurrentTemplate>
    {
        public string Guid { get; private set; }
        public string Name { get; private set; }

        List<LayerDefinition> _layers = new List<LayerDefinition>();
        List<Tuple<string, Texture2D>> _tilesets = new List<Tuple<string, Texture2D>>();

        public int TileSize { get; private set; }
        public int TilesetLineWidth { get; private set; }

        public int UnitSize { get; private set; }

        public Tuple<string, Texture2D> Tileset(string name)
        {
            foreach (var ts in _tilesets)
            {
                if (name == ts.Item1)
                {
                    return ts;
                }
            }

            return _tilesets[0];
        }

        public List<LayerDefinition> Layers
        {
            get
            {
                return _layers;
            }
        }

        public void Load(Stream stream)
        {
            foreach (var tileset in _tilesets)
            {
                tileset.Item2.Dispose();
            }
            _tilesets.Clear();
            _layers.Clear();

            byte[] buffer = new byte[4096];     // 4K is optimum


            ZipFile zf = new ZipFile(stream);

            foreach (ZipEntry zipEntry in zf)
            {
                if (!zipEntry.IsFile)
                {
                    continue;           // Ignore directories
                }

                String entryFileName = zipEntry.Name;
                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.


                Stream zipStream = zf.GetInputStream(zipEntry);

                MemoryStream fileStream = new MemoryStream();
                StreamUtils.Copy(zipStream, fileStream, buffer);
                zipStream.Close();

                fileStream = new MemoryStream(fileStream.GetBuffer());

                string dir = Path.GetDirectoryName(entryFileName);
                string file = Path.GetFileNameWithoutExtension(entryFileName);
                string ext = Path.GetExtension(entryFileName).ToLowerInvariant();

                if (entryFileName == "Definition.xml")
                {
                    ParseDefinition(XFile.Create(fileStream, entryFileName));
                    continue;
                }

                switch (dir)
                {
                    case "Tilesets":
                        {
                            if (ext == ".png")
                            {
                                Texture2D texture = Texture2D.FromStream(AppMain.Current.GraphicsDevice, fileStream);
                                _tilesets.Add(new Tuple<string, Texture2D>(file, texture));
                            }
                            break;
                        }
                }

                fileStream.Close();
            }
        }

        void ParseDefinition(XNode node)
        {
            if (node.Tag != "Template")
            {
                throw new Exception("Invalid Definition file.");
            }

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "Layers":
                        ParseLayers(cn);
                        break;

                    case "Properties":
                        ParseProperties(cn);
                        break;
                }
            }
        }

        void ParseLayers(XNode node)
        {
            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "TiledLayer":
                        _layers.Add(TiledLayerDefinition.FromNode(cn));
                        break;

                    case "VectorLayer":
                        _layers.Add(VectorLayerDefinition.FromNode(cn));
                        break;
                }
            }
        }

        void ParseProperties(XNode node)
        {
            XNodeAttributes attr = new XNodeAttributes(node);

            Name = attr.AsString("Name");
            Guid = attr.AsString("Guid");
            TilesetLineWidth = attr.AsInt32("TilesetLineWidth", 8);

            TileSize = UnitSize = attr.AsInt32("UnitSize", TileSize);
        }


    }
}
