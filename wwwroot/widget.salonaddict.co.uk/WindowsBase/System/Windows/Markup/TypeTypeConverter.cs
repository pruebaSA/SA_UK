namespace System.Windows.Markup
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    internal class TypeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            (sourceType == typeof(string));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string qualifiedTypeName = value as string;
            if ((context != null) && (qualifiedTypeName != null))
            {
                IXamlTypeResolver service = (IXamlTypeResolver) context.GetService(typeof(IXamlTypeResolver));
                if (service != null)
                {
                    return service.Resolve(qualifiedTypeName);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}

