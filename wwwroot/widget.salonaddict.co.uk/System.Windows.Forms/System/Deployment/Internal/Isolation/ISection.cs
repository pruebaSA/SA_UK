namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("285a8862-c84a-11d7-850f-005cd062464f"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISection
    {
        object _NewEnum { [return: MarshalAs(UnmanagedType.Interface)] get; }
        uint Count { get; }
        uint SectionID { get; }
        string SectionName { [return: MarshalAs(UnmanagedType.LPWStr)] get; }
    }
}

