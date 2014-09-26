using System;
using System.IO;
using Sitana.Framework.Graphics;

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
