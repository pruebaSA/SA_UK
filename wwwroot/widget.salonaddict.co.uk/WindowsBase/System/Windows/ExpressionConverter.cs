namespace System.Windows
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class ExpressionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            false;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
            false;

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw base.GetConvertFromException(value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw base.GetConvertToException(value, destinationType);
        }
    }
}

