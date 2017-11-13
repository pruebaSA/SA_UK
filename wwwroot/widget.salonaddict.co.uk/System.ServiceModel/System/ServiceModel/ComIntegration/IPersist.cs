namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;

    [Guid("0000010c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersist
    {
        void GetClassID(out Guid pClassID);
    }
}

