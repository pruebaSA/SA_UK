namespace System.Runtime.InteropServices
{
    using System;

    [Guid("FA1F3615-ACB9-486d-9EAC-1BEF87E36B09"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITypeLibExporterNameProvider
    {
        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_BSTR)]
        string[] GetNames();
    }
}

