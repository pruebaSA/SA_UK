namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true), CLSCompliant(false)]
    public struct UInt32 : IComparable, IFormattable, IConvertible, IComparable<uint>, IEquatable<uint>
    {
        public const uint MaxValue = 0xffffffff;
        public const uint MinValue = 0;
        private uint m_value;
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is uint))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeUInt32"));
            }
            uint num = (uint) value;
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

        public int CompareTo(uint value)
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
            ((obj is uint) && (this == ((uint) obj)));

        public bool Equals(uint obj) => 
            (this == obj);

        public override unsafe int GetHashCode() => 
            *(((int*) this));

        public override string ToString() => 
            Number.FormatUInt32(this, null, NumberFormatInfo.CurrentInfo);

        public string ToString(IFormatProvider provider) => 
            Number.FormatUInt32(this, null, NumberFormatInfo.GetInstance(provider));

        public string ToString(string format) => 
            Number.FormatUInt32(this, format, NumberFormatInfo.CurrentInfo);

        public string ToString(string format, IFormatProvider provider) => 
            Number.FormatUInt32(this, format, NumberFormatInfo.GetInstance(provider));

        [CLSCompliant(false)]
        public static uint Parse(string s) => 
            Number.ParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        [CLSCompliant(false)]
        public static uint Parse(string s, NumberStyles style)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.ParseUInt32(s, style, NumberFormatInfo.CurrentInfo);
        }

        [CLSCompliant(false)]
        public static uint Parse(string s, IFormatProvider provider) => 
            Number.ParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));

        [CLSCompliant(false)]
        public static uint Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.ParseUInt32(s, style, NumberFormatInfo.GetInstance(provider));
        }

        [CLSCompliant(false)]
        public static bool TryParse(string s, out uint result) => 
            Number.TryParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);

        [CLSCompliant(false)]
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out uint result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Number.TryParseUInt32(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        public TypeCode GetTypeCode() => 
            TypeCode.UInt32;

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
            Convert.ToInt32(this);

        uint IConvertible.ToUInt32(IFormatProvider provider) => 
            this;

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
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidCast_FromTo"), new object[] { "UInt32", "DateTime" }));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider) => 
            Convert.DefaultToType(this, type, provider);
    }
}

