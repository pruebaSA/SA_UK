namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class StringValidatorAttribute : ConfigurationValidatorAttribute
    {
        private string _invalidChars;
        private int _maxLength = 0x7fffffff;
        private int _minLength;

        public string InvalidCharacters
        {
            get => 
                this._invalidChars;
            set
            {
                this._invalidChars = value;
            }
        }

        public int MaxLength
        {
            get => 
                this._maxLength;
            set
            {
                if (this._minLength > value)
                {
                    throw new ArgumentOutOfRangeException("value", System.Configuration.SR.GetString("Validator_min_greater_than_max"));
                }
                this._maxLength = value;
            }
        }

        public int MinLength
        {
            get => 
                this._minLength;
            set
            {
                if (this._maxLength < value)
                {
                    throw new ArgumentOutOfRangeException("value", System.Configuration.SR.GetString("Validator_min_greater_than_max"));
                }
                this._minLength = value;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance =>
            new StringValidator(this._minLength, this._maxLength, this._invalidChars);
    }
}

