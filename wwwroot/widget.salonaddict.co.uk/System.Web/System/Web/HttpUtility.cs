namespace System.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public sealed class HttpUtility
    {
        private static char[] s_entityEndingChars = new char[] { ';', '&' };

        internal static string AspCompatUrlEncode(string s)
        {
            s = UrlEncode(s);
            s = s.Replace("!", "%21");
            s = s.Replace("*", "%2A");
            s = s.Replace("(", "%28");
            s = s.Replace(")", "%29");
            s = s.Replace("-", "%2D");
            s = s.Replace(".", "%2E");
            s = s.Replace("_", "%5F");
            s = s.Replace(@"\", "%5C");
            return s;
        }

        internal static string CollapsePercentUFromStringInternal(string s, Encoding e)
        {
            int length = s.Length;
            UrlDecoder decoder = new UrlDecoder(length, e);
            if (s.IndexOf("%u", StringComparison.Ordinal) == -1)
            {
                return s;
            }
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                if (((ch == '%') && (i < (length - 5))) && (s[i + 1] == 'u'))
                {
                    int num4 = HexToInt(s[i + 2]);
                    int num5 = HexToInt(s[i + 3]);
                    int num6 = HexToInt(s[i + 4]);
                    int num7 = HexToInt(s[i + 5]);
                    if (((num4 >= 0) && (num5 >= 0)) && ((num6 >= 0) && (num7 >= 0)))
                    {
                        ch = (char) ((((num4 << 12) | (num5 << 8)) | (num6 << 4)) | num7);
                        i += 5;
                        decoder.AddChar(ch);
                        continue;
                    }
                }
                if ((ch & 0xff80) == 0)
                {
                    decoder.AddByte((byte) ch);
                }
                else
                {
                    decoder.AddChar(ch);
                }
            }
            return decoder.GetString();
        }

        internal static string FormatHttpCookieDateTime(DateTime dt)
        {
            if ((dt < DateTime.MaxValue.AddDays(-1.0)) && (dt > DateTime.MinValue.AddDays(1.0)))
            {
                dt = dt.ToUniversalTime();
            }
            return dt.ToString("ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'", DateTimeFormatInfo.InvariantInfo);
        }

        internal static string FormatHttpDateTime(DateTime dt)
        {
            if ((dt < DateTime.MaxValue.AddDays(-1.0)) && (dt > DateTime.MinValue.AddDays(1.0)))
            {
                dt = dt.ToUniversalTime();
            }
            return dt.ToString("R", DateTimeFormatInfo.InvariantInfo);
        }

        internal static string FormatHttpDateTimeUtc(DateTime dt) => 
            dt.ToString("R", DateTimeFormatInfo.InvariantInfo);

        internal static string FormatPlainTextAsHtml(string s)
        {
            if (s == null)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            StringWriter output = new StringWriter(sb);
            FormatPlainTextAsHtml(s, output);
            return sb.ToString();
        }

        internal static void FormatPlainTextAsHtml(string s, TextWriter output)
        {
            if (s != null)
            {
                int length = s.Length;
                char ch = '\0';
                for (int i = 0; i < length; i++)
                {
                    char ch2 = s[i];
                    switch (ch2)
                    {
                        case '\n':
                            output.Write("<br>");
                            goto Label_0113;

                        case '\r':
                            goto Label_0113;

                        case ' ':
                            if (ch != ' ')
                            {
                                break;
                            }
                            output.Write("&nbsp;");
                            goto Label_0113;

                        case '"':
                            output.Write("&quot;");
                            goto Label_0113;

                        case '&':
                            output.Write("&amp;");
                            goto Label_0113;

                        case '<':
                            output.Write("&lt;");
                            goto Label_0113;

                        case '>':
                            output.Write("&gt;");
                            goto Label_0113;

                        default:
                            if ((ch2 >= '\x00a0') && (ch2 < 'Ā'))
                            {
                                output.Write("&#");
                                output.Write(((int) ch2).ToString(NumberFormatInfo.InvariantInfo));
                                output.Write(';');
                            }
                            else
                            {
                                output.Write(ch2);
                            }
                            goto Label_0113;
                    }
                    output.Write(ch2);
                Label_0113:
                    ch = ch2;
                }
            }
        }

        internal static string FormatPlainTextSpacesAsHtml(string s)
        {
            if (s == null)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                if (ch == ' ')
                {
                    writer.Write("&nbsp;");
                }
                else
                {
                    writer.Write(ch);
                }
            }
            return sb.ToString();
        }

        private static int HexToInt(char h)
        {
            if ((h >= '0') && (h <= '9'))
            {
                return (h - '0');
            }
            if ((h >= 'a') && (h <= 'f'))
            {
                return ((h - 'a') + 10);
            }
            if ((h >= 'A') && (h <= 'F'))
            {
                return ((h - 'A') + 10);
            }
            return -1;
        }

        public static string HtmlAttributeEncode(string s)
        {
            if (s == null)
            {
                return null;
            }
            int num = IndexOfHtmlAttributeEncodingChars(s, 0);
            if (num == -1)
            {
                return s;
            }
            StringBuilder builder = new StringBuilder(s.Length + 5);
            int length = s.Length;
            int startIndex = 0;
        Label_002A:
            if (num > startIndex)
            {
                builder.Append(s, startIndex, num - startIndex);
            }
            switch (s[num])
            {
                case '"':
                    builder.Append("&quot;");
                    break;

                case '&':
                    builder.Append("&amp;");
                    break;

                case '<':
                    builder.Append("&lt;");
                    break;
            }
            startIndex = num + 1;
            if (startIndex < length)
            {
                num = IndexOfHtmlAttributeEncodingChars(s, startIndex);
                if (num != -1)
                {
                    goto Label_002A;
                }
                builder.Append(s, startIndex, length - startIndex);
            }
            return builder.ToString();
        }

        public static unsafe void HtmlAttributeEncode(string s, TextWriter output)
        {
            if (s != null)
            {
                int num = IndexOfHtmlAttributeEncodingChars(s, 0);
                if (num == -1)
                {
                    output.Write(s);
                }
                else
                {
                    int num2 = s.Length - num;
                    fixed (char* str = ((char*) s))
                    {
                        char* chPtr2 = str;
                        while (num-- > 0)
                        {
                            chPtr2++;
                            output.Write(chPtr2[0]);
                        }
                        while (num2-- > 0)
                        {
                            chPtr2++;
                            char ch = chPtr2[0];
                            if (ch > '<')
                            {
                                goto Label_00A2;
                            }
                            char ch2 = ch;
                            if (ch2 != '"')
                            {
                                if (ch2 == '&')
                                {
                                    goto Label_008B;
                                }
                                if (ch2 != '<')
                                {
                                    goto Label_0098;
                                }
                                output.Write("&lt;");
                            }
                            else
                            {
                                output.Write("&quot;");
                            }
                            continue;
                        Label_008B:
                            output.Write("&amp;");
                            continue;
                        Label_0098:
                            output.Write(ch);
                            continue;
                        Label_00A2:
                            output.Write(ch);
                        }
                    }
                }
            }
        }

        internal static void HtmlAttributeEncodeInternal(string s, HttpWriter writer)
        {
            int num = IndexOfHtmlAttributeEncodingChars(s, 0);
            if (num == -1)
            {
                writer.Write(s);
                return;
            }
            int length = s.Length;
            int index = 0;
        Label_001D:
            if (num > index)
            {
                writer.WriteString(s, index, num - index);
            }
            switch (s[num])
            {
                case '"':
                    writer.Write("&quot;");
                    break;

                case '&':
                    writer.Write("&amp;");
                    break;

                case '<':
                    writer.Write("&lt;");
                    break;
            }
            index = num + 1;
            if (index < length)
            {
                num = IndexOfHtmlAttributeEncodingChars(s, index);
                if (num != -1)
                {
                    goto Label_001D;
                }
                writer.WriteString(s, index, length - index);
            }
        }

        public static string HtmlDecode(string s)
        {
            if (s == null)
            {
                return null;
            }
            if (s.IndexOf('&') < 0)
            {
                return s;
            }
            StringBuilder sb = new StringBuilder();
            StringWriter output = new StringWriter(sb);
            HtmlDecode(s, output);
            return sb.ToString();
        }

        public static void HtmlDecode(string s, TextWriter output)
        {
            if (s != null)
            {
                if (s.IndexOf('&') < 0)
                {
                    output.Write(s);
                }
                else
                {
                    int length = s.Length;
                    for (int i = 0; i < length; i++)
                    {
                        char ch = s[i];
                        if (ch == '&')
                        {
                            int num3 = s.IndexOfAny(s_entityEndingChars, i + 1);
                            if ((num3 > 0) && (s[num3] == ';'))
                            {
                                string entity = s.Substring(i + 1, (num3 - i) - 1);
                                if ((entity.Length > 1) && (entity[0] == '#'))
                                {
                                    try
                                    {
                                        if ((entity[1] == 'x') || (entity[1] == 'X'))
                                        {
                                            ch = (char) int.Parse(entity.Substring(2), NumberStyles.AllowHexSpecifier);
                                        }
                                        else
                                        {
                                            ch = (char) int.Parse(entity.Substring(1));
                                        }
                                        i = num3;
                                    }
                                    catch (FormatException)
                                    {
                                        i++;
                                    }
                                    catch (ArgumentException)
                                    {
                                        i++;
                                    }
                                }
                                else
                                {
                                    i = num3;
                                    char ch2 = HtmlEntities.Lookup(entity);
                                    if (ch2 != '\0')
                                    {
                                        ch = ch2;
                                    }
                                    else
                                    {
                                        output.Write('&');
                                        output.Write(entity);
                                        output.Write(';');
                                        continue;
                                    }
                                }
                            }
                        }
                        output.Write(ch);
                    }
                }
            }
        }

        public static string HtmlEncode(string s)
        {
            if (s == null)
            {
                return null;
            }
            int num = IndexOfHtmlEncodingChars(s, 0);
            if (num == -1)
            {
                return s;
            }
            StringBuilder builder = new StringBuilder(s.Length + 5);
            int length = s.Length;
            int startIndex = 0;
        Label_002A:
            if (num > startIndex)
            {
                builder.Append(s, startIndex, num - startIndex);
            }
            char ch = s[num];
            if (ch > '>')
            {
                builder.Append("&#");
                builder.Append(((int) ch).ToString(NumberFormatInfo.InvariantInfo));
                builder.Append(';');
            }
            else
            {
                char ch2 = ch;
                if (ch2 != '"')
                {
                    switch (ch2)
                    {
                        case '<':
                            builder.Append("&lt;");
                            break;

                        case '>':
                            builder.Append("&gt;");
                            break;

                        case '&':
                            builder.Append("&amp;");
                            break;
                    }
                }
                else
                {
                    builder.Append("&quot;");
                }
            }
            startIndex = num + 1;
            if (startIndex < length)
            {
                num = IndexOfHtmlEncodingChars(s, startIndex);
                if (num != -1)
                {
                    goto Label_002A;
                }
                builder.Append(s, startIndex, length - startIndex);
            }
            return builder.ToString();
        }

        public static unsafe void HtmlEncode(string s, TextWriter output)
        {
            if (s != null)
            {
                int num = IndexOfHtmlEncodingChars(s, 0);
                if (num == -1)
                {
                    output.Write(s);
                }
                else
                {
                    int num2 = s.Length - num;
                    fixed (char* str = ((char*) s))
                    {
                        char* chPtr2 = str;
                        while (num-- > 0)
                        {
                            chPtr2++;
                            output.Write(chPtr2[0]);
                        }
                        while (num2-- > 0)
                        {
                            chPtr2++;
                            char ch = chPtr2[0];
                            if (ch > '>')
                            {
                                goto Label_00C4;
                            }
                            char ch2 = ch;
                            if (ch2 != '"')
                            {
                                switch (ch2)
                                {
                                    case '<':
                                    {
                                        output.Write("&lt;");
                                        continue;
                                    }
                                    case '>':
                                    {
                                        output.Write("&gt;");
                                        continue;
                                    }
                                    case '&':
                                        goto Label_00AD;
                                }
                                goto Label_00BA;
                            }
                            output.Write("&quot;");
                            continue;
                        Label_00AD:
                            output.Write("&amp;");
                            continue;
                        Label_00BA:
                            output.Write(ch);
                            continue;
                        Label_00C4:
                            if ((ch >= '\x00a0') && (ch < 'Ā'))
                            {
                                output.Write("&#");
                                output.Write(((int) ch).ToString(NumberFormatInfo.InvariantInfo));
                                output.Write(';');
                            }
                            else
                            {
                                output.Write(ch);
                            }
                        }
                    }
                }
            }
        }

        private static unsafe int IndexOfHtmlAttributeEncodingChars(string s, int startPos)
        {
            int num = s.Length - startPos;
            fixed (char* str = ((char*) s))
            {
                char* chPtr = str;
                char* chPtr2 = chPtr + startPos;
                while (num > 0)
                {
                    char ch = chPtr2[0];
                    if (ch <= '<')
                    {
                        switch (ch)
                        {
                            case '"':
                            case '&':
                            case '<':
                                return (s.Length - num);
                        }
                    }
                    chPtr2++;
                    num--;
                }
            }
            return -1;
        }

        private static unsafe int IndexOfHtmlEncodingChars(string s, int startPos)
        {
            int num = s.Length - startPos;
            fixed (char* str = ((char*) s))
            {
                char* chPtr = str;
                char* chPtr2 = chPtr + startPos;
                while (num > 0)
                {
                    char ch = chPtr2[0];
                    if (ch <= '>')
                    {
                        switch (ch)
                        {
                            case '<':
                            case '>':
                            case '"':
                            case '&':
                                return (s.Length - num);
                        }
                    }
                    else if ((ch >= '\x00a0') && (ch < 'Ā'))
                    {
                        return (s.Length - num);
                    }
                    chPtr2++;
                    num--;
                }
            }
            return -1;
        }

        internal static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char) (n + 0x30);
            }
            return (char) ((n - 10) + 0x61);
        }

        private static bool IsNonAsciiByte(byte b)
        {
            if (b < 0x7f)
            {
                return (b < 0x20);
            }
            return true;
        }

        internal static bool IsSafe(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }

        public static NameValueCollection ParseQueryString(string query) => 
            ParseQueryString(query, Encoding.UTF8);

        public static NameValueCollection ParseQueryString(string query, Encoding encoding)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            if ((query.Length > 0) && (query[0] == '?'))
            {
                query = query.Substring(1);
            }
            return new HttpValueCollection(query, false, true, encoding);
        }

        public static string UrlDecode(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlDecode(str, Encoding.UTF8);
        }

        public static string UrlDecode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return UrlDecodeStringFromStringInternal(str, e);
        }

        public static string UrlDecode(byte[] bytes, Encoding e)
        {
            if (bytes == null)
            {
                return null;
            }
            return UrlDecode(bytes, 0, bytes.Length, e);
        }

        public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e)
        {
            if ((bytes == null) && (count == 0))
            {
                return null;
            }
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if ((offset < 0) || (offset > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || ((offset + count) > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return UrlDecodeStringFromBytesInternal(bytes, offset, count, e);
        }

        private static byte[] UrlDecodeBytesFromBytesInternal(byte[] buf, int offset, int count)
        {
            int length = 0;
            byte[] sourceArray = new byte[count];
            for (int i = 0; i < count; i++)
            {
                int index = offset + i;
                byte num4 = buf[index];
                if (num4 == 0x2b)
                {
                    num4 = 0x20;
                }
                else if ((num4 == 0x25) && (i < (count - 2)))
                {
                    int num5 = HexToInt((char) buf[index + 1]);
                    int num6 = HexToInt((char) buf[index + 2]);
                    if ((num5 >= 0) && (num6 >= 0))
                    {
                        num4 = (byte) ((num5 << 4) | num6);
                        i += 2;
                    }
                }
                sourceArray[length++] = num4;
            }
            if (length < sourceArray.Length)
            {
                byte[] destinationArray = new byte[length];
                Array.Copy(sourceArray, destinationArray, length);
                sourceArray = destinationArray;
            }
            return sourceArray;
        }

        private static string UrlDecodeStringFromBytesInternal(byte[] buf, int offset, int count, Encoding e)
        {
            UrlDecoder decoder = new UrlDecoder(count, e);
            for (int i = 0; i < count; i++)
            {
                int index = offset + i;
                byte b = buf[index];
                if (b == 0x2b)
                {
                    b = 0x20;
                }
                else if ((b == 0x25) && (i < (count - 2)))
                {
                    if ((buf[index + 1] == 0x75) && (i < (count - 5)))
                    {
                        int num4 = HexToInt((char) buf[index + 2]);
                        int num5 = HexToInt((char) buf[index + 3]);
                        int num6 = HexToInt((char) buf[index + 4]);
                        int num7 = HexToInt((char) buf[index + 5]);
                        if (((num4 < 0) || (num5 < 0)) || ((num6 < 0) || (num7 < 0)))
                        {
                            goto Label_00DA;
                        }
                        char ch = (char) ((((num4 << 12) | (num5 << 8)) | (num6 << 4)) | num7);
                        i += 5;
                        decoder.AddChar(ch);
                        continue;
                    }
                    int num8 = HexToInt((char) buf[index + 1]);
                    int num9 = HexToInt((char) buf[index + 2]);
                    if ((num8 >= 0) && (num9 >= 0))
                    {
                        b = (byte) ((num8 << 4) | num9);
                        i += 2;
                    }
                }
            Label_00DA:
                decoder.AddByte(b);
            }
            return decoder.GetString();
        }

        private static string UrlDecodeStringFromStringInternal(string s, Encoding e)
        {
            int length = s.Length;
            UrlDecoder decoder = new UrlDecoder(length, e);
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                if (ch == '+')
                {
                    ch = ' ';
                }
                else if ((ch == '%') && (i < (length - 2)))
                {
                    if ((s[i + 1] == 'u') && (i < (length - 5)))
                    {
                        int num3 = HexToInt(s[i + 2]);
                        int num4 = HexToInt(s[i + 3]);
                        int num5 = HexToInt(s[i + 4]);
                        int num6 = HexToInt(s[i + 5]);
                        if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0)))
                        {
                            goto Label_0106;
                        }
                        ch = (char) ((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                        i += 5;
                        decoder.AddChar(ch);
                        continue;
                    }
                    int num7 = HexToInt(s[i + 1]);
                    int num8 = HexToInt(s[i + 2]);
                    if ((num7 >= 0) && (num8 >= 0))
                    {
                        byte b = (byte) ((num7 << 4) | num8);
                        i += 2;
                        decoder.AddByte(b);
                        continue;
                    }
                }
            Label_0106:
                if ((ch & 0xff80) == 0)
                {
                    decoder.AddByte((byte) ch);
                }
                else
                {
                    decoder.AddChar(ch);
                }
            }
            return decoder.GetString();
        }

        public static byte[] UrlDecodeToBytes(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlDecodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlDecodeToBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return UrlDecodeToBytes(bytes, 0, (bytes != null) ? bytes.Length : 0);
        }

        public static byte[] UrlDecodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return UrlDecodeToBytes(e.GetBytes(str));
        }

        public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
        {
            if ((bytes == null) && (count == 0))
            {
                return null;
            }
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if ((offset < 0) || (offset > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || ((offset + count) > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return UrlDecodeBytesFromBytesInternal(bytes, offset, count);
        }

        public static string UrlEncode(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncode(str, Encoding.UTF8);
        }

        public static string UrlEncode(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes));
        }

        public static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        public static string UrlEncode(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, offset, count));
        }

        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char ch = (char) bytes[offset + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsSafe(ch))
                {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0))
            {
                return bytes;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++)
            {
                byte num6 = bytes[offset + j];
                char ch2 = (char) num6;
                if (IsSafe(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte) IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte) IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        private static byte[] UrlEncodeBytesToBytesInternalNonAscii(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int num = 0;
            for (int i = 0; i < count; i++)
            {
                if (IsNonAsciiByte(bytes[offset + i]))
                {
                    num++;
                }
            }
            if (!alwaysCreateReturnValue && (num == 0))
            {
                return bytes;
            }
            byte[] buffer = new byte[count + (num * 2)];
            int num3 = 0;
            for (int j = 0; j < count; j++)
            {
                byte b = bytes[offset + j];
                if (IsNonAsciiByte(b))
                {
                    buffer[num3++] = 0x25;
                    buffer[num3++] = (byte) IntToHex((b >> 4) & 15);
                    buffer[num3++] = (byte) IntToHex(b & 15);
                }
                else
                {
                    buffer[num3++] = b;
                }
            }
            return buffer;
        }

        internal static string UrlEncodeNonAscii(string str, Encoding e)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            if (e == null)
            {
                e = Encoding.UTF8;
            }
            byte[] bytes = e.GetBytes(str);
            bytes = UrlEncodeBytesToBytesInternalNonAscii(bytes, 0, bytes.Length, false);
            return Encoding.ASCII.GetString(bytes);
        }

        internal static string UrlEncodeSpaces(string str)
        {
            if ((str != null) && (str.IndexOf(' ') >= 0))
            {
                str = str.Replace(" ", "%20");
            }
            return str;
        }

        public static byte[] UrlEncodeToBytes(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if ((bytes == null) && (count == 0))
            {
                return null;
            }
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if ((offset < 0) || (offset > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || ((offset + count) > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return UrlEncodeBytesToBytesInternal(bytes, offset, count, true);
        }

        public static string UrlEncodeUnicode(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncodeUnicodeStringToStringInternal(str, false);
        }

        private static string UrlEncodeUnicodeStringToStringInternal(string s, bool ignoreAscii)
        {
            int length = s.Length;
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                if ((ch & 0xff80) == 0)
                {
                    if (ignoreAscii || IsSafe(ch))
                    {
                        builder.Append(ch);
                    }
                    else if (ch == ' ')
                    {
                        builder.Append('+');
                    }
                    else
                    {
                        builder.Append('%');
                        builder.Append(IntToHex((ch >> 4) & '\x000f'));
                        builder.Append(IntToHex(ch & '\x000f'));
                    }
                }
                else
                {
                    builder.Append("%u");
                    builder.Append(IntToHex((ch >> 12) & '\x000f'));
                    builder.Append(IntToHex((ch >> 8) & '\x000f'));
                    builder.Append(IntToHex((ch >> 4) & '\x000f'));
                    builder.Append(IntToHex(ch & '\x000f'));
                }
            }
            return builder.ToString();
        }

        public static byte[] UrlEncodeUnicodeToBytes(string str)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.ASCII.GetBytes(UrlEncodeUnicode(str));
        }

        public static string UrlPathEncode(string str)
        {
            if (str == null)
            {
                return null;
            }
            int index = str.IndexOf('?');
            if (index >= 0)
            {
                return (UrlPathEncode(str.Substring(0, index)) + str.Substring(index));
            }
            return UrlEncodeSpaces(UrlEncodeNonAscii(str, Encoding.UTF8));
        }

        private class UrlDecoder
        {
            private int _bufferSize;
            private byte[] _byteBuffer;
            private char[] _charBuffer;
            private Encoding _encoding;
            private int _numBytes;
            private int _numChars;

            internal UrlDecoder(int bufferSize, Encoding encoding)
            {
                this._bufferSize = bufferSize;
                this._encoding = encoding;
                this._charBuffer = new char[bufferSize];
            }

            internal void AddByte(byte b)
            {
                if (this._byteBuffer == null)
                {
                    this._byteBuffer = new byte[this._bufferSize];
                }
                this._byteBuffer[this._numBytes++] = b;
            }

            internal void AddChar(char ch)
            {
                if (this._numBytes > 0)
                {
                    this.FlushBytes();
                }
                this._charBuffer[this._numChars++] = ch;
            }

            private void FlushBytes()
            {
                if (this._numBytes > 0)
                {
                    this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
                    this._numBytes = 0;
                }
            }

            internal string GetString()
            {
                if (this._numBytes > 0)
                {
                    this.FlushBytes();
                }
                if (this._numChars > 0)
                {
                    return new string(this._charBuffer, 0, this._numChars);
                }
                return string.Empty;
            }
        }
    }
}

