namespace MS.Win32
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.CompilerServices;
    using System.Security;

    [FriendAccessAllowed, SecurityCritical]
    internal delegate IntPtr HwndWrapperHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
}

