namespace System.Data.Services.Client
{
    using System;

    [Flags]
    public enum EntityStates
    {
        Added = 4,
        Deleted = 8,
        Detached = 1,
        Modified = 0x10,
        Unchanged = 2
    }
}

