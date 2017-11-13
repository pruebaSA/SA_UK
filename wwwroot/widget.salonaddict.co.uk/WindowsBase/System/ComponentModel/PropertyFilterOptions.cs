namespace System.ComponentModel
{
    using System;

    [Flags]
    public enum PropertyFilterOptions
    {
        All = 15,
        Invalid = 1,
        None = 0,
        SetValues = 2,
        UnsetValues = 4,
        Valid = 8
    }
}

