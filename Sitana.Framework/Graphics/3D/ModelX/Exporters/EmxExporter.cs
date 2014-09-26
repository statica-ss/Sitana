using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sitana.Framework.Graphics;

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
