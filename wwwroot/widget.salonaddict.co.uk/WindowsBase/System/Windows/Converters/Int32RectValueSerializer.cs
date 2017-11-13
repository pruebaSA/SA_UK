namespace System.Windows.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Markup;

    public class Int32RectValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context) => 
            true;

        public override bool CanConvertToString(object value, IValueSerializerContext context) => 
            (value is Int32Rect);

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            if (value != null)
            {
                return Int32Rect.Parse(value);
            }
            return base.ConvertFromString(value, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if (value is Int32Rect)
            {
                Int32Rect rect = (Int32Rect) value;
                return rect.ConvertToString(null, TypeConverterHelper.EnglishUSCulture);
            }
            return base.ConvertToString(value, context);
        }
    }
}

