namespace MS.Internal.Security.RightsManagement
{
    using System;

    [Flags]
    internal enum AcquireLicenseFlags : uint
    {
        Cancel = 4,
        FetchAdvisory = 8,
        NonSilent = 1,
        NoPersist = 2,
        NoUI = 0x10
    }
}

