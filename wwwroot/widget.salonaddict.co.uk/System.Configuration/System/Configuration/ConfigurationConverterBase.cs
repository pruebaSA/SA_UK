namespace System.Configuration
{
    using System;
    using System.ComponentModel;

    public abstract class ConfigurationConverterBase : TypeConverter
    {
        protected ConfigurationConverterBase()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext ctx, Type type) => 
            (type == typeof(string));

        public override bool CanConvertTo(ITypeDescriptorContext ctx, Type type) => 
            (type == typeof(string));

        internal void ValidateType(object value, Type expected)
        {
            if ((value != null) && (value.GetType() != expected))
            {
                throw new ArgumentException(System.Configuration.SR.GetString("Converter_unsupported_value_type", new object[] { expected.Name }));
            }
        }
    }
}

