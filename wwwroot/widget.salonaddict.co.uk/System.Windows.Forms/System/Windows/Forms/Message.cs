namespace System.Windows.Forms
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [StructLayout(LayoutKind.Sequential), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public struct Message
    {
        private IntPtr hWnd;
        private int msg;
        private IntPtr wparam;
        private IntPtr lparam;
        private IntPtr result;
        public IntPtr HWnd
        {
            get => 
                this.hWnd;
            set
            {
                this.hWnd = value;
            }
        }
        public int Msg
        {
            get => 
                this.msg;
            set
            {
                this.msg = value;
            }
        }
        public IntPtr WParam
        {
            get => 
                this.wparam;
            set
            {
                this.wparam = value;
            }
        }
        public IntPtr LParam
        {
            get => 
                this.lparam;
            set
            {
                this.lparam = value;
            }
        }
        public IntPtr Result
        {
            get => 
                this.result;
            set
            {
                this.result = value;
            }
        }
        public object GetLParam(System.Type cls) => 
            System.Windows.Forms.UnsafeNativeMethods.PtrToStructure(this.lparam, cls);

        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) => 
            new Message { 
                hWnd = hWnd,
                msg = msg,
                wparam = wparam,
                lparam = lparam,
                result = IntPtr.Zero
            };

        public override bool Equals(object o)
        {
            if (!(o is Message))
            {
                return false;
            }
            Message message = (Message) o;
            return ((((this.hWnd == message.hWnd) && (this.msg == message.msg)) && ((this.wparam == message.wparam) && (this.lparam == message.lparam))) && (this.result == message.result));
        }

        public static bool operator !=(Message a, Message b) => 
            !a.Equals(b);

        public static bool operator ==(Message a, Message b) => 
            a.Equals(b);

        public override int GetHashCode() => 
            ((((int) this.hWnd) << 4) | this.msg);

        public override string ToString()
        {
            bool flag = false;
            try
            {
                IntSecurity.UnmanagedCode.Demand();
                flag = true;
            }
            catch (SecurityException)
            {
            }
            if (flag)
            {
                return MessageDecoder.ToString(this);
            }
            return base.ToString();
        }
    }
}

