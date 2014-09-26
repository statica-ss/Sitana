using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ebatianos.Graphics;

namespace Ebatianos.Graphics.Model.Exporters
{
    public interface IExporter
    {
        void Export(Stream stream, ModelX model);
        String SupportedFileExt { get; }
    }
}
