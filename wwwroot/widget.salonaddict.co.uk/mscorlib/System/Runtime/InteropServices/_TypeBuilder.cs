namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection.Emit;

    [TypeLibImportClass(typeof(TypeBuilder)), ComVisible(true), Guid("7E5678EE-48B3-3F83-B076-C58543498A58"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), CLSCompliant(false)]
    public interface _TypeBuilder
    {
        void GetTypeInfoCount(out uint pcTInfo);
        void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);
        void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);
        void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
    }
}

