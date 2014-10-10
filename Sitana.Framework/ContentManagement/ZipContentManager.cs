using System;
using Microsoft.Xna.Framework.Content;
using ICSharpCode.SharpZipLib.Zip;

namespace Sitana.Framework.Content
{
    public class ZipContentManager : ContentManager
    {
        private ZipFile _zipFile = null;

        public ZipContentManager(IServiceProvider services, ZipFile zipFile)
            : base(services, "")
        {
            _zipFile = zipFile;
        }

        protected override System.IO.Stream OpenStream(string assetName)
        {
            var entry = _zipFile.GetEntry(assetName.Replace('\\', '/') + ".xnb");
            if (entry != null)
            {
                return _zipFile.GetInputStream(entry);
            }
            return null;
        }
    }
}
