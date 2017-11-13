namespace System.Data.Services.Parsing
{
    using System;
    using System.Data.Services.Client;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    internal static class WebConvert
    {
        private const string HexValues = "0123456789ABCDEF";
        private const string XmlHexEncodePrefix = "0x";

        private static string AppendDecimalMarkerToDouble(string input)
        {
            foreach (char ch in input)
            {
                if (!char.IsDigit(ch))
                {
                    return input;
                }
            }
            return (input + ".0");
        }

        internal static string ConvertByteArrayToKeyString(byte[] byteArray)
        {
            StringBuilder builder = new StringBuilder(3 + (byteArray.Length * 2));
            builder.Append("X");
            builder.Append("'");
            for (int i = 0; i < byteArray.Length; i++)
            {
                builder.Append("0123456789ABCDEF"[byteArray[i] >> 4]);
                builder.Append("0123456789ABCDEF"[byteArray[i] & 15]);
            }
            builder.Append("'");
            return builder.ToString();
        }

        internal static bool IsKeyTypeQuoted(Type type)
        {
            if (type != typeof(XElement))
            {
                return (type == typeof(string));
            }
            return true;
        }

        internal static bool TryKeyPrimitiveToString(object value, out string result)
        {
            if (value.GetType() == typeof(byte[]))
            {
                result = ConvertByteArrayToKeyString((byte[]) value);
            }
            else
            {
                if (!TryXmlPrimitiveToString(value, out result))
                {
                    return false;
                }
                if (value.GetType() == typeof(DateTime))
                {
                    result = "datetime'" + result + "'";
                }
                else if (value.GetType() == typeof(decimal))
                {
                    result = result + "M";
                }
                else if (value.GetType() == typeof(Guid))
                {
                    result = "guid'" + result + "'";
                }
                else if (value.GetType() == typeof(long))
                {
                    result = result + "L";
                }
                else if (value.GetType() == typeof(float))
                {
                    result = result + "f";
                }
                else if (value.GetType() == typeof(double))
                {
                    result = AppendDecimalMarkerToDouble(result);
                }
                else if (IsKeyTypeQuoted(value.GetType()))
                {
                    result = "'" + result.Replace("'", "''") + "'";
                }
            }
            return true;
        }

        internal static bool TryXmlPrimitiveToString(object value, out string result)
        {
            result = null;
            Type nullableType = value.GetType();
            nullableType = Nullable.GetUnderlyingType(nullableType) ?? nullableType;
            if (typeof(string) == nullableType)
            {
                result = (string) value;
            }
            else if (typeof(bool) == nullableType)
            {
                result = XmlConvert.ToString((bool) value);
            }
            else if (typeof(byte) == nullableType)
            {
                result = XmlConvert.ToString((byte) value);
            }
            else if (typeof(DateTime) == nullableType)
            {
                result = XmlConvert.ToString((DateTime) value, XmlDateTimeSerializationMode.RoundtripKind);
            }
            else if (typeof(decimal) == nullableType)
            {
                result = XmlConvert.ToString((decimal) value);
            }
            else if (typeof(double) == nullableType)
            {
                result = XmlConvert.ToString((double) value);
            }
            else if (typeof(Guid) == nullableType)
            {
                result = value.ToString();
            }
            else if (typeof(short) == nullableType)
            {
                result = XmlConvert.ToString((short) value);
            }
            else if (typeof(int) == nullableType)
            {
                result = XmlConvert.ToString((int) value);
            }
            else if (typeof(long) == nullableType)
            {
                result = XmlConvert.ToString((long) value);
            }
            else if (typeof(sbyte) == nullableType)
            {
                result = XmlConvert.ToString((sbyte) value);
            }
            else if (typeof(float) == nullableType)
            {
                result = XmlConvert.ToString((float) value);
            }
            else if (typeof(byte[]) == nullableType)
            {
                byte[] inArray = (byte[]) value;
                result = Convert.ToBase64String(inArray);
            }
            else
            {
                if (ClientConvert.IsBinaryValue(value))
                {
                    return ClientConvert.TryKeyBinaryToString(value, out result);
                }
                if (typeof(XElement) == nullableType)
                {
                    result = ((XElement) value).ToString(SaveOptions.None);
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            return true;
        }
    }
}

