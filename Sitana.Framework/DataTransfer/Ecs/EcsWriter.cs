/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (C)2013-2014 Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF SEBASTIAN SEJUD AND IS NOT TO BE 
/// RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT THE EXPRESSED WRITTEN 
/// CONSENT OF SEBASTIAN SEJUD.
/// 
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. SEBASTIAN SEJUD GRANTS
/// TO YOU (ONE SOFTWARE DEVELOPER) THE LIMITED RIGHT TO USE THIS SOFTWARE 
/// ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// essentials@sejud.com
/// sejud.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;

namespace Sitana.Framework.DataTransfer
{
    /// <summary>
    /// Ebatiano's Compact & Safe Protocol Writer
    /// </summary>
    public class EcsWriter
    {
        Encoding _encoding;
        List<byte> _data = new List<byte>();

        public EcsWriter(Encoding encoding = null)
        {
            _encoding = encoding != null ? encoding : Encoding.UTF8;
        }

        public byte[] Data
        {
            get
            {
                return _data.ToArray();
            }
        }

        internal void WriteRaw(byte[] data)
        {
            foreach (var d in data)
            {
                _data.Add(d);
            }
        }

        public void Write(byte id, sbyte val)
        {
            _data.Add(id);
            _data.Add(1);
            _data.Add((byte)val);
        }

        public void Write(byte id, short val)
        {
            if ( Math.Abs(val) < (2<<7))
            {
                sbyte byteVal = (sbyte)val;
                Write(id, byteVal);
            }
            else
            {
                _data.Add(id);
                _data.Add(2);
                BitConverterLE.AddBytes(_data, val);
            }
        }

        private void Write24(byte id, int val)
        {
            if ((val & 0xffff) == val)
            {
                Write(id, (ushort)val);
            }
            else
            {
                _data.Add(id);
                _data.Add(3);
                BitConverterLE.AddBytes24(_data, val);
            }
        }

        public void Write(byte id, int val)
        {
            if (Math.Abs(val) < (1 << 15))
            {
                short shortValue = (short)val;
                Write(id, shortValue);
            }
            else if (Math.Abs(val) < (1<<23))
            {
                Write24(id, val);
            }
            else
            {
                _data.Add(id);
                _data.Add(4);

                BitConverterLE.AddBytes(_data, val);
            }
        }

        // ---

        public void Write(byte id, byte val)
        {
            _data.Add(id);
            _data.Add(17);
            _data.Add((byte)val);
        }

        public void Write(byte id, ushort val)
        {
            if ((val & 0xff) == val)
            {
                Write(id, (byte)val);
            }
            else
            {
                _data.Add(id);
                _data.Add(18);

                BitConverterLE.AddBytes(_data, val);
            }
        }

        private void Write24(byte id, uint val)
        {
            if ((val & 0xffff) == val)
            {
                Write(id, (ushort)val);
            }
            else
            {
                _data.Add(id);
                _data.Add(19);
                BitConverterLE.AddBytes24(_data, val);
            }
        }

        public void Write(byte id, uint val)
        {
            if ((val & 0xffffff) == val)
            {
                Write24(id, val);
            }
            else
            {
                _data.Add(id);
                _data.Add(20);

                BitConverterLE.AddBytes(_data, val);
            }
        }

        // ---

        public void Write(byte id, Single val)
        {
            _data.Add(id);
            _data.Add(36);

            BitConverterLE.AddBytes(_data, val);
        }

        public void Write(byte id, Double val)
        {
            _data.Add(id);
            _data.Add(40);

            BitConverterLE.AddBytes(_data, val);
        }

        // ---

        public void Write(byte id, string value)
        {
            _data.Add(id);

            byte[] bytes = _encoding.GetBytes(value);

            WriteTypeAndNumBytes(240, bytes.Length);
            Write(bytes);
        }

        public void Write(byte id, byte[] bytes)
        {
            _data.Add(id);

            WriteTypeAndNumBytes(250, bytes.Length);
            Write(bytes);
        }
        // ---

        public void Write(byte id, IEcsStructure data)
        {
            var writer = new EcsWriter(_encoding);
            
            writer.Write(255, data.GetType().Name);
            data.Write(writer);

            byte[] bytes = writer.Data;
            Write(id, bytes);
        }

        private void Write(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                _data.Add(b);
            }
        }

        private void WriteTypeAndNumBytes(byte type0, int length)
        {
            if (length < 256)
            {
                _data.Add((byte)(type0 + 1));
                _data.Add((byte)length);
            }
            else if (length < 65536)
            {
                _data.Add((byte)(type0 + 2));
                BitConverterLE.AddBytes(_data, (ushort)length);
            }
            else
            {
                _data.Add((byte)(type0 + 4));
                BitConverterLE.AddBytes(_data, length);
            }
        }
    }
}
