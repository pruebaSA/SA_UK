namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LongValidatorAttribute : ConfigurationValidatorAttribute
    {
        private bool _excludeRange;
        private long _max = 0x7fffffffffffffffL;
        private long _min = -9223372036854775808L;

        public bool ExcludeRange
        {
            get => 
                this._excludeRange;
            set
            {
                this._excludeRange = value;
            }
        }

        public long MaxValue
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

        public long MinValue
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
            new LongValidator(this._min, this._max, this._excludeRange);
    }
}

