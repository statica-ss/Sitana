using Ionic.Zlib;
using System.IO;

namespace Sitana.Framework
{
    public static class Compression
    {
        static public byte[] DecompressWithZlib(byte[] compressedData, int compressedDataSize)
        {
            MemoryStream memoryStream = new MemoryStream(compressedData, 0, compressedDataSize);

            using (var zipFile = new ZlibStream(memoryStream, CompressionMode.Decompress))
            {
                using (MemoryStream outMemory = new MemoryStream())
                {
                    zipFile.CopyTo(outMemory);
                    return outMemory.ToArray();
                }
            }
        }

        static public byte[] DecompressWithZlib(byte[] compressedData)
        {
            return DecompressWithZlib(compressedData, compressedData.Length);
        }

        static public byte[] DecompressWithGzip(byte[] compressedData, int compressedDataSize)
        {
            MemoryStream memoryStream = new MemoryStream(compressedData, 0, compressedDataSize);

            using (var zipFile = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                using (MemoryStream outMemory = new MemoryStream())
                {
                    zipFile.CopyTo(outMemory);
                    return outMemory.ToArray();
                }
            }
        }

        static public byte[] DecompressWithGzip(byte[] compressedData)
        {
            return DecompressWithGzip(compressedData, compressedData.Length);
        }

        static public byte[] Deflate(byte[] compressedData, int compressedDataSize)
        {
            MemoryStream memoryStream = new MemoryStream(compressedData, 0, compressedDataSize);

            using (var zipFile = new DeflateStream(memoryStream, CompressionMode.Decompress))
            {
                using (MemoryStream outMemory = new MemoryStream())
                {
                    zipFile.CopyTo(outMemory);
                    return outMemory.ToArray();
                }
            }
        }

        static public byte[] Deflate(byte[] compressedData)
        {
            return Deflate(compressedData, compressedData.Length);
        }
    }
}
