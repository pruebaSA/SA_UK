namespace System.Xml.Xsl
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class XPathConvert
    {
        public static readonly double[] C10toN = new double[] { 
            1.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, 10000000.0, 100000000.0, 1000000000.0, 10000000000, 100000000000, 1000000000000, 10000000000000, 100000000000000, 1E+15,
            1E+16, 1E+17, 1E+18, 1E+19, 1E+20, 1E+21, 1E+22
        };

        public static uint AddU(ref uint u1, uint u2)
        {
            u1 += u2;
            if (u1 >= u2)
            {
                return 0;
            }
            return 1;
        }

        public static int CbitZeroLeft(uint u)
        {
            int num = 0;
            if ((u & 0xffff0000) == 0)
            {
                num += 0x10;
                u = u << 0x10;
            }
            if ((u & 0xff000000) == 0)
            {
                num += 8;
                u = u << 8;
            }
            if ((u & 0xf0000000) == 0)
            {
                num += 4;
                u = u << 4;
            }
            if ((u & 0xc0000000) == 0)
            {
                num += 2;
                u = u << 2;
            }
            if ((u & 0x80000000) == 0)
            {
                num++;
                u = u << 1;
            }
            return num;
        }

        public static uint DblHi(double dbl) => 
            ((uint) (BitConverter.DoubleToInt64Bits(dbl) >> 0x20));

        public static uint DblLo(double dbl) => 
            ((uint) BitConverter.DoubleToInt64Bits(dbl));

        public static unsafe string DoubleToString(double dbl)
        {
            int exponent;
            int num6;
            if (IsInteger(dbl, out num6))
            {
                return IntToString(num6);
            }
            if (IsSpecial(dbl))
            {
                if (double.IsNaN(dbl))
                {
                    return "NaN";
                }
                if (dbl >= 0.0)
                {
                    return "Infinity";
                }
                return "-Infinity";
            }
            FloatingDecimal num7 = new FloatingDecimal(dbl);
            int num3 = num7.MantissaSize - num7.Exponent;
            if (num3 > 0)
            {
                exponent = (num7.Exponent > 0) ? num7.Exponent : 0;
            }
            else
            {
                exponent = num7.Exponent;
                num3 = 0;
            }
            int num = (exponent + num3) + 4;
            char* chPtr = (char*) stackalloc byte[(2 * num)];
            char* chPtr2 = chPtr;
            if (num7.Sign < 0)
            {
                chPtr2++;
                chPtr2[0] = '-';
            }
            int mantissaSize = num7.MantissaSize;
            int num5 = 0;
            if (exponent != 0)
            {
                do
                {
                    if (mantissaSize != 0)
                    {
                        chPtr2++;
                        chPtr2[0] = (char) (num7[num5++] | 0x30);
                        mantissaSize--;
                    }
                    else
                    {
                        chPtr2++;
                        chPtr2[0] = '0';
                    }
                }
                while (--exponent != 0);
            }
            else
            {
                chPtr2++;
                chPtr2[0] = '0';
            }
            if (num3 != 0)
            {
                chPtr2++;
                chPtr2[0] = '.';
                while (num3 > mantissaSize)
                {
                    chPtr2++;
                    chPtr2[0] = '0';
                    num3--;
                }
                while (mantissaSize != 0)
                {
                    chPtr2++;
                    chPtr2[0] = (char) (num7[num5++] | 0x30);
                    mantissaSize--;
                }
            }
            return new string(chPtr, 0, (int) ((long) ((chPtr2 - chPtr) / 2)));
        }

        private static unsafe string IntToString(int val)
        {
            char* chPtr = (char*) stackalloc byte[(2 * 12)];
            char* chPtr2 = chPtr += 12;
            uint num = (val < 0) ? ((uint) -val) : ((uint) val);
            while (num >= 10)
            {
                uint num2 = ((uint) ((0x66666667L * num) >> 0x20)) >> 2;
                *(--chPtr2) = (char) ((num - (num2 * 10)) + 0x30);
                num = num2;
            }
            *(--chPtr2) = (char) (num + 0x30);
            if (val < 0)
            {
                *(--chPtr2) = '-';
            }
            return new string(chPtr2, 0, (int) ((long) ((chPtr - chPtr2) / 2)));
        }

        private static bool IsAsciiDigit(char ch) => 
            ((ch - '0') <= 9);

        public static bool IsInteger(double dbl, out int value)
        {
            if (!IsSpecial(dbl))
            {
                int num = (int) dbl;
                double num2 = num;
                if (dbl == num2)
                {
                    value = num;
                    return true;
                }
            }
            value = 0;
            return false;
        }

        public static bool IsSpecial(double dbl) => 
            (0 == (~DblHi(dbl) & 0x7ff00000));

        private static bool IsWhitespace(char ch)
        {
            if (((ch != ' ') && (ch != '\t')) && (ch != '\n'))
            {
                return (ch == '\r');
            }
            return true;
        }

        public static uint MulU(uint u1, uint u2, out uint uHi)
        {
            ulong num = u1 * u2;
            uHi = (uint) (num >> 0x20);
            return (uint) num;
        }

        public static uint NotZero(uint u)
        {
            if (u == 0)
            {
                return 0;
            }
            return 1;
        }

        private static unsafe char* SkipWhitespace(char* pch)
        {
            while (IsWhitespace(pch[0]))
            {
                pch++;
            }
            return pch;
        }

        public static unsafe double StringToDouble(string s)
        {
            double num6;
            fixed (char* str = ((char*) s))
            {
                byte[] buffer;
                char* chPtr = str;
                int num = 0;
                char* pch = chPtr;
                char* chPtr3 = null;
                int num2 = 1;
                int num3 = 0;
            Label_001D:
                pch++;
                char ch = pch[0];
                if (!IsAsciiDigit(ch))
                {
                    switch (ch)
                    {
                        case '-':
                            if (num2 < 0)
                            {
                                break;
                            }
                            num2 = -1;
                            goto Label_001D;

                        case '.':
                            if (!IsAsciiDigit(pch[0]))
                            {
                                break;
                            }
                            goto Label_00C8;

                        default:
                            if (IsWhitespace(ch) && (num2 > 0))
                            {
                                pch = SkipWhitespace(pch);
                                goto Label_001D;
                            }
                            break;
                    }
                    return double.NaN;
                }
                if (ch == '0')
                {
                    do
                    {
                        pch++;
                        ch = pch[0];
                    }
                    while (ch == '0');
                    if (!IsAsciiDigit(ch))
                    {
                        goto Label_00C2;
                    }
                }
                chPtr3 = pch - 1;
                do
                {
                    pch++;
                    ch = pch[0];
                }
                while (IsAsciiDigit(ch));
                num = ((int) ((long) ((pch - chPtr3) / 2))) - 1;
            Label_00C2:
                if (ch != '.')
                {
                    goto Label_0110;
                }
            Label_00C8:
                pch++;
                ch = pch[0];
                if (chPtr3 == null)
                {
                    while (ch == '0')
                    {
                        num3--;
                        pch++;
                        ch = pch[0];
                    }
                    chPtr3 = pch - 1;
                }
                while (IsAsciiDigit(ch))
                {
                    num3--;
                    num++;
                    pch++;
                    ch = pch[0];
                }
            Label_0110:
                pch--;
                char* chPtr4 = chPtr + s.Length;
                if ((pch < chPtr4) && (SkipWhitespace(pch) < chPtr4))
                {
                    return double.NaN;
                }
                if (num == 0)
                {
                    return 0.0;
                }
                if ((num3 == 0) && (num <= 9))
                {
                    int num4 = chPtr3[0] & '\x000f';
                    while (--num != 0)
                    {
                        chPtr3++;
                        num4 = (num4 * 10) + (chPtr3[0] & '\x000f');
                    }
                    return ((num2 < 0) ? ((double) -num4) : ((double) num4));
                }
                if (num > 50)
                {
                    pch -= num - 50;
                    num3 += num - 50;
                    num = 50;
                }
                do
                {
                    while (*(--pch) == '0')
                    {
                        num--;
                        num3++;
                    }
                }
                while (pch[0] == '.');
                pch++;
                FloatingDecimal num5 = new FloatingDecimal {
                    Exponent = num3 + num,
                    Sign = num2,
                    MantissaSize = num
                };
                if (((buffer = num5.Mantissa) == null) || (buffer.Length == 0))
                {
                    numRef = null;
                    goto Label_0215;
                }
                fixed (byte* numRef = buffer)
                {
                    byte* numPtr;
                Label_0215:
                    numPtr = numRef;
                    while (chPtr3 < pch)
                    {
                        if (chPtr3[0] != '.')
                        {
                            numPtr[0] = (byte) (chPtr3[0] & '\x000f');
                            numPtr++;
                        }
                        chPtr3++;
                    }
                }
                num6 = (double) num5;
            }
            return num6;
        }

        private class BigInteger : IComparable
        {
            private int capacity = 30;
            private uint[] digits = new uint[30];
            private const int InitCapacity = 30;
            private int length = 0;

            public void Add(XPathConvert.BigInteger bi)
            {
                int length;
                int num3;
                if ((length = this.length) < (num3 = bi.length))
                {
                    length = bi.length;
                    num3 = this.length;
                    this.Ensure(length + 1);
                }
                uint num4 = 0;
                int index = 0;
                while (index < num3)
                {
                    if (num4 != 0)
                    {
                        num4 = XPathConvert.AddU(ref this.digits[index], num4);
                    }
                    num4 += XPathConvert.AddU(ref this.digits[index], bi.digits[index]);
                    index++;
                }
                if (this.length >= bi.length)
                {
                    while ((num4 != 0) && (index < length))
                    {
                        num4 = XPathConvert.AddU(ref this.digits[index], num4);
                        index++;
                    }
                }
                else
                {
                    while (index < length)
                    {
                        this.digits[index] = bi.digits[index];
                        if (num4 != 0)
                        {
                            num4 = XPathConvert.AddU(ref this.digits[index], num4);
                        }
                        index++;
                    }
                    this.length = length;
                }
                if (num4 != 0)
                {
                    this.Ensure(this.length + 1);
                    this.digits[this.length++] = num4;
                }
            }

            [Conditional("DEBUG")]
            private void AssertValid()
            {
            }

            [Conditional("DEBUG")]
            private void AssertValidNoVal()
            {
            }

            public int CompareTo(object obj)
            {
                XPathConvert.BigInteger integer = (XPathConvert.BigInteger) obj;
                if (this.length <= integer.length)
                {
                    if (this.length < integer.length)
                    {
                        return -1;
                    }
                    if (this.length == 0)
                    {
                        return 0;
                    }
                    int index = this.length - 1;
                    while (this.digits[index] == integer.digits[index])
                    {
                        if (index == 0)
                        {
                            return 0;
                        }
                        index--;
                    }
                    if (this.digits[index] <= integer.digits[index])
                    {
                        return -1;
                    }
                }
                return 1;
            }

            public uint DivRem(XPathConvert.BigInteger bi)
            {
                int num5;
                int length = bi.length;
                if (this.length < length)
                {
                    return 0;
                }
                uint num3 = this.digits[length - 1] / (bi.digits[length - 1] + 1);
                switch (num3)
                {
                    case 0:
                        break;

                    case 1:
                        this.Subtract(bi);
                        break;

                    default:
                    {
                        uint num7 = 0;
                        uint num4 = 1;
                        int index = 0;
                        while (index < length)
                        {
                            uint num6;
                            uint num8 = XPathConvert.MulU(num3, bi.digits[index], out num6);
                            num7 = num6 + XPathConvert.AddU(ref num8, num7);
                            if ((num8 != 0) || (num4 == 0))
                            {
                                num4 = XPathConvert.AddU(ref this.digits[index], ~num8 + num4);
                            }
                            index++;
                        }
                        while ((--index >= 0) && (this.digits[index] == 0))
                        {
                        }
                        this.length = index + 1;
                        break;
                    }
                }
                if ((num3 < 9) && ((num5 = this.CompareTo(bi)) >= 0))
                {
                    num3++;
                    if (num5 == 0)
                    {
                        this.length = 0;
                        return num3;
                    }
                    this.Subtract(bi);
                }
                return num3;
            }

            private void Ensure(int cu)
            {
                if (cu > this.capacity)
                {
                    cu += cu;
                    uint[] array = new uint[cu];
                    this.digits.CopyTo(array, 0);
                    this.digits = array;
                    this.capacity = cu;
                }
            }

            public void InitFromBigint(XPathConvert.BigInteger biSrc)
            {
                this.InitFromRgu(biSrc.digits, biSrc.length);
            }

            public void InitFromDigits(uint u0, uint u1, int cu)
            {
                this.length = cu;
                this.digits[0] = u0;
                this.digits[1] = u1;
            }

            public void InitFromFloatingDecimal(XPathConvert.FloatingDecimal dec)
            {
                int cu = (dec.MantissaSize + 8) / 9;
                int mantissaSize = dec.MantissaSize;
                this.Ensure(cu);
                this.length = 0;
                uint uAdd = 0;
                uint uMul = 1;
                for (int i = 0; i < mantissaSize; i++)
                {
                    if (0x3b9aca00 == uMul)
                    {
                        this.MulAdd(uMul, uAdd);
                        uMul = 1;
                        uAdd = 0;
                    }
                    uMul *= 10;
                    uAdd = (uAdd * 10) + dec[i];
                }
                this.MulAdd(uMul, uAdd);
            }

            public void InitFromRgu(uint[] rgu, int cu)
            {
                this.Ensure(cu);
                this.length = cu;
                for (int i = 0; i < cu; i++)
                {
                    this.digits[i] = rgu[i];
                }
            }

            public void MulAdd(uint uMul, uint uAdd)
            {
                for (int i = 0; i < this.length; i++)
                {
                    uint num3;
                    uint num2 = XPathConvert.MulU(this.digits[i], uMul, out num3);
                    if (uAdd != 0)
                    {
                        num3 += XPathConvert.AddU(ref num2, uAdd);
                    }
                    this.digits[i] = num2;
                    uAdd = num3;
                }
                if (uAdd != 0)
                {
                    this.Ensure(this.length + 1);
                    this.digits[this.length++] = uAdd;
                }
            }

            public void MulPow5(int c5)
            {
                int num = (c5 + 12) / 13;
                if ((this.length != 0) && (c5 != 0))
                {
                    this.Ensure(this.length + num);
                    while (c5 >= 13)
                    {
                        this.MulAdd(0x48c27395, 0);
                        c5 -= 13;
                    }
                    if (c5 > 0)
                    {
                        uint uMul = 5;
                        while (--c5 > 0)
                        {
                            uMul *= 5;
                        }
                        this.MulAdd(uMul, 0);
                    }
                }
            }

            public void ShiftLeft(int cbit)
            {
                int num;
                uint num3;
                if ((cbit == 0) || (this.length == 0))
                {
                    return;
                }
                int num2 = cbit >> 5;
                cbit &= 0x1f;
                if (cbit > 0)
                {
                    num = this.length - 1;
                    num3 = this.digits[num] >> (0x20 - cbit);
                    while (true)
                    {
                        this.digits[num] = this.digits[num] << cbit;
                        if (num == 0)
                        {
                            goto Label_0085;
                        }
                        this.digits[num] |= this.digits[num - 1] >> (0x20 - cbit);
                        num--;
                    }
                }
                num3 = 0;
            Label_0085:
                if ((num2 > 0) || (num3 != 0))
                {
                    num = (this.length + ((num3 != 0) ? 1 : 0)) + num2;
                    this.Ensure(num);
                    if (num2 > 0)
                    {
                        int length = this.length;
                        while (length-- != 0)
                        {
                            this.digits[num2 + length] = this.digits[length];
                        }
                        for (int i = 0; i < num2; i++)
                        {
                            this.digits[i] = 0;
                        }
                        this.length += num2;
                    }
                    if (num3 != 0)
                    {
                        this.digits[this.length++] = num3;
                    }
                }
            }

            public void ShiftRight(int cbit)
            {
                int cu = cbit >> 5;
                cbit &= 0x1f;
                if (cu > 0)
                {
                    this.ShiftUsRight(cu);
                }
                if ((cbit == 0) || (this.length == 0))
                {
                    return;
                }
                int index = 0;
            Label_0023:
                this.digits[index] = this.digits[index] >> cbit;
                if (++index >= this.length)
                {
                    if (this.digits[index - 1] == 0)
                    {
                        this.length--;
                    }
                }
                else
                {
                    this.digits[index - 1] |= this.digits[index] << (0x20 - cbit);
                    goto Label_0023;
                }
            }

            public void ShiftUsRight(int cu)
            {
                if (cu >= this.length)
                {
                    this.length = 0;
                }
                else if (cu > 0)
                {
                    for (int i = 0; i < (this.length - cu); i++)
                    {
                        this.digits[i] = this.digits[cu + i];
                    }
                    this.length -= cu;
                }
            }

            public void Subtract(XPathConvert.BigInteger bi)
            {
                if (this.length >= bi.length)
                {
                    uint num2 = 1;
                    int index = 0;
                    while (index < bi.length)
                    {
                        uint num3 = bi.digits[index];
                        if ((num3 != 0) || (num2 == 0))
                        {
                            num2 = XPathConvert.AddU(ref this.digits[index], ~num3 + num2);
                        }
                        index++;
                    }
                    while ((num2 == 0) && (index < this.length))
                    {
                        num2 = XPathConvert.AddU(ref this.digits[index], uint.MaxValue);
                    }
                    if (num2 != 0)
                    {
                        if (index == this.length)
                        {
                            while ((--index >= 0) && (this.digits[index] == 0))
                            {
                            }
                            this.length = index + 1;
                        }
                        return;
                    }
                }
                this.length = 0;
            }

            public uint this[int idx] =>
                this.digits[idx];

            public int Length =>
                this.length;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BigNumber
        {
            private uint u0;
            private uint u1;
            private uint u2;
            private int exp;
            private uint error;
            private static readonly XPathConvert.BigNumber[] TenPowersPos;
            private static readonly XPathConvert.BigNumber[] TenPowersNeg;
            public uint Error =>
                this.error;
            public BigNumber(uint u0, uint u1, uint u2, int exp, uint error)
            {
                this.u0 = u0;
                this.u1 = u1;
                this.u2 = u2;
                this.exp = exp;
                this.error = error;
            }

            public BigNumber(XPathConvert.FloatingDecimal dec)
            {
                int num = 0;
                int exponent = dec.Exponent;
                int mantissaSize = dec.MantissaSize;
                this.u2 = (uint) (dec[num] << 0x1c);
                this.u1 = 0;
                this.u0 = 0;
                this.exp = 4;
                this.error = 0;
                exponent--;
                this.Normalize();
                while (++num < mantissaSize)
                {
                    uint uExtra = this.MulTenAdd(dec[num]);
                    exponent--;
                    if (uExtra != 0)
                    {
                        this.Round(uExtra);
                        if (num < (mantissaSize - 1))
                        {
                            this.error++;
                        }
                        break;
                    }
                }
                if (exponent != 0)
                {
                    XPathConvert.BigNumber[] tenPowersNeg;
                    if (exponent < 0)
                    {
                        tenPowersNeg = TenPowersNeg;
                        exponent = -exponent;
                    }
                    else
                    {
                        tenPowersNeg = TenPowersPos;
                    }
                    int num4 = exponent & 0x1f;
                    if (num4 > 0)
                    {
                        this.Mul(ref tenPowersNeg[num4 - 1]);
                    }
                    num4 = (exponent >> 5) & 15;
                    if (num4 > 0)
                    {
                        this.Mul(ref tenPowersNeg[num4 + 30]);
                    }
                }
            }

            private unsafe uint MulTenAdd(uint digit)
            {
                this.exp += 3;
                uint* numPtr = stackalloc uint[5];
                for (int i = 0; i < 5; i++)
                {
                    numPtr[i] = 0;
                }
                if (digit != 0)
                {
                    int index = 3 - (this.exp >> 5);
                    if (index < 0)
                    {
                        numPtr[0] = 1;
                    }
                    else
                    {
                        int num3 = this.exp & 0x1f;
                        if (num3 < 4)
                        {
                            numPtr[index + 1] = digit >> num3;
                            if (num3 > 0)
                            {
                                numPtr[index] = digit << (0x20 - num3);
                            }
                        }
                        else
                        {
                            numPtr[index] = digit << (0x20 - num3);
                        }
                    }
                }
                uint* numPtr1 = numPtr + 1;
                numPtr1[0] += XPathConvert.AddU(ref (uint) ref numPtr, this.u0 << 30);
                uint* numPtr2 = numPtr + 2;
                numPtr2[0] += XPathConvert.AddU(ref this.u0, (this.u0 >> 2) + (this.u1 << 30));
                if (numPtr[1] != 0)
                {
                    uint* numPtr3 = numPtr + 2;
                    numPtr3[0] += XPathConvert.AddU(ref this.u0, numPtr[1]);
                }
                uint* numPtr4 = numPtr + 3;
                numPtr4[0] += XPathConvert.AddU(ref this.u1, (this.u1 >> 2) + (this.u2 << 30));
                if (numPtr[2] != 0)
                {
                    uint* numPtr5 = numPtr + 3;
                    numPtr5[0] += XPathConvert.AddU(ref this.u1, numPtr[2]);
                }
                numPtr[4] = XPathConvert.AddU(ref this.u2, (this.u2 >> 2) + numPtr[3]);
                if (numPtr[4] != 0)
                {
                    numPtr[0] = ((numPtr[0] >> 1) | (numPtr[0] & 1)) | (this.u0 << 0x1f);
                    this.u0 = (this.u0 >> 1) | (this.u1 << 0x1f);
                    this.u1 = (this.u1 >> 1) | (this.u2 << 0x1f);
                    this.u2 = (this.u2 >> 1) | 0x80000000;
                    this.exp++;
                }
                return numPtr[0];
            }

            private void Round(uint uExtra)
            {
                if (((uExtra & 0x80000000) == 0) || (((uExtra & 0x7fffffff) == 0) && ((this.u0 & 1) == 0)))
                {
                    if (uExtra != 0)
                    {
                        this.error++;
                    }
                }
                else
                {
                    this.error++;
                    if (((XPathConvert.AddU(ref this.u0, 1) != 0) && (XPathConvert.AddU(ref this.u1, 1) != 0)) && (XPathConvert.AddU(ref this.u2, 1) != 0))
                    {
                        this.u2 = 0x80000000;
                        this.exp++;
                    }
                }
            }

            private bool IsZero =>
                (((this.u2 == 0) && (this.u1 == 0)) && (0 == this.u0));
            private void Normalize()
            {
                if (this.u2 == 0)
                {
                    if (this.u1 == 0)
                    {
                        if (this.u0 == 0)
                        {
                            this.exp = 0;
                            return;
                        }
                        this.u2 = this.u0;
                        this.u0 = 0;
                        this.exp -= 0x40;
                    }
                    else
                    {
                        this.u2 = this.u1;
                        this.u1 = this.u0;
                        this.u0 = 0;
                        this.exp -= 0x20;
                    }
                }
                int num = XPathConvert.CbitZeroLeft(this.u2);
                if (num != 0)
                {
                    int num2 = 0x20 - num;
                    this.u2 = (this.u2 << num) | (this.u1 >> num2);
                    this.u1 = (this.u1 << num) | (this.u0 >> num2);
                    this.u0 = this.u0 << num;
                    this.exp -= num;
                }
            }

            private void Mul(ref XPathConvert.BigNumber numOp)
            {
                uint num7;
                uint num8;
                uint num10;
                uint num = 0;
                uint num2 = 0;
                uint num3 = 0;
                uint num4 = 0;
                uint num5 = 0;
                uint num6 = 0;
                uint num9 = this.u0;
                if (num9 != 0)
                {
                    num = XPathConvert.MulU(num9, numOp.u0, out num8);
                    num2 = num8;
                    num7 = XPathConvert.MulU(num9, numOp.u1, out num8);
                    num10 = XPathConvert.AddU(ref num2, num7);
                    XPathConvert.AddU(ref num3, num8 + num10);
                    num7 = XPathConvert.MulU(num9, numOp.u2, out num8);
                    num10 = XPathConvert.AddU(ref num3, num7);
                    XPathConvert.AddU(ref num4, num8 + num10);
                }
                num9 = this.u1;
                if (num9 != 0)
                {
                    num7 = XPathConvert.MulU(num9, numOp.u0, out num8);
                    num10 = XPathConvert.AddU(ref num2, num7);
                    if ((XPathConvert.AddU(ref num3, num8 + num10) != 0) && (XPathConvert.AddU(ref num4, 1) != 0))
                    {
                        XPathConvert.AddU(ref num5, 1);
                    }
                    num7 = XPathConvert.MulU(num9, numOp.u1, out num8);
                    num10 = XPathConvert.AddU(ref num3, num7);
                    if (XPathConvert.AddU(ref num4, num8 + num10) != 0)
                    {
                        XPathConvert.AddU(ref num5, 1);
                    }
                    num7 = XPathConvert.MulU(num9, numOp.u2, out num8);
                    num10 = XPathConvert.AddU(ref num4, num7);
                    XPathConvert.AddU(ref num5, num8 + num10);
                }
                num9 = this.u2;
                num7 = XPathConvert.MulU(num9, numOp.u0, out num8);
                num10 = XPathConvert.AddU(ref num3, num7);
                if ((XPathConvert.AddU(ref num4, num8 + num10) != 0) && (XPathConvert.AddU(ref num5, 1) != 0))
                {
                    XPathConvert.AddU(ref num6, 1);
                }
                num7 = XPathConvert.MulU(num9, numOp.u1, out num8);
                num10 = XPathConvert.AddU(ref num4, num7);
                if (XPathConvert.AddU(ref num5, num8 + num10) != 0)
                {
                    XPathConvert.AddU(ref num6, 1);
                }
                num7 = XPathConvert.MulU(num9, numOp.u2, out num8);
                num10 = XPathConvert.AddU(ref num5, num7);
                XPathConvert.AddU(ref num6, num8 + num10);
                this.exp += numOp.exp;
                this.error += numOp.error;
                if ((num6 & 0x80000000) == 0)
                {
                    if ((((num3 & 0x40000000) != 0) && ((((num3 & 0xbfffffff) != 0) || (num2 != 0)) || (num != 0))) && (((XPathConvert.AddU(ref num3, 0x40000000) != 0) && (XPathConvert.AddU(ref num4, 1) != 0)) && (XPathConvert.AddU(ref num5, 1) != 0)))
                    {
                        XPathConvert.AddU(ref num6, 1);
                        if ((num6 & 0x80000000) != 0)
                        {
                            goto Label_0314;
                        }
                    }
                    this.u2 = (num6 << 1) | (num5 >> 0x1f);
                    this.u1 = (num5 << 1) | (num4 >> 0x1f);
                    this.u0 = (num4 << 1) | (num3 >> 0x1f);
                    this.exp--;
                    this.error = this.error << 1;
                    if ((((num3 & 0x7fffffff) != 0) || (num2 != 0)) || (num != 0))
                    {
                        this.error++;
                    }
                    return;
                }
                if ((((num3 & 0x80000000) != 0) && ((((num4 & 1) != 0) || ((num3 & 0x7fffffff) != 0)) || ((num2 != 0) || (num != 0)))) && (((XPathConvert.AddU(ref num4, 1) != 0) && (XPathConvert.AddU(ref num5, 1) != 0)) && (XPathConvert.AddU(ref num6, 1) != 0)))
                {
                    num6 = 0x80000000;
                    this.exp++;
                }
            Label_0314:
                this.u2 = num6;
                this.u1 = num5;
                this.u0 = num4;
                if (((num3 != 0) || (num2 != 0)) || (num != 0))
                {
                    this.error++;
                }
            }

            public static explicit operator double(XPathConvert.BigNumber bn)
            {
                uint num;
                uint num3;
                uint num4;
                int num2 = bn.exp + 0x3fe;
                if (num2 >= 0x7ff)
                {
                    return double.PositiveInfinity;
                }
                if (num2 > 0)
                {
                    num3 = (uint) ((num2 << 20) | ((bn.u2 & 0x7fffffff) >> 11));
                    num4 = (bn.u2 << 0x15) | (bn.u1 >> 11);
                    num = (bn.u1 << 0x15) | XPathConvert.NotZero(bn.u0);
                }
                else if (num2 > -20)
                {
                    int num5 = 12 - num2;
                    num3 = bn.u2 >> num5;
                    num4 = (bn.u2 << (0x20 - num5)) | (bn.u1 >> num5);
                    num = (bn.u1 << (0x20 - num5)) | XPathConvert.NotZero(bn.u0);
                }
                else if (num2 == -20)
                {
                    num3 = 0;
                    num4 = bn.u2;
                    num = bn.u1 | ((bn.u0 != 0) ? 1 : 0);
                }
                else if (num2 > -52)
                {
                    int num6 = -num2 - 20;
                    num3 = 0;
                    num4 = bn.u2 >> num6;
                    num = ((bn.u2 << (0x20 - num6)) | XPathConvert.NotZero(bn.u1)) | XPathConvert.NotZero(bn.u0);
                }
                else if (num2 == -52)
                {
                    num3 = 0;
                    num4 = 0;
                    num = (bn.u2 | XPathConvert.NotZero(bn.u1)) | XPathConvert.NotZero(bn.u0);
                }
                else
                {
                    return 0.0;
                }
                if ((((num & 0x80000000) != 0) && (((num & 0x7fffffff) != 0) || ((num4 & 1) != 0))) && (XPathConvert.AddU(ref num4, 1) != 0))
                {
                    XPathConvert.AddU(ref num3, 1);
                }
                return BitConverter.Int64BitsToDouble((long) ((num3 << 0x20) | num4));
            }

            private uint UMod1()
            {
                if (this.exp <= 0)
                {
                    return 0;
                }
                uint num = this.u2 >> (0x20 - this.exp);
                this.u2 &= ((uint) 0x7fffffff) >> (this.exp - 1);
                this.Normalize();
                return num;
            }

            public void MakeUpperBound()
            {
                uint num = (uint) ((this.error + 1) >> 1);
                if (((num != 0) && (XPathConvert.AddU(ref this.u0, num) != 0)) && ((XPathConvert.AddU(ref this.u1, 1) != 0) && (XPathConvert.AddU(ref this.u2, 1) != 0)))
                {
                    this.u2 = 0x80000000;
                    this.u0 = (this.u0 >> 1) + (this.u0 & 1);
                    this.exp++;
                }
                this.error = 0;
            }

            public void MakeLowerBound()
            {
                uint num = (uint) ((this.error + 1) >> 1);
                if (((num != 0) && (XPathConvert.AddU(ref this.u0, -num) == 0)) && (XPathConvert.AddU(ref this.u1, uint.MaxValue) == 0))
                {
                    XPathConvert.AddU(ref this.u2, uint.MaxValue);
                    if ((0x80000000 & this.u2) == 0)
                    {
                        this.Normalize();
                    }
                }
                this.error = 0;
            }

            public static bool DblToRgbFast(double dbl, byte[] mantissa, out int exponent, out int mantissaSize)
            {
                XPathConvert.BigNumber number;
                XPathConvert.BigNumber number4;
                XPathConvert.BigNumber number5;
                XPathConvert.BigNumber number6;
                int num;
                int num2;
                byte num5;
                int num18;
                int num14 = 0;
                uint num16 = XPathConvert.DblHi(dbl);
                uint num17 = XPathConvert.DblLo(dbl);
                int num13 = ((int) (num16 >> 20)) & 0x7ff;
                if (num13 > 0)
                {
                    uint num3;
                    if (((num13 >= 0x3ff) && (num13 <= 0x433)) && (dbl == Math.Floor(dbl)))
                    {
                        double num15 = dbl;
                        num2 = 0;
                        if (num15 >= XPathConvert.C10toN[num2 + 8])
                        {
                            num2 += 8;
                        }
                        if (num15 >= XPathConvert.C10toN[num2 + 4])
                        {
                            num2 += 4;
                        }
                        if (num15 >= XPathConvert.C10toN[num2 + 2])
                        {
                            num2 += 2;
                        }
                        if (num15 >= XPathConvert.C10toN[num2 + 1])
                        {
                            num2++;
                        }
                        exponent = num2 + 1;
                        num = 0;
                        while (0.0 != num15)
                        {
                            num5 = (byte) (num15 / XPathConvert.C10toN[num2]);
                            num15 -= num5 * XPathConvert.C10toN[num2];
                            mantissa[num++] = num5;
                            num2--;
                        }
                        mantissaSize = num;
                        goto Label_05CB;
                    }
                    number5.u2 = ((uint) (-2147483648 | ((num16 & 0xffffff) << 11))) | (num17 >> 0x15);
                    number5.u1 = num17 << 11;
                    number5.u0 = 0;
                    number5.exp = num13 - 0x3fe;
                    number5.error = 0;
                    number = number5;
                    number.u1 |= 0x400;
                    number4 = number5;
                    if ((0x80000000 == number4.u2) && (number4.u1 == 0))
                    {
                        num3 = 0xfffffe00;
                    }
                    else
                    {
                        num3 = 0xfffffc00;
                    }
                    if (XPathConvert.AddU(ref number4.u1, num3) == 0)
                    {
                        XPathConvert.AddU(ref number4.u2, uint.MaxValue);
                        if ((0x80000000 & number4.u2) == 0)
                        {
                            number4.Normalize();
                        }
                    }
                }
                else
                {
                    number5.u2 = num16 & 0xfffff;
                    number5.u1 = num17;
                    number5.u0 = 0;
                    number5.exp = -1010;
                    number5.error = 0;
                    number = number5;
                    number.u0 = 0x80000000;
                    number4 = number;
                    if (XPathConvert.AddU(ref number4.u1, uint.MaxValue) == 0)
                    {
                        XPathConvert.AddU(ref number4.u2, uint.MaxValue);
                    }
                    number5.Normalize();
                    number.Normalize();
                    number4.Normalize();
                }
                if (number.exp >= 0x20)
                {
                    num2 = ((number.exp - 0x19) * 15) / -TenPowersNeg[0x2d].exp;
                    if (num2 > 0)
                    {
                        number6 = TenPowersNeg[30 + num2];
                        number.Mul(ref number6);
                        number4.Mul(ref number6);
                        num14 += num2 * 0x20;
                    }
                    if (number.exp >= 0x20)
                    {
                        num2 = ((number.exp - 0x19) * 0x20) / -TenPowersNeg[0x1f].exp;
                        number6 = TenPowersNeg[num2 - 1];
                        number.Mul(ref number6);
                        number4.Mul(ref number6);
                        num14 += num2;
                    }
                }
                else if (number.exp < 1)
                {
                    num2 = ((0x19 - number.exp) * 15) / TenPowersPos[0x2d].exp;
                    if (num2 > 0)
                    {
                        number6 = TenPowersPos[30 + num2];
                        number.Mul(ref number6);
                        number4.Mul(ref number6);
                        num14 -= num2 * 0x20;
                    }
                    if (number.exp < 1)
                    {
                        num2 = ((0x19 - number.exp) * 0x20) / TenPowersPos[0x1f].exp;
                        number6 = TenPowersPos[num2 - 1];
                        number.Mul(ref number6);
                        number4.Mul(ref number6);
                        num14 -= num2;
                    }
                }
                XPathConvert.BigNumber number2 = number;
                number.MakeUpperBound();
                number2.MakeLowerBound();
                uint num9 = number.UMod1();
                uint num10 = number2.UMod1();
                XPathConvert.BigNumber number3 = number4;
                number3.MakeUpperBound();
                number4.MakeLowerBound();
                uint num11 = number3.UMod1();
                uint num12 = number4.UMod1();
                uint num4 = 1;
                if (num9 >= 0x5f5e100)
                {
                    num4 = 0x5f5e100;
                    num14 += 8;
                }
                else
                {
                    if (num9 >= 0x2710)
                    {
                        num4 = 0x2710;
                        num14 += 4;
                    }
                    if (num9 >= (100 * num4))
                    {
                        num4 *= 100;
                        num14 += 2;
                    }
                }
                if (num9 >= (10 * num4))
                {
                    num4 *= 10;
                    num14++;
                }
                num14++;
                num = 0;
            Label_03B4:
                num5 = (byte) (num9 / num4);
                num9 = num9 % num4;
                byte num8 = (byte) (num12 / num4);
                num12 = num12 % num4;
                if (num5 == num8)
                {
                    mantissa[num++] = num5;
                    if (1 == num4)
                    {
                        num4 = 0x989680;
                        number.Mul(ref TenPowersPos[7]);
                        number.MakeUpperBound();
                        num9 = number.UMod1();
                        if (num9 >= 0x5f5e100)
                        {
                            goto Label_05CD;
                        }
                        number2.Mul(ref TenPowersPos[7]);
                        number2.MakeLowerBound();
                        num10 = number2.UMod1();
                        number3.Mul(ref TenPowersPos[7]);
                        number3.MakeUpperBound();
                        num11 = number3.UMod1();
                        number4.Mul(ref TenPowersPos[7]);
                        number4.MakeLowerBound();
                        num12 = number4.UMod1();
                    }
                    else
                    {
                        num4 /= 10;
                    }
                    goto Label_03B4;
                }
                byte num7 = (byte) ((num11 / num4) % 10);
                num11 = num11 % num4;
                byte num6 = (byte) ((num10 / num4) % 10);
                num10 = num10 % num4;
                if (num7 >= num6)
                {
                    goto Label_05CD;
                }
                if (((num7 != 0) || (num11 != 0)) || (!number3.IsZero || ((num17 & 1) != 0)))
                {
                    if ((num6 - num7) > 1)
                    {
                        mantissa[num++] = (byte) (((num6 + num7) + 1) / 2);
                    }
                    else
                    {
                        if (((num10 == 0) && number2.IsZero) && ((num17 & 1) != 0))
                        {
                            goto Label_05CD;
                        }
                        mantissa[num++] = num6;
                    }
                }
                exponent = num14;
                mantissaSize = num;
            Label_05CB:
                return true;
            Label_05CD:
                mantissaSize = num18 = 0;
                exponent = num18;
                return false;
            }

            public static void DblToRgbPrecise(double dbl, byte[] mantissa, out int exponent, out int mantissaSize)
            {
                XPathConvert.BigInteger integer6;
                int num6;
                int num8;
                int num9;
                int num10;
                int num11;
                double num12;
                XPathConvert.BigInteger integer = new XPathConvert.BigInteger();
                XPathConvert.BigInteger bi = new XPathConvert.BigInteger();
                XPathConvert.BigInteger biSrc = new XPathConvert.BigInteger();
                XPathConvert.BigInteger integer4 = new XPathConvert.BigInteger();
                XPathConvert.BigInteger integer5 = new XPathConvert.BigInteger();
                uint num15 = XPathConvert.DblHi(dbl);
                uint num16 = XPathConvert.DblLo(dbl);
                bi.InitFromDigits(1, 0, 1);
                biSrc.InitFromDigits(1, 0, 1);
                int num5 = ((int) ((num15 & 0x7ff00000) >> 20)) - 0x433;
                uint uMul = num15 & 0xfffff;
                uint num13 = num16;
                int cu = 2;
                bool flag = false;
                if (num5 == -1075)
                {
                    if (uMul == 0)
                    {
                        cu = 1;
                    }
                    num12 = BitConverter.Int64BitsToDouble(0x4ff0000000000000L) * dbl;
                    num6 = ((int) ((XPathConvert.DblHi(num12) & 0x7ff00000) >> 20)) - 0x4ff;
                    num15 = XPathConvert.DblHi(num12) & 0xfffff;
                    num15 |= 0x3ff00000;
                    num12 = BitConverter.Int64BitsToDouble((long) ((num15 << 0x20) | XPathConvert.DblLo(num12)));
                    num5++;
                }
                else
                {
                    num15 &= 0xfffff;
                    num15 |= 0x3ff00000;
                    num12 = BitConverter.Int64BitsToDouble((long) ((num15 << 0x20) | num16));
                    num6 = num5 + 0x34;
                    if (((num13 == 0) && (uMul == 0)) && (num5 > -1074))
                    {
                        uMul = 0x200000;
                        num5--;
                        flag = true;
                    }
                    else
                    {
                        uMul |= 0x100000;
                    }
                }
                num12 = (((num12 - 1.5) * 0.289529654602168) + 0.1760912590558) + (num6 * 0.301029995663981);
                int num4 = (int) num12;
                if ((num12 < 0.0) && (num12 != num4))
                {
                    num4--;
                }
                if (num5 >= 0)
                {
                    num8 = num5;
                    num9 = 0;
                }
                else
                {
                    num8 = 0;
                    num9 = -num5;
                }
                if (num4 >= 0)
                {
                    num10 = 0;
                    num11 = num4;
                    num9 += num4;
                }
                else
                {
                    num8 -= num4;
                    num10 = -num4;
                    num11 = 0;
                }
                if ((num8 > 0) && (num9 > 0))
                {
                    num6 = (num8 < num9) ? num8 : num9;
                    num8 -= num6;
                    num9 -= num6;
                }
                num8++;
                num9++;
                if (num10 > 0)
                {
                    biSrc.MulPow5(num10);
                    integer.InitFromBigint(biSrc);
                    if (1 == cu)
                    {
                        integer.MulAdd(num13, 0);
                    }
                    else
                    {
                        integer.MulAdd(uMul, 0);
                        integer.ShiftLeft(0x20);
                        if (num13 != 0)
                        {
                            integer5.InitFromBigint(biSrc);
                            integer5.MulAdd(num13, 0);
                            integer.Add(integer5);
                        }
                    }
                }
                else
                {
                    integer.InitFromDigits(num13, uMul, cu);
                    if (num11 > 0)
                    {
                        bi.MulPow5(num11);
                    }
                }
                num6 = ((XPathConvert.CbitZeroLeft(bi[bi.Length - 1]) + 0x1c) - num9) & 0x1f;
                num8 += num6;
                num9 += num6;
                integer.ShiftLeft(num8);
                if (num8 > 1)
                {
                    biSrc.ShiftLeft(num8 - 1);
                }
                bi.ShiftLeft(num9);
                if (flag)
                {
                    integer6 = integer4;
                    integer6.InitFromBigint(biSrc);
                    biSrc.ShiftLeft(1);
                }
                else
                {
                    integer6 = biSrc;
                }
                int num2 = 0;
                while (true)
                {
                    byte num = (byte) integer.DivRem(bi);
                    if ((num2 == 0) && (num == 0))
                    {
                        num4--;
                    }
                    else
                    {
                        int num7;
                        num6 = integer.CompareTo(integer6);
                        if (bi.CompareTo(biSrc) < 0)
                        {
                            num7 = 1;
                        }
                        else
                        {
                            integer5.InitFromBigint(bi);
                            integer5.Subtract(biSrc);
                            num7 = integer.CompareTo(integer5);
                        }
                        if ((num7 == 0) && ((num16 & 1) == 0))
                        {
                            if (num == 9)
                            {
                                goto Label_041A;
                            }
                            if (num6 > 0)
                            {
                                num = (byte) (num + 1);
                            }
                            mantissa[num2++] = num;
                            goto Label_042F;
                        }
                        if ((num6 < 0) || ((num6 == 0) && ((num16 & 1) == 0)))
                        {
                            if (num7 > 0)
                            {
                                integer.ShiftLeft(1);
                                num7 = integer.CompareTo(bi);
                                if ((num7 > 0) || ((num7 == 0) && ((num & 1) != 0)))
                                {
                                    num = (byte) (num + 1);
                                    if (num == 9)
                                    {
                                        goto Label_041A;
                                    }
                                }
                            }
                            mantissa[num2++] = num;
                            goto Label_042F;
                        }
                        if (num7 > 0)
                        {
                            if (num == 9)
                            {
                                goto Label_041A;
                            }
                            mantissa[num2++] = (byte) (num + 1);
                            goto Label_042F;
                        }
                        mantissa[num2++] = num;
                    }
                    integer.MulAdd(10, 0);
                    biSrc.MulAdd(10, 0);
                    if (integer6 != biSrc)
                    {
                        integer6.MulAdd(10, 0);
                    }
                }
            Label_03F0:
                if (mantissa[--num2] != 9)
                {
                    mantissa[num2++] = (byte) (mantissa[num2++] + 1);
                    goto Label_042F;
                }
            Label_041A:
                if (num2 > 0)
                {
                    goto Label_03F0;
                }
                num4++;
                mantissa[num2++] = 1;
            Label_042F:
                exponent = num4 + 1;
                mantissaSize = num2;
            }

            static BigNumber()
            {
                TenPowersPos = new XPathConvert.BigNumber[] { 
                    new XPathConvert.BigNumber(0, 0, 0xa0000000, 4, 0), new XPathConvert.BigNumber(0, 0, 0xc8000000, 7, 0), new XPathConvert.BigNumber(0, 0, 0xfa000000, 10, 0), new XPathConvert.BigNumber(0, 0, 0x9c400000, 14, 0), new XPathConvert.BigNumber(0, 0, 0xc3500000, 0x11, 0), new XPathConvert.BigNumber(0, 0, 0xf4240000, 20, 0), new XPathConvert.BigNumber(0, 0, 0x98968000, 0x18, 0), new XPathConvert.BigNumber(0, 0, 0xbebc2000, 0x1b, 0), new XPathConvert.BigNumber(0, 0, 0xee6b2800, 30, 0), new XPathConvert.BigNumber(0, 0, 0x9502f900, 0x22, 0), new XPathConvert.BigNumber(0, 0, 0xba43b740, 0x25, 0), new XPathConvert.BigNumber(0, 0, 0xe8d4a510, 40, 0), new XPathConvert.BigNumber(0, 0, 0x9184e72a, 0x2c, 0), new XPathConvert.BigNumber(0, 0x80000000, 0xb5e620f4, 0x2f, 0), new XPathConvert.BigNumber(0, 0xa0000000, 0xe35fa931, 50, 0), new XPathConvert.BigNumber(0, 0x4000000, 0x8e1bc9bf, 0x36, 0),
                    new XPathConvert.BigNumber(0, 0xc5000000, 0xb1a2bc2e, 0x39, 0), new XPathConvert.BigNumber(0, 0x76400000, 0xde0b6b3a, 60, 0), new XPathConvert.BigNumber(0, 0x89e80000, 0x8ac72304, 0x40, 0), new XPathConvert.BigNumber(0, 0xac620000, 0xad78ebc5, 0x43, 0), new XPathConvert.BigNumber(0, 0x177a8000, 0xd8d726b7, 70, 0), new XPathConvert.BigNumber(0, 0x6eac9000, 0x87867832, 0x4a, 0), new XPathConvert.BigNumber(0, 0xa57b400, 0xa968163f, 0x4d, 0), new XPathConvert.BigNumber(0, 0xcceda100, 0xd3c21bce, 80, 0), new XPathConvert.BigNumber(0, 0x401484a0, 0x84595161, 0x54, 0), new XPathConvert.BigNumber(0, 0x9019a5c8, 0xa56fa5b9, 0x57, 0), new XPathConvert.BigNumber(0, 0xf4200f3a, 0xcecb8f27, 90, 0), new XPathConvert.BigNumber(0x40000000, 0xf8940984, 0x813f3978, 0x5e, 0), new XPathConvert.BigNumber(0x50000000, 0x36b90be5, 0xa18f07d7, 0x61, 0), new XPathConvert.BigNumber(0xa4000000, 0x4674ede, 0xc9f2c9cd, 100, 0), new XPathConvert.BigNumber(0x4d000000, 0x45812296, 0xfc6f7c40, 0x67, 0), new XPathConvert.BigNumber(0xf0200000, 0x2b70b59d, 0x9dc5ada8, 0x6b, 0),
                    new XPathConvert.BigNumber(0x3cbf6b72, 0xffcfa6d5, 0xc2781f49, 0xd5, 1), new XPathConvert.BigNumber(0xc5cfe94f, 0xc59b14a2, 0xefb3ab16, 0x13f, 1), new XPathConvert.BigNumber(0xc66f336c, 0x80e98cdf, 0x93ba47c9, 0x1aa, 1), new XPathConvert.BigNumber(0x577b986b, 0x7fe617aa, 0xb616a12b, 0x214, 1), new XPathConvert.BigNumber(0x85bbe254, 0x3927556a, 0xe070f78d, 0x27e, 1), new XPathConvert.BigNumber(0x82bd6b71, 0xe33cc92f, 0x8a5296ff, 0x2e9, 1), new XPathConvert.BigNumber(0xddbb901c, 0x9df9de8d, 0xaa7eebfb, 0x353, 1), new XPathConvert.BigNumber(0x73832eec, 0x5c6a2f8c, 0xd226fc19, 0x3bd, 1), new XPathConvert.BigNumber(0xe6a11583, 0xf2cce375, 0x81842f29, 0x428, 1), new XPathConvert.BigNumber(0x5ebf18b7, 0xdb900ad2, 0x9fa42700, 0x492, 1), new XPathConvert.BigNumber(0x1027fff5, 0xaef8aa17, 0xc4c5e310, 0x4fc, 1), new XPathConvert.BigNumber(0xb5e54f71, 0xe9b09c58, 0xf28a9c07, 0x566, 1), new XPathConvert.BigNumber(0xa7ea9c88, 0xebf7f3d3, 0x957a4ae1, 0x5d1, 1), new XPathConvert.BigNumber(0x7df40a74, 0x795a262, 0xb83ed8dc, 0x63b, 1)
                };
                TenPowersNeg = new XPathConvert.BigNumber[] { 
                    new XPathConvert.BigNumber(0xcccccccd, 0xcccccccc, 0xcccccccc, -3, 1), new XPathConvert.BigNumber(0x3d70a3d7, 0x70a3d70a, 0xa3d70a3d, -6, 1), new XPathConvert.BigNumber(0x645a1cac, 0x8d4fdf3b, 0x83126e97, -9, 1), new XPathConvert.BigNumber(0xd3c36113, 0xe219652b, 0xd1b71758, -13, 1), new XPathConvert.BigNumber(0xfcf80dc, 0x1b478423, 0xa7c5ac47, -16, 1), new XPathConvert.BigNumber(0xa63f9a4a, 0xaf6c69b5, 0x8637bd05, -19, 1), new XPathConvert.BigNumber(0x3d329076, 0xe57a42bc, 0xd6bf94d5, -23, 1), new XPathConvert.BigNumber(0xfdc20d2b, 0x8461cefc, 0xabcc7711, -26, 1), new XPathConvert.BigNumber(0x31680a89, 0x36b4a597, 0x89705f41, -29, 1), new XPathConvert.BigNumber(0xb573440e, 0xbdedd5be, 0xdbe6fece, -33, 1), new XPathConvert.BigNumber(0xf78f69a5, 0xcb24aafe, 0xafebff0b, -36, 1), new XPathConvert.BigNumber(0xf93f87b7, 0x6f5088cb, 0x8cbccc09, -39, 1), new XPathConvert.BigNumber(0x2865a5f2, 0x4bb40e13, 0xe12e1342, -43, 1), new XPathConvert.BigNumber(0x538484c2, 0x95cd80f, 0xb424dc35, -46, 1), new XPathConvert.BigNumber(0xf9d3701, 0x3ab0acd9, 0x901d7cf7, -49, 1), new XPathConvert.BigNumber(0x4c2ebe68, 0xc44de15b, 0xe69594be, -53, 1),
                    new XPathConvert.BigNumber(0x9befeba, 0x36a4b449, 0xb877aa32, -56, 1), new XPathConvert.BigNumber(0x3aff322e, 0x921d5d07, 0x9392ee8e, -59, 1), new XPathConvert.BigNumber(0x2b31e9e4, 0xb69561a5, 0xec1e4a7d, -63, 1), new XPathConvert.BigNumber(0x88f4bb1d, 0x92111aea, 0xbce50864, -66, 1), new XPathConvert.BigNumber(0xd3f6fc17, 0x74da7bee, 0x971da050, -69, 1), new XPathConvert.BigNumber(0x5324c68b, 0xbaf72cb1, 0xf1c90080, -73, 1), new XPathConvert.BigNumber(0x75b7053c, 0x95928a27, 0xc16d9a00, -76, 1), new XPathConvert.BigNumber(0xc4926a96, 0x44753b52, 0x9abe14cd, -79, 1), new XPathConvert.BigNumber(0x3a83ddbe, 0xd3eec551, 0xf79687ae, -83, 1), new XPathConvert.BigNumber(0x95364afe, 0x76589dda, 0xc6120625, -86, 1), new XPathConvert.BigNumber(0x775ea265, 0x91e07e48, 0x9e74d1b7, -89, 1), new XPathConvert.BigNumber(0x8bca9d6e, 0x8300ca0d, 0xfd87b5f2, -93, 1), new XPathConvert.BigNumber(0x96ee458, 0x359a3b3e, 0xcad2f7f5, -96, 1), new XPathConvert.BigNumber(0xa125837a, 0x5e14fc31, 0xa2425ff7, -99, 1), new XPathConvert.BigNumber(0x80eacf95, 0x4b43fcf4, 0x81ceb32c, -102, 1), new XPathConvert.BigNumber(0x67de18ee, 0x453994ba, 0xcfb11ead, -106, 1),
                    new XPathConvert.BigNumber(0x3f2398d7, 0xa539e9a5, 0xa87fea27, -212, 1), new XPathConvert.BigNumber(0x11dbcb02, 0xfd75539b, 0x88b402f7, -318, 1), new XPathConvert.BigNumber(0xac7cb3f7, 0x64bce4a0, 0xddd0467c, -425, 1), new XPathConvert.BigNumber(0x59ed2167, 0xdb73a093, 0xb3f4e093, -531, 1), new XPathConvert.BigNumber(0x7b6306a3, 0x5423cc06, 0x91ff8377, -637, 1), new XPathConvert.BigNumber(0xa4f8bf56, 0x4a314ebd, 0xece53cec, -744, 1), new XPathConvert.BigNumber(0xfa911156, 0x637a1939, 0xc0314325, -850, 1), new XPathConvert.BigNumber(0x4ee367f9, 0x836ac577, 0x9becce62, -956, 1), new XPathConvert.BigNumber(0x8920b099, 0x478238d0, 0xfd00b897, -1063, 1), new XPathConvert.BigNumber(0x92757c, 0x46f34f7d, 0xcd42a113, -1169, 1), new XPathConvert.BigNumber(0x88dba000, 0xb11b0857, 0xa686e3e8, -1275, 1), new XPathConvert.BigNumber(0x1a4eb007, 0x3ffc68a6, 0x871a4981, -1381, 1), new XPathConvert.BigNumber(0x84c663cf, 0xb6074244, 0xdb377599, -1488, 1), new XPathConvert.BigNumber(0x61eb52e2, 0x79007736, 0xb1d983b4, -1594, 1)
                };
            }
        }

        private class FloatingDecimal
        {
            private int exponent;
            private byte[] mantissa;
            private int mantissaSize;
            public const int MaxDigits = 50;
            private const int MaxExp10 = 310;
            private const int MinExp10 = -325;
            private int sign;

            public FloatingDecimal()
            {
                this.mantissa = new byte[50];
                this.exponent = 0;
                this.sign = 1;
                this.mantissaSize = 0;
            }

            public FloatingDecimal(double dbl)
            {
                this.mantissa = new byte[50];
                this.InitFromDouble(dbl);
            }

            private double AdjustDbl(double dbl)
            {
                int num;
                int num2;
                int num3;
                int num4;
                int num13;
                XPathConvert.BigInteger integer = new XPathConvert.BigInteger();
                XPathConvert.BigInteger integer2 = new XPathConvert.BigInteger();
                integer.InitFromFloatingDecimal(this);
                int num9 = this.exponent - this.mantissaSize;
                if (num9 >= 0)
                {
                    num3 = num = num9;
                    num4 = num2 = 0;
                }
                else
                {
                    num3 = num = 0;
                    num4 = num2 = -num9;
                }
                uint num12 = XPathConvert.DblHi(dbl);
                uint num11 = XPathConvert.DblLo(dbl);
                int num10 = ((int) (num12 >> 20)) & 0x7ff;
                num12 &= 0xfffff;
                uint num5 = 1;
                if (num10 != 0)
                {
                    if (((num12 == 0) && (num11 == 0)) && (1 != num10))
                    {
                        num5 = 2;
                        num12 = 0x200000;
                        num10--;
                    }
                    else
                    {
                        num12 |= 0x100000;
                    }
                    num10 -= 0x434;
                }
                else
                {
                    num10 = -1075;
                }
                num12 = (num12 << 1) | (num11 >> 0x1f);
                num11 = num11 << 1;
                if ((num11 == 0) && (num12 == 0))
                {
                    num13 = 0;
                }
                else if (num12 == 0)
                {
                    num13 = 1;
                }
                else
                {
                    num13 = 2;
                }
                integer2.InitFromDigits(num11, num12, num13);
                if (num10 >= 0)
                {
                    num2 += num10;
                }
                else
                {
                    num += -num10;
                }
                if (num2 > num)
                {
                    num2 -= num;
                    num = 0;
                    int cu = 0;
                    while ((num2 >= 0x20) && (integer[cu] == 0))
                    {
                        num2 -= 0x20;
                        cu++;
                    }
                    if (cu > 0)
                    {
                        integer.ShiftUsRight(cu);
                    }
                    uint num6 = integer[0];
                    cu = 0;
                    while ((cu < num2) && (0L == (num6 & (((long) 1L) << cu))))
                    {
                        cu++;
                    }
                    if (cu > 0)
                    {
                        num2 -= cu;
                        integer.ShiftRight(cu);
                    }
                }
                else
                {
                    num -= num2;
                    num2 = 0;
                }
                if (num4 > 0)
                {
                    integer2.MulPow5(num4);
                }
                else if (num3 > 0)
                {
                    integer.MulPow5(num3);
                }
                if (num2 > 0)
                {
                    integer2.ShiftLeft(num2);
                }
                else if (num > 0)
                {
                    integer.ShiftLeft(num);
                }
                int num7 = integer2.CompareTo(integer);
                if (num7 != 0)
                {
                    if (num7 > 0)
                    {
                        if (XPathConvert.AddU(ref num11, uint.MaxValue) == 0)
                        {
                            XPathConvert.AddU(ref num12, uint.MaxValue);
                        }
                        integer2.InitFromDigits(num11, num12, 1 + ((num12 != 0) ? 1 : 0));
                        if (num4 > 0)
                        {
                            integer2.MulPow5(num4);
                        }
                        if (num2 > 0)
                        {
                            integer2.ShiftLeft(num2);
                        }
                        num7 = integer2.CompareTo(integer);
                        if ((num7 > 0) || ((num7 == 0) && ((XPathConvert.DblLo(dbl) & 1) != 0)))
                        {
                            dbl = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(dbl) - 1L);
                        }
                        return dbl;
                    }
                    if (XPathConvert.AddU(ref num11, num5) != 0)
                    {
                        XPathConvert.AddU(ref num12, 1);
                    }
                    integer2.InitFromDigits(num11, num12, 1 + ((num12 != 0) ? 1 : 0));
                    if (num4 > 0)
                    {
                        integer2.MulPow5(num4);
                    }
                    if (num2 > 0)
                    {
                        integer2.ShiftLeft(num2);
                    }
                    num7 = integer2.CompareTo(integer);
                    if ((num7 >= 0) && ((num7 != 0) || ((XPathConvert.DblLo(dbl) & 1) == 0)))
                    {
                        return dbl;
                    }
                    dbl = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(dbl) + 1L);
                }
                return dbl;
            }

            private void InitFromDouble(double dbl)
            {
                if ((0.0 == dbl) || XPathConvert.IsSpecial(dbl))
                {
                    this.exponent = 0;
                    this.sign = 1;
                    this.mantissaSize = 0;
                }
                else
                {
                    if (dbl < 0.0)
                    {
                        this.sign = -1;
                        dbl = -dbl;
                    }
                    else
                    {
                        this.sign = 1;
                    }
                    if (!XPathConvert.BigNumber.DblToRgbFast(dbl, this.mantissa, out this.exponent, out this.mantissaSize))
                    {
                        XPathConvert.BigNumber.DblToRgbPrecise(dbl, this.mantissa, out this.exponent, out this.mantissaSize);
                    }
                }
            }

            public static explicit operator double(XPathConvert.FloatingDecimal dec)
            {
                double positiveInfinity;
                int mantissaSize = dec.mantissaSize;
                int index = dec.exponent - mantissaSize;
                if (((mantissaSize <= 15) && (index >= -22)) && (dec.exponent <= 0x25))
                {
                    if (mantissaSize <= 9)
                    {
                        uint num = 0;
                        for (int i = 0; i < mantissaSize; i++)
                        {
                            num = (num * 10) + dec[i];
                        }
                        positiveInfinity = num;
                    }
                    else
                    {
                        positiveInfinity = 0.0;
                        for (int j = 0; j < mantissaSize; j++)
                        {
                            positiveInfinity = (positiveInfinity * 10.0) + dec[j];
                        }
                    }
                    if (index > 0)
                    {
                        if (index > 0x16)
                        {
                            positiveInfinity *= XPathConvert.C10toN[index - 0x16];
                            positiveInfinity *= XPathConvert.C10toN[0x16];
                        }
                        else
                        {
                            positiveInfinity *= XPathConvert.C10toN[index];
                        }
                    }
                    else if (index < 0)
                    {
                        positiveInfinity /= XPathConvert.C10toN[-index];
                    }
                }
                else if (dec.exponent >= 310)
                {
                    positiveInfinity = double.PositiveInfinity;
                }
                else if (dec.exponent <= -325)
                {
                    positiveInfinity = 0.0;
                }
                else
                {
                    XPathConvert.BigNumber number = new XPathConvert.BigNumber(dec);
                    if (number.Error == 0)
                    {
                        positiveInfinity = (double) number;
                    }
                    else
                    {
                        XPathConvert.BigNumber number2 = number;
                        number2.MakeUpperBound();
                        XPathConvert.BigNumber number3 = number;
                        number3.MakeLowerBound();
                        positiveInfinity = (double) number2;
                        double num4 = (double) number3;
                        if (positiveInfinity != num4)
                        {
                            positiveInfinity = dec.AdjustDbl((double) number);
                        }
                    }
                }
                if (dec.sign >= 0)
                {
                    return positiveInfinity;
                }
                return -positiveInfinity;
            }

            public int Exponent
            {
                get => 
                    this.exponent;
                set
                {
                    this.exponent = value;
                }
            }

            public byte this[int ib] =>
                this.mantissa[ib];

            public byte[] Mantissa =>
                this.mantissa;

            public int MantissaSize
            {
                get => 
                    this.mantissaSize;
                set
                {
                    this.mantissaSize = value;
                }
            }

            public int Sign
            {
                get => 
                    this.sign;
                set
                {
                    this.sign = value;
                }
            }
        }
    }
}

