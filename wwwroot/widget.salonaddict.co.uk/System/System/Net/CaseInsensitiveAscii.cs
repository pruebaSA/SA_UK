﻿namespace System.Net
{
    using System;
    using System.Collections;

    internal class CaseInsensitiveAscii : IEqualityComparer, IComparer
    {
        internal static readonly byte[] AsciiToLower = new byte[] { 
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            0x10, 0x11, 0x12, 0x13, 20, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 30, 0x1f,
            0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 40, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
            0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 60, 0x3d, 0x3e, 0x3f,
            0x40, 0x61, 0x62, 0x63, 100, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 110, 0x6f,
            0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 120, 0x79, 0x7a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
            0x60, 0x61, 0x62, 0x63, 100, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 110, 0x6f,
            0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 120, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,
            0x80, 0x81, 130, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 140, 0x8d, 0x8e, 0x8f,
            0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 150, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c, 0x9d, 0x9e, 0x9f,
            160, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 170, 0xab, 0xac, 0xad, 0xae, 0xaf,
            0xb0, 0xb1, 0xb2, 0xb3, 180, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 190, 0xbf,
            0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 200, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
            0xd0, 0xd1, 210, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 220, 0xdd, 0xde, 0xdf,
            0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 230, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
            240, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 250, 0xfb, 0xfc, 0xfd, 0xfe, 0xff
        };
        internal static readonly CaseInsensitiveAscii StaticInstance = new CaseInsensitiveAscii();

        public int Compare(object firstObject, object secondObject)
        {
            string str = firstObject as string;
            string str2 = secondObject as string;
            if (str == null)
            {
                if (str2 != null)
                {
                    return -1;
                }
                return 0;
            }
            if (str2 == null)
            {
                return 1;
            }
            int num = str.Length - str2.Length;
            int num2 = (num > 0) ? str2.Length : str.Length;
            for (int i = 0; i < num2; i++)
            {
                int num3 = AsciiToLower[str[i]] - AsciiToLower[str2[i]];
                if (num3 != 0)
                {
                    return num3;
                }
            }
            return num;
        }

        public bool Equals(object firstObject, object secondObject)
        {
            string myString = firstObject as string;
            string str2 = secondObject as string;
            if (myString == null)
            {
                return (str2 == null);
            }
            if (str2 != null)
            {
                int length = myString.Length;
                if ((length == str2.Length) && (this.FastGetHashCode(myString) == this.FastGetHashCode(str2)))
                {
                    int num1 = myString.Length;
                    while (length > 0)
                    {
                        length--;
                        if (AsciiToLower[myString[length]] != AsciiToLower[str2[length]])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private int FastGetHashCode(string myString)
        {
            int length = myString.Length;
            if (length != 0)
            {
                length ^= (AsciiToLower[(byte) myString[0]] << 0x18) ^ (AsciiToLower[(byte) myString[length - 1]] << 0x10);
            }
            return length;
        }

        public int GetHashCode(object myObject)
        {
            string str = myObject as string;
            if (myObject == null)
            {
                return 0;
            }
            int length = str.Length;
            if (length == 0)
            {
                return 0;
            }
            return (length ^ ((AsciiToLower[(byte) str[0]] << 0x18) ^ (AsciiToLower[(byte) str[length - 1]] << 0x10)));
        }
    }
}
