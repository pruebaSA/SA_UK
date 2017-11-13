﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    internal class TimeSpanOrInfiniteValidator : TimeSpanValidator
    {
        public TimeSpanOrInfiniteValidator(TimeSpan minValue, TimeSpan maxValue) : base(minValue, maxValue)
        {
        }

        public override void Validate(object value)
        {
            if ((value.GetType() != typeof(TimeSpan)) || (((TimeSpan) value) != TimeSpan.MaxValue))
            {
                base.Validate(value);
            }
        }
    }
}

