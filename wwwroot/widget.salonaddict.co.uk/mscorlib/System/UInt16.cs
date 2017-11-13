namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true), CLSCompliant(false)]
    public struct UInt16 : IComparable, IFormattable, IConvertible, IComparable<ushort>, IEquatable<ushort>
    {
        public const ushort MaxValue = 0xffff;
        public const ushort MinValue = 0;
        private ushort m_value;
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is ushort))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeUInt16"));
            }
            return (this - ((ushort) value));
        }

        public int CompareTo(ushort value) => 
            (this - value);

        public override bool Equals(object obj) => 
            ((obj is ushort) && (this == ((ushort) obj)));

        public bool Equals(ushort obj) => 
            (this == obj);

        public override int GetHashCode() => 
            this;

        public override string ToString() => 
            Number.FormatUInt32(this, null, NumberFormatInfo.CurrentInfo);

        public string ToString(IFormatProvider provider) => 
            Number.FormatUInt32(this, null, NumberFormatInfo.GetInstance(provider));

        public string ToString(string format) => 
            Number.FormatUInt32(this, format, NumberFormatInfo.CurrentInfo);

        public string ToString(string format, IFormatProvider provider) => 
            Number.FormatUInt32(this, format, NumberFormatInfo.GetInstance(provider));

        [CLSCompliant(false)]
        public static ushort Parse(string s) => 
            Parse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        [CLSCompliant(false)]
        public static ushort Parse(string s, NumberStyles style)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Parse(s, style, NumberFormatInfo.CurrentInfo);
        }

        [CLSCompliant(false)]
        public static ushort Parse(string s, IFormatProvider provider) => 
            Parse(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));

        [CLSCompliant(false)]
        public static ushort Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Parse(s, style, NumberFormatInfo.GetInstance(provider));
        }

        private static ushort Parse(string s, NumberStyles style, NumberFormatInfo info)
        {
            uint num = 0;
            try
            {
                num = Number.ParseUInt32(s, style, info);
            }
            catch (OverflowException exception)
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_UInt16"), exception);
            }
            if (num > 0xffff)
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_UInt16"));
            }
            return (ushort) num;
        }

        [CLSCompliant(false)]
        public static bool TryParse(string s, out ushort result) => 
            TryParse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);

        [CLSCompliant(false)]
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out ushort result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return TryParse(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        private static bool TryParse(string s, NumberStyles style, NumberFormatInfo info, out ushort result)
        {
            uint num;
            result = 0;
            if (!Number.TryParseUInt32(s, style, info, out num))
            {
                return false;
            }
            if (num > 0xffff)
            {
                return false;
            }
            result = (ushort) num;
            return true;
        }

        public TypeCode GetTypeCode() => 
            TypeCode.UInt16;

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
            this;

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
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidCast_FromTo"), new object[] { "UInt16", "DateTime" }));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider) => 
            Convert.DefaultToType(this, type, provider);
    }
}

