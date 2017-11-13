namespace System.Data.Services.Client
{
    using System;

    [Flags]
    public enum SaveChangesOptions
    {
        Batch = 1,
        ContinueOnError = 2,
        None = 0,
        ReplaceOnUpdate = 4
    }
}

