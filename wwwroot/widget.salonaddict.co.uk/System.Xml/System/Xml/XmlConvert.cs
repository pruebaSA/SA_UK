namespace System.Xml
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Schema;

    public class XmlConvert
    {
        private static Regex c_DecodeCharPattern;
        private static Regex c_EncodeCharPattern;
        private static readonly int c_EncodedCharLength = 7;
        internal static char[] crt = new char[] { '\n', '\r', '\t' };
        private static string[] s_allDateTimeFormats;
        internal const int SurHighEnd = 0xdbff;
        internal const int SurHighStart = 0xd800;
        internal const int SurLowEnd = 0xdfff;
        internal const int SurLowStart = 0xdc00;
        internal const int SurMask = 0xfc00;
        internal static readonly char[] WhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

        private static void CreateAllDateTimeFormats()
        {
            if (s_allDateTimeFormats == null)
            {
                s_allDateTimeFormats = new string[] { 
                    "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzzzzz", "yyyy-MM-ddTHH:mm:ss.FFFFFFF", "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ", "HH:mm:ss.FFFFFFF", "HH:mm:ss.FFFFFFFZ", "HH:mm:ss.FFFFFFFzzzzzz", "yyyy-MM-dd", "yyyy-MM-ddZ", "yyyy-MM-ddzzzzzz", "yyyy-MM", "yyyy-MMZ", "yyyy-MMzzzzzz", "yyyy", "yyyyZ", "yyyyzzzzzz", "--MM-dd",
                    "--MM-ddZ", "--MM-ddzzzzzz", "---dd", "---ddZ", "---ddzzzzzz", "--MM--", "--MM--Z", "--MM--zzzzzz"
                };
            }
        }

        internal static Exception CreateException(string res, ExceptionType exceptionType)
        {
            switch (exceptionType)
            {
                case ExceptionType.ArgumentException:
                    return new ArgumentException(Res.GetString(res));
            }
            return new XmlException(res, string.Empty);
        }

        internal static Exception CreateException(string res, string arg, ExceptionType exceptionType)
        {
            switch (exceptionType)
            {
                case ExceptionType.ArgumentException:
                    return new ArgumentException(Res.GetString(res, new object[] { arg }));
            }
            return new XmlException(res, arg);
        }

        internal static Exception CreateException(string res, string[] args, ExceptionType exceptionType)
        {
            switch (exceptionType)
            {
                case ExceptionType.ArgumentException:
                    return new ArgumentException(Res.GetString(res, args));
            }
            return new XmlException(res, args);
        }

        internal static Exception CreateInvalidCharException(char invChar) => 
            CreateInvalidCharException(invChar, ExceptionType.ArgumentException);

        internal static Exception CreateInvalidCharException(char invChar, ExceptionType exceptionType) => 
            CreateException("Xml_InvalidCharacter", XmlException.BuildCharExceptionStr(invChar), exceptionType);

        internal static Exception CreateInvalidHighSurrogateCharException(char hi) => 
            CreateInvalidHighSurrogateCharException(hi, ExceptionType.ArgumentException);

        internal static Exception CreateInvalidHighSurrogateCharException(char hi, ExceptionType exceptionType)
        {
            uint num = hi;
            return CreateException("Xml_InvalidSurrogateHighChar", num.ToString("X", CultureInfo.InvariantCulture), exceptionType);
        }

        internal static ArgumentException CreateInvalidNameArgumentException(string name, string argumentName)
        {
            if (name != null)
            {
                return new ArgumentException(Res.GetString("Xml_EmptyName"), argumentName);
            }
            return new ArgumentNullException(argumentName);
        }

        internal static Exception CreateInvalidSurrogatePairException(char low, char hi) => 
            CreateInvalidSurrogatePairException(low, hi, ExceptionType.ArgumentException);

        internal static Exception CreateInvalidSurrogatePairException(char low, char hi, ExceptionType exceptionType)
        {
            string[] args = new string[] { ((uint) hi).ToString("X", CultureInfo.InvariantCulture), ((uint) low).ToString("X", CultureInfo.InvariantCulture) };
            return CreateException("Xml_InvalidSurrogatePairWithArgs", args, exceptionType);
        }

        public static string DecodeName(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                return name;
            }
            StringBuilder builder = null;
            int length = name.Length;
            int startIndex = 0;
            int index = name.IndexOf('_');
            IEnumerator enumerator = null;
            if (index < 0)
            {
                return name;
            }
            if (c_DecodeCharPattern == null)
            {
                c_DecodeCharPattern = new Regex("_[Xx]([0-9a-fA-F]{4}|[0-9a-fA-F]{8})_");
            }
            enumerator = c_DecodeCharPattern.Matches(name, index).GetEnumerator();
            int num4 = -1;
            if ((enumerator != null) && enumerator.MoveNext())
            {
                Match current = (Match) enumerator.Current;
                num4 = current.Index;
            }
            for (int i = 0; i < ((length - c_EncodedCharLength) + 1); i++)
            {
                if (i == num4)
                {
                    if (enumerator.MoveNext())
                    {
                        Match match2 = (Match) enumerator.Current;
                        num4 = match2.Index;
                    }
                    if (builder == null)
                    {
                        builder = new StringBuilder(length + 20);
                    }
                    builder.Append(name, startIndex, i - startIndex);
                    if (name[i + 6] != '_')
                    {
                        int num6 = (((((((FromHex(name[i + 2]) * 0x10000000) + (FromHex(name[i + 3]) * 0x1000000)) + (FromHex(name[i + 4]) * 0x100000)) + (FromHex(name[i + 5]) * 0x10000)) + (FromHex(name[i + 6]) * 0x1000)) + (FromHex(name[i + 7]) * 0x100)) + (FromHex(name[i + 8]) * 0x10)) + FromHex(name[i + 9]);
                        if (num6 >= 0x10000)
                        {
                            if (num6 <= 0x10ffff)
                            {
                                startIndex = (i + c_EncodedCharLength) + 4;
                                char ch = (char) (((num6 - 0x10000) / 0x400) + 0xd800);
                                char ch2 = (char) (((num6 - ((ch - 0xd800) * 0x400)) - 0x10000) + 0xdc00);
                                builder.Append(ch);
                                builder.Append(ch2);
                            }
                        }
                        else
                        {
                            startIndex = (i + c_EncodedCharLength) + 4;
                            builder.Append((char) num6);
                        }
                        i += (c_EncodedCharLength - 1) + 4;
                    }
                    else
                    {
                        startIndex = i + c_EncodedCharLength;
                        builder.Append((char) ((((FromHex(name[i + 2]) * 0x1000) + (FromHex(name[i + 3]) * 0x100)) + (FromHex(name[i + 4]) * 0x10)) + FromHex(name[i + 5])));
                        i += c_EncodedCharLength - 1;
                    }
                }
            }
            if (startIndex == 0)
            {
                return name;
            }
            if (startIndex < length)
            {
                builder.Append(name, startIndex, length - startIndex);
            }
            return builder.ToString();
        }

        public static string EncodeLocalName(string name) => 
            EncodeName(name, true, true);

        public static string EncodeName(string name) => 
            EncodeName(name, true, false);

        private static string EncodeName(string name, bool first, bool local)
        {
            if (name == null)
            {
                return name;
            }
            if (name.Length == 0)
            {
                if (!first)
                {
                    throw new XmlException("Xml_InvalidNmToken", name);
                }
                return name;
            }
            StringBuilder builder = null;
            int length = name.Length;
            int startIndex = 0;
            int num3 = 0;
            XmlCharType instance = XmlCharType.Instance;
            int index = name.IndexOf('_');
            IEnumerator enumerator = null;
            if (index >= 0)
            {
                if (c_EncodeCharPattern == null)
                {
                    c_EncodeCharPattern = new Regex("(?<=_)[Xx]([0-9a-fA-F]{4}|[0-9a-fA-F]{8})_");
                }
                enumerator = c_EncodeCharPattern.Matches(name, index).GetEnumerator();
            }
            int num5 = -1;
            if ((enumerator != null) && enumerator.MoveNext())
            {
                Match current = (Match) enumerator.Current;
                num5 = current.Index - 1;
            }
            if (first && ((!instance.IsStartNCNameChar(name[0]) && (local || (!local && (name[0] != ':')))) || (num5 == 0)))
            {
                if (builder == null)
                {
                    builder = new StringBuilder(length + 20);
                }
                builder.Append("_x");
                if ((((length > 1) && (name[0] >= 0xd800)) && ((name[0] <= 0xdbff) && (name[1] >= 0xdc00))) && (name[1] <= 0xdfff))
                {
                    int num6 = name[0];
                    int num7 = name[1];
                    builder.Append(((((num6 - 0xd800) * 0x400) + (num7 - 0xdc00)) + 0x10000).ToString("X8", CultureInfo.InvariantCulture));
                    num3++;
                    startIndex = 2;
                }
                else
                {
                    builder.Append(((int) name[0]).ToString("X4", CultureInfo.InvariantCulture));
                    startIndex = 1;
                }
                builder.Append("_");
                num3++;
                if ((num5 == 0) && enumerator.MoveNext())
                {
                    Match match2 = (Match) enumerator.Current;
                    num5 = match2.Index - 1;
                }
            }
            while (num3 < length)
            {
                if (((local && !instance.IsNCNameChar(name[num3])) || (!local && !instance.IsNameChar(name[num3]))) || (num5 == num3))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(length + 20);
                    }
                    if ((num5 == num3) && enumerator.MoveNext())
                    {
                        Match match3 = (Match) enumerator.Current;
                        num5 = match3.Index - 1;
                    }
                    builder.Append(name, startIndex, num3 - startIndex);
                    builder.Append("_x");
                    if ((((length > (num3 + 1)) && (name[num3] >= 0xd800)) && ((name[num3] <= 0xdbff) && (name[num3 + 1] >= 0xdc00))) && (name[num3 + 1] <= 0xdfff))
                    {
                        int num9 = name[num3];
                        int num10 = name[num3 + 1];
                        builder.Append(((((num9 - 0xd800) * 0x400) + (num10 - 0xdc00)) + 0x10000).ToString("X8", CultureInfo.InvariantCulture));
                        startIndex = num3 + 2;
                        num3++;
                    }
                    else
                    {
                        builder.Append(((int) name[num3]).ToString("X4", CultureInfo.InvariantCulture));
                        startIndex = num3 + 1;
                    }
                    builder.Append("_");
                }
                num3++;
            }
            if (startIndex == 0)
            {
                return name;
            }
            if (startIndex < length)
            {
                builder.Append(name, startIndex, length - startIndex);
            }
            return builder.ToString();
        }

        public static string EncodeNmToken(string name) => 
            EncodeName(name, false, false);

        internal static string EscapeValueForDebuggerDisplay(string value)
        {
            StringBuilder builder = null;
            int num = 0;
            int startIndex = 0;
            while (num < value.Length)
            {
                char ch = value[num];
                if ((ch < ' ') || (ch == '"'))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(value.Length + 4);
                    }
                    if ((num - startIndex) > 0)
                    {
                        builder.Append(value, startIndex, num - startIndex);
                    }
                    startIndex = num + 1;
                    switch (ch)
                    {
                        case '\t':
                            builder.Append(@"\t");
                            goto Label_00AE;

                        case '\n':
                            builder.Append(@"\n");
                            goto Label_00AE;

                        case '\r':
                            builder.Append(@"\r");
                            goto Label_00AE;

                        case '"':
                            builder.Append("\\\"");
                            goto Label_00AE;
                    }
                    builder.Append(ch);
                }
            Label_00AE:
                num++;
            }
            if (builder == null)
            {
                return value;
            }
            if ((num - startIndex) > 0)
            {
                builder.Append(value, startIndex, num - startIndex);
            }
            return builder.ToString();
        }

        internal static byte[] FromBinHexString(string s) => 
            FromBinHexString(s, true);

        internal static byte[] FromBinHexString(string s, bool allowOddCount) => 
            BinHexDecoder.Decode(s.ToCharArray(), allowOddCount);

        private static int FromHex(char digit)
        {
            if (digit > '9')
            {
                return (((digit <= 'F') ? (digit - 'A') : (digit - 'a')) + 10);
            }
            return (digit - '0');
        }

        internal static bool IsNegativeZero(double value) => 
            ((value == 0.0) && (BitConverter.DoubleToInt64Bits(value) == BitConverter.DoubleToInt64Bits(0.0)));

        internal static string[] SplitString(string value) => 
            value.Split(WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);

        internal static bool StrEqual(char[] chars, int strPos1, int strLen1, string str2)
        {
            if (strLen1 != str2.Length)
            {
                return false;
            }
            int num = 0;
            while ((num < strLen1) && (chars[strPos1 + num] == str2[num]))
            {
                num++;
            }
            return (num == strLen1);
        }

        private static DateTime SwitchToLocalTime(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(value.Ticks, DateTimeKind.Local);

                case DateTimeKind.Utc:
                    return value.ToLocalTime();

                case DateTimeKind.Local:
                    return value;
            }
            return value;
        }

        private static DateTime SwitchToUtcTime(DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(value.Ticks, DateTimeKind.Utc);

                case DateTimeKind.Utc:
                    return value;

                case DateTimeKind.Local:
                    return value.ToUniversalTime();
            }
            return value;
        }

        internal static string ToBinHexString(byte[] inArray) => 
            BinHexEncoder.Encode(inArray, 0, inArray.Length);

        public static bool ToBoolean(string s)
        {
            s = TrimString(s);
            if ((s == "1") || (s == "true"))
            {
                return true;
            }
            if ((s != "0") && (s != "false"))
            {
                throw new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Boolean" }));
            }
            return false;
        }

        public static byte ToByte(string s) => 
            byte.Parse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, (IFormatProvider) NumberFormatInfo.InvariantInfo);

        public static char ToChar(string s) => 
            char.Parse(s);

        [Obsolete("Use XmlConvert.ToDateTime() that takes in XmlDateTimeSerializationMode")]
        public static DateTime ToDateTime(string s) => 
            ToDateTime(s, AllDateTimeFormats);

        public static DateTime ToDateTime(string s, string format) => 
            DateTime.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite);

        public static DateTime ToDateTime(string s, string[] formats) => 
            DateTime.ParseExact(s, formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite);

        public static DateTime ToDateTime(string s, XmlDateTimeSerializationMode dateTimeOption)
        {
            XsdDateTime time = new XsdDateTime(s, XsdDateTimeFlags.AllXsd);
            DateTime time2 = (DateTime) time;
            switch (dateTimeOption)
            {
                case XmlDateTimeSerializationMode.Local:
                    return SwitchToLocalTime(time2);

                case XmlDateTimeSerializationMode.Utc:
                    return SwitchToUtcTime(time2);

                case XmlDateTimeSerializationMode.Unspecified:
                    return new DateTime(time2.Ticks, DateTimeKind.Unspecified);

                case XmlDateTimeSerializationMode.RoundtripKind:
                    return time2;
            }
            throw new ArgumentException(Res.GetString("Sch_InvalidDateTimeOption", new object[] { dateTimeOption }));
        }

        public static DateTimeOffset ToDateTimeOffset(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            XsdDateTime time = new XsdDateTime(s, XsdDateTimeFlags.AllXsd);
            return (DateTimeOffset) time;
        }

        public static DateTimeOffset ToDateTimeOffset(string s, string format)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            return DateTimeOffset.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite);
        }

        public static DateTimeOffset ToDateTimeOffset(string s, string[] formats)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            return DateTimeOffset.ParseExact(s, formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite);
        }

        public static decimal ToDecimal(string s) => 
            decimal.Parse(s, NumberStyles.AllowDecimalPoint | NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

        public static double ToDouble(string s)
        {
            s = TrimString(s);
            if (s == "-INF")
            {
                return double.NegativeInfinity;
            }
            if (s == "INF")
            {
                return double.PositiveInfinity;
            }
            double num = double.Parse(s, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            if ((num == 0.0) && (s[0] == '-'))
            {
                return 0.0;
            }
            return num;
        }

        public static Guid ToGuid(string s) => 
            new Guid(s);

        public static short ToInt16(string s) => 
            short.Parse(s, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo);

        public static int ToInt32(string s) => 
            int.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

        public static long ToInt64(string s) => 
            long.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

        internal static decimal ToInteger(string s) => 
            decimal.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

        [CLSCompliant(false)]
        public static sbyte ToSByte(string s) => 
            sbyte.Parse(s, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo);

        public static float ToSingle(string s)
        {
            s = TrimString(s);
            if (s == "-INF")
            {
                return float.NegativeInfinity;
            }
            if (s == "INF")
            {
                return float.PositiveInfinity;
            }
            float num = float.Parse(s, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            if ((num == 0f) && (s[0] == '-'))
            {
                return 0f;
            }
            return num;
        }

        public static string ToString(bool value)
        {
            if (!value)
            {
                return "false";
            }
            return "true";
        }

        public static string ToString(byte value) => 
            value.ToString(null, NumberFormatInfo.InvariantInfo);

        public static string ToString(char value) => 
            value.ToString((IFormatProvider) null);

        [Obsolete("Use XmlConvert.ToString() that takes in XmlDateTimeSerializationMode")]
        public static string ToString(DateTime value) => 
            ToString(value, "yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz");

        public static string ToString(DateTimeOffset value)
        {
            XsdDateTime time = new XsdDateTime(value);
            return time.ToString();
        }

        public static string ToString(decimal value) => 
            value.ToString(null, NumberFormatInfo.InvariantInfo);

        public static string ToString(double value)
        {
            if (double.IsNegativeInfinity(value))
            {
                return "-INF";
            }
            if (double.IsPositiveInfinity(value))
            {
                return "INF";
            }
            if (IsNegativeZero(value))
            {
                return "-0";
            }
            return value.ToString("R", NumberFormatInfo.InvariantInfo);
        }

        public static string ToString(Guid value) => 
            value.ToString();

        public static string ToString(short value) => 
            value.ToString(null, (IFormatProvider) NumberFormatInfo.InvariantInfo);

        public static string ToString(int value) => 
            value.ToString(null, NumberFormatInfo.InvariantInfo);

        public static string ToString(long value) => 
            value.ToString(null, NumberFormatInfo.InvariantInfo);

        [CLSCompliant(false)]
        public static string ToString(sbyte value) => 
            value.ToString(null, (IFormatProvider) NumberFormatInfo.InvariantInfo);

        public static string ToString(float value)
        {
            if (float.IsNegativeInfinity(value))
            {
                return "-INF";
            }
            if (float.IsPositiveInfinity(value))
            {
                return "INF";
            }
            if (IsNegativeZero((double) value))
            {
                return "-0";
            }
            return value.ToString("R", NumberFormatInfo.InvariantInfo);
        }

        public static string ToString(TimeSpan value)
        {
            XsdDuration duration = new XsdDuration(value);
            return duration.ToString();
        }

        [CLSCompliant(false)]
        public static string ToString(ushort value) => 
            value.ToString(null, NumberFormatInfo.InvariantInfo);

        [CLSCompliant(false)]
        public static string ToString(uint value) => 
            value.ToString(null, NumberFormatInfo.InvariantInfo);

        [CLSCompliant(false)]
        public static string ToString(ulong value) => 
            value.ToString(null, NumberFormatInfo.InvariantInfo);

        public static string ToString(DateTime value, string format) => 
            value.ToString(format, DateTimeFormatInfo.InvariantInfo);

        public static string ToString(DateTime value, XmlDateTimeSerializationMode dateTimeOption)
        {
            switch (dateTimeOption)
            {
                case XmlDateTimeSerializationMode.Local:
                    value = SwitchToLocalTime(value);
                    break;

                case XmlDateTimeSerializationMode.Utc:
                    value = SwitchToUtcTime(value);
                    break;

                case XmlDateTimeSerializationMode.Unspecified:
                    value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
                    break;

                case XmlDateTimeSerializationMode.RoundtripKind:
                    break;

                default:
                    throw new ArgumentException(Res.GetString("Sch_InvalidDateTimeOption", new object[] { dateTimeOption }));
            }
            XsdDateTime time = new XsdDateTime(value, XsdDateTimeFlags.DateTime);
            return time.ToString();
        }

        public static string ToString(DateTimeOffset value, string format) => 
            value.ToString(format, DateTimeFormatInfo.InvariantInfo);

        public static TimeSpan ToTimeSpan(string s)
        {
            XsdDuration duration;
            try
            {
                duration = new XsdDuration(s);
            }
            catch (Exception)
            {
                throw new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "TimeSpan" }));
            }
            return duration.ToTimeSpan();
        }

        [CLSCompliant(false)]
        public static ushort ToUInt16(string s) => 
            ushort.Parse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, (IFormatProvider) NumberFormatInfo.InvariantInfo);

        [CLSCompliant(false)]
        public static uint ToUInt32(string s) => 
            uint.Parse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo);

        [CLSCompliant(false)]
        public static ulong ToUInt64(string s) => 
            ulong.Parse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo);

        internal static Uri ToUri(string s)
        {
            Uri uri;
            if ((s != null) && (s.Length > 0))
            {
                s = TrimString(s);
                if ((s.Length == 0) || (s.IndexOf("##", StringComparison.Ordinal) != -1))
                {
                    throw new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Uri" }));
                }
            }
            if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out uri))
            {
                throw new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Uri" }));
            }
            return uri;
        }

        internal static double ToXPathDouble(object o)
        {
            string str = o as string;
            if (str != null)
            {
                double num;
                str = TrimString(str);
                if (((str.Length != 0) && (str[0] != '+')) && double.TryParse(str, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite, (IFormatProvider) NumberFormatInfo.InvariantInfo, out num))
                {
                    return num;
                }
                return double.NaN;
            }
            if (o is double)
            {
                return (double) o;
            }
            if (o is bool)
            {
                if (!((bool) o))
                {
                    return 0.0;
                }
                return 1.0;
            }
            try
            {
                return Convert.ToDouble(o, NumberFormatInfo.InvariantInfo);
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            return double.NaN;
        }

        internal static string ToXPathString(object value)
        {
            string str = value as string;
            if (str != null)
            {
                return str;
            }
            if (value is double)
            {
                double num = (double) value;
                return num.ToString("R", NumberFormatInfo.InvariantInfo);
            }
            if (!(value is bool))
            {
                return Convert.ToString(value, NumberFormatInfo.InvariantInfo);
            }
            if (!((bool) value))
            {
                return "false";
            }
            return "true";
        }

        internal static string TrimString(string value) => 
            value.Trim(WhitespaceChars);

        internal static Exception TryToBoolean(string s, out bool result)
        {
            s = TrimString(s);
            if ((s == "0") || (s == "false"))
            {
                result = false;
                return null;
            }
            if ((s == "1") || (s == "true"))
            {
                result = true;
                return null;
            }
            result = false;
            return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Boolean" }));
        }

        internal static Exception TryToByte(string s, out byte result)
        {
            if (!byte.TryParse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Byte" }));
            }
            return null;
        }

        internal static Exception TryToChar(string s, out char result)
        {
            if (!char.TryParse(s, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Char" }));
            }
            return null;
        }

        internal static Exception TryToDecimal(string s, out decimal result)
        {
            if (!decimal.TryParse(s, NumberStyles.AllowDecimalPoint | NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Decimal" }));
            }
            return null;
        }

        internal static Exception TryToDouble(string s, out double result)
        {
            s = TrimString(s);
            if (s == "-INF")
            {
                result = double.NegativeInfinity;
                return null;
            }
            if (s == "INF")
            {
                result = double.PositiveInfinity;
                return null;
            }
            if (!double.TryParse(s, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Double" }));
            }
            if ((result == 0.0) && (s[0] == '-'))
            {
                result = 0.0;
            }
            return null;
        }

        internal static Exception TryToGuid(string s, out Guid result)
        {
            Exception exception = null;
            result = Guid.Empty;
            try
            {
                result = new Guid(s);
            }
            catch (ArgumentException)
            {
                exception = new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Guid" }));
            }
            catch (FormatException)
            {
                exception = new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Guid" }));
            }
            return exception;
        }

        internal static Exception TryToInt16(string s, out short result)
        {
            if (!short.TryParse(s, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Int16" }));
            }
            return null;
        }

        internal static Exception TryToInt32(string s, out int result)
        {
            if (!int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Int32" }));
            }
            return null;
        }

        internal static Exception TryToInt64(string s, out long result)
        {
            if (!long.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Int64" }));
            }
            return null;
        }

        internal static Exception TryToInteger(string s, out decimal result)
        {
            if (!decimal.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Integer" }));
            }
            return null;
        }

        internal static Exception TryToSByte(string s, out sbyte result)
        {
            if (!sbyte.TryParse(s, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "SByte" }));
            }
            return null;
        }

        internal static Exception TryToSingle(string s, out float result)
        {
            s = TrimString(s);
            if (s == "-INF")
            {
                result = float.NegativeInfinity;
                return null;
            }
            if (s == "INF")
            {
                result = float.PositiveInfinity;
                return null;
            }
            if (!float.TryParse(s, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Single" }));
            }
            if ((result == 0f) && (s[0] == '-'))
            {
                result = 0f;
            }
            return null;
        }

        internal static Exception TryToTimeSpan(string s, out TimeSpan result)
        {
            XsdDuration duration;
            Exception exception = XsdDuration.TryParse(s, out duration);
            if (exception != null)
            {
                result = TimeSpan.MinValue;
                return exception;
            }
            return duration.TryToTimeSpan(out result);
        }

        internal static Exception TryToUInt16(string s, out ushort result)
        {
            if (!ushort.TryParse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "UInt16" }));
            }
            return null;
        }

        internal static Exception TryToUInt32(string s, out uint result)
        {
            if (!uint.TryParse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "UInt32" }));
            }
            return null;
        }

        internal static Exception TryToUInt64(string s, out ulong result)
        {
            if (!ulong.TryParse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, NumberFormatInfo.InvariantInfo, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "UInt64" }));
            }
            return null;
        }

        internal static Exception TryToUri(string s, out Uri result)
        {
            result = null;
            if ((s != null) && (s.Length > 0))
            {
                s = TrimString(s);
                if ((s.Length == 0) || (s.IndexOf("##", StringComparison.Ordinal) != -1))
                {
                    return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Uri" }));
                }
            }
            if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out result))
            {
                return new FormatException(Res.GetString("XmlConvert_BadFormat", new object[] { s, "Uri" }));
            }
            return null;
        }

        internal static Exception TryVerifyName(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                return new XmlException("Xml_EmptyName", string.Empty);
            }
            XmlCharType instance = XmlCharType.Instance;
            char ch = name[0];
            if (!instance.IsStartNameChar(ch) && (ch != ':'))
            {
                return new XmlException("Xml_BadStartNameChar", XmlException.BuildCharExceptionStr(ch));
            }
            for (int i = 1; i < name.Length; i++)
            {
                ch = name[i];
                if (!instance.IsNameChar(ch) && (ch != ':'))
                {
                    return new XmlException("Xml_BadNameChar", XmlException.BuildCharExceptionStr(ch));
                }
            }
            return null;
        }

        internal static Exception TryVerifyNCName(string name)
        {
            int offsetBadChar = ValidateNames.ParseNCName(name, 0);
            if ((offsetBadChar != 0) && (offsetBadChar == name.Length))
            {
                return null;
            }
            return ValidateNames.GetInvalidNameException(name, 0, offsetBadChar);
        }

        internal static Exception TryVerifyNMTOKEN(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                return new XmlException("Xml_EmptyName", string.Empty);
            }
            XmlCharType instance = XmlCharType.Instance;
            for (int i = 0; i < name.Length; i++)
            {
                if (!instance.IsNameChar(name[i]))
                {
                    return new XmlException("Xml_BadNameChar", XmlException.BuildCharExceptionStr(name[i]));
                }
            }
            return null;
        }

        internal static Exception TryVerifyNormalizedString(string str)
        {
            if (str.IndexOfAny(crt) != -1)
            {
                return new XmlSchemaException("Sch_NotNormalizedString", str);
            }
            return null;
        }

        internal static Exception TryVerifyTOKEN(string token)
        {
            if ((token == null) || (token.Length == 0))
            {
                return null;
            }
            if (((token[0] != ' ') && (token[token.Length - 1] != ' ')) && ((token.IndexOfAny(crt) == -1) && (token.IndexOf("  ", StringComparison.Ordinal) == -1)))
            {
                return null;
            }
            return new XmlException("Sch_NotTokenString", token);
        }

        internal static unsafe void VerifyCharData(string data, ExceptionType exceptionType)
        {
            if ((data == null) || (data.Length == 0))
            {
                return;
            }
            XmlCharType instance = XmlCharType.Instance;
            int num = 0;
            int length = data.Length;
            while (true)
            {
                while ((num < length) && ((instance.charProperties[data[num]] & 0x10) != 0))
                {
                    num++;
                }
                if (num == length)
                {
                    return;
                }
                char ch = data[num];
                if ((ch < 0xd800) || (ch > 0xdbff))
                {
                    throw CreateInvalidCharException(data[num]);
                }
                if ((num + 1) == length)
                {
                    throw CreateException("Xml_InvalidSurrogateMissingLowChar", exceptionType);
                }
                ch = data[num + 1];
                if ((ch < 0xdc00) || (ch > 0xdfff))
                {
                    throw CreateInvalidSurrogatePairException(data[num + 1], data[num], exceptionType);
                }
                num += 2;
            }
        }

        internal static unsafe void VerifyCharData(char[] data, int offset, int len, ExceptionType exceptionType)
        {
            if ((data == null) || (len == 0))
            {
                return;
            }
            XmlCharType instance = XmlCharType.Instance;
            int index = offset;
            int num2 = offset + len;
            while (true)
            {
                while ((index < num2) && ((instance.charProperties[data[index]] & 0x10) != 0))
                {
                    index++;
                }
                if (index == num2)
                {
                    return;
                }
                char ch = data[index];
                if ((ch < 0xd800) || (ch > 0xdbff))
                {
                    throw CreateInvalidCharException(data[index]);
                }
                if ((index + 1) == num2)
                {
                    throw CreateException("Xml_InvalidSurrogateMissingLowChar", exceptionType);
                }
                ch = data[index + 1];
                if ((ch < 0xdc00) || (ch > 0xdfff))
                {
                    throw CreateInvalidSurrogatePairException(data[index + 1], data[index], exceptionType);
                }
                index += 2;
            }
        }

        public static unsafe string VerifyName(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                throw new ArgumentNullException("name");
            }
            XmlCharType instance = XmlCharType.Instance;
            char index = name[0];
            if (((instance.charProperties[index] & 4) == 0) && (index != ':'))
            {
                throw new XmlException("Xml_BadStartNameChar", XmlException.BuildCharExceptionStr(index));
            }
            for (int i = 1; i < name.Length; i++)
            {
                if (((instance.charProperties[name[i]] & 8) == 0) && (name[i] != ':'))
                {
                    throw new XmlException("Xml_BadNameChar", XmlException.BuildCharExceptionStr(name[i]));
                }
            }
            return name;
        }

        public static string VerifyNCName(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                throw new ArgumentNullException("name");
            }
            return ValidateNames.ParseNCNameThrow(name);
        }

        public static string VerifyNMTOKEN(string name) => 
            VerifyNMTOKEN(name, ExceptionType.XmlException);

        internal static string VerifyNMTOKEN(string name, ExceptionType exceptionType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                throw CreateException("Xml_InvalidNmToken", name, exceptionType);
            }
            XmlCharType instance = XmlCharType.Instance;
            for (int i = 0; i < name.Length; i++)
            {
                if (!instance.IsNameChar(name[i]))
                {
                    throw CreateException("Xml_BadNameChar", XmlException.BuildCharExceptionStr(name[i]), exceptionType);
                }
            }
            return name;
        }

        internal static string VerifyNormalizedString(string str)
        {
            if (str.IndexOfAny(crt) != -1)
            {
                throw new XmlSchemaException("Sch_NotNormalizedString", str);
            }
            return str;
        }

        internal static string VerifyQName(string name) => 
            VerifyQName(name, ExceptionType.XmlException);

        internal static unsafe string VerifyQName(string name, ExceptionType exceptionType)
        {
            if ((name == null) || (name.Length == 0))
            {
                throw new ArgumentException("name");
            }
            XmlCharType instance = XmlCharType.Instance;
            int length = name.Length;
            int num2 = 0;
            int num3 = -1;
        Label_0027:
            if ((instance.charProperties[name[num2]] & 4) != 0)
            {
                num2++;
                while ((num2 < length) && ((instance.charProperties[name[num2]] & 8) != 0))
                {
                    num2++;
                }
                if (num2 == length)
                {
                    return name;
                }
                if (((name[num2] == ':') && (num3 == -1)) && ((num2 + 1) < length))
                {
                    num3 = num2;
                    num2++;
                    goto Label_0027;
                }
            }
            throw CreateException("Xml_BadNameChar", XmlException.BuildCharExceptionStr(name[num2]), exceptionType);
        }

        public static string VerifyTOKEN(string token)
        {
            if (((token != null) && (token.Length != 0)) && (((token[0] == ' ') || (token[token.Length - 1] == ' ')) || ((token.IndexOfAny(crt) != -1) || (token.IndexOf("  ", StringComparison.Ordinal) != -1))))
            {
                throw new XmlException("Sch_NotTokenString", token);
            }
            return token;
        }

        internal static double XPathRound(double value)
        {
            double num = Math.Round(value);
            if ((value - num) != 0.5)
            {
                return num;
            }
            return (num + 1.0);
        }

        private static string[] AllDateTimeFormats
        {
            get
            {
                if (s_allDateTimeFormats == null)
                {
                    CreateAllDateTimeFormats();
                }
                return s_allDateTimeFormats;
            }
        }
    }
}

