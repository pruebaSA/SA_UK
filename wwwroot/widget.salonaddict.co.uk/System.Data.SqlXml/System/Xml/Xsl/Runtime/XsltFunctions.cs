namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XsltFunctions
    {
        private static readonly CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;

        public static string BaseUri(XPathNavigator navigator) => 
            navigator.BaseURI;

        public static bool Contains(string s1, string s2) => 
            (compareInfo.IndexOf(s1, s2, CompareOptions.Ordinal) >= 0);

        public static string EXslObjectType(IList<XPathItem> value)
        {
            if (value.Count != 1)
            {
                return "node-set";
            }
            XPathItem item = value[0];
            if (item is RtfNavigator)
            {
                return "RTF";
            }
            if (item.IsNode)
            {
                return "node-set";
            }
            object typedValue = item.TypedValue;
            if (typedValue is string)
            {
                return "string";
            }
            if (typedValue is double)
            {
                return "number";
            }
            if (typedValue is bool)
            {
                return "boolean";
            }
            return "external";
        }

        private static CultureInfo GetCultureInfo(string lang)
        {
            CultureInfo info;
            if (lang.Length == 0)
            {
                return CultureInfo.CurrentCulture;
            }
            try
            {
                info = new CultureInfo(lang);
            }
            catch (ArgumentException)
            {
                throw new XslTransformException("Xslt_InvalidLanguage", new string[] { lang });
            }
            return info;
        }

        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        private static extern int GetDateFormat(int locale, uint dwFlags, ref SystemTime sysTime, string format, StringBuilder sb, int sbSize);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        private static extern int GetTimeFormat(int locale, uint dwFlags, ref SystemTime sysTime, string format, StringBuilder sb, int sbSize);
        public static bool Lang(string value, XPathNavigator context)
        {
            string xmlLang = context.XmlLang;
            if (!xmlLang.StartsWith(value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (xmlLang.Length != value.Length)
            {
                return (xmlLang[value.Length] == '-');
            }
            return true;
        }

        public static string MSFormatDateTime(string dateTime, string format, string lang, bool isDate)
        {
            try
            {
                XsdDateTime time;
                int lCID = GetCultureInfo(lang).LCID;
                if (!XsdDateTime.TryParse(dateTime, XsdDateTimeFlags.XdrTimeNoTz | XsdDateTimeFlags.XdrDateTime | XsdDateTimeFlags.AllXsd, out time))
                {
                    return string.Empty;
                }
                SystemTime sysTime = new SystemTime(time.ToZulu());
                StringBuilder sb = new StringBuilder(format.Length + 0x10);
                if (format.Length == 0)
                {
                    format = null;
                }
                if (isDate)
                {
                    if (GetDateFormat(lCID, 0, ref sysTime, format, sb, sb.Capacity) == 0)
                    {
                        int capacity = GetDateFormat(lCID, 0, ref sysTime, format, sb, 0);
                        if (capacity != 0)
                        {
                            sb = new StringBuilder(capacity);
                            capacity = GetDateFormat(lCID, 0, ref sysTime, format, sb, sb.Capacity);
                        }
                    }
                }
                else if (GetTimeFormat(lCID, 0, ref sysTime, format, sb, sb.Capacity) == 0)
                {
                    int num3 = GetTimeFormat(lCID, 0, ref sysTime, format, sb, 0);
                    if (num3 != 0)
                    {
                        sb = new StringBuilder(num3);
                        num3 = GetTimeFormat(lCID, 0, ref sysTime, format, sb, sb.Capacity);
                    }
                }
                return sb.ToString();
            }
            catch (ArgumentException)
            {
                return string.Empty;
            }
        }

        public static string MSLocalName(string name)
        {
            int num;
            if (ValidateNames.ParseQName(name, 0, out num) != name.Length)
            {
                return string.Empty;
            }
            if (num == 0)
            {
                return name;
            }
            return name.Substring(num + 1);
        }

        public static string MSNamespaceUri(string name, XPathNavigator currentNode)
        {
            int num;
            if (ValidateNames.ParseQName(name, 0, out num) == name.Length)
            {
                string prefix = name.Substring(0, num);
                if (prefix == "xmlns")
                {
                    return string.Empty;
                }
                string str2 = currentNode.LookupNamespace(prefix);
                if (str2 != null)
                {
                    return str2;
                }
                if (prefix == "xml")
                {
                    return "http://www.w3.org/XML/1998/namespace";
                }
            }
            return string.Empty;
        }

        public static double MSNumber(IList<XPathItem> value)
        {
            string str;
            double naN;
            if (value.Count == 0)
            {
                return double.NaN;
            }
            XPathItem item = value[0];
            if (item.IsNode)
            {
                str = item.Value;
            }
            else
            {
                Type valueType = item.ValueType;
                if (valueType == XsltConvert.StringType)
                {
                    str = item.Value;
                }
                else
                {
                    if (valueType == XsltConvert.DoubleType)
                    {
                        return item.ValueAsDouble;
                    }
                    if (!item.ValueAsBoolean)
                    {
                        return 0.0;
                    }
                    return 1.0;
                }
            }
            if (XmlConvert.TryToDouble(str, out naN) != null)
            {
                naN = double.NaN;
            }
            return naN;
        }

        public static double MSStringCompare(string s1, string s2, string lang, string options)
        {
            CultureInfo cultureInfo = GetCultureInfo(lang);
            CompareOptions none = CompareOptions.None;
            bool flag = false;
            for (int i = 0; i < options.Length; i++)
            {
                switch (options[i])
                {
                    case 'i':
                        none = CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreCase;
                        break;

                    case 'u':
                        flag = true;
                        break;

                    default:
                        flag = true;
                        none = CompareOptions.IgnoreCase;
                        break;
                }
            }
            if (flag)
            {
                if (none != CompareOptions.None)
                {
                    throw new XslTransformException("Xslt_InvalidCompareOption", new string[] { options });
                }
                none = CompareOptions.IgnoreCase;
            }
            int num2 = cultureInfo.CompareInfo.Compare(s1, s2, none);
            if (flag && (num2 == 0))
            {
                num2 = -cultureInfo.CompareInfo.Compare(s1, s2, CompareOptions.None);
            }
            return (double) num2;
        }

        public static string MSUtc(string dateTime)
        {
            XsdDateTime time;
            DateTime time2;
            try
            {
                if (!XsdDateTime.TryParse(dateTime, XsdDateTimeFlags.XdrTimeNoTz | XsdDateTimeFlags.XdrDateTime | XsdDateTimeFlags.AllXsd, out time))
                {
                    return string.Empty;
                }
                time2 = time.ToZulu();
            }
            catch (ArgumentException)
            {
                return string.Empty;
            }
            char[] text = "----------T00:00:00.000".ToCharArray();
            switch (time.TypeCode)
            {
                case XmlTypeCode.DateTime:
                    PrintDate(text, time2);
                    PrintTime(text, time2);
                    break;

                case XmlTypeCode.Time:
                    PrintTime(text, time2);
                    break;

                case XmlTypeCode.Date:
                    PrintDate(text, time2);
                    break;

                case XmlTypeCode.GYearMonth:
                    PrintYear(text, time2.Year);
                    ShortToCharArray(text, 5, time2.Month);
                    break;

                case XmlTypeCode.GYear:
                    PrintYear(text, time2.Year);
                    break;

                case XmlTypeCode.GMonthDay:
                    ShortToCharArray(text, 5, time2.Month);
                    ShortToCharArray(text, 8, time2.Day);
                    break;

                case XmlTypeCode.GDay:
                    ShortToCharArray(text, 8, time2.Day);
                    break;

                case XmlTypeCode.GMonth:
                    ShortToCharArray(text, 5, time2.Month);
                    break;
            }
            return new string(text);
        }

        public static string NormalizeSpace(string value)
        {
            XmlCharType instance = XmlCharType.Instance;
            StringBuilder builder = null;
            int startIndex = 0;
            int num3 = 0;
            int num = 0;
            while (num < value.Length)
            {
                if (instance.IsWhiteSpace(value[num]))
                {
                    if (num == startIndex)
                    {
                        startIndex++;
                    }
                    else if ((value[num] != ' ') || (num3 == num))
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder(value.Length);
                        }
                        else
                        {
                            builder.Append(' ');
                        }
                        if (num3 == num)
                        {
                            builder.Append(value, startIndex, (num - startIndex) - 1);
                        }
                        else
                        {
                            builder.Append(value, startIndex, num - startIndex);
                        }
                        startIndex = num + 1;
                    }
                    else
                    {
                        num3 = num + 1;
                    }
                }
                num++;
            }
            if (builder == null)
            {
                if (startIndex == num)
                {
                    return string.Empty;
                }
                if ((startIndex == 0) && (num3 != num))
                {
                    return value;
                }
                builder = new StringBuilder(value.Length);
            }
            else if (num != startIndex)
            {
                builder.Append(' ');
            }
            if (num3 == num)
            {
                builder.Append(value, startIndex, (num - startIndex) - 1);
            }
            else
            {
                builder.Append(value, startIndex, num - startIndex);
            }
            return builder.ToString();
        }

        public static string OuterXml(XPathNavigator navigator)
        {
            RtfNavigator navigator2 = navigator as RtfNavigator;
            if (navigator2 == null)
            {
                return navigator.OuterXml;
            }
            StringBuilder output = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                CheckCharacters = false
            };
            XmlWriter writer = XmlWriter.Create(output, settings);
            navigator2.CopyToWriter(writer);
            writer.Close();
            return output.ToString();
        }

        private static void PrintDate(char[] text, DateTime dt)
        {
            PrintYear(text, dt.Year);
            ShortToCharArray(text, 5, dt.Month);
            ShortToCharArray(text, 8, dt.Day);
        }

        private static void PrintMsec(char[] text, int value)
        {
            if (value != 0)
            {
                text[20] = (char) (((value / 100) % 10) + 0x30);
                text[0x15] = (char) (((value / 10) % 10) + 0x30);
                text[0x16] = (char) (((value / 1) % 10) + 0x30);
            }
        }

        private static void PrintTime(char[] text, DateTime dt)
        {
            ShortToCharArray(text, 11, dt.Hour);
            ShortToCharArray(text, 14, dt.Minute);
            ShortToCharArray(text, 0x11, dt.Second);
            PrintMsec(text, dt.Millisecond);
        }

        private static void PrintYear(char[] text, int value)
        {
            text[0] = (char) (((value / 0x3e8) % 10) + 0x30);
            text[1] = (char) (((value / 100) % 10) + 0x30);
            text[2] = (char) (((value / 10) % 10) + 0x30);
            text[3] = (char) (((value / 1) % 10) + 0x30);
        }

        public static double Round(double value)
        {
            double num = Math.Round(value);
            if ((value - num) != 0.5)
            {
                return num;
            }
            return (num + 1.0);
        }

        private static void ShortToCharArray(char[] text, int start, int value)
        {
            text[start] = (char) ((value / 10) + 0x30);
            text[start + 1] = (char) ((value % 10) + 0x30);
        }

        public static bool StartsWith(string s1, string s2) => 
            ((s1.Length >= s2.Length) && (string.CompareOrdinal(s1, 0, s2, 0, s2.Length) == 0));

        public static string Substring(string value, double startIndex)
        {
            startIndex = Round(startIndex);
            if (startIndex <= 0.0)
            {
                return value;
            }
            if (startIndex <= value.Length)
            {
                return value.Substring(((int) startIndex) - 1);
            }
            return string.Empty;
        }

        public static string Substring(string value, double startIndex, double length)
        {
            startIndex = Round(startIndex) - 1.0;
            if (startIndex >= value.Length)
            {
                return string.Empty;
            }
            double num = startIndex + Round(length);
            startIndex = (startIndex <= 0.0) ? 0.0 : startIndex;
            if (startIndex >= num)
            {
                return string.Empty;
            }
            if (num > value.Length)
            {
                num = value.Length;
            }
            return value.Substring((int) startIndex, (int) (num - startIndex));
        }

        public static string SubstringAfter(string s1, string s2)
        {
            if (s2.Length == 0)
            {
                return s1;
            }
            int num = compareInfo.IndexOf(s1, s2, CompareOptions.Ordinal);
            if (num >= 0)
            {
                return s1.Substring(num + s2.Length);
            }
            return string.Empty;
        }

        public static string SubstringBefore(string s1, string s2)
        {
            if (s2.Length == 0)
            {
                return s2;
            }
            int length = compareInfo.IndexOf(s1, s2, CompareOptions.Ordinal);
            if (length >= 1)
            {
                return s1.Substring(0, length);
            }
            return string.Empty;
        }

        public static XPathItem SystemProperty(XmlQualifiedName name)
        {
            if (name.Namespace == "http://www.w3.org/1999/XSL/Transform")
            {
                switch (name.Name)
                {
                    case "version":
                        return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double), 1.0);

                    case "vendor":
                        return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), "Microsoft");

                    case "vendor-url":
                        return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), "http://www.microsoft.com");
                }
            }
            else if ((name.Namespace == "urn:schemas-microsoft-com:xslt") && (name.Name == "version"))
            {
                return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), typeof(XsltLibrary).Assembly.ImageRuntimeVersion);
            }
            return new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), string.Empty);
        }

        public static string Translate(string arg, string mapString, string transString)
        {
            if (mapString.Length == 0)
            {
                return arg;
            }
            StringBuilder builder = new StringBuilder(arg.Length);
            for (int i = 0; i < arg.Length; i++)
            {
                int index = mapString.IndexOf(arg[i]);
                if (index < 0)
                {
                    builder.Append(arg[i]);
                }
                else if (index < transString.Length)
                {
                    builder.Append(transString[index]);
                }
            }
            return builder.ToString();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SystemTime
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort Year;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Month;
            [MarshalAs(UnmanagedType.U2)]
            public ushort DayOfWeek;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Day;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Hour;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Minute;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Second;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Milliseconds;
            public SystemTime(DateTime dateTime)
            {
                this.Year = (ushort) dateTime.Year;
                this.Month = (ushort) dateTime.Month;
                this.DayOfWeek = (ushort) dateTime.DayOfWeek;
                this.Day = (ushort) dateTime.Day;
                this.Hour = (ushort) dateTime.Hour;
                this.Minute = (ushort) dateTime.Minute;
                this.Second = (ushort) dateTime.Second;
                this.Milliseconds = (ushort) dateTime.Millisecond;
            }
        }
    }
}

