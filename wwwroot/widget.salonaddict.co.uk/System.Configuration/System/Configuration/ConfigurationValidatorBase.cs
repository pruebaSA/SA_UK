namespace System.Configuration
{
    using System;

    public abstract class ConfigurationValidatorBase
    {
        protected ConfigurationValidatorBase()
        {
        }

        public virtual bool CanValidate(Type type) => 
            false;

        public abstract void Validate(object value);
    }
}

