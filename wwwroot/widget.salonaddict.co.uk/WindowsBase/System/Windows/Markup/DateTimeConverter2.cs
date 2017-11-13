namespace System.Windows.Markup
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    internal class DateTimeConverter2 : TypeConverter
    {
        private DateTimeValueSerializer _dateTimeValueSerializer = new DateTimeValueSerializer();
        private IValueSerializerContext _valueSerializerContext = new DateTimeValueSerializerContext();

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
            ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => 
            this._dateTimeValueSerializer.ConvertFromString(value as string, this._valueSerializerContext);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType != null) && (value is DateTime))
            {
                this._dateTimeValueSerializer.ConvertToString(value as string, this._valueSerializerContext);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

