namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.Text;

    internal class WebEncodingValidator : ConfigurationValidatorBase
    {
        public override bool CanValidate(Type type) => 
            (type == typeof(Encoding));

        public override void Validate(object value)
        {
            Encoding encoding = value as Encoding;
            if ((encoding == null) || (((encoding != Encoding.UTF8) && (encoding != Encoding.Unicode)) && (encoding != Encoding.BigEndianUnicode)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR2.GetString(SR2.JsonEncodingNotSupported, new object[0]));
            }
        }
    }
}

