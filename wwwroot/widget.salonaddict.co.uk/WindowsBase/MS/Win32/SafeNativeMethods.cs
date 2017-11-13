namespace MS.Win32
{
    using MS.Internal.WindowsBase;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security;

    [FriendAccessAllowed]
    internal static class SafeNativeMethods
    {
        public const ushort C1_BLANK = 0x40;
        public const ushort C1_PUNCT = 0x10;
        public const ushort C1_SPACE = 8;
        public const ushort C3_DIACRITIC = 2;
        public const ushort C3_FULLWIDTH = 0x80;
        public const ushort C3_HALFWIDTH = 0x40;
        public const ushort C3_HIRAGANA = 0x20;
        public const ushort C3_IDEOGRAPH = 0x100;
        public const ushort C3_KASHIDA = 0x200;
        public const ushort C3_KATAKANA = 0x10;
        public const ushort C3_NONSPACING = 1;
        public const ushort C3_VOWELMARK = 4;
        public const uint CT_CTYPE1 = 1;
        public const uint CT_CTYPE2 = 2;
        public const uint CT_CTYPE3 = 4;

        [SecurityCritical, SecurityTreatAsSafe]
        public static IntPtr ActivateKeyboardLayout(HandleRef hkl, int uFlags) => 
            SafeNativeMethodsPrivate.ActivateKeyboardLayout(hkl, uFlags);

        [SecurityCritical, SecurityTreatAsSafe]
        internal static bool AdjustWindowRectEx(ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle)
        {
            bool flag = SafeNativeMethodsPrivate.IntAdjustWindowRectEx(ref lpRect, dwStyle, bMenu, dwExStyle);
            if (!flag)
            {
                throw new Win32Exception();
            }
            return flag;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool DestroyCaret()
        {
            bool flag = SafeNativeMethodsPrivate.DestroyCaret();
            Marshal.GetLastWin32Error();
            return flag;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public static IntPtr GetCapture() => 
            SafeNativeMethodsPrivate.GetCapture();

        [SecurityCritical, SecurityTreatAsSafe]
        public static int GetCaretBlinkTime()
        {
            int caretBlinkTime = SafeNativeMethodsPrivate.GetCaretBlinkTime();
            Marshal.GetLastWin32Error();
            return caretBlinkTime;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static void GetClientRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect)
        {
            if (!SafeNativeMethodsPrivate.IntGetClientRect(hWnd, ref rect))
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public static int GetCurrentProcessId() => 
            SafeNativeMethodsPrivate.GetCurrentProcessId();

        [SecurityTreatAsSafe, SecurityCritical]
        public static int GetCurrentThreadId() => 
            SafeNativeMethodsPrivate.GetCurrentThreadId();

        [SecurityCritical, SecurityTreatAsSafe]
        public static IntPtr GetCursor() => 
            SafeNativeMethodsPrivate.GetCursor();

        [SecurityTreatAsSafe, SecurityCritical]
        public static int GetDoubleClickTime() => 
            SafeNativeMethodsPrivate.GetDoubleClickTime();

        [SecurityTreatAsSafe, SecurityCritical]
        public static IntPtr GetKeyboardLayout(int dwLayout) => 
            SafeNativeMethodsPrivate.GetKeyboardLayout(dwLayout);

        [SecurityCritical, SecurityTreatAsSafe]
        public static int GetKeyboardLayoutList(int size, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] hkls)
        {
            UnsafeNativeMethods.SetLastError(0);
            int keyboardLayoutList = SafeNativeMethodsPrivate.GetKeyboardLayoutList(size, hkls);
            if (keyboardLayoutList == 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return keyboardLayoutList;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public static int GetMessagePos() => 
            SafeNativeMethodsPrivate.GetMessagePos();

        [SecurityTreatAsSafe, SecurityCritical]
        internal static int GetMessageTime() => 
            SafeNativeMethodsPrivate.GetMessageTime();

        [SecurityTreatAsSafe, SecurityCritical]
        internal static void GetMonitorInfo(HandleRef hmonitor, [In, Out] NativeMethods.MONITORINFOEX info)
        {
            if (!SafeNativeMethodsPrivate.IntGetMonitorInfo(hmonitor, info))
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool GetStringTypeEx(uint locale, uint infoType, char[] sourceString, int count, ushort[] charTypes)
        {
            bool flag = SafeNativeMethodsPrivate.GetStringTypeEx(locale, infoType, sourceString, count, charTypes);
            int error = Marshal.GetLastWin32Error();
            if (!flag)
            {
                throw new Win32Exception(error);
            }
            return flag;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public static int GetSysColor(int nIndex) => 
            SafeNativeMethodsPrivate.GetSysColor(nIndex);

        [SecurityTreatAsSafe, SecurityCritical]
        public static int GetTickCount() => 
            SafeNativeMethodsPrivate.GetTickCount();

        [SecurityTreatAsSafe, SecurityCritical]
        internal static void GetWindowRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect)
        {
            if (!SafeNativeMethodsPrivate.IntGetWindowRect(hWnd, ref rect))
            {
                throw new Win32Exception();
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static int GetWindowStyle(HandleRef hWnd, bool exStyle)
        {
            int nIndex = exStyle ? -20 : -16;
            return UnsafeNativeMethods.GetWindowLong(hWnd, nIndex);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static bool InSendMessage() => 
            SafeNativeMethodsPrivate.InSendMessage();

        [SecurityTreatAsSafe, SecurityCritical]
        public static bool IsDebuggerPresent() => 
            SafeNativeMethodsPrivate.IsDebuggerPresent();

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool IsUxThemeActive() => 
            (SafeNativeMethodsPrivate.IsThemeActive() != 0);

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool IsWindowEnabled(HandleRef hWnd) => 
            SafeNativeMethodsPrivate.IsWindowEnabled(hWnd);

        [SecurityTreatAsSafe, SecurityCritical]
        public static bool IsWindowUnicode(HandleRef hWnd) => 
            SafeNativeMethodsPrivate.IsWindowUnicode(hWnd);

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool IsWindowVisible(HandleRef hWnd) => 
            SafeNativeMethodsPrivate.IsWindowVisible(hWnd);

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool KillTimer(HandleRef hwnd, int idEvent) => 
            SafeNativeMethodsPrivate.KillTimer(hwnd, idEvent);

        [SecurityCritical, SecurityTreatAsSafe]
        public static NativeMethods.CursorHandle LoadCursor(HandleRef hInst, IntPtr iconId)
        {
            NativeMethods.CursorHandle handle = SafeNativeMethodsPrivate.LoadCursor(hInst, iconId);
            if ((handle == null) || handle.IsInvalid)
            {
                throw new Win32Exception();
            }
            return handle;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static int MapVirtualKey(int nVirtKey, int nMapType) => 
            SafeNativeMethodsPrivate.MapVirtualKey(nVirtKey, nMapType);

        [SecurityCritical, SecurityTreatAsSafe]
        public static IntPtr MonitorFromPoint(NativeMethods.POINTSTRUCT pt, int flags) => 
            SafeNativeMethodsPrivate.MonitorFromPoint(pt, flags);

        [SecurityCritical, SecurityTreatAsSafe]
        public static IntPtr MonitorFromRect(ref NativeMethods.RECT rect, int flags) => 
            SafeNativeMethodsPrivate.MonitorFromRect(ref rect, flags);

        [SecurityTreatAsSafe, SecurityCritical]
        public static IntPtr MonitorFromWindow(HandleRef handle, int flags) => 
            SafeNativeMethodsPrivate.MonitorFromWindow(handle, flags);

        [SecurityCritical, SecurityTreatAsSafe]
        public static void QueryPerformanceCounter(out long lpPerformanceCount)
        {
            if (!SafeNativeMethodsPrivate.QueryPerformanceCounter(out lpPerformanceCount))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public static void QueryPerformanceFrequency(out long lpFrequency)
        {
            if (!SafeNativeMethodsPrivate.QueryPerformanceFrequency(out lpFrequency))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static bool ReleaseCapture()
        {
            bool flag = SafeNativeMethodsPrivate.IntReleaseCapture();
            if (!flag)
            {
                throw new Win32Exception();
            }
            return flag;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public static void ScreenToClient(HandleRef hWnd, [In, Out] NativeMethods.POINT pt)
        {
            if (SafeNativeMethodsPrivate.IntScreenToClient(hWnd, pt) == 0)
            {
                throw new Win32Exception();
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public static IntPtr SetCapture(HandleRef hwnd) => 
            SafeNativeMethodsPrivate.SetCapture(hwnd);

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool SetCaretPos(int x, int y)
        {
            bool flag = SafeNativeMethodsPrivate.SetCaretPos(x, y);
            Marshal.GetLastWin32Error();
            return flag;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public static IntPtr SetCursor(HandleRef hcursor) => 
            SafeNativeMethodsPrivate.SetCursor(hcursor);

        [SecurityCritical, SecurityTreatAsSafe]
        public static IntPtr SetCursor(SafeHandle hcursor) => 
            SafeNativeMethodsPrivate.SetCursor(hcursor);

        [SecurityTreatAsSafe, SecurityCritical]
        public static void SetTimer(HandleRef hWnd, int nIDEvent, int uElapse)
        {
            if (SafeNativeMethodsPrivate.SetTimer(hWnd, nIDEvent, uElapse, null) == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public static int ShowCursor(bool show) => 
            SafeNativeMethodsPrivate.ShowCursor(show);

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool TrackMouseEvent(NativeMethods.TRACKMOUSEEVENT tme)
        {
            bool flag = SafeNativeMethodsPrivate.TrackMouseEvent(tme);
            int error = Marshal.GetLastWin32Error();
            if (!flag && (error != 0))
            {
                throw new Win32Exception(error);
            }
            return flag;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public static bool TrySetTimer(HandleRef hWnd, int nIDEvent, int uElapse)
        {
            if (SafeNativeMethodsPrivate.TrySetTimer(hWnd, nIDEvent, uElapse, null) == IntPtr.Zero)
            {
                return false;
            }
            return true;
        }

        [Flags]
        internal enum PlaySoundFlags
        {
            SND_ALIAS = 0x10000,
            SND_APPLICATION = 0x80,
            SND_ASYNC = 1,
            SND_FILENAME = 0x20000,
            SND_LOOP = 8,
            SND_MEMORY = 4,
            SND_NODEFAULT = 2,
            SND_NOSTOP = 0x10,
            SND_NOWAIT = 0x2000,
            SND_PURGE = 0x40,
            SND_RESOURCE = 0x40000,
            SND_SYNC = 0
        }

        [SuppressUnmanagedCodeSecurity, SuppressUnmanagedCodeSecurity, SecurityCritical(SecurityCriticalScope.Everything)]
        private static class SafeNativeMethodsPrivate
        {
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern IntPtr ActivateKeyboardLayout(HandleRef hkl, int uFlags);
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            public static extern bool DestroyCaret();
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern IntPtr GetCapture();
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            public static extern int GetCaretBlinkTime();
            [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern int GetCurrentProcessId();
            [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern int GetCurrentThreadId();
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern IntPtr GetCursor();
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern int GetDoubleClickTime();
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern IntPtr GetKeyboardLayout(int dwLayout);
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
            public static extern int GetKeyboardLayoutList(int size, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] hkls);
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern int GetMessagePos();
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            internal static extern int GetMessageTime();
            [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            public static extern bool GetStringTypeEx(uint locale, uint infoType, char[] sourceString, int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] ushort[] charTypes);
            [DllImport("user32.dll", CharSet=CharSet.Auto)]
            public static extern int GetSysColor(int nIndex);
            [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern int GetTickCount();
            [DllImport("user32.dll", CharSet=CharSet.Auto)]
            internal static extern bool InSendMessage();
            [DllImport("user32.dll", EntryPoint="AdjustWindowRectEx", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
            public static extern bool IntAdjustWindowRectEx(ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);
            [DllImport("user32.dll", EntryPoint="GetClientRect", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
            public static extern bool IntGetClientRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect);
            [DllImport("user32.dll", EntryPoint="GetMonitorInfo", CharSet=CharSet.Auto, SetLastError=true)]
            public static extern bool IntGetMonitorInfo(HandleRef hmonitor, [In, Out] NativeMethods.MONITORINFOEX info);
            [DllImport("user32.dll", EntryPoint="GetWindowRect", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
            public static extern bool IntGetWindowRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect);
            [DllImport("user32.dll", EntryPoint="ReleaseCapture", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
            public static extern bool IntReleaseCapture();
            [DllImport("user32.dll", EntryPoint="ScreenToClient", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
            public static extern int IntScreenToClient(HandleRef hWnd, [In, Out] NativeMethods.POINT pt);
            [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            internal static extern bool IsDebuggerPresent();
            [DllImport("uxtheme.dll", CharSet=CharSet.Unicode)]
            public static extern int IsThemeActive();
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern bool IsWindowEnabled(HandleRef hWnd);
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern bool IsWindowUnicode(HandleRef hWnd);
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern bool IsWindowVisible(HandleRef hWnd);
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern bool KillTimer(HandleRef hwnd, int idEvent);
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            public static extern NativeMethods.CursorHandle LoadCursor(HandleRef hInst, IntPtr iconId);
            [DllImport("user32.dll", CharSet=CharSet.Auto)]
            internal static extern int MapVirtualKey(int nVirtKey, int nMapType);
            [DllImport("user32.dll", ExactSpelling=true)]
            public static extern IntPtr MonitorFromPoint(NativeMethods.POINTSTRUCT pt, int flags);
            [DllImport("user32.dll", ExactSpelling=true)]
            public static extern IntPtr MonitorFromRect(ref NativeMethods.RECT rect, int flags);
            [DllImport("user32.dll", ExactSpelling=true)]
            public static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);
            [DllImport("kernel32.dll", SetLastError=true)]
            public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
            [DllImport("kernel32.dll", SetLastError=true)]
            public static extern bool QueryPerformanceFrequency(out long lpFrequency);
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern IntPtr SetCapture(HandleRef hwnd);
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
            public static extern bool SetCaretPos(int x, int y);
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern IntPtr SetCursor(HandleRef hcursor);
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern IntPtr SetCursor(SafeHandle hcursor);
            [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
            public static extern IntPtr SetTimer(HandleRef hWnd, int nIDEvent, int uElapse, NativeMethods.TimerProc lpTimerFunc);
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
            public static extern int ShowCursor(bool show);
            [DllImport("user32.dll", SetLastError=true, ExactSpelling=true)]
            public static extern bool TrackMouseEvent(NativeMethods.TRACKMOUSEEVENT tme);
            [DllImport("user32.dll", EntryPoint="SetTimer", CharSet=CharSet.Auto)]
            public static extern IntPtr TrySetTimer(HandleRef hWnd, int nIDEvent, int uElapse, NativeMethods.TimerProc lpTimerFunc);
        }
    }
}

