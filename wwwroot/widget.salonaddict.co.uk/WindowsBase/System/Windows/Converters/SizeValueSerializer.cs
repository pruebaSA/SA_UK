namespace System.Windows.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Markup;

    public class SizeValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context) => 
            true;

        public override bool CanConvertToString(object value, IValueSerializerContext context) => 
            (value is Size);

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            if (value != null)
            {
                return Size.Parse(value);
            }
            return base.ConvertFromString(value, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if (value is Size)
            {
                Size size = (Size) value;
                return size.ConvertToString(null, TypeConverterHelper.EnglishUSCulture);
            }
            return base.ConvertToString(value, context);
        }
    }
}

