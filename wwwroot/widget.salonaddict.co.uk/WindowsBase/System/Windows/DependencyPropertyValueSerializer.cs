namespace System.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Markup;

    internal class DependencyPropertyValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context) => 
            (ValueSerializer.GetSerializerFor(typeof(Type), context) != null);

        public override bool CanConvertToString(object value, IValueSerializerContext context) => 
            (ValueSerializer.GetSerializerFor(typeof(Type), context) != null);

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            ValueSerializer serializerFor = ValueSerializer.GetSerializerFor(typeof(Type), context);
            if (serializerFor != null)
            {
                int index = value.IndexOf('.');
                if (index >= 0)
                {
                    string str = value.Substring(0, index - 1);
                    Type ownerType = serializerFor.ConvertFromString(str, context) as Type;
                    if (ownerType != null)
                    {
                        return DependencyProperty.FromName(str, ownerType);
                    }
                }
            }
            throw base.GetConvertFromException(value);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            DependencyProperty property = value as DependencyProperty;
            if (property != null)
            {
                ValueSerializer serializerFor = ValueSerializer.GetSerializerFor(typeof(Type), context);
                if (serializerFor != null)
                {
                    return (serializerFor.ConvertToString(property.OwnerType, context) + "." + property.Name);
                }
            }
            throw base.GetConvertToException(value, typeof(string));
        }

        public override IEnumerable<Type> TypeReferences(object value, IValueSerializerContext context)
        {
            DependencyProperty property = value as DependencyProperty;
            if (property != null)
            {
                return new Type[] { property.OwnerType };
            }
            return base.TypeReferences(value, context);
        }
    }
}

