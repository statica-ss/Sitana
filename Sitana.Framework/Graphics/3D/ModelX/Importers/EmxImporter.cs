// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace Sitana.Framework.Graphics.Model.Importers
{
    public class EmxImporter : IImporter
    {
        public String SupportedFileExt
        {
            get
            {
                return ".emx";
            }
        }

        public ModelX Import(Stream stream, OpenFileDelegate openFileDelegate)
        {
            return ModelX.Load(stream);
        }
    }
}
