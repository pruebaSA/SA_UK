namespace System.Windows.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Markup;

    public class PointValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context) => 
            true;

        public override bool CanConvertToString(object value, IValueSerializerContext context) => 
            (value is Point);

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            if (value != null)
            {
                return Point.Parse(value);
            }
            return base.ConvertFromString(value, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if (value is Point)
            {
                Point point = (Point) value;
                return point.ConvertToString(null, TypeConverterHelper.EnglishUSCulture);
            }
            return base.ConvertToString(value, context);
        }
    }
}

