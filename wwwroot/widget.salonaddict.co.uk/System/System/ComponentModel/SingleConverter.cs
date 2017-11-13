namespace System.ComponentModel
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class SingleConverter : BaseNumberConverter
    {
        internal override object FromString(string value, CultureInfo culture) => 
            float.Parse(value, culture);

        internal override object FromString(string value, NumberFormatInfo formatInfo) => 
            float.Parse(value, NumberStyles.Float, (IFormatProvider) formatInfo);

        internal override object FromString(string value, int radix) => 
            Convert.ToSingle(value, CultureInfo.CurrentCulture);

        internal override string ToString(object value, NumberFormatInfo formatInfo)
        {
            float num = (float) value;
            return num.ToString("R", formatInfo);
        }

        internal override bool AllowHex =>
            false;

        internal override Type TargetType =>
            typeof(float);
    }
}

