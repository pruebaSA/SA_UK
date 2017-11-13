namespace MS.Win32
{
    using System;
    using System.Security;

    internal class MessageOnlyHwndWrapper : HwndWrapper
    {
        [SecurityCritical]
        public MessageOnlyHwndWrapper() : base(0, 0, 0, 0, 0, 0, 0, "", NativeMethods.HWND_MESSAGE, null)
        {
        }
    }
}

