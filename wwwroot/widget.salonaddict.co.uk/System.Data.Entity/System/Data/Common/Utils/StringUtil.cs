namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal static class StringUtil
    {
        private const string s_defaultDelimiter = ", ";

        internal static string BuildDelimitedList<T>(IEnumerable<T> values, ToStringConverter<T> converter, string delimiter)
        {
            if (values == null)
            {
                return string.Empty;
            }
            if (converter == null)
            {
                converter = new ToStringConverter<T>(StringUtil.InvariantConvertToString<T>);
            }
            if (delimiter == null)
            {
                delimiter = ", ";
            }
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (T local in values)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(delimiter);
                }
                builder.Append(converter(local));
            }
            return builder.ToString();
        }

        internal static string FormatInvariant(string format, params object[] args) => 
            string.Format(CultureInfo.InvariantCulture, format, args);

        internal static StringBuilder FormatStringBuilder(StringBuilder builder, string format, params object[] args)
        {
            builder.AppendFormat(CultureInfo.InvariantCulture, format, args);
            return builder;
        }

        internal static StringBuilder IndentNewLine(StringBuilder builder, int indent)
        {
            builder.AppendLine();
            for (int i = 0; i < indent; i++)
            {
                builder.Append("    ");
            }
            return builder;
        }

        private static string InvariantConvertToString<T>(T value) => 
            string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { value });

        internal static bool IsNullOrEmptyOrWhiteSpace(string value) => 
            IsNullOrEmptyOrWhiteSpace(value, 0);

        internal static bool IsNullOrEmptyOrWhiteSpace(string value, int offset)
        {
            if (value != null)
            {
                for (int i = offset; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool IsNullOrEmptyOrWhiteSpace(string value, int offset, int length)
        {
            if (value != null)
            {
                length = Math.Min(value.Length, length);
                for (int i = offset; i < length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static string ToCommaSeparatedString(IEnumerable list) => 
            ToSeparatedString(list, ", ", string.Empty);

        internal static void ToCommaSeparatedString(StringBuilder builder, IEnumerable list)
        {
            ToSeparatedStringPrivate(builder, list, ", ", string.Empty, false);
        }

        internal static string ToCommaSeparatedStringSorted(IEnumerable list) => 
            ToSeparatedStringSorted(list, ", ", string.Empty);

        internal static void ToCommaSeparatedStringSorted(StringBuilder builder, IEnumerable list)
        {
            ToSeparatedStringPrivate(builder, list, ", ", string.Empty, true);
        }

        internal static string ToSeparatedString(IEnumerable list, string separator, string nullValue)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ToSeparatedString(stringBuilder, list, separator, nullValue);
            return stringBuilder.ToString();
        }

        internal static void ToSeparatedString(StringBuilder builder, IEnumerable list, string separator)
        {
            ToSeparatedStringPrivate(builder, list, separator, string.Empty, false);
        }

        internal static void ToSeparatedString(StringBuilder stringBuilder, IEnumerable list, string separator, string nullValue)
        {
            ToSeparatedStringPrivate(stringBuilder, list, separator, nullValue, false);
        }

        private static void ToSeparatedStringPrivate(StringBuilder stringBuilder, IEnumerable list, string separator, string nullValue, bool toSort)
        {
            if (list != null)
            {
                bool flag = true;
                List<string> list2 = new List<string>();
                foreach (object obj2 in list)
                {
                    string str;
                    if (obj2 == null)
                    {
                        str = nullValue;
                    }
                    else
                    {
                        str = FormatInvariant("{0}", new object[] { obj2 });
                    }
                    list2.Add(str);
                }
                if (toSort)
                {
                    list2.Sort(StringComparer.Ordinal);
                }
                foreach (string str2 in list2)
                {
                    if (!flag)
                    {
                        stringBuilder.Append(separator);
                    }
                    stringBuilder.Append(str2);
                    flag = false;
                }
            }
        }

        internal static string ToSeparatedStringSorted(IEnumerable list, string separator, string nullValue)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ToSeparatedStringPrivate(stringBuilder, list, separator, nullValue, true);
            return stringBuilder.ToString();
        }

        internal static void ToSeparatedStringSorted(StringBuilder builder, IEnumerable list, string separator)
        {
            ToSeparatedStringPrivate(builder, list, separator, string.Empty, true);
        }

        internal delegate string ToStringConverter<T>(T value);
    }
}

