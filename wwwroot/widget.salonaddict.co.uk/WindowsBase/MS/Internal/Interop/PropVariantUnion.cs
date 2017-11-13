namespace MS.Internal.Interop
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    [StructLayout(LayoutKind.Explicit), FriendAccessAllowed]
    internal struct PropVariantUnion
    {
        [FieldOffset(0)]
        internal BLOB blob;
        [FieldOffset(0)]
        internal short boolVal;
        [FieldOffset(0)]
        internal BSTRBLOB bstrblobVal;
        [FieldOffset(0)]
        internal IntPtr bstrVal;
        [FieldOffset(0)]
        internal byte bVal;
        [FieldOffset(0)]
        internal CArray cArray;
        [FieldOffset(0)]
        internal sbyte cVal;
        [FieldOffset(0)]
        internal CY cyVal;
        [FieldOffset(0)]
        internal double date;
        [FieldOffset(0)]
        internal double dblVal;
        [FieldOffset(0)]
        internal System.Runtime.InteropServices.ComTypes.FILETIME filetime;
        [FieldOffset(0)]
        internal float fltVal;
        [FieldOffset(0)]
        internal long hVal;
        [FieldOffset(0)]
        internal int intVal;
        [FieldOffset(0)]
        internal short iVal;
        [FieldOffset(0)]
        internal int lVal;
        [FieldOffset(0)]
        internal IntPtr parray;
        [FieldOffset(0)]
        internal IntPtr pboolVal;
        [FieldOffset(0)]
        internal IntPtr pbstrVal;
        [FieldOffset(0)]
        internal IntPtr pbVal;
        [FieldOffset(0)]
        internal IntPtr pclipdata;
        [FieldOffset(0)]
        internal IntPtr pcVal;
        [FieldOffset(0)]
        internal IntPtr pcyVal;
        [FieldOffset(0)]
        internal IntPtr pdate;
        [FieldOffset(0)]
        internal IntPtr pdblVal;
        [FieldOffset(0)]
        internal IntPtr pdecVal;
        [FieldOffset(0)]
        internal IntPtr pdispVal;
        [FieldOffset(0)]
        internal IntPtr pfltVal;
        [FieldOffset(0)]
        internal IntPtr pintVal;
        [FieldOffset(0)]
        internal IntPtr piVal;
        [FieldOffset(0)]
        internal IntPtr plVal;
        [FieldOffset(0)]
        internal IntPtr pparray;
        [FieldOffset(0)]
        internal IntPtr ppdispVal;
        [FieldOffset(0)]
        internal IntPtr ppunkVal;
        [FieldOffset(0)]
        internal IntPtr pscode;
        [FieldOffset(0)]
        internal IntPtr pStorage;
        [FieldOffset(0)]
        internal IntPtr pStream;
        [FieldOffset(0)]
        internal IntPtr pszVal;
        [FieldOffset(0)]
        internal IntPtr puintVal;
        [FieldOffset(0)]
        internal IntPtr puiVal;
        [FieldOffset(0)]
        internal IntPtr pulVal;
        [FieldOffset(0)]
        internal IntPtr punkVal;
        [FieldOffset(0)]
        internal IntPtr puuid;
        [FieldOffset(0)]
        internal IntPtr pvarVal;
        [FieldOffset(0)]
        internal IntPtr pVersionedStream;
        [FieldOffset(0)]
        internal IntPtr pwszVal;
        [FieldOffset(0)]
        internal int scode;
        [FieldOffset(0)]
        internal ulong uhVal;
        [FieldOffset(0)]
        internal uint uintVal;
        [FieldOffset(0)]
        internal ushort uiVal;
        [FieldOffset(0)]
        internal uint ulVal;
    }
}

