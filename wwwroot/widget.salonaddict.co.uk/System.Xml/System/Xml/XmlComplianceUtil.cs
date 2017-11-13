namespace System.Xml
{
    using System;
    using System.Text;

    internal static class XmlComplianceUtil
    {
        public static string CDataNormalize(string value)
        {
            int length = value.Length;
            if (length <= 0)
            {
                return string.Empty;
            }
            int num2 = 0;
            int startIndex = 0;
            StringBuilder builder = null;
            while (num2 < length)
            {
                char ch = value[num2];
                if ((ch >= ' ') || (((ch != '\t') && (ch != '\n')) && (ch != '\r')))
                {
                    num2++;
                }
                else
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(length);
                    }
                    if (startIndex < num2)
                    {
                        builder.Append(value, startIndex, num2 - startIndex);
                    }
                    builder.Append(' ');
                    if (((ch == '\r') && ((num2 + 1) < length)) && (value[num2 + 1] == '\n'))
                    {
                        num2 += 2;
                    }
                    else
                    {
                        num2++;
                    }
                    startIndex = num2;
                }
            }
            if (builder == null)
            {
                return value;
            }
            if (num2 > startIndex)
            {
                builder.Append(value, startIndex, num2 - startIndex);
            }
            return builder.ToString();
        }

        public static bool IsValidLanguageID(char[] value, int startPos, int length)
        {
            int num = length;
            if (num >= 2)
            {
                bool flag = false;
                int index = startPos;
                XmlCharType instance = XmlCharType.Instance;
                char ch = value[index];
                if (instance.IsLetter(ch))
                {
                    if (instance.IsLetter(value[++index]))
                    {
                        if (num == 2)
                        {
                            return true;
                        }
                        num--;
                        index++;
                    }
                    else if ((('I' != ch) && ('i' != ch)) && (('X' != ch) && ('x' != ch)))
                    {
                        return false;
                    }
                    if (value[index] != '-')
                    {
                        return false;
                    }
                    num -= 2;
                    while (num-- > 0)
                    {
                        ch = value[++index];
                        if (instance.IsLetter(ch))
                        {
                            flag = true;
                        }
                        else
                        {
                            if ((ch == '-') && flag)
                            {
                                flag = false;
                                continue;
                            }
                            return false;
                        }
                    }
                    if (flag)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string NonCDataNormalize(string value)
        {
            int length = value.Length;
            if (length <= 0)
            {
                return string.Empty;
            }
            int startIndex = 0;
            StringBuilder builder = null;
            XmlCharType instance = XmlCharType.Instance;
            while (instance.IsWhiteSpace(value[startIndex]))
            {
                startIndex++;
                if (startIndex == length)
                {
                    return " ";
                }
            }
            int num3 = startIndex;
            while (num3 < length)
            {
                if (!instance.IsWhiteSpace(value[num3]))
                {
                    num3++;
                }
                else
                {
                    int num4 = num3 + 1;
                    while ((num4 < length) && instance.IsWhiteSpace(value[num4]))
                    {
                        num4++;
                    }
                    if (num4 == length)
                    {
                        if (builder == null)
                        {
                            return value.Substring(startIndex, num3 - startIndex);
                        }
                        builder.Append(value, startIndex, num3 - startIndex);
                        return builder.ToString();
                    }
                    if ((num4 > (num3 + 1)) || (value[num3] != ' '))
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder(length);
                        }
                        builder.Append(value, startIndex, num3 - startIndex);
                        builder.Append(' ');
                        startIndex = num4;
                        num3 = num4;
                        continue;
                    }
                    num3++;
                }
            }
            if (builder != null)
            {
                if (startIndex < num3)
                {
                    builder.Append(value, startIndex, num3 - startIndex);
                }
                return builder.ToString();
            }
            if (startIndex > 0)
            {
                return value.Substring(startIndex, length - startIndex);
            }
            return value;
        }

        public static string StripSpaces(string value)
        {
            int length = value.Length;
            if (length <= 0)
            {
                return string.Empty;
            }
            int startIndex = 0;
            StringBuilder builder = null;
            while (value[startIndex] == ' ')
            {
                startIndex++;
                if (startIndex == length)
                {
                    return " ";
                }
            }
            int num3 = startIndex;
            while (num3 < length)
            {
                if (value[num3] == ' ')
                {
                    int num4 = num3 + 1;
                    while ((num4 < length) && (value[num4] == ' '))
                    {
                        num4++;
                    }
                    if (num4 == length)
                    {
                        if (builder == null)
                        {
                            return value.Substring(startIndex, num3 - startIndex);
                        }
                        builder.Append(value, startIndex, num3 - startIndex);
                        return builder.ToString();
                    }
                    if (num4 > (num3 + 1))
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder(length);
                        }
                        builder.Append(value, startIndex, (num3 - startIndex) + 1);
                        startIndex = num4;
                        num3 = num4 - 1;
                    }
                }
                num3++;
            }
            if (builder == null)
            {
                if (startIndex != 0)
                {
                    return value.Substring(startIndex, length - startIndex);
                }
                return value;
            }
            if (num3 > startIndex)
            {
                builder.Append(value, startIndex, num3 - startIndex);
            }
            return builder.ToString();
        }

        public static void StripSpaces(char[] value, int index, ref int len)
        {
            if (len > 0)
            {
                int num = index;
                int num2 = index + len;
                while (value[num] == ' ')
                {
                    num++;
                    if (num == num2)
                    {
                        len = 1;
                        return;
                    }
                }
                int num3 = num - index;
                for (int i = num; i < num2; i++)
                {
                    char ch = value[i];
                    if (ch == ' ')
                    {
                        int num5 = i + 1;
                        while ((num5 < num2) && (value[num5] == ' '))
                        {
                            num5++;
                        }
                        if (num5 == num2)
                        {
                            num3 += num5 - i;
                            break;
                        }
                        if (num5 > (i + 1))
                        {
                            num3 += (num5 - i) - 1;
                            i = num5 - 1;
                        }
                    }
                    value[i - num3] = ch;
                }
                len -= num3;
            }
        }
    }
}

