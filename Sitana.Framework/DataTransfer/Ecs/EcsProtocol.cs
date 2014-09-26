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
using System.IO;

namespace Ebatianos.DataTransfer
{
    public static class EcsProtocol
    {
        public static byte[] ToBytes(IEcsStructure data, Encoding encoding = null)
        {
            EcsWriter writer = new EcsWriter(encoding);

            writer.WriteRaw(Encoding.UTF8.GetBytes("ECS1"));

            writer.Write(0, data);

            return writer.Data;
        }

        public static IEcsStructure GetData(byte[] bytes, int offset, int size, Encoding encoding = null, Type[] types = null)
        {
            if (Encoding.UTF8.GetString(bytes, offset, 4) != "ECS1")
            {
                throw new InvalidDataException("Invalid protocol. Data: " + Encoding.UTF8.GetString(bytes));
            }

            offset += 4;
            size -= 4;

            int structSize = int.MaxValue;

            try
            {
                structSize = EcsReader.GetStructureSize(bytes, offset+1) + 1;
            }
            catch
            {
            }

            if (size >= structSize)
            {
                EcsReader reader = new EcsReader(bytes, offset, size);

                if (types != null)
                {
                    foreach (var t in types)
                    {
                        reader.RegisterType(t);
                    }
                }

                IEcsStructure obj = reader.ReadStructure(bytes[offset]);

                return obj;
            }

            return null;
        }

        public static T GetData<T>(byte[] bytes, int offset, int size, Encoding encoding = null, Type[] types = null) where T : IEcsStructure
        {
            IEcsStructure val = GetData(bytes, offset, size, encoding, types);

            if (val is T)
            {
                return (T)val;
            }

            throw new InvalidDataException(String.Format("Invalid type: {0}, expected: {1}", val.GetType().Name, typeof(T).Name));
        }
    }
}
