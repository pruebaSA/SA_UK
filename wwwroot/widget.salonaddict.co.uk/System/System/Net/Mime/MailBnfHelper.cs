namespace System.Net.Mime
{
    using System;
    using System.Globalization;
    using System.Net.Mail;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class MailBnfHelper
    {
        private static bool[] s_atext = new bool[0x80];
        private static string[] s_days;
        private static bool[] s_digits = new bool[0x80];
        private static bool[] s_dtext = new bool[0x80];
        private static bool[] s_fdtext = new bool[0x80];
        private static bool[] s_fqtext = new bool[0x80];
        private static bool[] s_ftext = new bool[0x80];
        private static string[] s_months;
        private static bool[] s_qtext = new bool[0x80];
        private static bool[] s_ttext = new bool[0x80];

        static MailBnfHelper()
        {
            string[] strArray = new string[13];
            strArray[1] = "Jan";
            strArray[2] = "Feb";
            strArray[3] = "Mar";
            strArray[4] = "Apr";
            strArray[5] = "May";
            strArray[6] = "Jun";
            strArray[7] = "Jul";
            strArray[8] = "Aug";
            strArray[9] = "Sep";
            strArray[10] = "Oct";
            strArray[11] = "Nov";
            strArray[12] = "Dec";
            s_months = strArray;
            s_days = new string[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            for (int i = 0x30; i <= 0x39; i++)
            {
                s_atext[i] = true;
            }
            for (int j = 0x41; j <= 90; j++)
            {
                s_atext[j] = true;
            }
            for (int k = 0x61; k <= 0x7a; k++)
            {
                s_atext[k] = true;
            }
            s_atext[0x21] = true;
            s_atext[0x23] = true;
            s_atext[0x24] = true;
            s_atext[0x25] = true;
            s_atext[0x26] = true;
            s_atext[0x27] = true;
            s_atext[0x2a] = true;
            s_atext[0x2b] = true;
            s_atext[0x2d] = true;
            s_atext[0x2f] = true;
            s_atext[0x3d] = true;
            s_atext[0x3f] = true;
            s_atext[0x5e] = true;
            s_atext[0x5f] = true;
            s_atext[0x60] = true;
            s_atext[0x7b] = true;
            s_atext[0x7c] = true;
            s_atext[0x7d] = true;
            s_atext[0x7e] = true;
            for (int m = 1; m <= 8; m++)
            {
                s_qtext[m] = true;
            }
            s_qtext[11] = true;
            s_qtext[12] = true;
            for (int n = 14; n <= 0x1f; n++)
            {
                s_qtext[n] = true;
            }
            s_qtext[0x21] = true;
            for (int num6 = 0x23; num6 <= 0x5b; num6++)
            {
                s_qtext[num6] = true;
            }
            for (int num7 = 0x5d; num7 <= 0x7f; num7++)
            {
                s_qtext[num7] = true;
            }
            for (int num8 = 1; num8 <= 9; num8++)
            {
                s_fqtext[num8] = true;
            }
            s_fqtext[11] = true;
            s_fqtext[12] = true;
            for (int num9 = 14; num9 <= 0x21; num9++)
            {
                s_fqtext[num9] = true;
            }
            for (int num10 = 0x23; num10 <= 0x5b; num10++)
            {
                s_fqtext[num10] = true;
            }
            for (int num11 = 0x5d; num11 <= 0x7f; num11++)
            {
                s_fqtext[num11] = true;
            }
            for (int num12 = 1; num12 <= 8; num12++)
            {
                s_dtext[num12] = true;
            }
            s_dtext[11] = true;
            s_dtext[12] = true;
            for (int num13 = 14; num13 <= 0x1f; num13++)
            {
                s_dtext[num13] = true;
            }
            for (int num14 = 0x21; num14 <= 90; num14++)
            {
                s_dtext[num14] = true;
            }
            for (int num15 = 0x5e; num15 <= 0x7f; num15++)
            {
                s_dtext[num15] = true;
            }
            for (int num16 = 1; num16 <= 9; num16++)
            {
                s_fdtext[num16] = true;
            }
            s_fdtext[11] = true;
            s_fdtext[12] = true;
            for (int num17 = 14; num17 <= 90; num17++)
            {
                s_fdtext[num17] = true;
            }
            for (int num18 = 0x5e; num18 <= 0x7f; num18++)
            {
                s_fdtext[num18] = true;
            }
            for (int num19 = 0x21; num19 <= 0x39; num19++)
            {
                s_ftext[num19] = true;
            }
            for (int num20 = 0x3b; num20 <= 0x7e; num20++)
            {
                s_ftext[num20] = true;
            }
            for (int num21 = 0x21; num21 <= 0x7e; num21++)
            {
                s_ttext[num21] = true;
            }
            s_ttext[40] = false;
            s_ttext[0x29] = false;
            s_ttext[60] = false;
            s_ttext[0x3e] = false;
            s_ttext[0x40] = false;
            s_ttext[0x2c] = false;
            s_ttext[0x3b] = false;
            s_ttext[0x3a] = false;
            s_ttext[0x5c] = false;
            s_ttext[0x22] = false;
            s_ttext[0x2f] = false;
            s_ttext[0x5b] = false;
            s_ttext[0x5d] = false;
            s_ttext[0x3f] = false;
            s_ttext[0x3d] = false;
            for (int num22 = 0x30; num22 <= 0x39; num22++)
            {
                s_digits[num22] = true;
            }
        }

        internal static string GetDateTimeString(DateTime value, StringBuilder builder)
        {
            StringBuilder builder2 = (builder != null) ? builder : new StringBuilder();
            builder2.Append(value.Day);
            builder2.Append(' ');
            builder2.Append(s_months[value.Month]);
            builder2.Append(' ');
            builder2.Append(value.Year);
            builder2.Append(' ');
            if (value.Hour <= 9)
            {
                builder2.Append('0');
            }
            builder2.Append(value.Hour);
            builder2.Append(':');
            if (value.Minute <= 9)
            {
                builder2.Append('0');
            }
            builder2.Append(value.Minute);
            builder2.Append(':');
            if (value.Second <= 9)
            {
                builder2.Append('0');
            }
            builder2.Append(value.Second);
            string str = TimeZone.CurrentTimeZone.GetUtcOffset(value).ToString();
            if (str[0] != '-')
            {
                builder2.Append(" +");
            }
            else
            {
                builder2.Append(" ");
            }
            string[] strArray = str.Split(new char[] { ':' });
            builder2.Append(strArray[0]);
            builder2.Append(strArray[1]);
            if (builder == null)
            {
                return builder2.ToString();
            }
            return null;
        }

        internal static string GetDotAtomOrDomainLiteral(string data, StringBuilder builder)
        {
            int num = 0;
            int startIndex = 0;
            while (num < data.Length)
            {
                if (data[num] > s_atext.Length)
                {
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                }
                if ((data[num] != '.') && !s_atext[data[num]])
                {
                    StringBuilder builder2 = (builder != null) ? builder : new StringBuilder();
                    builder.Append('[');
                    while (num < data.Length)
                    {
                        if (data[num] > s_fdtext.Length)
                        {
                            throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                        }
                        if (!s_fdtext[data[num]])
                        {
                            builder.Append(data, startIndex, num - startIndex);
                            builder.Append('\\');
                            startIndex = num;
                        }
                        num++;
                    }
                    builder.Append(data, startIndex, num - startIndex);
                    builder.Append(']');
                    if (builder == null)
                    {
                        return builder2.ToString();
                    }
                    return null;
                }
                num++;
            }
            if (builder != null)
            {
                builder.Append(data);
                return null;
            }
            return data;
        }

        internal static string GetDotAtomOrQuotedString(string data, StringBuilder builder)
        {
            bool flag = data.StartsWith("\"") && data.EndsWith("\"");
            int num = flag ? 1 : 0;
            int startIndex = 0;
            while (num < data.Length)
            {
                if (data[num] > s_atext.Length)
                {
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                }
                if (((data[num] != '.') && !s_atext[data[num]]) || (data[num] == ' '))
                {
                    StringBuilder builder2 = (builder != null) ? builder : new StringBuilder();
                    if (!flag)
                    {
                        builder.Append('"');
                    }
                    while ((!flag && (num < data.Length)) || (flag && (num < (data.Length - 1))))
                    {
                        if (data[num] > s_fqtext.Length)
                        {
                            throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                        }
                        if (!s_fqtext[data[num]])
                        {
                            builder.Append(data, startIndex, num - startIndex);
                            builder.Append('\\');
                            startIndex = num;
                        }
                        num++;
                    }
                    builder.Append(data, startIndex, num - startIndex);
                    builder.Append('"');
                    if (builder == null)
                    {
                        return builder2.ToString();
                    }
                    return null;
                }
                num++;
            }
            if (builder != null)
            {
                builder.Append(data);
                return null;
            }
            return data;
        }

        internal static string GetTokenOrQuotedString(string data, StringBuilder builder)
        {
            int num = 0;
            int startIndex = 0;
            while (num < data.Length)
            {
                if (data[num] > s_ttext.Length)
                {
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                }
                if (!s_ttext[data[num]] || (data[num] == ' '))
                {
                    StringBuilder builder2 = (builder != null) ? builder : new StringBuilder();
                    builder.Append('"');
                    while (num < data.Length)
                    {
                        if (data[num] > s_fqtext.Length)
                        {
                            throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                        }
                        if (!s_fqtext[data[num]])
                        {
                            builder.Append(data, startIndex, num - startIndex);
                            builder.Append('\\');
                            startIndex = num;
                        }
                        num++;
                    }
                    builder.Append(data, startIndex, num - startIndex);
                    builder.Append('"');
                    if (builder == null)
                    {
                        return builder2.ToString();
                    }
                    return null;
                }
                num++;
            }
            if (data.Length == 0)
            {
                if (builder == null)
                {
                    return "\"\"";
                }
                builder.Append("\"\"");
            }
            if (builder != null)
            {
                builder.Append(data);
                return null;
            }
            return data;
        }

        internal static bool HasCROrLF(string data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if ((data[i] == '\r') || (data[i] == '\n'))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsValidDOW(string data, ref int offset)
        {
            if ((offset + 3) >= data.Length)
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            for (int i = 0; i < s_days.Length; i++)
            {
                if (string.Compare(s_days[i], 0, data, offset, 3, true, CultureInfo.InvariantCulture) == 0)
                {
                    offset += 3;
                    return true;
                }
            }
            return false;
        }

        internal static string ReadAddressSpecDomain(string data, ref int offset, StringBuilder builder)
        {
            if (offset >= data.Length)
            {
                throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
            }
            builder.Append('@');
            SkipCFWS(data, ref offset);
            if (data[offset] == '[')
            {
                ReadDomainLiteral(data, ref offset, builder);
            }
            else
            {
                ReadDotAtom(data, ref offset, builder);
            }
            SkipCFWS(data, ref offset);
            return builder.ToString();
        }

        internal static string ReadAngleAddress(string data, ref int offset, StringBuilder builder)
        {
            if (offset >= data.Length)
            {
                throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
            }
            SkipCFWS(data, ref offset);
            if (data[offset] == '"')
            {
                bool containsUnescapedUnicode = false;
                ReadQuotedString(data, ref offset, builder, true, ref containsUnescapedUnicode);
                ThrowForUnescapedUnicode(containsUnescapedUnicode);
            }
            else
            {
                ReadDotAtom(data, ref offset, builder);
            }
            SkipCFWS(data, ref offset);
            if ((offset >= data.Length) || (data[offset] != '@'))
            {
                throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
            }
            offset++;
            SkipCFWS(data, ref offset);
            string str = ReadAddressSpecDomain(data, ref offset, builder);
            if (!SkipCFWS(data, ref offset) || (data[offset++] != '>'))
            {
                throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
            }
            return str;
        }

        internal static string ReadAtom(string data, ref int offset, StringBuilder builder)
        {
            string str;
            int startIndex = offset;
            while (offset < data.Length)
            {
                if (data[offset] > s_atext.Length)
                {
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                }
                if (!s_atext[data[offset]])
                {
                    if (offset == startIndex)
                    {
                        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                    }
                    str = data.Substring(startIndex, offset - startIndex);
                    if (builder != null)
                    {
                        builder.Append(str);
                        return null;
                    }
                    return str;
                }
                offset++;
            }
            if (offset == startIndex)
            {
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
            }
            str = (startIndex == 0) ? data : data.Substring(startIndex);
            if (builder != null)
            {
                builder.Append(str);
                return null;
            }
            return str;
        }

        private static int ReadDateNumber(string data, ref int offset, int maxSize)
        {
            int num = 0;
            int num2 = offset + maxSize;
            if (offset >= data.Length)
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            while ((offset < data.Length) && (offset < num2))
            {
                if ((data[offset] < '0') || (data[offset] > '9'))
                {
                    break;
                }
                num = (num * 10) + (data[offset] - '0');
                offset++;
            }
            if (num == 0)
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            return num;
        }

        internal static DateTime ReadDateTime(string data, ref int offset)
        {
            if (!SkipCFWS(data, ref offset))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            if (IsValidDOW(data, ref offset))
            {
                if ((offset >= data.Length) || (data[offset] != ','))
                {
                    throw new FormatException(SR.GetString("MailDateInvalidFormat"));
                }
                offset++;
            }
            if (!SkipFWS(data, ref offset))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            int day = ReadDateNumber(data, ref offset, 2);
            if ((offset >= data.Length) || ((data[offset] != ' ') && (data[offset] != '\t')))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            if (!SkipFWS(data, ref offset))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            int month = ReadMonth(data, ref offset);
            if ((offset >= data.Length) || ((data[offset] != ' ') && (data[offset] != '\t')))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            if (!SkipFWS(data, ref offset))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            int year = ReadDateNumber(data, ref offset, 4);
            if ((offset >= data.Length) || ((data[offset] != ' ') && (data[offset] != '\t')))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            if (!SkipFWS(data, ref offset))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            int hour = ReadDateNumber(data, ref offset, 2);
            if ((offset >= data.Length) || (data[offset] != ':'))
            {
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
            }
            offset++;
            int minute = ReadDateNumber(data, ref offset, 2);
            int second = 0;
            if ((offset < data.Length) && (data[offset] == ':'))
            {
                offset++;
                second = ReadDateNumber(data, ref offset, 2);
            }
            if (!SkipFWS(data, ref offset))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            if ((offset >= data.Length) || ((data[offset] != '-') && (data[offset] != '+')))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            offset++;
            ReadDateNumber(data, ref offset, 4);
            return new DateTime(year, month, day, hour, minute, second);
        }

        internal static string ReadDomainLiteral(string data, ref int offset, StringBuilder builder)
        {
            int startIndex = ++offset;
            StringBuilder builder2 = (builder != null) ? builder : new StringBuilder();
            while (offset < data.Length)
            {
                if (data[offset] == '\\')
                {
                    builder2.Append(data, startIndex, offset - startIndex);
                    startIndex = ++offset;
                }
                else
                {
                    if (data[offset] == ']')
                    {
                        builder2.Append(data, startIndex, offset - startIndex);
                        offset++;
                        if (builder == null)
                        {
                            return builder2.ToString();
                        }
                        return null;
                    }
                    if ((data[offset] > s_fdtext.Length) || !s_fdtext[data[offset]])
                    {
                        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                    }
                }
                offset++;
            }
            throw new FormatException(SR.GetString("MailHeaderFieldMalformedHeader"));
        }

        internal static string ReadDotAtom(string data, ref int offset, StringBuilder builder)
        {
            bool flag = true;
            if (builder == null)
            {
                flag = false;
                builder = new StringBuilder();
            }
            if (data[offset] != '.')
            {
                ReadAtom(data, ref offset, builder);
            }
            while ((offset < data.Length) && (data[offset] == '.'))
            {
                builder.Append(data[offset++]);
                ReadAtom(data, ref offset, builder);
            }
            if (flag)
            {
                return null;
            }
            return builder.ToString();
        }

        internal static MailAddress ReadMailAddress(string data, ref int offset)
        {
            string encodedDisplayName = null;
            return new MailAddress(ReadMailAddress(data, ref offset, out encodedDisplayName), encodedDisplayName, 0);
        }

        internal static string ReadMailAddress(string data, ref int offset, out string displayName)
        {
            string str = null;
            Exception exception = null;
            displayName = string.Empty;
            StringBuilder builder = new StringBuilder();
            try
            {
                SkipCFWS(data, ref offset);
                if (offset >= data.Length)
                {
                    exception = new FormatException(SR.GetString("MailAddressInvalidFormat"));
                    goto Label_0281;
                }
                if (data[offset] == '<')
                {
                    offset++;
                    return ReadAngleAddress(data, ref offset, builder);
                }
                bool containsUnescapedUnicode = false;
                ReadPhrase(data, ref offset, builder, ref containsUnescapedUnicode);
                if (offset >= data.Length)
                {
                    exception = new FormatException(SR.GetString("MailAddressInvalidFormat"));
                    goto Label_0281;
                }
                char ch = data[offset];
                if (ch <= '.')
                {
                    switch (ch)
                    {
                        case '"':
                            goto Label_017D;

                        case '.':
                            goto Label_00E4;
                    }
                    goto Label_0222;
                }
                switch (ch)
                {
                    case ':':
                        exception = new FormatException(SR.GetString("MailAddressUnsupportedFormat"));
                        goto Label_0281;

                    case ';':
                        goto Label_0222;

                    case '<':
                        displayName = builder.ToString();
                        builder = new StringBuilder();
                        offset++;
                        str = ReadAngleAddress(data, ref offset, builder);
                        goto Label_0234;

                    default:
                        if (ch != '@')
                        {
                            goto Label_0222;
                        }
                        ThrowForUnescapedUnicode(containsUnescapedUnicode);
                        offset++;
                        str = ReadAddressSpecDomain(data, ref offset, builder);
                        goto Label_0234;
                }
            Label_00E4:
                ReadDotAtom(data, ref offset, builder);
                SkipCFWS(data, ref offset);
                if (offset >= data.Length)
                {
                    exception = new FormatException(SR.GetString("MailAddressInvalidFormat"));
                }
                else
                {
                    if (data[offset] == '@')
                    {
                        ThrowForUnescapedUnicode(containsUnescapedUnicode);
                        offset++;
                        str = ReadAddressSpecDomain(data, ref offset, builder);
                        goto Label_0234;
                    }
                    if (data[offset] == '<')
                    {
                        displayName = builder.ToString();
                        builder = new StringBuilder();
                        offset++;
                        str = ReadAngleAddress(data, ref offset, builder);
                        goto Label_0234;
                    }
                    exception = new FormatException(SR.GetString("MailAddressInvalidFormat"));
                }
                goto Label_0281;
            Label_017D:
                offset++;
                if (offset >= data.Length)
                {
                    exception = new FormatException(SR.GetString("MailAddressInvalidFormat"));
                    goto Label_0281;
                }
                SkipCFWS(data, ref offset);
                if (offset >= data.Length)
                {
                    exception = new FormatException(SR.GetString("MailAddressInvalidFormat"));
                    goto Label_0281;
                }
                if (data[offset] == '<')
                {
                    offset++;
                    str = ReadAngleAddress(data, ref offset, builder);
                }
                else
                {
                    str = ReadAddressSpecDomain(data, ref offset, builder);
                }
                goto Label_0234;
            Label_0222:
                exception = new FormatException(SR.GetString("MailAddressInvalidFormat"));
                goto Label_0281;
            Label_0234:
                if (offset < data.Length)
                {
                    SkipCFWS(data, ref offset);
                    if ((offset < data.Length) && (data[offset] != ','))
                    {
                        exception = new FormatException(SR.GetString("MailAddressInvalidFormat"));
                        goto Label_0281;
                    }
                }
            }
            catch (FormatException)
            {
                throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
            }
        Label_0281:
            if (exception != null)
            {
                throw exception;
            }
            return str;
        }

        private static int ReadMonth(string data, ref int offset)
        {
            if (offset >= (data.Length - 3))
            {
                throw new FormatException(SR.GetString("MailDateInvalidFormat"));
            }
            switch (data[offset++])
            {
                case 'J':
                case 'j':
                    switch (data[offset++])
                    {
                        case 'a':
                        case 'A':
                        {
                            char ch3 = data[offset++];
                            if ((ch3 != 'N') && (ch3 != 'n'))
                            {
                                break;
                            }
                            return 1;
                        }
                        case 'u':
                        case 'U':
                            switch (data[offset++])
                            {
                                case 'L':
                                case 'l':
                                    return 7;

                                case 'N':
                                case 'n':
                                    return 6;
                            }
                            break;
                    }
                    break;

                case 'M':
                case 'm':
                {
                    char ch7 = data[offset++];
                    if ((ch7 != 'A') && (ch7 != 'a'))
                    {
                        break;
                    }
                    char ch8 = data[offset++];
                    if (ch8 <= 'Y')
                    {
                        switch (ch8)
                        {
                            case 'R':
                                goto Label_021D;

                            case 'Y':
                                goto Label_021B;
                        }
                        break;
                    }
                    if (ch8 == 'r')
                    {
                        goto Label_021D;
                    }
                    if (ch8 != 'y')
                    {
                        break;
                    }
                    goto Label_021B;
                }
                case 'N':
                case 'n':
                {
                    char ch16 = data[offset++];
                    if ((ch16 != 'O') && (ch16 != 'o'))
                    {
                        break;
                    }
                    char ch17 = data[offset++];
                    if ((ch17 != 'V') && (ch17 != 'v'))
                    {
                        break;
                    }
                    return 11;
                }
                case 'O':
                case 'o':
                {
                    char ch14 = data[offset++];
                    if ((ch14 != 'C') && (ch14 != 'c'))
                    {
                        break;
                    }
                    char ch15 = data[offset++];
                    if ((ch15 != 'T') && (ch15 != 't'))
                    {
                        break;
                    }
                    return 10;
                }
                case 'S':
                case 's':
                {
                    char ch12 = data[offset++];
                    if ((ch12 != 'E') && (ch12 != 'e'))
                    {
                        break;
                    }
                    char ch13 = data[offset++];
                    if ((ch13 != 'P') && (ch13 != 'p'))
                    {
                        break;
                    }
                    return 9;
                }
                case 'D':
                case 'd':
                    switch (data[offset++])
                    {
                        case 'E':
                        case 'e':
                        {
                            char ch19 = data[offset++];
                            if ((ch19 == 'C') || (ch19 == 'c'))
                            {
                                return 12;
                            }
                            break;
                        }
                    }
                    break;

                case 'F':
                case 'f':
                {
                    char ch5 = data[offset++];
                    if ((ch5 != 'E') && (ch5 != 'e'))
                    {
                        break;
                    }
                    char ch6 = data[offset++];
                    if ((ch6 != 'B') && (ch6 != 'b'))
                    {
                        break;
                    }
                    return 2;
                }
                case 'A':
                case 'a':
                    switch (data[offset++])
                    {
                        case 'p':
                        case 'P':
                        {
                            char ch10 = data[offset++];
                            if ((ch10 != 'R') && (ch10 != 'r'))
                            {
                                break;
                            }
                            return 4;
                        }
                        case 'u':
                        case 'U':
                        {
                            char ch11 = data[offset++];
                            if ((ch11 != 'G') && (ch11 != 'g'))
                            {
                                break;
                            }
                            return 8;
                        }
                    }
                    break;
            }
            throw new FormatException(SR.GetString("MailDateInvalidFormat"));
        Label_021B:
            return 5;
        Label_021D:
            return 3;
        }

        internal static string ReadParameterAttribute(string data, ref int offset, StringBuilder builder)
        {
            if (!SkipCFWS(data, ref offset))
            {
                return null;
            }
            return ReadToken(data, ref offset, null);
        }

        internal static string ReadPhrase(string data, ref int offset, StringBuilder builder, ref bool containsUnescapedUnicode)
        {
            StringBuilder builder2 = (builder != null) ? builder : new StringBuilder();
            bool flag = false;
            SkipCFWS(data, ref offset);
            int num = offset;
            if (SkipCFWS(data, ref offset) && (data[offset] == '"'))
            {
                string str = ReadQuotedString(data, ref offset, null, true, ref containsUnescapedUnicode);
                if (!SkipCFWS(data, ref offset) || (((data[offset] != '"') && !s_atext[data[offset]]) && (data[offset] != '.')))
                {
                    if (builder != null)
                    {
                        builder.Append(str);
                        return null;
                    }
                    return str;
                }
                offset = num;
            }
            while (SkipCFWS(data, ref offset))
            {
                if (data[offset] == '"')
                {
                    if (flag)
                    {
                        builder2.Append(' ');
                    }
                    ReadQuotedString(data, ref offset, builder2, false, ref containsUnescapedUnicode);
                    flag = true;
                }
                else
                {
                    if (!s_atext[data[offset]])
                    {
                        break;
                    }
                    if (flag)
                    {
                        builder2.Append(' ');
                    }
                    ReadAtom(data, ref offset, builder2);
                    flag = true;
                }
            }
            if (num == offset)
            {
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
            }
            if (builder == null)
            {
                return builder2.ToString();
            }
            return null;
        }

        internal static string ReadQuotedString(string data, ref int offset, StringBuilder builder)
        {
            bool containsUnescapedUnicode = false;
            string str = ReadQuotedString(data, ref offset, builder, false, ref containsUnescapedUnicode);
            ThrowForUnescapedUnicode(containsUnescapedUnicode);
            return str;
        }

        internal static string ReadQuotedString(string data, ref int offset, StringBuilder builder, bool returnQuotes, ref bool containsUnescapedUnicode)
        {
            if (data[offset] != '"')
            {
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
            }
            offset++;
            int startIndex = offset;
            StringBuilder builder2 = (builder != null) ? builder : new StringBuilder();
            if (returnQuotes)
            {
                builder2.Append('"');
            }
            while (offset < data.Length)
            {
                if (data[offset] == '\\')
                {
                    builder2.Append(data, startIndex, offset - startIndex);
                    startIndex = ++offset;
                }
                else
                {
                    if (data[offset] == '"')
                    {
                        builder2.Append(data, startIndex, offset - startIndex);
                        offset++;
                        if (returnQuotes)
                        {
                            builder2.Append('"');
                        }
                        if (builder == null)
                        {
                            return builder2.ToString();
                        }
                        return null;
                    }
                    if (data[offset] >= s_fqtext.Length)
                    {
                        containsUnescapedUnicode = true;
                    }
                    else if (!s_fqtext[data[offset]])
                    {
                        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                    }
                }
                offset++;
            }
            throw new FormatException(SR.GetString("MailHeaderFieldMalformedHeader"));
        }

        internal static string ReadToken(string data, ref int offset, StringBuilder builder)
        {
            int startIndex = offset;
            while (offset < data.Length)
            {
                if (data[offset] > s_ttext.Length)
                {
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                }
                if (!s_ttext[data[offset]])
                {
                    break;
                }
                offset++;
            }
            if (startIndex == offset)
            {
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
            }
            return data.Substring(startIndex, offset - startIndex);
        }

        internal static string ReadUnQuotedString(string data, ref int offset, StringBuilder builder)
        {
            int startIndex = offset;
            StringBuilder builder2 = (builder != null) ? builder : new StringBuilder();
            while (offset < data.Length)
            {
                if (data[offset] == '\\')
                {
                    builder2.Append(data, startIndex, offset - startIndex);
                    startIndex = ++offset;
                }
                else
                {
                    if (data[offset] == '"')
                    {
                        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                    }
                    if (!s_fqtext[data[offset]])
                    {
                        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                    }
                }
                offset++;
            }
            builder2.Append(data, startIndex, offset - startIndex);
            if (builder == null)
            {
                return builder2.ToString();
            }
            return null;
        }

        internal static bool SkipCFWS(string data, ref int offset)
        {
            int num = 0;
            while (offset < data.Length)
            {
                if (data[offset] > '\x007f')
                {
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                }
                if ((data[offset] == '\\') && (num > 0))
                {
                    offset += 2;
                }
                else if (data[offset] == '(')
                {
                    num++;
                }
                else if (data[offset] == ')')
                {
                    num--;
                }
                else if (((data[offset] != ' ') && (data[offset] != '\t')) && (num == 0))
                {
                    return true;
                }
                if (num < 0)
                {
                    throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
                }
                offset++;
            }
            return false;
        }

        internal static bool SkipFWS(string data, ref int offset)
        {
            while (offset < data.Length)
            {
                if ((data[offset] != ' ') && (data[offset] != '\t'))
                {
                    return true;
                }
                offset++;
            }
            return false;
        }

        internal static void ThrowForUnescapedUnicode(bool unescapedUnicode)
        {
            if (unescapedUnicode)
            {
                throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter"));
            }
        }

        internal static void ValidateHeaderName(string data)
        {
            int num = 0;
            while (num < data.Length)
            {
                if ((data[num] > s_ftext.Length) || !s_ftext[data[num]])
                {
                    throw new FormatException(SR.GetString("InvalidHeaderName"));
                }
                num++;
            }
            if (num == 0)
            {
                throw new FormatException(SR.GetString("InvalidHeaderName"));
            }
        }
    }
}

