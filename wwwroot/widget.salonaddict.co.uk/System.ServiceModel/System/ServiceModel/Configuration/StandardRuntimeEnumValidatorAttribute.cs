﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class StandardRuntimeEnumValidatorAttribute : ConfigurationValidatorAttribute
    {
        private Type enumType;

        public StandardRuntimeEnumValidatorAttribute(Type enumType)
        {
            this.EnumType = enumType;
        }

        public Type EnumType
        {
            get => 
                this.enumType;
            set
            {
                this.enumType = value;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance =>
            new StandardRuntimeEnumValidator(this.enumType);
    }
}

