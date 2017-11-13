namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class InternalEnumValidatorAttribute : ConfigurationValidatorAttribute
    {
        private Type enumHelperType;

        public InternalEnumValidatorAttribute(Type enumHelperType)
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
            new InternalEnumValidator(this.enumHelperType);
    }
}

