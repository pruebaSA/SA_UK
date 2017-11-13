namespace System
{
    internal class IPv6AddressHelper
    {
        private const string CanonicalNumberFormat = "{0:X4}";
        private const int NumberOfLabels = 8;

        private IPv6AddressHelper()
        {
        }

        private static unsafe string CreateCanonicalName(ushort* numbers) => 
            string.Concat(new object[] { 
                '[', $"{numbers[0]:X4}", ':', $"{numbers[1]:X4}", ':', $"{numbers[2]:X4}", ':', $"{numbers[3]:X4}", ':', $"{numbers[4]:X4}", ':', $"{numbers[5]:X4}", ':', $"{numbers[6]:X4}", ':', $"{numbers[7]:X4}",
                ']'
            });

        internal static unsafe bool IsValid(char* name, int start, ref int end)
        {
            int num = 0;
            int num2 = 0;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = true;
            int num3 = 1;
            int index = start;
            while (index < end)
            {
                if (flag3 ? ((name[index] >= '0') && (name[index] <= '9')) : Uri.IsHexDigit(name[index]))
                {
                    num2++;
                    flag4 = false;
                    goto Label_013A;
                }
                if (num2 <= 4)
                {
                    if (num2 != 0)
                    {
                        num++;
                        num3 = index - num2;
                    }
                    switch (name[index])
                    {
                        case ':':
                            if ((index <= 0) || (name[index - 1] != ':'))
                            {
                                goto Label_00F9;
                            }
                            if (flag)
                            {
                                return false;
                            }
                            flag = true;
                            flag4 = false;
                            goto Label_0138;

                        case ']':
                            goto Label_00D0;

                        case '.':
                            if (!flag2)
                            {
                                goto Label_0114;
                            }
                            return false;

                        case '/':
                            goto Label_00FE;

                        case '%':
                            goto Label_00A9;
                    }
                }
                return false;
            Label_00A9:
                if (++index == end)
                {
                    return false;
                }
                if (name[index] != ']')
                {
                    if (name[index] != '/')
                    {
                        goto Label_00A9;
                    }
                    goto Label_00FE;
                }
            Label_00D0:
                start = index;
                index = end;
                goto Label_013A;
            Label_00F9:
                flag4 = true;
                goto Label_0138;
            Label_00FE:
                if ((num == 0) || flag3)
                {
                    return false;
                }
                flag3 = true;
                flag4 = true;
                goto Label_0138;
            Label_0114:
                index = end;
                if (!IPv4AddressHelper.IsValid(name, num3, ref index, true, false))
                {
                    return false;
                }
                num++;
                flag2 = true;
                index--;
            Label_0138:
                num2 = 0;
            Label_013A:
                index++;
            }
            if (!flag3 || ((num2 >= 1) && (num2 <= 2)))
            {
                int num5 = 8 + (flag3 ? 1 : 0);
                if ((flag4 || (num2 > 4)) || !(flag ? (num < num5) : (num == num5)))
                {
                    return false;
                }
                if (index == (end + 1))
                {
                    end = start + 1;
                    return true;
                }
            }
            return false;
        }

        internal static unsafe bool Parse(string address, ushort* numbers, int start, ref string scopeId)
        {
            int num = 0;
            int num2 = 0;
            int num3 = -1;
            bool flag = true;
            int num4 = 0;
            if (address[start] == '[')
            {
                start++;
            }
            int num5 = start;
            while ((num5 < address.Length) && (address[num5] != ']'))
            {
                switch (address[num5])
                {
                    case '%':
                    {
                        if (flag)
                        {
                            numbers[num2++] = (ushort) num;
                            flag = false;
                        }
                        start = num5;
                        num5++;
                        while ((address[num5] != ']') && (address[num5] != '/'))
                        {
                            num5++;
                        }
                        scopeId = address.Substring(start, num5 - start);
                        while (address[num5] != ']')
                        {
                            num5++;
                        }
                        continue;
                    }
                    case '/':
                    {
                        if (flag)
                        {
                            numbers[num2++] = (ushort) num;
                            flag = false;
                        }
                        num5++;
                        while (address[num5] != ']')
                        {
                            num4 = (num4 * 10) + (address[num5] - '0');
                            num5++;
                        }
                        continue;
                    }
                    case ':':
                    {
                        numbers[num2++] = (ushort) num;
                        num = 0;
                        num5++;
                        if (address[num5] == ':')
                        {
                            num3 = num2;
                            num5++;
                        }
                        else if ((num3 < 0) && (num2 < 6))
                        {
                            continue;
                        }
                        for (int i = num5; (((address[i] != ']') && (address[i] != ':')) && ((address[i] != '%') && (address[i] != '/'))) && (i < (num5 + 4)); i++)
                        {
                            if (address[i] == '.')
                            {
                                while (((address[i] != ']') && (address[i] != '/')) && (address[i] != '%'))
                                {
                                    i++;
                                }
                                num = IPv4AddressHelper.ParseHostNumber(address, num5, i);
                                numbers[num2++] = (ushort) (num >> 0x10);
                                numbers[num2++] = (ushort) num;
                                num5 = i;
                                num = 0;
                                flag = false;
                                break;
                            }
                        }
                        continue;
                    }
                }
                num = (num * 0x10) + Uri.FromHex(address[num5++]);
            }
            if (flag)
            {
                numbers[num2++] = (ushort) num;
            }
            if (num3 > 0)
            {
                int num7 = 7;
                int index = num2 - 1;
                for (int j = num2 - num3; j > 0; j--)
                {
                    numbers[num7--] = numbers[index];
                    numbers[index--] = 0;
                }
            }
            if ((((numbers[0] != 0) || (numbers[1] != 0)) || ((numbers[2] != 0) || (numbers[3] != 0))) || (numbers[4] != 0))
            {
                return false;
            }
            if (((numbers[5] != 0) || (numbers[6] != 0)) || (numbers[7] != 1))
            {
                if ((numbers[6] != 0x7f00) || (numbers[7] != 1))
                {
                    return false;
                }
                if (numbers[5] != 0)
                {
                    return (numbers[5] == 0xffff);
                }
            }
            return true;
        }

        internal static unsafe string ParseCanonicalName(string str, int start, ref bool isLoopback, ref string scopeId)
        {
            ushort* numbers = (ushort*) stackalloc byte[(2 * 8)];
            *((long*) numbers) = 0L;
            *((long*) (numbers + 4)) = 0L;
            isLoopback = Parse(str, numbers, start, ref scopeId);
            return CreateCanonicalName(numbers);
        }
    }
}

