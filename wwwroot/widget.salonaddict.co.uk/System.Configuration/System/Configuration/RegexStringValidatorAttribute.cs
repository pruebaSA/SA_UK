namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RegexStringValidatorAttribute : ConfigurationValidatorAttribute
    {
        private string _regex;

        public RegexStringValidatorAttribute(string regex)
        {
            this._regex = regex;
        }

        public string Regex =>
            this._regex;

        public override ConfigurationValidatorBase ValidatorInstance =>
            new RegexStringValidator(this._regex);
    }
}

