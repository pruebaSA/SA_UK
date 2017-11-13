namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct Int32 : IComparable, IFormattable, IConvertible, IComparable<int>, IEquatable<int>
    {
        public const int MaxValue = 0x7fffffff;
        public const int MinValue = -2147483648;
        internal int m_value;
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is int))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeInt32"));
            }
            int num = (int) value;
            if (this < num)
            {
                return -1;
            }
            if (this > num)
            {
                return 1;
            }
            return 0;
        }

        public int CompareTo(int value)
        {
            if (this < value)
            {
                return -1;
            }
            if (this > value)
            {
                return 1;
            }
            return 0;
        }

        public override bool Equals(object obj) => 
            ((obj is int) && (this == ((int) obj)));

        public bool Equals(int obj) => 
            (this == obj);

        public override int GetHashCode() => 
            this;

        public override string ToString() => 
            Number.FormatInt32(this, null, NumberFormatInfo.CurrentInfo);

        public string ToString(string format) => 
            Number.FormatInt32(this, format, NumberFormatInfo.CurrentInfo);

        public string ToString(IFormatProvider provider) => 
            Number.FormatInt32(this, null, NumberFormatInfo.GetInstance(provider));

        public string ToString(string format, IFormatProvider provider) => 
            Number.FormatInt32(this, format, NumberFormatInfo.GetInstance(provider));

        public static int Parse(string s) => 
            Number.ParseInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        public static int Parse(string s, NumberStyles style)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.ParseInt32(s, style, NumberFormatInfo.CurrentInfo);
        }

        public static int Parse(string s, IFormatProvider provider) => 
            Number.ParseInt32(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));

        public static int Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.ParseInt32(s, style, NumberFormatInfo.GetInstance(provider));
        }

        public static bool TryParse(string s, out int result) => 
            Number.TryParseInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out int result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.TryParseInt32(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        public TypeCode GetTypeCode() => 
            TypeCode.Int32;

        bool IConvertible.ToBoolean(IFormatProvider provider) => 
            Convert.ToBoolean(this);

        char IConvertible.ToChar(IFormatProvider provider) => 
            Convert.ToChar(this);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => 
            Convert.ToSByte(this);

        byte IConvertible.ToByte(IFormatProvider provider) => 
            Convert.ToByte(this);

        short IConvertible.ToInt16(IFormatProvider provider) => 
            Convert.ToInt16(this);

        ushort IConvertible.ToUInt16(IFormatProvider provider) => 
            Convert.ToUInt16(this);

        int IConvertible.ToInt32(IFormatProvider provider) => 
            this;

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
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidCast_FromTo"), new object[] { "Int32", "DateTime" }));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider) => 
            Convert.DefaultToType(this, type, provider);
    }
}

