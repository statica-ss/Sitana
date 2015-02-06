// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Graphics.Model.Importers
{
    public class PlyImporter : IImporter
    {
        public String SupportedFileExt
        {
            get
            {
                return ".ply";
            }
        }

        public ModelX Import(Stream stream, OpenFileDelegate openFileDelegate)
        {
            return Load(stream);
        }

        private ModelX Load(Stream stream)
        {
            String[] vertexDeclaration = new String[] { "x", "y", "z", "nx", "ny", "nz", "s", "t" };

            List<Int16> indices = new List<Int16>();

            ModelSubset[] subsets = new ModelSubset[1];

            VertexPositionNormalTexture[] vertices = null;

            StreamReader reader = new StreamReader(stream);

            int numVertices = 0;
            int vertexIndex = 0;

            int numFaces = 0;

            int vertexDeclarationIndex = 0;

            Boolean checkVertexDeclaration = false;

            Boolean readVertices = false;
            Boolean readFaces = false;

            while (!reader.EndOfStream)
            {
                String line = reader.ReadLine();
                String[] parts = line.Split(' ');

                if (readVertices)
                {
                    vertices[vertexIndex] = new VertexPositionNormalTexture(
                        new Vector3(ParseSingle(parts[0]), ParseSingle(parts[1]), ParseSingle(parts[2])),
                        new Vector3(ParseSingle(parts[3]), ParseSingle(parts[4]), ParseSingle(parts[5])),
                        new Vector2(ParseSingle(parts[6]), ParseSingle(parts[7])));

                    vertexIndex++;

                    if (vertexIndex == numVertices)
                    {
                        readVertices = false;
                        readFaces = true;
                    }

                    continue;
                }

                if (readFaces)
                {
                    if (parts[0] == "3")
                    {
                        indices.Add(Int16.Parse(parts[1]));
                        indices.Add(Int16.Parse(parts[2]));
                        indices.Add(Int16.Parse(parts[3]));

                        if (indices.Count == numFaces * 3)
                        {
                            readFaces = false;
                            break;
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("Faces other than triangles are not supported.");
                    }
                }

                if (parts[0] == "property")
                {
                    if (checkVertexDeclaration)
                    {
                        if (parts[2] != vertexDeclaration[vertexDeclarationIndex])
                        {
                            throw new InvalidDataException("Invalid vertex declaration. Supported is: x,y,z,nx,ny,nz,s,t");
                        }

                        vertexDeclarationIndex++;

                        if (vertexDeclarationIndex == vertexDeclaration.Length)
                        {
                            checkVertexDeclaration = false;
                        }
                    }

                    continue;
                }

                if (parts[0] == "element")
                {
                    if (parts[1] == "vertex")
                    {
                        numVertices = int.Parse(parts[2]);
                        vertices = new VertexPositionNormalTexture[numVertices];
                        checkVertexDeclaration = true;
                    }

                    if (parts[1] == "face")
                    {
                        if (checkVertexDeclaration)
                        {
                            throw new InvalidDataException("Invalid vertex declaration. Supported is: x,y,z,nx,ny,nz,s,t");
                        }

                        numFaces = int.Parse(parts[2]);
                    }
                }

                if (parts[0] == "end_header")
                {
                    readVertices = true;
                }
            }

            if (vertices == null)
            {
                throw new InvalidDataException("Unknown error while parsing file.");

            }

            Material material = new Material(null);

            subsets[0] = new ModelSubset(0, indices.ToArray());
            return new ModelX(new List<Material[]>(){new Material[1]{material}}, subsets, vertices);
        }

        float ParseSingle(String text)
        {
            return float.Parse(text, CultureInfo.InvariantCulture);
        }
    }
}
