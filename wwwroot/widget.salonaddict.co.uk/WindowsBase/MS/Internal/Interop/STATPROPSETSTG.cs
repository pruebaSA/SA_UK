namespace MS.Internal.Interop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    [StructLayout(LayoutKind.Sequential)]
    internal struct STATPROPSETSTG
    {
        private Guid fmtid;
        private Guid clsid;
        private uint grfFlags;
        private System.Runtime.InteropServices.ComTypes.FILETIME mtime;
        private System.Runtime.InteropServices.ComTypes.FILETIME ctime;
        private System.Runtime.InteropServices.ComTypes.FILETIME atime;
        private uint dwOSVersion;
    }
}

