namespace System.ComponentModel
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class Int32Converter : BaseNumberConverter
    {
        internal override object FromString(string value, CultureInfo culture) => 
            int.Parse(value, culture);

        internal override object FromString(string value, NumberFormatInfo formatInfo) => 
            int.Parse(value, NumberStyles.Integer, formatInfo);

        internal override object FromString(string value, int radix) => 
            Convert.ToInt32(value, radix);

        internal override string ToString(object value, NumberFormatInfo formatInfo)
        {
            int num = (int) value;
            return num.ToString("G", formatInfo);
        }

        internal override Type TargetType =>
            typeof(int);
    }
}

