namespace System.Windows.Interop
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        [SecurityCritical]
        private IntPtr _hwnd;
        [SecurityCritical]
        private int _message;
        [SecurityCritical]
        private IntPtr _wParam;
        [SecurityCritical]
        private IntPtr _lParam;
        [SecurityCritical]
        private int _time;
        [SecurityCritical]
        private int _pt_x;
        [SecurityCritical]
        private int _pt_y;
        [FriendAccessAllowed, SecurityCritical]
        internal MSG(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, int time, int pt_x, int pt_y)
        {
            this._hwnd = hwnd;
            this._message = message;
            this._wParam = wParam;
            this._lParam = lParam;
            this._time = time;
            this._pt_x = pt_x;
            this._pt_y = pt_y;
        }

        public IntPtr hwnd
        {
            [SecurityCritical]
            get => 
                this._hwnd;
            [SecurityCritical]
            set
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                this._hwnd = value;
            }
        }
        public int message
        {
            [SecurityCritical]
            get => 
                this._message;
            [SecurityCritical]
            set
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                this._message = value;
            }
        }
        public IntPtr wParam
        {
            [SecurityCritical]
            get => 
                this._wParam;
            [SecurityCritical]
            set
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                this._wParam = value;
            }
        }
        public IntPtr lParam
        {
            [SecurityCritical]
            get => 
                this._lParam;
            [SecurityCritical]
            set
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                this._lParam = value;
            }
        }
        public int time
        {
            [SecurityCritical]
            get => 
                this._time;
            [SecurityCritical]
            set
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                this._time = value;
            }
        }
        public int pt_x
        {
            [SecurityCritical]
            get => 
                this._pt_x;
            [SecurityCritical]
            set
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                this._pt_x = value;
            }
        }
        public int pt_y
        {
            [SecurityCritical]
            get => 
                this._pt_y;
            [SecurityCritical]
            set
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                this._pt_y = value;
            }
        }
    }
}

