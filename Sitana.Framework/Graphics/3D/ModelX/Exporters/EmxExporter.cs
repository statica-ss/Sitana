// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace Sitana.Framework.Graphics.Model.Exporters
{
    public class EmxExporter : IExporter
    {
        public String SupportedFileExt
        {
            get
            {
                return ".emx";
            }
        }

        public void Export(Stream stream, ModelX model)
        {
            model.Save(stream);
        }
    }
}
