﻿namespace System.Runtime.InteropServices
{
    using System;

    [ComImport, Obsolete("Use System.Runtime.InteropServices.ComTypes.IEnumConnectionPoints instead. http://go.microsoft.com/fwlink/?linkid=14202", false), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("B196B285-BAB4-101A-B69C-00AA00341D07")]
    public interface UCOMIEnumConnectionPoints
    {
        [PreserveSig]
        int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] UCOMIConnectionPoint[] rgelt, out int pceltFetched);
        [PreserveSig]
        int Skip(int celt);
        [PreserveSig]
        int Reset();
        void Clone(out UCOMIEnumConnectionPoints ppenum);
    }
}

