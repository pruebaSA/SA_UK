namespace System.EnterpriseServices
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("da91b74e-5388-4783-949d-c1cd5fb00506")]
    internal interface IManagedPoolAction
    {
        void LastRelease();
    }
}

