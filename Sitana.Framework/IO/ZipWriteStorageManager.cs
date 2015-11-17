using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Sitana.Framework.Cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.IO
{
    public class ZipWriteStorageManager: StorageManager
    {
        class ZipEntryStream: Stream
        {
            static byte[] _buffer = new byte[4096];
            //static Crc32 _crc = new Crc32();
            static MemoryStream _output = new MemoryStream();

            ZipOutputStream _zipStream;
            
            ZipEntry _entry;

            public override bool CanWrite
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanTimeout
            {
                get
                {
                    return _output.CanTimeout;
                }
            }

            public override int WriteTimeout
            {
                get
                {
                    return _output.WriteTimeout;
                }
                set
                {
                    _output.WriteTimeout = value;
                }
            }

            public ZipEntryStream(ZipOutputStream zipStream, ZipEntry entry)
            {
                _output.SetLength(0);
                _output.Position = 0;

                _zipStream = zipStream;
                _entry = entry;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _output.Write(buffer, offset, count);
            }


            public override void WriteByte(byte value)
            {
                throw new NotImplementedException();
            }            

            public override void Flush()
            {
                _output.Flush();
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    _entry.DateTime = DateTime.Now;
                    _entry.Size = (int)_output.Position;

                    _zipStream.PutNextEntry(_entry);

                    _output.Position = 0;
                    _output.SetLength(_entry.Size);

                    StreamUtils.Copy(_output, _zipStream, _buffer);

                    _zipStream.CloseEntry();
                }
            }

            public override long Length
            {
                get { throw new NotImplementedException(); }
            }

            public override long Position
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }
        }

        ZipOutputStream _output;

        public ZipWriteStorageManager(Stream output)
            : this(output, 3, null)
        {
        }

        public ZipWriteStorageManager(Stream output, int level)
            : this(output, level, null)
        {
        }

        public ZipWriteStorageManager(Stream output, int level, string password)
        {
            _output = new ZipOutputStream(output);
            _output.SetLevel(level);
            _output.Password = password;
            _output.UseZip64 = UseZip64.Off;
        }

        public override bool FileExists(string path)
        {
            throw new NotImplementedException();
        }

        public override bool DirectoryExists(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFileNames(string wildcard)
        {
            throw new NotImplementedException();
        }

        public override void CreateDirectory(string name)
        {
            string entryName = ZipEntry.CleanName(name.TrimEnd('\\', '/') + '/');
            ZipEntry newEntry = new ZipEntry(entryName);
            _output.PutNextEntry(newEntry);
            _output.CloseEntry();
        }

        public override void DeleteFile(string name)
        {
            throw new NotImplementedException();
        }

        public override void DeleteDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenFile(string name, FileMode mode)
        {
            if(mode != FileMode.Create)
            {
                throw new NotImplementedException();
            }

            string entryName = ZipEntry.CleanName(name);
            ZipEntry newEntry = new ZipEntry(entryName);

            var stream = new ZipEntryStream(_output, newEntry);

            return stream;
        }

        public override void Dispose()
        {
            _output.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            _output.Finish();
            _output.Flush();
            _output.Close();
        }
    }
}
