namespace System.ServiceProcess
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SessionChangeDescription
    {
        private SessionChangeReason _reason;
        private int _id;
        internal SessionChangeDescription(SessionChangeReason reason, int id)
        {
            this._reason = reason;
            this._id = id;
        }

        public SessionChangeReason Reason =>
            this._reason;
        public int SessionId =>
            this._id;
        public override bool Equals(object obj) => 
            (((obj != null) && (obj is SessionChangeDescription)) && this.Equals((SessionChangeDescription) obj));

        public override int GetHashCode() => 
            (((int) this._reason) ^ this._id);

        public bool Equals(SessionChangeDescription changeDescription) => 
            ((this._reason == changeDescription._reason) && (this._id == changeDescription._id));

        public static bool operator ==(SessionChangeDescription a, SessionChangeDescription b) => 
            a.Equals(b);

        public static bool operator !=(SessionChangeDescription a, SessionChangeDescription b) => 
            !a.Equals(b);
    }
}

