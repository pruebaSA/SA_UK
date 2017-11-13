namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TimeSpanValidatorAttribute : ConfigurationValidatorAttribute
    {
        private bool _excludeRange;
        private TimeSpan _max = TimeSpan.MaxValue;
        private TimeSpan _min = TimeSpan.MinValue;
        public const string TimeSpanMaxValue = "10675199.02:48:05.4775807";
        public const string TimeSpanMinValue = "-10675199.02:48:05.4775808";

        public bool ExcludeRange
        {
            get => 
                this._excludeRange;
            set
            {
                this._excludeRange = value;
            }
        }

        public TimeSpan MaxValue =>
            this._max;

        public string MaxValueString
        {
            get => 
                this._max.ToString();
            set
            {
                TimeSpan span = TimeSpan.Parse(value);
                if (this._min > span)
                {
                    throw new ArgumentOutOfRangeException("value", System.Configuration.SR.GetString("Validator_min_greater_than_max"));
                }
                this._max = span;
            }
        }

        public TimeSpan MinValue =>
            this._min;

        public string MinValueString
        {
            get => 
                this._min.ToString();
            set
            {
                TimeSpan span = TimeSpan.Parse(value);
                if (this._max < span)
                {
                    throw new ArgumentOutOfRangeException("value", System.Configuration.SR.GetString("Validator_min_greater_than_max"));
                }
                this._min = span;
            }
        }

        public override ConfigurationValidatorBase ValidatorInstance =>
            new TimeSpanValidator(this._min, this._max, this._excludeRange);
    }
}

