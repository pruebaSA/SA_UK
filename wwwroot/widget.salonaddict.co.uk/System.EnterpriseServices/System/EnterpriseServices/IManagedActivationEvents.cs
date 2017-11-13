namespace System.EnterpriseServices
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("a5f325af-572f-46da-b8ab-827c3d95d99e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IManagedActivationEvents
    {
        void CreateManagedStub(IManagedObjectInfo pInfo, [MarshalAs(UnmanagedType.Bool)] bool fDist);
        void DestroyManagedStub(IManagedObjectInfo pInfo);
    }
}

