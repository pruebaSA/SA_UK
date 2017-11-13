namespace MS.Internal.Security.RightsManagement
{
    using System;

    [Flags]
    internal enum EnumerateLicenseFlags : uint
    {
        ClientLicensor = 0x80,
        ClientLicensorLid = 0x100,
        Eul = 0x20,
        EulLid = 0x40,
        Expired = 0x1000,
        GroupIdentity = 2,
        GroupIdentityLid = 8,
        GroupIdentityName = 4,
        Machine = 1,
        RevocationList = 0x400,
        RevocationListLid = 0x800,
        SpecifiedClientLicensor = 0x200,
        SpecifiedGroupIdentity = 0x10
    }
}

