/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Ebatianos.Platform
{
    public abstract class Map
    {
        public class Layer
        {
            // Version of layer.
            private static Byte Version = 4;

            /// <summary>
            /// Name of layer.
            /// </summary>
            public String Name { get; internal set; }

            /// <summary>
            /// Name of tileset used to create layer.
            /// </summary>
            public String TileCollection { get; set; }

            /// <summary>
            /// Array of tiles.
            /// </summary>
            public UInt16[,] Tiles { get; set; }

            /// <summary>
            /// Array of events.
            /// </summary>
            public Byte[,] Events { get; private set; }

            /// <summary>
            /// Width of the layer.
            /// </summary>
            public UInt16 Width { get; private set; }

            /// <summary>
            /// Height of the layer.
            /// </summary>
            public UInt16 Height { get; private set; }

            /// <summary>
            /// Horizontal speed determining how fast for observer move this layer moves.
            /// </summary>
            public Double HorizontalSpeed { get; set; }

            /// <summary>
            /// Vertical speed determining how fast for observer move this layer moves.
            /// </summary>
            public Double VerticalSpeed { get; set; }

            /// <summary>
            /// Flag defining if layer should be wrapped horizontaly.
            /// </summary>
            public Boolean IsTiledWidth { get; set; }

            /// <summary>
            /// Flag defining if layer should be wrapped verticaly.
            /// </summary>
            public Boolean IsTiledHeight { get; set; }

            /// <summary>
            /// Opacity of the layer.
            /// </summary>
            public Double Opacity { get; set; }

            // Color of layer.
            public Color Color { get; set; }

            /// <summary>
            /// Scale.
            /// </summary>
            public Double Scale { get; set; }

            /// <summary>
            /// Scale exponent.
            /// </summary>
            public Double ScaleExponent { get; set; }

            /// <summary>
            /// Returns if the layer is main layer (and has events).
            /// </summary>
            public Boolean IsMainLayer { get; private set; }

            public Boolean LinkMainLayer { get; set; }

            public Dictionary<Point, UInt16> EventTagsOnMap { get; private set; }

            /// <summary>
            /// Constructs new layer.
            /// </summary>
            /// <param name="name">Name of the layer.</param>
            /// <param name="isMainLayer">Defines if layer is main layer (and has events).</param>
            /// <param name="width">Width of the layer.</param>
            /// <param name="height">Height of the layer.</param>
            protected Layer(String name, Boolean isMainLayer, Boolean linkMainLayer, UInt16 width, UInt16 height)
            {
                // Create array of tiles.
                Tiles = new UInt16[width, height];

                // If main layer, create array of events.
                if (isMainLayer)
                {
                    Events = new Byte[width, height];
                }

                // Set layer attributes.
                Width = width;
                Height = height;

                Name = name;

                Color = Color.White;
                TileCollection = "";
                HorizontalSpeed = 1;
                VerticalSpeed = 1;
                Scale = 1;
                ScaleExponent = 1;
                IsTiledWidth = false;
                IsTiledHeight = false;
                Opacity = 1;
                IsMainLayer = isMainLayer;
                LinkMainLayer = linkMainLayer | isMainLayer;
                EventTagsOnMap = new Dictionary<Point, UInt16>();
            }

            public UInt16 EventTag(Int32 x, Int32 y)
            {
                UInt16 value = 0;
                EventTagsOnMap.TryGetValue(new Point(x, y), out value);

                return value;
            }

            public void EventTag(Int32 x, Int32 y, UInt16 tag)
            {
                EventTagsOnMap.Remove(new Point(x, y));

                if (tag != 0)
                {
                    EventTagsOnMap.Add(new Point(x, y), tag);
                }
            }

            /// <summary>
            /// Resizes layer without removing cropped data.
            /// </summary>
            /// <param name="width">New width for layer.</param>
            /// <param name="height">New height form layer.</param>
            public void Resize(UInt16 width, UInt16 height)
            {
                // Check if array should be resized.
                if (width > Tiles.GetLength(0) || height > Tiles.GetLength(1))
                {
                    // Compute new dimensions of an array.
                    Int32 newWidth = Math.Max(width, Tiles.GetLength(0));
                    Int32 newHeight = Math.Max(height, Tiles.GetLength(1));

                    // Create new array.
                    UInt16[,] newTiles = new UInt16[newWidth, newHeight];

                    // Copy data to new array.
                    for (Int32 x = 0; x < Tiles.GetLength(0); ++x)
                    {
                        for (Int32 y = 0; y < Tiles.GetLength(1); ++y)
                        {
                            newTiles[x, y] = Tiles[x, y];
                        }
                    }

                    Tiles = newTiles;

                    // If layer has events, do the same with events' array.
                    if (Events != null)
                    {
                        Byte[,] newEvents = new Byte[newWidth, newHeight];

                        for (Int32 x = 0; x < Events.GetLength(0); ++x)
                        {
                            for (Int32 y = 0; y < Events.GetLength(1); ++y)
                            {
                                newEvents[x, y] = Events[x, y];
                            }
                        }

                        Events = newEvents;
                    }
                }

                // Set layer width and height.
                Width = width;
                Height = height;
            }

            /// <summary>
            /// Serializes layer into BinaryWriter.
            /// </summary>
            /// <param name="writer">BinaryWriter used to store data in stream.</param>
            protected void Serialize(BinaryWriter writer)
            {
                writer.Write(Version);
                writer.Write(TileCollection);
                writer.Write(HorizontalSpeed);
                writer.Write(VerticalSpeed);
                writer.Write(Scale);
                writer.Write(ScaleExponent);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write((Byte)(IsTiledWidth ? 1 : 0));
                writer.Write((Byte)(IsTiledHeight ? 1 : 0));
                writer.Write(Opacity);

                writer.Flush();

                for (Int32 idx = 0; idx < Width; ++idx)
                {
                    for (Int32 idy = 0; idy < Height; ++idy)
                    {
                        writer.Write(Tiles[idx, idy]);
                    }

                    writer.Flush();
                }

                writer.Flush();

                if (Events != null)
                {
                    for (Int32 idx = 0; idx < Width; ++idx)
                    {
                        for (Int32 idy = 0; idy < Height; ++idy)
                        {
                            writer.Write(Events[idx, idy]);
                        }

                        writer.Flush();
                    }
                }

                writer.Write(EventTagsOnMap.Count);

                foreach (var tag in EventTagsOnMap)
                {
                    writer.Write(tag.Key.X);
                    writer.Write(tag.Key.Y);
                    writer.Write(tag.Value);
                }

                writer.Write(LinkMainLayer);
            }

            /// <summary>
            /// Unserializes layer from BinaryReader.
            /// </summary>
            /// <param name="reader">BinaryReader used to read data from stream.</param>
            protected void Unserialize(BinaryReader reader)
            {
                // Get serialized layer version.
                Byte version = reader.ReadByte();

                // Select proper version and userialize layer.
                switch (version)
                {
                    case 1:
                        UnserializeVer1(reader);
                        break;

                    case 2:
                        UnserializeVer2(reader);
                        break;

                    case 3:
                        UnserializeVer3(reader);
                        break;

                    case 4:
                        UnserializeVer4(reader);
                        break;
                }
            }

            // Unserializes layer version 3.
            protected void UnserializeVer4(BinaryReader reader)
            {
                UnserializeVer1(reader);

                Int32 countEventTags = reader.ReadInt32();

                for (Int32 idx = 0; idx < countEventTags; ++idx)
                {
                    Int32 xx = reader.ReadInt32();
                    Int32 yy = reader.ReadInt32();
                    UInt16 tag = reader.ReadUInt16();

                    EventTag(xx, yy, tag);
                }

                LinkMainLayer = reader.ReadBoolean();
            }

            // Unserializes layer version 3.
            protected void UnserializeVer3(BinaryReader reader)
            {
                UnserializeVer2(reader);

                LinkMainLayer = reader.ReadBoolean();
            }

            // Unserializes layer version 2.
            protected void UnserializeVer2(BinaryReader reader)
            {
                UnserializeVer1(reader);

                Int32 countEventTags = reader.ReadInt32();

                for (Int32 idx = 0; idx < countEventTags; ++idx)
                {
                    Int32 xx = reader.ReadInt32();
                    Int32 yy = reader.ReadInt32();
                    Byte tag = reader.ReadByte();

                    EventTag(xx, yy, tag);
                }
            }

            // Unserializes layer version 1.
            protected void UnserializeVer1(BinaryReader reader)
            {
                TileCollection = reader.ReadString();
                HorizontalSpeed = reader.ReadDouble();
                VerticalSpeed = reader.ReadDouble();
                Scale = reader.ReadDouble();
                ScaleExponent = reader.ReadDouble();

                UInt16 width = reader.ReadUInt16();
                UInt16 height = reader.ReadUInt16();

                IsTiledWidth = reader.ReadByte() == 1;
                IsTiledHeight = reader.ReadByte() == 1;

                Opacity = reader.ReadDouble();

                Resize(width, height);

                try
                {
                    for (Int32 idx = 0; idx < Width; ++idx)
                    {
                        for (Int32 idy = 0; idy < Height; ++idy)
                        {
                            Tiles[idx, idy] = reader.ReadUInt16();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw;
                }

                try
                {
                    if (Events != null)
                    {
                        for (Int32 idx = 0; idx < Width; ++idx)
                        {
                            for (Int32 idy = 0; idy < Height; ++idy)
                            {
                                Events[idx, idy] = reader.ReadByte();
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw;
                }

            }
        }

        /// <summary>
        /// Class to encapsulate layer functionality for serialize and unserialize.
        /// </summary>
        private class Layer0 : Layer
        {
            /// <summary>
            /// Constructs new object.
            /// </summary>
            /// <param name="name">Name of layer.</param>
            /// <param name="isMainLayer">Should layer be main layer (and have events).</param>
            /// <param name="width">Width of layer.</param>
            /// <param name="height">Height of layer.</param>
            public Layer0(String name, Boolean isMainLayer, Boolean linkMainLayer, UInt16 width, UInt16 height)
                : base(name, isMainLayer, linkMainLayer, width, height)
            {
            }

            /// <summary>
            /// Serializes layer to BinaryWriter.
            /// </summary>
            /// <param name="writer">BinaryWriter used to write data to stream.</param>
            public void SerializeLayer(BinaryWriter writer)
            {
                base.Serialize(writer);
            }

            /// <summary>
            /// Unserializes layer from BinaryReader.
            /// </summary>
            /// <param name="reader">BinaryReader used to read data from stream.</param>
            public void UnserializeLayer(BinaryReader reader)
            {
                base.Unserialize(reader);
            }
        }

        /// <summary>
        /// Map layers number.
        /// </summary>
        public Int32 LayersNo
        {
            get
            {
                return layers.Count;
            }
        }

        /// <summary>
        /// Size of the tiles.
        /// </summary>
        public Int32 TileSize { get; protected set; }

        /// <summary>
        /// Size of the tiles.
        /// </summary>
        public Int32 DisplayTileSize { get; protected set; }

        /// <summary>
        /// Size of the tiles.
        /// </summary>
        public Int32 EditTileSize { get; protected set; }

        // List of all layers in map.
        private List<Layer0> layers = new List<Layer0>();

        public Color Background { get; protected set; }

        public Boolean EventsCentered { get; protected set; }

        public Map()
        {
            Background = Color.CornflowerBlue;
            EventsCentered = false;
        }

        /// <summary>
        /// Returns layer at specyfied index.
        /// </summary>
        /// <param name="index">Layer index.</param>
        /// <returns>Layer object.</returns>
        public Layer GetLayer(Int32 index)
        {
            return layers[index];
        }

        /// <summary>
        /// Adds layter to map.
        /// </summary>
        /// <param name="name">Name of the layer.</param>
        /// <param name="isMainLayer">Flag defining if layer should be main layer.</param>
        /// <param name="width">Width of the layer.</param>
        /// <param name="height">Height of the layer.</param>
        /// <returns>Created layer.</returns>
        protected Layer AddLayer(String name, Boolean isMainLayer, Boolean linkMainLayer, UInt16 width, UInt16 height)
        {
            // Create new layer.
            Layer0 layer = new Layer0(name, isMainLayer, linkMainLayer, width, height);

            // Add to list.
            layers.Add(layer);

            // Return created layer.
            return layer;
        }

        protected Layer InsertLayer(Int32 index, String name, Boolean linkMainLayer, UInt16 width, UInt16 height)
        {
            String tileset = "";

            if (linkMainLayer)
            {
                foreach (var layer in layers)
                {
                    if (layer.IsMainLayer)
                    {
                        width = layer.Width;
                        height = layer.Height;
                        tileset = layer.TileCollection;
                        break;
                    }
                }
            }

            // Create new layer.
            Layer0 newLayer = new Layer0(name, false, linkMainLayer, width, height);

            newLayer.TileCollection = tileset;

            // Add to list.
            layers.Insert(index, newLayer);

            // Return created layer.
            return newLayer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="writer"></param>
        protected void SerializeLayer(Int32 index, BinaryWriter writer)
        {
            layers[index].SerializeLayer(writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="reader"></param>
        protected void UnserializeLayer(Int32 index, BinaryReader reader)
        {
            layers[index].UnserializeLayer(reader);
        }

        /// <summary>
        /// Creates new map.
        /// </summary>
        public abstract void New();

        /// <summary>
        /// Loads map from stream.
        /// </summary>
        /// <param name="stream">Stream to load map from.</param>
        public abstract void Load(Stream stream);

        /// <summary>
        /// Saves map to stream.
        /// </summary>
        /// <param name="stream">Stream to save map to.</param>
        public abstract void Save(Stream stream);

        /// <summary>
        /// Returns Event id (string) for event index.
        /// </summary>
        /// <param name="index">Event index.</param>
        /// <returns>Event id (string).</returns>
        public abstract String EventId(Byte index);

        /// <summary>
        /// Template name of map.
        /// </summary>
        /// <returns>Template name.</returns>
        public abstract String Template();

        /// <summary>
        /// Returns list of possible event tags.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>List of possible event tags.</returns>
        public virtual String[] EventParameters(Byte index)
        {
            return null;
        }

        public virtual List<KeyValuePair<String, Boolean>> EventGroups(Byte index)
        {
            return null;
        }
    }
}
