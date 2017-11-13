namespace MS.Win32
{
    using MS.Internal;
    using MS.Internal.WindowsBase;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Windows.Threading;

    [FriendAccessAllowed]
    internal class HwndWrapper : DispatcherObject, IDisposable
    {
        private ushort _classAtom;
        private SecurityCriticalDataClass<IntPtr> _handle;
        private SecurityCriticalDataClass<WeakReferenceList> _hooks;
        private bool _isDisposed;
        private bool _isInCreateWindow;
        private SecurityCriticalDataForSet<int> _ownerThreadID;
        private SecurityCriticalData<HwndWrapperHook> _wndProc;
        [SecurityCritical]
        private static int s_msgGCMemory = MS.Win32.UnsafeNativeMethods.RegisterWindowMessage("HwndWrapper.GetGCMemMessage");

        public event EventHandler Disposed;

        [SecurityCritical]
        public HwndWrapper(int classStyle, int style, int exStyle, int x, int y, int width, int height, string name, IntPtr parent, HwndWrapperHook[] hooks)
        {
            string friendlyName;
            string str2;
            this._ownerThreadID = new SecurityCriticalDataForSet<int>(Thread.CurrentThread.ManagedThreadId);
            if (hooks != null)
            {
                int index = 0;
                int length = hooks.Length;
                while (index < length)
                {
                    if (hooks[index] != null)
                    {
                        this.AddHook(hooks[index]);
                    }
                    index++;
                }
            }
            this._wndProc = new SecurityCriticalData<HwndWrapperHook>(new HwndWrapperHook(this.WndProc));
            HwndSubclass subclass = new HwndSubclass(this._wndProc.Value);
            NativeMethods.WNDCLASSEX_D wndclassex_d = new NativeMethods.WNDCLASSEX_D();
            IntPtr ptr = MS.Win32.UnsafeNativeMethods.CriticalGetStockObject(5);
            if (ptr == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            IntPtr moduleHandle = MS.Win32.UnsafeNativeMethods.GetModuleHandle(null);
            MS.Win32.NativeMethods.WndProc proc = new MS.Win32.NativeMethods.WndProc(subclass.SubclassWndProc);
            if ((AppDomain.CurrentDomain.FriendlyName != null) && (0x80 <= AppDomain.CurrentDomain.FriendlyName.Length))
            {
                friendlyName = AppDomain.CurrentDomain.FriendlyName.Substring(0, 0x80);
            }
            else
            {
                friendlyName = AppDomain.CurrentDomain.FriendlyName;
            }
            if ((Thread.CurrentThread.Name != null) && (0x40 <= Thread.CurrentThread.Name.Length))
            {
                str2 = Thread.CurrentThread.Name.Substring(0, 0x40);
            }
            else
            {
                str2 = Thread.CurrentThread.Name;
            }
            this._classAtom = 0;
            string str3 = Guid.NewGuid().ToString();
            string lpszClassName = string.Format(CultureInfo.InvariantCulture, "HwndWrapper[{0};{1};{2}]", new object[] { friendlyName, str2, str3 });
            wndclassex_d.cbSize = Marshal.SizeOf(typeof(NativeMethods.WNDCLASSEX_D));
            wndclassex_d.style = classStyle;
            wndclassex_d.lpfnWndProc = proc;
            wndclassex_d.cbClsExtra = 0;
            wndclassex_d.cbWndExtra = 0;
            wndclassex_d.hInstance = moduleHandle;
            wndclassex_d.hIcon = IntPtr.Zero;
            wndclassex_d.hCursor = IntPtr.Zero;
            wndclassex_d.hbrBackground = ptr;
            wndclassex_d.lpszMenuName = "";
            wndclassex_d.lpszClassName = lpszClassName;
            wndclassex_d.hIconSm = IntPtr.Zero;
            this._classAtom = MS.Win32.UnsafeNativeMethods.RegisterClassEx(wndclassex_d);
            this._isInCreateWindow = true;
            try
            {
                this._handle = new SecurityCriticalDataClass<IntPtr>(MS.Win32.UnsafeNativeMethods.CreateWindowEx(exStyle, lpszClassName, name, style, x, y, width, height, new HandleRef(null, parent), new HandleRef(null, IntPtr.Zero), new HandleRef(null, IntPtr.Zero), null));
            }
            finally
            {
                this._isInCreateWindow = false;
                if ((this._handle == null) || (this._handle.Value == IntPtr.Zero))
                {
                    new UIPermission(UIPermissionWindow.AllWindows).Assert();
                    try
                    {
                        subclass.Dispose();
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }
                }
            }
            GC.KeepAlive(proc);
        }

        [SecurityCritical]
        public void AddHook(HwndWrapperHook hook)
        {
            if (this._hooks == null)
            {
                this._hooks = new SecurityCriticalDataClass<WeakReferenceList>(new WeakReferenceList());
            }
            this._hooks.Value.Insert(0, hook);
        }

        [SecurityCritical]
        internal void AddHookLast(HwndWrapperHook hook)
        {
            if (this._hooks == null)
            {
                this._hooks = new SecurityCriticalDataClass<WeakReferenceList>(new WeakReferenceList());
            }
            this._hooks.Value.Add(hook);
        }

        private void CheckForCreateWindowFailure(IntPtr result, bool handled)
        {
            if (this._isInCreateWindow && ((IntPtr.Zero != result) && handled))
            {
                if (!Debugger.IsAttached)
                {
                    throw new InvalidOperationException();
                }
                Debugger.Break();
            }
        }

        [SecurityCritical]
        internal static object DestroyWindow(object args)
        {
            SecurityCriticalDataClass<IntPtr> handle = ((DestroyWindowArgs) args).Handle;
            ushort classAtom = ((DestroyWindowArgs) args).ClassAtom;
            Invariant.Assert((handle != null) && (handle.Value != IntPtr.Zero), "Attempting to destroy an invalid hwnd");
            MS.Win32.UnsafeNativeMethods.DestroyWindow(new HandleRef(null, handle.Value));
            UnregisterClass(classAtom);
            return null;
        }

        public virtual void Dispose()
        {
            this.Dispose(true, false);
            GC.SuppressFinalize(this);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private void Dispose(bool disposing, bool isHwndBeingDestroyed)
        {
            if (!this._isDisposed)
            {
                if (disposing && (this.Disposed != null))
                {
                    this.Disposed(this, EventArgs.Empty);
                }
                this._isDisposed = true;
                if (isHwndBeingDestroyed)
                {
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(HwndWrapper.UnregisterClass), this._classAtom);
                }
                else if ((this._handle != null) && (this._handle.Value != IntPtr.Zero))
                {
                    if (Thread.CurrentThread.ManagedThreadId == this._ownerThreadID.Value)
                    {
                        DestroyWindow(new DestroyWindowArgs(this._handle, this._classAtom));
                    }
                    else
                    {
                        base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(HwndWrapper.DestroyWindow), new DestroyWindowArgs(this._handle, this._classAtom));
                    }
                }
                this._classAtom = 0;
                this._handle = null;
            }
        }

        ~HwndWrapper()
        {
            this.Dispose(false, false);
        }

        [SecurityCritical]
        public void RemoveHook(HwndWrapperHook hook)
        {
            if (this._hooks != null)
            {
                this._hooks.Value.Remove(hook);
            }
        }

        [SecurityCritical]
        internal static object UnregisterClass(object arg)
        {
            ushort num = (ushort) arg;
            if (num != 0)
            {
                IntPtr moduleHandle = MS.Win32.UnsafeNativeMethods.GetModuleHandle(null);
                MS.Win32.UnsafeNativeMethods.UnregisterClass(new IntPtr(num), moduleHandle);
            }
            return null;
        }

        [SecurityCritical]
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr zero = IntPtr.Zero;
            if (this._hooks != null)
            {
                WeakReferenceListEnumerator enumerator = this._hooks.Value.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    HwndWrapperHook current = (HwndWrapperHook) enumerator.Current;
                    zero = current(hwnd, msg, wParam, lParam, ref handled);
                    this.CheckForCreateWindowFailure(zero, handled);
                    if (handled)
                    {
                        break;
                    }
                }
            }
            if (msg == 130)
            {
                this.Dispose(true, true);
                GC.SuppressFinalize(this);
                handled = false;
            }
            else if (msg == s_msgGCMemory)
            {
                zero = (IntPtr) GC.GetTotalMemory(wParam == new IntPtr(1));
                handled = true;
            }
            this.CheckForCreateWindowFailure(zero, true);
            return zero;
        }

        public IntPtr Handle
        {
            [SecurityCritical]
            get
            {
                SecurityCriticalDataClass<IntPtr> class2 = this._handle;
                if (class2 != null)
                {
                    return class2.Value;
                }
                return IntPtr.Zero;
            }
        }

        internal class DestroyWindowArgs
        {
            private ushort _classAtom;
            private SecurityCriticalDataClass<IntPtr> _handle;

            public DestroyWindowArgs(SecurityCriticalDataClass<IntPtr> handle, ushort classAtom)
            {
                this._handle = handle;
                this._classAtom = classAtom;
            }

            public ushort ClassAtom =>
                this._classAtom;

            public SecurityCriticalDataClass<IntPtr> Handle =>
                this._handle;
        }
    }
}

