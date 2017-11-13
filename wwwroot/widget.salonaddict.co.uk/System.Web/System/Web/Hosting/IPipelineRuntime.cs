namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("c96cb854-aec2-4208-9ada-a86a96860cb6")]
    internal interface IPipelineRuntime
    {
        void StartProcessing();
        void StopProcessing();
        void InitializeApplication([In] IntPtr appContext);
        IntPtr GetExecuteDelegate();
        IntPtr GetDisposeDelegate();
        IntPtr GetRoleDelegate();
    }
}

