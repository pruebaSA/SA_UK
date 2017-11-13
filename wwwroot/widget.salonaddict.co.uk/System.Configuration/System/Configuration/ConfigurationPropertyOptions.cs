namespace System.Configuration
{
    using System;

    [Flags]
    public enum ConfigurationPropertyOptions
    {
        IsDefaultCollection = 1,
        IsKey = 4,
        IsRequired = 2,
        None = 0
    }
}

