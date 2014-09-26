// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace Sitana.Framework.Graphics.Model.Importers
{
    public delegate Stream OpenFileDelegate(String name);

    public interface IImporter
    {
        ModelX Import(Stream stream, OpenFileDelegate openFileDelegate);
        String SupportedFileExt { get; }
    }
}
