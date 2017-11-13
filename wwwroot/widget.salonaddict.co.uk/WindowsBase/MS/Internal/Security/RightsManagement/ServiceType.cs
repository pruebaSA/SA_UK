namespace MS.Internal.Security.RightsManagement
{
    using System;

    [Flags]
    internal enum ServiceType : uint
    {
        Activation = 1,
        Certification = 2,
        ClientLicensor = 8,
        Publishing = 4
    }
}

