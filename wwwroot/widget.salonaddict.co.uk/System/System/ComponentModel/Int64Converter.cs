namespace System.ComponentModel
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class Int64Converter : BaseNumberConverter
    {
        internal override object FromString(string value, CultureInfo culture) => 
            long.Parse(value, culture);

        internal override object FromString(string value, NumberFormatInfo formatInfo) => 
            long.Parse(value, NumberStyles.Integer, formatInfo);

        internal override object FromString(string value, int radix) => 
            Convert.ToInt64(value, radix);

        internal override string ToString(object value, NumberFormatInfo formatInfo)
        {
            long num = (long) value;
            return num.ToString("G", formatInfo);
        }

        internal override Type TargetType =>
            typeof(long);
    }
}

