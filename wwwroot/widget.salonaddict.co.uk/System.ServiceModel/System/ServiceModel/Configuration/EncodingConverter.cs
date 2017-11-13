namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.ServiceModel;
    using System.Text;

    internal class EncodingConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            ((typeof(string) == sourceType) || base.CanConvertFrom(context, sourceType));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
            ((typeof(InstanceDescriptor) == destinationType) || base.CanConvertTo(context, destinationType));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }
            string name = (string) value;
            Encoding encoding = Encoding.GetEncoding(name);
            if (encoding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", System.ServiceModel.SR.GetString("ConfigInvalidEncodingValue", new object[] { name }));
            }
            return encoding;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((typeof(string) == destinationType) && (value is Encoding))
            {
                Encoding encoding = (Encoding) value;
                return encoding.HeaderName;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

