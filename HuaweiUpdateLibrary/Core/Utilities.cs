﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace HuaweiUpdateLibrary.Core
{
    internal static class Utilities
    {
        public const Int32 UintSize = sizeof (UInt32);
        public const Int32 UshortSize = sizeof (ushort);

        public static bool ByteToType<T>(BinaryReader reader, out T result)
        {
            var objSize = Marshal.SizeOf(typeof(T));
            var bytes = reader.ReadBytes(objSize);
            if (bytes.Length == 0 || bytes.Length != objSize)
            {
                result = default(T);
                return false;
            }

            var ptr = Marshal.AllocHGlobal(objSize);

            try
            {
                Marshal.Copy(bytes, 0, ptr, objSize);
                result = (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return true;
        }

        public static bool TypeToByte<T>(T type, out byte[] result)
        {
            var objSize = Marshal.SizeOf(typeof(T));
            result = new byte[objSize];
            var ptr = Marshal.AllocHGlobal(objSize);

            try
            {
                Marshal.StructureToPtr(type, ptr, true);
                Marshal.Copy(ptr, result, 0, objSize);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return true;
        }

        public static void SetCharArray(string source, byte[] destination)
        {
            // Reset values
            for (var c = 0; c < destination.Length; c++) { destination[c] = 0; }
            
            // Calculate string length
            var valueLength = Math.Min(source.Length, destination.Length);
            
            // Copy value
            Array.Copy(Encoding.ASCII.GetBytes(source.ToCharArray(0, valueLength)), destination, valueLength);
        }

        public static string GetString(byte[] source)
        {
            // Find first zero
            var index = Array.FindIndex(source, b => b == 0);

            // If not found use complete length
            if (index == -1) 
                index = source.Length;

            // Resturn string
            return Encoding.ASCII.GetString(source, 0, index);
        }
    }
}
