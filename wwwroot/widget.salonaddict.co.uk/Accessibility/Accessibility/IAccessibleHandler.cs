namespace Accessibility
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType((short) 1), TypeLibType((short) 0x110), Guid("03022430-ABC4-11D0-BDE2-00AA001A1953")]
    public interface IAccessibleHandler
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void AccessibleObjectFromID([In] int hwnd, [In] int lObjectID, [MarshalAs(UnmanagedType.Interface)] out IAccessible pIAccessible);
    }
}

