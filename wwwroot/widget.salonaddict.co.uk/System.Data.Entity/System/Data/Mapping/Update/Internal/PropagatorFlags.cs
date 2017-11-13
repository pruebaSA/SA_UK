namespace System.Data.Mapping.Update.Internal
{
    using System;

    [Flags]
    internal enum PropagatorFlags : byte
    {
        ConcurrencyValue = 2,
        Key = 0x10,
        NoFlags = 0,
        Preserve = 1,
        Unknown = 8
    }
}

