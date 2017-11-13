namespace MS.Internal.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct STATPROPSTG
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        private string lpwstrName;
        private uint propid;
        private VARTYPE vt;
    }
}

