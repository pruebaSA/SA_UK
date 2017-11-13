namespace MS.Win32
{
    using MS.Internal.WindowsBase;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    [FriendAccessAllowed]
    internal class HwndSubclass : IDisposable
    {
        [SecurityCritical]
        private NativeMethods.WndProc _attachedWndProc;
        private Bond _bond;
        private DispatcherOperationCallback _dispatcherOperationCallback;
        private GCHandle _gcHandle;
        [SecurityCritical]
        private WeakReference _hook;
        [SecurityCritical]
        private IntPtr _hwndAttached;
        [SecurityCritical]
        private HandleRef _hwndHandleRef;
        [SecurityCritical]
        private IntPtr _oldWndProc;
        [SecurityCritical, ThreadStatic]
        private static DispatcherOperationCallbackParameter _paramDispatcherCallbackOperation;
        [SecurityCritical]
        private static IntPtr DefWndProc;
        [SecurityCritical]
        private static NativeMethods.WndProc DefWndProcStub = new NativeMethods.WndProc(HwndSubclass.DefWndProcWrapper);
        internal static readonly int DetachMessage = UnsafeNativeMethods.RegisterWindowMessage("HwndSubclass.DetachMessage");

        [SecurityCritical]
        static HwndSubclass()
        {
            if (DetachMessage == 0)
            {
                throw new Win32Exception();
            }
            IntPtr moduleHandle = UnsafeNativeMethods.GetModuleHandle("user32.dll");
            DefWndProc = UnsafeNativeMethods.GetProcAddress(new HandleRef(null, moduleHandle), "DefWindowProcW");
        }

        [SecurityCritical]
        internal HwndSubclass(HwndWrapperHook hook)
        {
            if (hook == null)
            {
                throw new ArgumentNullException("hook");
            }
            this._bond = Bond.Unattached;
            this._hook = new WeakReference(hook);
            this._gcHandle = GCHandle.Alloc(this);
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal IntPtr Attach(IntPtr hwnd)
        {
            SecurityHelper.DemandUIWindowPermission();
            if (this._bond != Bond.Unattached)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("HwndSubclassMultipleAttach"));
            }
            return this.CriticalAttach(hwnd);
        }

        [SecurityCritical]
        private IntPtr CallOldWindowProc(IntPtr oldWndProc, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam) => 
            UnsafeNativeMethods.CallWindowProc(oldWndProc, hwnd, msg, wParam, lParam);

        [SecurityCritical]
        internal IntPtr CriticalAttach(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                throw new ArgumentNullException("hwnd");
            }
            if (this._bond != Bond.Unattached)
            {
                throw new InvalidOperationException();
            }
            NativeMethods.WndProc newWndProc = new NativeMethods.WndProc(this.SubclassWndProc);
            IntPtr windowLongPtr = UnsafeNativeMethods.GetWindowLongPtr(new HandleRef(this, hwnd), -4);
            this.HookWindowProc(hwnd, newWndProc, windowLongPtr);
            return (IntPtr) this._gcHandle;
        }

        [SecurityCritical]
        internal bool CriticalDetach(bool force)
        {
            if ((this._bond == Bond.Detached) || (this._bond == Bond.Unattached))
            {
                return true;
            }
            this._bond = Bond.Orphaned;
            return this.DisposeImpl(force);
        }

        [SecurityCritical]
        private static IntPtr DefWndProcWrapper(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam) => 
            UnsafeNativeMethods.CallWindowProc(DefWndProc, hwnd, msg, wParam, lParam);

        [SecurityCritical]
        internal bool Detach(bool force)
        {
            SecurityHelper.DemandUIWindowPermission();
            return this.CriticalDetach(force);
        }

        [SecurityCritical]
        private object DispatcherCallbackOperation(object o)
        {
            DispatcherOperationCallbackParameter parameter = (DispatcherOperationCallbackParameter) o;
            parameter.handled = false;
            parameter.retVal = IntPtr.Zero;
            if (this._bond == Bond.Attached)
            {
                HwndWrapperHook target = this._hook.Target as HwndWrapperHook;
                if (target != null)
                {
                    parameter.retVal = target(parameter.hwnd, parameter.msg, parameter.wParam, parameter.lParam, ref parameter.handled);
                }
            }
            return parameter;
        }

        [SecurityCritical]
        public virtual void Dispose()
        {
            SecurityHelper.DemandUIWindowPermission();
            this.DisposeImpl(false);
        }

        [SecurityCritical]
        private bool DisposeImpl(bool forceUnhook)
        {
            this._hook = null;
            return this.UnhookWindowProc(forceUnhook);
        }

        [SecurityCritical]
        private void HookWindowProc(IntPtr hwnd, NativeMethods.WndProc newWndProc, IntPtr oldWndProc)
        {
            this._hwndAttached = hwnd;
            this._hwndHandleRef = new HandleRef(null, this._hwndAttached);
            this._bond = Bond.Attached;
            this._attachedWndProc = newWndProc;
            this._oldWndProc = oldWndProc;
            UnsafeNativeMethods.CriticalSetWindowLong(this._hwndHandleRef, -4, this._attachedWndProc);
            ManagedWndProcTracker.TrackHwndSubclass(this, this._hwndAttached);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void RequestDetach(bool force)
        {
            if (this._hwndAttached != IntPtr.Zero)
            {
                RequestDetach(this._hwndAttached, (IntPtr) this._gcHandle, force);
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static void RequestDetach(IntPtr hwnd, IntPtr subclass, bool force)
        {
            SecurityHelper.DemandUIWindowPermission();
            if (hwnd == IntPtr.Zero)
            {
                throw new ArgumentNullException("hwnd");
            }
            if (subclass == IntPtr.Zero)
            {
                throw new ArgumentNullException("subclass");
            }
            int num = force ? 1 : 0;
            UnsafeNativeMethods.UnsafeSendMessage(hwnd, DetachMessage, subclass, (IntPtr) num);
        }

        [SecurityCritical]
        internal IntPtr SubclassWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr zero = IntPtr.Zero;
            bool handled = false;
            if (this._bond == Bond.Unattached)
            {
                this.HookWindowProc(hwnd, new NativeMethods.WndProc(this.SubclassWndProc), Marshal.GetFunctionPointerForDelegate(DefWndProcStub));
            }
            else if (this._bond == Bond.Detached)
            {
                throw new InvalidOperationException();
            }
            IntPtr oldWndProc = this._oldWndProc;
            if (msg == DetachMessage)
            {
                if ((wParam == IntPtr.Zero) || (wParam == ((IntPtr) this._gcHandle)))
                {
                    int num = (int) lParam;
                    bool force = num > 0;
                    zero = this.CriticalDetach(force) ? new IntPtr(1) : IntPtr.Zero;
                    handled = num < 2;
                }
            }
            else
            {
                Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
                if ((dispatcher != null) && !dispatcher.HasShutdownFinished)
                {
                    if (this._dispatcherOperationCallback == null)
                    {
                        this._dispatcherOperationCallback = new DispatcherOperationCallback(this.DispatcherCallbackOperation);
                    }
                    if (_paramDispatcherCallbackOperation == null)
                    {
                        _paramDispatcherCallbackOperation = new DispatcherOperationCallbackParameter();
                    }
                    DispatcherOperationCallbackParameter arg = _paramDispatcherCallbackOperation;
                    _paramDispatcherCallbackOperation = null;
                    arg.hwnd = hwnd;
                    arg.msg = msg;
                    arg.wParam = wParam;
                    arg.lParam = lParam;
                    if (dispatcher.Invoke(DispatcherPriority.Send, this._dispatcherOperationCallback, arg) != null)
                    {
                        handled = arg.handled;
                        zero = arg.retVal;
                    }
                    _paramDispatcherCallbackOperation = arg;
                }
                if (msg == 130)
                {
                    this.CriticalDetach(true);
                    handled = false;
                }
            }
            if (!handled)
            {
                zero = this.CallOldWindowProc(oldWndProc, hwnd, msg, wParam, lParam);
            }
            return zero;
        }

        [SecurityCritical]
        private bool UnhookWindowProc(bool force)
        {
            if ((this._bond != Bond.Unattached) && (this._bond != Bond.Detached))
            {
                if (!force)
                {
                    force = UnsafeNativeMethods.GetWindowLongWndProc(new HandleRef(this, this._hwndAttached)) == this._attachedWndProc;
                }
                if (!force)
                {
                    return false;
                }
                this._bond = Bond.Orphaned;
                ManagedWndProcTracker.UnhookHwndSubclass(this);
                try
                {
                    UnsafeNativeMethods.CriticalSetWindowLong(this._hwndHandleRef, -4, this._oldWndProc);
                }
                catch (Win32Exception exception)
                {
                    if (exception.NativeErrorCode != 0x578)
                    {
                        throw;
                    }
                }
                this._bond = Bond.Detached;
                this._oldWndProc = IntPtr.Zero;
                this._attachedWndProc = null;
                this._hwndAttached = IntPtr.Zero;
                this._hwndHandleRef = new HandleRef(null, IntPtr.Zero);
                this._gcHandle.Free();
            }
            return true;
        }

        private enum Bond
        {
            Unattached,
            Attached,
            Detached,
            Orphaned
        }

        private class DispatcherOperationCallbackParameter
        {
            internal bool handled;
            internal IntPtr hwnd;
            internal IntPtr lParam;
            internal int msg;
            internal IntPtr retVal;
            internal IntPtr wParam;
        }
    }
}

