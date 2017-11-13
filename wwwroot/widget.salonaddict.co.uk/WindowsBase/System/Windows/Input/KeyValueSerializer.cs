namespace System.Windows.Input
{
    using System;
    using System.ComponentModel;
    using System.Windows.Markup;

    public class KeyValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context) => 
            true;

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (!(value is Key))
            {
                return false;
            }
            Key key = (Key) value;
            return ((key >= Key.None) && (key <= Key.OemClear));
        }

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Key));
            if (converter != null)
            {
                return converter.ConvertFromString(value);
            }
            return base.ConvertFromString(value, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Key));
            if (converter != null)
            {
                return converter.ConvertToInvariantString(value);
            }
            return base.ConvertToString(value, context);
        }
    }
}

