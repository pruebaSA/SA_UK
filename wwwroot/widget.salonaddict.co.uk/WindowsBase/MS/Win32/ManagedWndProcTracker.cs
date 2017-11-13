namespace MS.Win32
{
    using MS.Internal;
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    internal static class ManagedWndProcTracker
    {
        private static IntPtr _cachedDefWindowProcA = IntPtr.Zero;
        private static IntPtr _cachedDefWindowProcW = IntPtr.Zero;
        private static bool _exiting = false;
        [SecurityCritical]
        private static Hashtable _hwndList = new Hashtable(10);

        [SecurityCritical, SecurityTreatAsSafe]
        static ManagedWndProcTracker()
        {
            new ManagedWndProcTrackerShutDownListener();
        }

        private static IntPtr GetDefWindowProcAddress(IntPtr hwnd)
        {
            if (SafeNativeMethods.IsWindowUnicode(new HandleRef(null, hwnd)))
            {
                if (_cachedDefWindowProcW == IntPtr.Zero)
                {
                    _cachedDefWindowProcW = GetUser32ProcAddress("DefWindowProcW");
                }
                return _cachedDefWindowProcW;
            }
            if (_cachedDefWindowProcA == IntPtr.Zero)
            {
                _cachedDefWindowProcA = GetUser32ProcAddress("DefWindowProcA");
            }
            return _cachedDefWindowProcA;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private static IntPtr GetUser32ProcAddress(string export)
        {
            SecurityHelper.DemandUnmanagedCode();
            IntPtr moduleHandle = MS.Win32.UnsafeNativeMethods.GetModuleHandle("user32.dll");
            if (moduleHandle != IntPtr.Zero)
            {
                return MS.Win32.UnsafeNativeMethods.GetProcAddress(new HandleRef(null, moduleHandle), export);
            }
            return IntPtr.Zero;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static void HookUpDefWindowProc(IntPtr hwnd)
        {
            SecurityHelper.DemandUnmanagedCode();
            IntPtr zero = IntPtr.Zero;
            if (hwnd != IntPtr.Zero)
            {
                IntPtr defWindowProcAddress = GetDefWindowProcAddress(hwnd);
                if (defWindowProcAddress != IntPtr.Zero)
                {
                    try
                    {
                        zero = MS.Win32.UnsafeNativeMethods.SetWindowLong(new HandleRef(null, hwnd), -4, defWindowProcAddress);
                    }
                    catch (Win32Exception exception)
                    {
                        if (exception.NativeErrorCode != 0x578)
                        {
                            throw;
                        }
                    }
                    if (zero != IntPtr.Zero)
                    {
                        MS.Win32.UnsafeNativeMethods.PostMessage(new HandleRef(null, hwnd), 0x10, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private static void OnAppDomainProcessExit()
        {
            _exiting = true;
            lock (_hwndList)
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
                try
                {
                    foreach (DictionaryEntry entry in _hwndList)
                    {
                        IntPtr handle = (IntPtr) entry.Value;
                        if ((MS.Win32.UnsafeNativeMethods.GetWindowLong(new HandleRef(null, handle), -16) & 0x40000000) != 0)
                        {
                            MS.Win32.UnsafeNativeMethods.SendMessage(handle, HwndSubclass.DetachMessage, IntPtr.Zero, (IntPtr) 2);
                        }
                        HookUpDefWindowProc(handle);
                    }
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }
        }

        [SecurityCritical]
        internal static void TrackHwndSubclass(HwndSubclass subclass, IntPtr hwnd)
        {
            lock (_hwndList)
            {
                _hwndList[subclass] = hwnd;
            }
        }

        [SecurityCritical]
        internal static void UnhookHwndSubclass(HwndSubclass subclass)
        {
            if (!_exiting)
            {
                lock (_hwndList)
                {
                    _hwndList.Remove(subclass);
                }
            }
        }

        private sealed class ManagedWndProcTrackerShutDownListener : ShutDownListener
        {
            [SecurityCritical, SecurityTreatAsSafe]
            public ManagedWndProcTrackerShutDownListener() : base(null, ShutDownEvents.AppDomain)
            {
            }

            internal override void OnShutDown(object target)
            {
                ManagedWndProcTracker.OnAppDomainProcessExit();
            }
        }
    }
}

