// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Globalization;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Graphics.Model.Importers
{
    public class ObjImporter: IImporter
    {
        

        public String SupportedFileExt
        {
            get
            {
                return ".obj";
            }
        }

        public ModelX Import(Stream stream, OpenFileDelegate openFileDelegate)
        {
            return Load(stream, openFileDelegate);
        }

        private ModelX Load(Stream stream, OpenFileDelegate openFileDelegate)
        {
            Dictionary<VertexPositionNormalTexture, int> vertexToIndex = new Dictionary<VertexPositionNormalTexture,int>();
            List<ModelSubset> subsets = new List<ModelSubset>();
            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();

            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> coords = new List<Vector2>();

            StreamReader reader = new StreamReader(stream);

            List<Int16> subsetIndices = new List<Int16>();

            List<Tuple<String, Material>> materials = new List<Tuple<String, Material>>();

            int subsetMaterial = -1;
            List<String> temp = new List<String>();

            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                String[] parts = line.Split(' ', '\t');

                temp.Clear();

                foreach (var part in parts)
                {
                    if (!String.IsNullOrWhiteSpace(part))
                    {
                        temp.Add(part);
                    }
                }

                if (temp.Count == 0)
                {
                    continue;
                }

                parts = temp.ToArray();

                switch (parts[0])
                {
                    case "mtllib":
                        {
                            String name = parts[1];

                            for (int idx = 2; idx < parts.Length; ++idx)
                            {
                                name += " " + parts[idx];
                            }

                            name = name.Trim();

                            if (name.StartsWith("./"))
                            {
                                name = name.Substring(2);
                            }

                            using (Stream matStream = openFileDelegate(name))
                            {
                                ReadMaterials(materials, matStream);
                            }
                        }

                        break;

                    case "v":
                        AddVector3(positions, parts);
                        break;

                    case "vn":
                        AddVector3(normals, parts);
                        break;

                    case "vt":
                        AddVector2(coords, parts);
                        break;

                    case "f":
                        AddIndices(subsetIndices, parts, vertices, vertexToIndex, positions, normals, coords);
                        break;

                    case "usemtl":

                        if (subsetMaterial >= 0) 
                        {
                            AddNewSubset(subsets, subsetIndices, ref subsetMaterial);
                        }
                        
                        subsetMaterial = FindMaterial(parts[1], materials);

                        if (subsetMaterial < 0)
                        {
                            throw new InvalidDataException(String.Format("Unknown material: {0}.", parts[1]));
                        }

                        break;
                }
            }

            if (subsetMaterial >= 0)
            {
                AddNewSubset(subsets, subsetIndices, ref subsetMaterial);
            }

            return new ModelX(new List<Material[]>(){GetArrayOfMaterials(materials)}, subsets.ToArray(), vertices.ToArray());
        }

        private static Material[] GetArrayOfMaterials(List<Tuple<String, Material>> materials)
        {
            Material[] mats = new Material[materials.Count];

            for(int idx = 0; idx < materials.Count;++idx)
            {
                mats[idx] = materials[idx].Item2;
            }

            return mats;
        }

        private static int FindMaterial(String name, List<Tuple<String, Material>> materials)
        {
            for ( int idx = 0; idx < materials.Count; ++idx )
            {
                if ( materials[idx].Item1 == name)
                {
                    return idx;
                }
            }

            return -1;
        }

        private static void ReadMaterials(List<Tuple<String, Material>> materials, Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

            String matName = null;
            String textureName = null;

            Color diffuse = Color.White;
            Color ambient = Color.White;
            Color specular = Color.White;
            float specularExponent = 1;

            float opacity = 1;

            List<String> temp = new List<String>();

            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine().Trim();
                String[] parts = line.Split(' ');

                temp.Clear();

                foreach (var part in parts)
                {
                    if (!String.IsNullOrWhiteSpace(part))
                    {
                        temp.Add(part);
                    }
                }

                if (temp.Count == 0)
                {
                    continue;
                }

                parts = temp.ToArray();

                switch (parts[0])
                {
                    case "newmtl":

                        if (matName != null)
                        {
                            materials.Add(new Tuple<String, Material>(matName, new Material(textureName, diffuse.ToVector3(), ambient.ToVector3(), specular.ToVector3(), specularExponent, Vector3.Zero, opacity)));
                        }

                        matName = parts[1];
                        specularExponent = 0;
                        diffuse = Color.White;
                        ambient = Color.White;
                        specular = Color.White;
                        textureName = null;
                        opacity = 1;
                        break;

                    case "Ns":
                        specularExponent = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        break;

                    case "d":
                        opacity = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        break;

                    case "Ka":
                        ambient = ParseMtlColor(parts);
                        break;

                    case "Kd":
                        diffuse = ParseMtlColor(parts);
                        break;

                    case "Ks":
                        specular = ParseMtlColor(parts);
                        break;

                    case "map_Kd":
                        textureName = parts[1];
                        break;
                }
            }

            if (matName != null)
            {
                materials.Add(new Tuple<String, Material>(matName, new Material(textureName, diffuse.ToVector3(), ambient.ToVector3(), specular.ToVector3(), specularExponent, Vector3.Zero, opacity)));
            }
        }

        private static Color ParseMtlColor(String[] parts)
        {
            try
            {
                return new Color(float.Parse(parts[1], CultureInfo.InvariantCulture),
                                 float.Parse(parts[2], CultureInfo.InvariantCulture),
                                 float.Parse(parts[3], CultureInfo.InvariantCulture));
            }
            catch(Exception ex)
            {
                throw new InvalidDataException("Only RGB color space is supported for material colors.", ex);
            }
        }

        private static void AddNewSubset(List<ModelSubset> subsets, List<Int16> subsetIndices, ref int material)
        {
            subsets.Add(new ModelSubset(material, subsetIndices.ToArray()));

            subsetIndices.Clear();
            material = -1;
        }

        private static void AddVector3(List<Vector3> list, String[] parts)
        {
            try
            {
                list.Add(new Vector3(float.Parse(parts[1], CultureInfo.InvariantCulture),
                                     float.Parse(parts[2], CultureInfo.InvariantCulture),
                                     float.Parse(parts[3], CultureInfo.InvariantCulture)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void AddVector2(List<Vector2> list, String[] parts)
        {
            list.Add(new Vector2(float.Parse(parts[1], CultureInfo.InvariantCulture),
                                 1-float.Parse(parts[2], CultureInfo.InvariantCulture)));
        }

        private static void AddIndices(List<Int16> indices, String[] parts, List<VertexPositionNormalTexture> vertices, Dictionary<VertexPositionNormalTexture, int> vertexToIndex, List<Vector3> positions, List<Vector3> normals, List<Vector2> coords)
        {
            for (int idx = 2; idx >= 0; --idx)
            {
                Tuple<int, int, int> vertexIndices;

                try
                {
                    vertexIndices = GetIndices(parts[1+idx]);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new InvalidDataException("Invalid face format. Face must have 3 vertice mappings.", ex);
                }

                try
                {
                    int index1 = vertexIndices.Item1;
                    int index2 = vertexIndices.Item2;
                    int index3 = vertexIndices.Item3;

                    index1 = index1 < 0 ? positions.Count + index1 : index1 - 1;
                    index2 = index2 < 0 ? coords.Count + index2 : index2 - 1;
                    index3 = index3 < 0 ? normals.Count + index3 : index3 - 1;

                    VertexPositionNormalTexture vertex = new VertexPositionNormalTexture(
                        positions[index1],
                        normals[index3],
                        coords[index2]
                        );

                    int index = 0;

                    if (!vertexToIndex.TryGetValue(vertex, out index))
                    {
                        index = vertices.Count;
                        vertices.Add(vertex);
                        vertexToIndex.Add(vertex, index);
                    }

                    indices.Add((Int16)index);
                }
                catch(Exception ex)
                {
                    throw new InvalidDataException("Invalid face format.", ex);
                }
            }
        }

        private static Tuple<int, int, int> GetIndices(String part)
        {
            try
            {
                String[] parts = part.Split('/');
                return new Tuple<int, int, int>(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            }
            catch
            {
                throw new InvalidDataException("Invalid face format. Vertex must be: position index, texture coordinate index, normal index.");
            }
        }
    }
}
