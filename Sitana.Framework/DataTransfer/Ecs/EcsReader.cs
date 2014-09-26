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
using System.IO;
using System.Text;
using Ebatianos.Cs;

namespace Ebatianos.DataTransfer
{
    /// <summary>
    /// Ebatiano's Compact & Safe Protocol Reader
    /// </summary>
    public class EcsReader
    {
        byte[]  _array;
        int   _size;

        Dictionary<int, int> _fields = new Dictionary<int, int>();

        Encoding _encoding;

        static Dictionary<string, Type> _commonTypes = new Dictionary<string, Type>();
        Dictionary<string, Type> _types = new Dictionary<string, Type>();

        public static void RegisterCommonType(Type type)
        {
            _commonTypes.Add(type.Name, type);
        }

        public void RegisterType(Type type)
        {
            _types.Remove(type.Name);
            _types.Add(type.Name, type);
        }

        public void RegisterTypes(Type[] types)
        {
            foreach (var type in types)
            {
                _types.Add(type.Name, type);
            }
        }

        internal EcsReader(byte[] array, int offset, int size, EcsReader parent): this(array, offset, size, parent._encoding)
        {
            _types = parent._types;
        }

        public EcsReader(byte[] array, int offset, int size, Encoding encoding = null)
        {
            _array = array;
            _size = offset + size;

            _encoding = encoding != null ? encoding : Encoding.UTF8;

            int fieldOffset = offset;
            int id = ReadField(ref offset);

            while (id >= 0)
            {
                _fields.Add(id, fieldOffset + 1);
                fieldOffset = offset;

                id = ReadField(ref offset);
            }
        }

        public IEcsStructure ReadStructure(int id)
        {
            int offset = 0;

            if (!_fields.TryGetValue(id, out offset))
            {
                return null;
            }

            byte type = _array[offset];

            if (type < 251 || type > 254)
            {
                throw new InvalidDataException("Invalid field type.");
            }

            int bytes = GetNumBytes(ref offset);

            EcsReader reader = new EcsReader(_array, offset, bytes, this);

            string name = reader.ReadString(255);

            Type classType = null;

            if (!_types.TryGetValue(name, out classType))
            {
                if (!_commonTypes.TryGetValue(name, out classType))
                {
                    throw new InvalidDataException(string.Format("unknown type: {0}", name));
                }
            }

            IEcsStructure val = (IEcsStructure)Activator.CreateInstance(classType);

            val.Read(reader);
            return val;
        }

        public T ReadStructure<T>(int id) where T : class
        {
            IEcsStructure val = ReadStructure(id);

            T retVal = val as T;

            if (retVal == null)
            {
                throw new InvalidDataException(string.Format("Invalid type: {0}, expected: {1}", val.GetType().Name, typeof(T).Name));
            }

            return retVal;
        }

        public byte[] ReadByteArray(int id)
        {
            int offset = 0;

            if (!_fields.TryGetValue(id, out offset))
            {
                return null;
            }

            byte type = _array[offset];

            if (type < 251 || type > 254)
            {
                throw new InvalidDataException("Invalid field type.");
            }

            int bytes = GetNumBytes(ref offset);

            byte[] val = new byte[bytes];

            for (int idx = 0; idx < bytes; ++idx)
            {
                val[idx] = _array[offset + idx];
            }

            return val;
        }

        private static int GetNumBytes(byte[] array, ref int offset)
        {
            int bytes = -1;

            byte type = array[offset];

            switch (type)
            {
                case 241:
                case 251:
                    bytes = array[offset + 1];
                    offset += 2;
                    break;

                case 242:
                case 252:
                    bytes = BitConverterLE.ToUInt16(array, offset + 1);
                    offset += 3;
                    break;

                case 243:
                case 253:
                    bytes = BitConverterLE.ToInt24(array, offset + 1);
                    offset += 4;
                    break;

                case 244:
                case 254:
                    bytes = BitConverterLE.ToInt32(array, offset + 1);
                    offset += 5;
                    break;
            }

            if (bytes < 0)
            {
                throw new InvalidDataException("Invalid field type.");
            }

            return bytes;
        }

        private int GetNumBytes(ref int offset)
        {
            return GetNumBytes(_array, ref offset);
        }

        private int GetNumBytesWithLength(int offset)
        {
            return GetStructureSize(_array, offset);
        }

        public static int GetStructureSize(byte[] data, int offset)
        {
            int oldOffset = offset;
            int size = GetNumBytes(data, ref offset);
            return size + offset - oldOffset;
        }

