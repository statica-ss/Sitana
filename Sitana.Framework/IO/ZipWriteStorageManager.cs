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
            ZipOutputStream _output;

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

            public ZipEntryStream(ZipOutputStream output, ZipEntry entry)
            {
                _output = output;
                _output.PutNextEntry(entry);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _output.Write(buffer, offset, count);
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return _output.BeginWrite(buffer, offset, count, callback, state);
            }

            public override Task CopyToAsync(Stream destination, int bufferSize, System.Threading.CancellationToken cancellationToken)
            {
                return _output.CopyToAsync(destination, bufferSize, cancellationToken);
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                _output.EndWrite(asyncResult);
            }

            public override Task FlushAsync(System.Threading.CancellationToken cancellationToken)
            {
                return _output.FlushAsync(cancellationToken);
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
            {
                return _output.WriteAsync(buffer, offset, count, cancellationToken);
            }

            public override void WriteByte(byte value)
            {
                _output.WriteByte(value);
            }            

            public override void Flush()
            {
                _output.Flush();
            }

            public override void Close()
            {
                base.Close();
                _output.CloseEntry();
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

            return new ZipEntryStream(_output, newEntry);
        }

        public override void Dispose()
        {
            _output.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            _output.Close();
        }
    }
}
