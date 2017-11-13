﻿namespace System.Runtime.Serialization.Configuration
{
    using System;
    using System.Configuration;

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class DeclaredTypeValidatorAttribute : ConfigurationValidatorAttribute
    {
        public override ConfigurationValidatorBase ValidatorInstance =>
            new DeclaredTypeValidator();
    }
}

