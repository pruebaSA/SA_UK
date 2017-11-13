namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class ServiceModelEnumValidatorAttribute : ConfigurationValidatorAttribute
    {
        private Type enumHelperType;

        public ServiceModelEnumValidatorAttribute(Type enumHelperType)
        {
            this.EnumHelperType = enumHelperType;
        }

        public Type EnumHelperType
        {
            get => 
                this.enumHelperType;
            set
            {
                this.enumHelperType = value;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance =>
            new ServiceModelEnumValidator(this.enumHelperType);
    }
}

