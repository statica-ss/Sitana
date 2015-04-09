// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Sitana.Framework
{
    public static class UuidGenerator
    {
        public static String GenerateString()
        {
            Byte[] array = GenerateArray();
            String guid = BitConverter.ToString(array, 0, 4).Replace("-", String.Empty) + "-";

            guid += BitConverter.ToString(array, 4, 2).Replace("-", String.Empty) + "-";
            guid += BitConverter.ToString(array, 6, 2).Replace("-", String.Empty) + "-";
            guid += BitConverter.ToString(array, 8, 2).Replace("-", String.Empty) + "-";
            guid += BitConverter.ToString(array, 10, 6).Replace("-", String.Empty);

            return guid;
        }

        public static Byte[] GenerateArray()
        {
            Byte[] bytes1 = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
            Byte[] bytes2 = BitConverter.GetBytes(Environment.TickCount);
            Byte[] bytes3 = BitConverter.GetBytes(new Random(Environment.TickCount / 2).Next());

            Byte[] retValue = new Byte[bytes1.Length + bytes2.Length + bytes3.Length];

            Array.Copy(bytes1, 0, retValue, 0, bytes1.Length);
            Array.Copy(bytes2, 0, retValue, bytes1.Length, bytes2.Length);
            Array.Copy(bytes3, 0, retValue, bytes1.Length + bytes2.Length, bytes3.Length);

            return retValue;
        }
    }
}

