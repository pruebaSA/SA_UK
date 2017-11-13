namespace System.Windows
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public sealed class SizeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
            ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw base.GetConvertFromException(value);
            }
            string source = value as string;
            if (source != null)
            {
                return Size.Parse(source);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType != null) && (value is Size))
            {
                Size size = (Size) value;
                if (destinationType == typeof(string))
                {
                    return size.ConvertToString(null, culture);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

