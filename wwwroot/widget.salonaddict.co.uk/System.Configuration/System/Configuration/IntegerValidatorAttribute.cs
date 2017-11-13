namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IntegerValidatorAttribute : ConfigurationValidatorAttribute
    {
        private bool _excludeRange;
        private int _max = 0x7fffffff;
        private int _min = -2147483648;

        public bool ExcludeRange
        {
            get => 
                this._excludeRange;
            set
            {
                this._excludeRange = value;
            }
        }

        public int MaxValue
        {
            get => 
                this._max;
            set
            {
                if (this._min > value)
                {
                    throw new ArgumentOutOfRangeException("value", System.Configuration.SR.GetString("Validator_min_greater_than_max"));
                }
                this._max = value;
            }
        }

        public int MinValue
        {
            get => 
                this._min;
            set
            {
                if (this._max < value)
                {
                    throw new ArgumentOutOfRangeException("value", System.Configuration.SR.GetString("Validator_min_greater_than_max"));
                }
                this._min = value;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance =>
            new IntegerValidator(this._min, this._max, this._excludeRange);
    }
}

