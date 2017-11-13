namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("BD3E2E12-42DD-40f4-A09A-95A50C58304B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IServiceCall
    {
        void OnCall();
    }
}

