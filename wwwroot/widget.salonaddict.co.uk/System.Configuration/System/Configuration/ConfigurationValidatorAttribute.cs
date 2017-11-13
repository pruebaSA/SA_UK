namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationValidatorAttribute : Attribute
    {
        internal Type _declaringType;
        private readonly Type _validator;

        protected ConfigurationValidatorAttribute()
        {
        }

        public ConfigurationValidatorAttribute(Type validator)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }
            if (!typeof(ConfigurationValidatorBase).IsAssignableFrom(validator))
            {
                throw new ArgumentException(System.Configuration.SR.GetString("Validator_Attribute_param_not_validator", new object[] { "ConfigurationValidatorBase" }));
            }
            this._validator = validator;
        }

        internal void SetDeclaringType(Type declaringType)
        {
            if (declaringType != null)
            {
                if (this._declaringType == null)
                {
                    this._declaringType = declaringType;
                }
                else if (this._declaringType != declaringType)
                {
                }
            }
        }

        public virtual ConfigurationValidatorBase ValidatorInstance =>
            ((ConfigurationValidatorBase) System.Configuration.TypeUtil.CreateInstanceRestricted(this._declaringType, this._validator));

        public Type ValidatorType =>
            this._validator;
    }
}

