namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataSourceCacheDurationConverter : Int32Converter
    {
        private TypeConverter.StandardValuesCollection _values;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
            ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }
            string str = value as string;
            if (str != null)
            {
                string a = str.Trim();
                if (a.Length == 0)
                {
                    return 0;
                }
                if (string.Equals(a, "infinite", StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (((value != null) && (destinationType == typeof(string))) && (((int) value) == 0))
            {
                return "Infinite";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this._values == null)
            {
                object[] values = new object[] { 0 };
                this._values = new TypeConverter.StandardValuesCollection(values);
            }
            return this._values;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            false;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
            true;
    }
}

