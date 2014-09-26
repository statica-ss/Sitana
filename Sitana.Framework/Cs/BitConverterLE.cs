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

namespace Sitana.Framework.Cs
{
    public class BitConverterLE
    {
        static byte[] _buffer = new byte[8];

        public static double ToDouble(byte[] data, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToDouble(data, offset);
            }
            else
            {
                _buffer[0] = data[7];
                _buffer[1] = data[6];
                _buffer[2] = data[5];
                _buffer[3] = data[4];
                _buffer[4] = data[3];
                _buffer[5] = data[2];
                _buffer[6] = data[1];
                _buffer[7] = data[0];

                return BitConverter.ToDouble(_buffer, 0);
            }
        }

        public static float ToSingle(byte[] data, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToSingle(data, offset);
            }
            else
            {
                _buffer[0] = data[3];
                _buffer[1] = data[2];
                _buffer[2] = data[1];
                _buffer[3] = data[0];

                return BitConverter.ToSingle(_buffer, 0);
            }
        }

        public static ushort ToUInt16(byte[] data, int offset)
        {
            byte b0 = data[offset + 0];
            ushort b1 = data[offset + 1];

            return (ushort)(b0 | (b1 << 8));
        }

        public static short ToInt16(byte[] data, int offset)
        {
            byte b0 = data[offset + 0];
            short b1 = data[offset + 1];

            Console.WriteLine("ToInt16: {0}, {1}", (int)b0, (int)b1);

            return (short)(b0 | (b1 << 8));
        }

        public static uint ToUInt24(byte[] data, int offset)
        {
            byte b0 = data[offset + 0];
            uint b1 = data[offset + 1];
            uint b2 = data[offset + 2];

            uint val = (uint)(b0 | (b1 << 8) | (b2 << 16));
            return val;
        }

        public static int ToInt24(byte[] data, int offset)
        {
            byte b0 = data[offset + 0];
            uint b1 = data[offset + 1];
            uint b2 = data[offset + 2];

            int val = (int)(b0 | (b1 << 8) | (b2 << 16));

            if ((b2 & 0x80) != 0)
            {
                val = val - (1 << 24);
            }

            return val;
        }

        public static uint ToUInt32(byte[] data, int offset)
        {
            uint b0 = data[offset + 0];
            uint b1 = data[offset + 1];
            uint b2 = data[offset + 2];
            uint b3 = data[offset + 3];

            return b0 | (b1 << 8) | (b2 << 16) | (b3 << 24);
        }

        public static int ToInt32(byte[] data, int offset)
        {
            int b0 = data[offset + 0];
            int b1 = data[offset + 1];
            int b2 = data[offset + 2];
            int b3 = data[offset + 3];

            return b0 | (b1 << 8) | (b2 << 16) | (b3 << 24);
        }

        public static void AddBytes(List<byte> list, uint val)
        {
            list.Add((byte)(val & 0xff));
            list.Add((byte)((val >> 8) & 0xff));
            list.Add((byte)((val >> 16) & 0xff));
            list.Add((byte)((val >> 24) & 0xff));
        }

        public static void AddBytes(List<byte> list, int val)
        {
            list.Add((byte)(val & 0xff));
            list.Add((byte)((val >> 8) & 0xff));
            list.Add((byte)((val >> 16) & 0xff));
            list.Add((byte)((val >> 24) & 0xff));
        }

        public static void AddBytes24(List<byte> list, uint val)
        {
            list.Add((byte)(val & 0xff));
            list.Add((byte)((val >> 8) & 0xff));
            list.Add((byte)((val >> 16) & 0xff));
        }

        public static void AddBytes24(List<byte> list, int val)
        {
            int sign = val < 0 ? 0x80 : 0;

            list.Add((byte)(val & 0xff));
            list.Add((byte)((val >> 8) & 0xff));
            list.Add((byte)(((val >> 16) & 0x7f) | sign));
        }

        public static void AddBytes(List<byte> list, short val)
        {
            list.Add((byte)(val & 0xff));
            list.Add((byte)((val >> 8) & 0xff));
        }

        public static void AddBytes(List<byte> list, ushort val)
        {
            list.Add((byte)(val & 0xff));
            list.Add((byte)((val >> 8) & 0xff));
        }

        public static void AddBytes(List<byte> list, float val)
        {
            byte[] bytes = BitConverter.GetBytes(val);

            if (BitConverter.IsLittleEndian)
            {
                list.Add(bytes[0]);
                list.Add(bytes[1]);
                list.Add(bytes[2]);
                list.Add(bytes[3]);
            }
            else
            {
                list.Add(bytes[3]);
                list.Add(bytes[2]);
                list.Add(bytes[1]);
                list.Add(bytes[0]);
            }
        }

        public static void AddBytes(List<byte> list, double val)
        {
            byte[] bytes = BitConverter.GetBytes(val);

            if (BitConverter.IsLittleEndian)
            {
                list.Add(bytes[0]);
                list.Add(bytes[1]);
                list.Add(bytes[2]);
                list.Add(bytes[3]);
                list.Add(bytes[4]);
                list.Add(bytes[5]);
                list.Add(bytes[6]);
                list.Add(bytes[7]);
            }
            else
            {
                list.Add(bytes[7]);
                list.Add(bytes[6]);
                list.Add(bytes[5]);
                list.Add(bytes[4]);
                list.Add(bytes[3]);
                list.Add(bytes[2]);
                list.Add(bytes[1]);
                list.Add(bytes[0]);
            }
        }
    }
}
