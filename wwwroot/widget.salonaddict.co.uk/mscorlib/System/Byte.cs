﻿namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct Byte : IComparable, IFormattable, IConvertible, IComparable<byte>, IEquatable<byte>
    {
        public const byte MaxValue = 0xff;
        public const byte MinValue = 0;
        private byte m_value;
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is byte))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeByte"));
            }
            return (this - ((byte) value));
        }

        public int CompareTo(byte value) => 
            (this - value);

        public override bool Equals(object obj) => 
            ((obj is byte) && (this == ((byte) obj)));

        public bool Equals(byte obj) => 
            (this == obj);

        public override int GetHashCode() => 
            this;

        public static byte Parse(string s) => 
            Parse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        public static byte Parse(string s, NumberStyles style)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Parse(s, style, NumberFormatInfo.CurrentInfo);
        }

        public static byte Parse(string s, IFormatProvider provider) => 
            Parse(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));

        public static byte Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Parse(s, style, NumberFormatInfo.GetInstance(provider));
        }

        private static byte Parse(string s, NumberStyles style, NumberFormatInfo info)
        {
            int num = 0;
            try
            {
                num = Number.ParseInt32(s, style, info);
            }
            catch (OverflowException exception)
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_Byte"), exception);
            }
            if ((num < 0) || (num > 0xff))
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_Byte"));
            }
            return (byte) num;
        }

        public static bool TryParse(string s, out byte result) => 
            TryParse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out byte result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return TryParse(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        private static bool TryParse(string s, NumberStyles style, NumberFormatInfo info, out byte result)
        {
            int num;
            result = 0;
            if (!Number.TryParseInt32(s, style, info, out num))
            {
                return false;
            }
            if ((num < 0) || (num > 0xff))
            {
                return false;
            }
            result = (byte) num;
            return true;
        }

        public override string ToString() => 
            Number.FormatInt32(this, null, NumberFormatInfo.CurrentInfo);

        public string ToString(string format) => 
            Number.FormatInt32(this, format, NumberFormatInfo.CurrentInfo);

        public string ToString(IFormatProvider provider) => 
            Number.FormatInt32(this, null, NumberFormatInfo.GetInstance(provider));

        public string ToString(string format, IFormatProvider provider) => 
            Number.FormatInt32(this, format, NumberFormatInfo.GetInstance(provider));

        public TypeCode GetTypeCode() => 
            TypeCode.Byte;

        bool IConvertible.ToBoolean(IFormatProvider provider) => 
            Convert.ToBoolean(this);

        char IConvertible.ToChar(IFormatProvider provider) => 
            Convert.ToChar(this);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => 
            Convert.ToSByte(this);

        byte IConvertible.ToByte(IFormatProvider provider) => 
            this;

        short IConvertible.ToInt16(IFormatProvider provider) => 
            Convert.ToInt16(this);

        ushort IConvertible.ToUInt16(IFormatProvider provider) => 
            Convert.ToUInt16(this);

        int IConvertible.ToInt32(IFormatProvider provider) => 
            Convert.ToInt32(this);

        uint IConvertible.ToUInt32(IFormatProvider provider) => 
            Convert.ToUInt32(this);

        long IConvertible.ToInt64(IFormatProvider provider) => 
            Convert.ToInt64(this);

        ulong IConvertible.ToUInt64(IFormatProvider provider) => 
            Convert.ToUInt64(this);

        float IConvertible.ToSingle(IFormatProvider provider) => 
            Convert.ToSingle(this);

        double IConvertible.ToDouble(IFormatProvider provider) => 
            Convert.ToDouble(this);

        decimal IConvertible.ToDecimal(IFormatProvider provider) => 
            Convert.ToDecimal(this);

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidCast_FromTo"), new object[] { "Byte", "DateTime" }));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider) => 
            Convert.DefaultToType(this, type, provider);
    }
}

