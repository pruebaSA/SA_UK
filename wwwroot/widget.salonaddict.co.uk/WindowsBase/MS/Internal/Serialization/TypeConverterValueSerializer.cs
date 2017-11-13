namespace MS.Internal.Serialization
{
    using System;
    using System.ComponentModel;
    using System.Windows.Markup;

    internal sealed class TypeConverterValueSerializer : ValueSerializer
    {
        private TypeConverter converter;

        public TypeConverterValueSerializer(TypeConverter converter)
        {
            this.converter = converter;
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context) => 
            true;

        public override bool CanConvertToString(object value, IValueSerializerContext context) => 
            this.converter.CanConvertTo(context, typeof(string));

        public override object ConvertFromString(string value, IValueSerializerContext context) => 
            this.converter.ConvertFrom(context, TypeConverterHelper.EnglishUSCulture, value);

        public override string ConvertToString(object value, IValueSerializerContext context) => 
            this.converter.ConvertToString(context, TypeConverterHelper.EnglishUSCulture, value);
    }
}

