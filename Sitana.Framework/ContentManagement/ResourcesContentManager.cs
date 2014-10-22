using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Sitana.Framework.Content
{
    public class ResourcesContentManager : ContentManager
    {
        private Assembly _assembly;
        private string _root;

        public ResourcesContentManager(IServiceProvider services, Assembly assembly, string root)
            : base(services, "" )
        {
            _assembly = assembly;
            _root = root;
        }

        protected override Stream OpenStream(string assetName)
        {
            return Open(assetName + ".xnb");
        }

        public Stream Open(string assetName)
        {
            return _assembly.GetManifestResourceStream(Path.Combine(_root, assetName).Replace('\\', '.').Replace('/', '.'));
        }
    }
}
