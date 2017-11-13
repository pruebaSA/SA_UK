namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SubclassTypeValidatorAttribute : ConfigurationValidatorAttribute
    {
        private Type _baseClass;

        public SubclassTypeValidatorAttribute(Type baseClass)
        {
            this._baseClass = baseClass;
        }

        public Type BaseClass =>
            this._baseClass;

        public override ConfigurationValidatorBase ValidatorInstance =>
            new SubclassTypeValidator(this._baseClass);
    }
}

