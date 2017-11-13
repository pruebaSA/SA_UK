namespace System.Configuration
{
    using System;

    public sealed class ConfigurationElementProperty
    {
        private ConfigurationValidatorBase _validator;

        public ConfigurationElementProperty(ConfigurationValidatorBase validator)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }
            this._validator = validator;
        }

        public ConfigurationValidatorBase Validator =>
            this._validator;
    }
}

