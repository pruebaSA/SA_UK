namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    internal class MinimizableAttributeTypeConverter : BooleanConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string a = value as string;
            return ((a?.Length > 0) && !string.Equals(a, "false", StringComparison.OrdinalIgnoreCase));
        }
    }
}

