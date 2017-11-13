namespace MS.Internal.Security.RightsManagement
{
    using System;

    internal enum StatusMessage : uint
    {
        AcquireAdvisory = 3,
        AcquireClientLicensor = 5,
        AcquireLicense = 2,
        ActivateGroupIdentity = 1,
        ActivateMachine = 0,
        SignIssuanceLicense = 4
    }
}

