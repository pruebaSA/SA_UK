namespace MS.Internal.Security.RightsManagement
{
    using System;

    [Flags]
    internal enum ActivationFlags : uint
    {
        Cancel = 8,
        Delayed = 0x40,
        GroupIdentity = 2,
        Machine = 1,
        SharedGroupIdentity = 0x20,
        Silent = 0x10,
        Temporary = 4
    }
}

