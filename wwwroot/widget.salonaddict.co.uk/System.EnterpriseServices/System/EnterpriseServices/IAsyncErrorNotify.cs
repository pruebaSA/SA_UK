namespace System.EnterpriseServices
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("FE6777FB-A674-4177-8F32-6D707E113484"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAsyncErrorNotify
    {
        void OnError(int hresult);
    }
}

