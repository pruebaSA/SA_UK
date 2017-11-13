﻿namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PositiveTimeSpanValidatorAttribute : ConfigurationValidatorAttribute
    {
        public override ConfigurationValidatorBase ValidatorInstance =>
            new PositiveTimeSpanValidator();
    }
}

