// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
using System;

namespace Ebatianos
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

