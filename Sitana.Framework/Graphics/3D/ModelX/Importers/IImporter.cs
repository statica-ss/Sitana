using System;
using System.IO;
using Ebatianos.Graphics;

namespace Ebatianos.Graphics.Model.Importers
{
    public delegate Stream OpenFileDelegate(String name);

    public interface IImporter
    {
        ModelX Import(Stream stream, OpenFileDelegate openFileDelegate);
        String SupportedFileExt { get; }
    }
}