        public string ReadString(int id)
        {
            int offset = 0;

            if (!_fields.TryGetValue(id, out offset))
            {
                return null;
            }

            byte type = _array[offset];

            if (type < 241 || type > 244 || type == 243)
            {
                throw new InvalidDataException("Invalid field type.");
            }

            int bytes = GetNumBytes(ref offset);

            return _encoding.GetString(_array, offset, bytes);
        }

        public sbyte ReadInt8(int id)
        {
            int val = ReadInteger(id);
            sbyte retVal = (sbyte)val;

            if (retVal != val)
            {
                throw new InvalidCastException("Read value is bigger than Int8.");
            }

            return retVal;
        }

        public short ReadInt16(int id)
        {
            int val = ReadInteger(id);
            short retVal = (short)val;

            if (retVal != val)
            {
                throw new InvalidCastException("Read value is bigger than short.");
            }

            return retVal;
        }

        public int ReadInt32(int id)
        {
            return (int)ReadInteger(id);
        }

        public byte ReadUInt8(int id)
        {
            uint val = ReadUnsigned(id);
            byte retVal = (byte)val;

            if (retVal != val)
            {
                throw new InvalidCastException("Read value is bigger than UInt8.");
            }

            return retVal;
        }

        public UInt16 ReadUInt16(int id)
        {
            uint val = ReadUnsigned(id);
            UInt16 retVal = (UInt16)val;

            if (retVal != val)
            {
                throw new InvalidCastException("Read value is bigger than UInt16.");
            }

            return retVal;
        }

        public uint ReadUInt32(int id)
        {
            return (uint)ReadUnsigned(id);
        }

        public float ReadSingle(int id)
        {
            return (float)ReadDouble(id);
        }

        public double ReadDouble(int id)
        {
            int offset = 0;

            if (!_fields.TryGetValue(id, out offset))
            {
                throw new InvalidOperationException(string.Format("No field with id={0}", id));
            }

            int type = _array[offset];

            switch (type)
            {
                case 1:
                case 2:
                case 4:
                    return (double)ReadInteger(id);

                case 17:
                case 18:
                case 19:
                case 20:
                    return (double)ReadUnsigned(id);

                case 36:
                    return BitConverterLE.ToSingle(_array, offset + 1);

                case 40:
                    return BitConverterLE.ToDouble(_array, offset + 1);
            }

            throw new InvalidDataException("Invalid field type.");
        }

        private int ReadInteger(int id)
        {
            int offset = 0;

            if (!_fields.TryGetValue(id, out offset))
            {
                throw new InvalidOperationException(string.Format("No field with id={0}", id));
            }

            int type = _array[offset];

            switch (type)
            {
                case 1:
                    return (sbyte)_array[offset+1];

                case 2:
                    return BitConverterLE.ToInt16(_array, offset + 1);

                case 3:
                    return BitConverterLE.ToInt24(_array, offset + 1);

                case 4:
                    return BitConverterLE.ToInt32(_array, offset + 1);

                case 17:
                case 18:
                case 19:
                case 20:
                    return (int)ReadUnsigned(id);
            }

            throw new InvalidDataException("Invalid field type.");
        }

        private uint ReadUnsigned(int id)
        {
            int offset = 0;

            if (!_fields.TryGetValue(id, out offset))
            {
                throw new InvalidOperationException(string.Format("No field with id={0}", id));
            }

            int type = _array[offset];

            switch (type)
            {
                case 17:
                    return _array[offset + 1];

                case 18:
                    return BitConverterLE.ToUInt16(_array, offset + 1);

                case 19:
                    return BitConverterLE.ToUInt24(_array, offset + 1);

                case 20:
                    return BitConverterLE.ToUInt32(_array, offset + 1);

                case 1:
                case 2:
                case 3:
                case 4:
                    return (uint)ReadInteger(id);
            }

            throw new InvalidDataException("Invalid field type.");
        }

        public bool HasField(byte id)
        {
            return _fields.ContainsKey(id);
        }

        private int ReadSize(int offset)
        {
            byte type = _array[offset];

            if (type == 0)
            {
                return 1;
            }

            if (type > 240)
            {
                return GetNumBytesWithLength(offset);
            }


            int len = type & 0xf;
            if (len == 1 || len == 2 || len == 4 || len == 3 || type == 40)
            {
                return len + 1;
            }

            throw new InvalidDataException("Unknown type of field.");
        }

        private int ReadField(ref int offset)
        {
            if (offset >= _size)
            {
                return -1;
            }

            byte id = _array[offset];

            offset++;

            int size = ReadSize(offset);
            offset += size;

            if (offset <= _size)
            {
                return id;
            }

            return -1;
        }
    }
}
