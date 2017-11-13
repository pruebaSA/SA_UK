namespace MS.Internal.Security.RightsManagement
{
    using System;

    [Flags]
    internal enum SignIssuanceLicenseFlags : uint
    {
        AutoGenerateKey = 0x10,
        Cancel = 4,
        Offline = 2,
        Online = 1,
        OwnerLicenseNoPersist = 0x20,
        ServerIssuanceLicense = 8
    }
}

