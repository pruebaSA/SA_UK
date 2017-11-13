namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), CLSCompliant(false), ComVisible(true)]
    public struct SByte : IComparable, IFormattable, IConvertible, IComparable<sbyte>, IEquatable<sbyte>
    {
        public const sbyte MaxValue = 0x7f;
        public const sbyte MinValue = -128;
        private sbyte m_value;
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is sbyte))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeSByte"));
            }
            return (this - ((sbyte) obj));
        }

        public int CompareTo(sbyte value) => 
            (this - value);

        public override bool Equals(object obj) => 
            ((obj is sbyte) && (this == ((sbyte) obj)));

        public bool Equals(sbyte obj) => 
            (this == obj);

        public override int GetHashCode() => 
            (this ^ (this << 8));

        public override string ToString() => 
            Number.FormatInt32(this, null, NumberFormatInfo.CurrentInfo);

        public string ToString(IFormatProvider provider) => 
            Number.FormatInt32(this, null, NumberFormatInfo.GetInstance(provider));

        public string ToString(string format) => 
            this.ToString(format, NumberFormatInfo.CurrentInfo);

        public string ToString(string format, IFormatProvider provider) => 
            this.ToString(format, NumberFormatInfo.GetInstance(provider));

        private string ToString(string format, NumberFormatInfo info)
        {
            if ((((this >= 0) || (format == null)) || (format.Length <= 0)) || ((format[0] != 'X') && (format[0] != 'x')))
            {
                return Number.FormatInt32(this, format, info);
            }
            uint num = (uint) (this & 0xff);
            return Number.FormatUInt32(num, format, info);
        }

        [CLSCompliant(false)]
        public static sbyte Parse(string s) => 
            Parse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

        [CLSCompliant(false)]
        public static sbyte Parse(string s, NumberStyles style)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Parse(s, style, NumberFormatInfo.CurrentInfo);
        }

        [CLSCompliant(false)]
        public static sbyte Parse(string s, IFormatProvider provider) => 
            Parse(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));

        [CLSCompliant(false)]
        public static sbyte Parse(string s, NumberStyles style, IFormatProvider provider)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return Parse(s, style, NumberFormatInfo.GetInstance(provider));
        }

        private static sbyte Parse(string s, NumberStyles style, NumberFormatInfo info)
        {
            int num = 0;
            try
            {
                num = Number.ParseInt32(s, style, info);
            }
            catch (OverflowException exception)
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_SByte"), exception);
            }
            if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
            {
                if ((num < 0) || (num > 0xff))
                {
                    throw new OverflowException(Environment.GetResourceString("Overflow_SByte"));
                }
                return (sbyte) num;
            }
            if ((num < -128) || (num > 0x7f))
            {
                throw new OverflowException(Environment.GetResourceString("Overflow_SByte"));
            }
            return (sbyte) num;
        }

        [CLSCompliant(false)]
        public static bool TryParse(string s, out sbyte result) => 
            TryParse(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);

        [CLSCompliant(false)]
        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out sbyte result)
        {
            NumberFormatInfo.ValidateParseStyleInteger(style);
            return TryParse(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        private static bool TryParse(string s, NumberStyles style, NumberFormatInfo info, out sbyte result)
        {
            int num;
            result = 0;
            if (!Number.TryParseInt32(s, style, info, out num))
            {
                return false;
            }
            if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
            {
                if ((num < 0) || (num > 0xff))
                {
                    return false;
                }
                result = (sbyte) num;
                return true;
            }
            if ((num < -128) || (num > 0x7f))
            {
                return false;
            }
            result = (sbyte) num;
            return true;
        }

        public TypeCode GetTypeCode() => 
            TypeCode.SByte;

        bool IConvertible.ToBoolean(IFormatProvider provider) => 
            Convert.ToBoolean(this);

        char IConvertible.ToChar(IFormatProvider provider) => 
            Convert.ToChar(this);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => 
            this;

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
            throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", new object[] { "SByte", "DateTime" }));
        }

        object IConvertible.ToType(Type type, IFormatProvider provider) => 
            Convert.DefaultToType(this, type, provider);
    }
}

