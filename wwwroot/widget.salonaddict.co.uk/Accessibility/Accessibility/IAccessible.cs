namespace Accessibility
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, TypeLibType((short) 0x1050), Guid("618736E0-3C3D-11CF-810C-00AA00389B71")]
    public interface IAccessible
    {
        [DispId(-5000)]
        object accParent { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5000)] get; }
        [DispId(-5001)]
        int accChildCount { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5001)] get; }
        [DispId(-5002)]
        object this[object varChild] { [return: MarshalAs(UnmanagedType.IDispatch)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-5002), TypeLibFunc((short) 0x40)] get; }
        [DispId(-5003)]
        string this[object varChild] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5003)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5003)] set; }
        [DispId(-5004)]
        string this[object varChild] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-5004), TypeLibFunc((short) 0x40)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5004)] set; }
        [DispId(-5005)]
        string this[object varChild] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-5005), TypeLibFunc((short) 0x40)] get; }
        [DispId(-5006)]
        object this[object varChild] { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-5006), TypeLibFunc((short) 0x40)] get; }
        [DispId(-5007)]
        object this[object varChild] { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-5007), TypeLibFunc((short) 0x40)] get; }
        [DispId(-5008)]
        string this[object varChild] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-5008), TypeLibFunc((short) 0x40)] get; }
        [DispId(-5009)]
        int this[ref string pszHelpFile, object varChild] { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-5009), TypeLibFunc((short) 0x40)] get; }
        [DispId(-5010)]
        string this[object varChild] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5010)] get; }
        [DispId(-5011)]
        object accFocus { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5011)] get; }
        [DispId(-5012)]
        object accSelection { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5012)] get; }
        [DispId(-5013)]
        string this[object varChild] { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5013)] get; }
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5014)]
        void accSelect([In] int flagsSelect, [In, Optional, MarshalAs(UnmanagedType.Struct)] object varChild);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5015)]
        void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, [In, Optional, MarshalAs(UnmanagedType.Struct)] object varChild);
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5016)]
        object accNavigate([In] int navDir, [In, Optional, MarshalAs(UnmanagedType.Struct)] object varStart);
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), TypeLibFunc((short) 0x40), DispId(-5017)]
        object accHitTest([In] int xLeft, [In] int yTop);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(-5018), TypeLibFunc((short) 0x40)]
        void accDoDefaultAction([In, Optional, MarshalAs(UnmanagedType.Struct)] object varChild);
    }
}

