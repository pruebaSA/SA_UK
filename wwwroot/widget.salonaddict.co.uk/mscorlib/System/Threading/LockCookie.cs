namespace System.Threading
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct LockCookie
    {
        private int _dwFlags;
        private int _dwWriterSeqNum;
        private int _wReaderAndWriterLevel;
        private int _dwThreadID;
        public override int GetHashCode() => 
            (((this._dwFlags + this._dwWriterSeqNum) + this._wReaderAndWriterLevel) + this._dwThreadID);

        public override bool Equals(object obj) => 
            ((obj is LockCookie) && this.Equals((LockCookie) obj));

        public bool Equals(LockCookie obj) => 
            ((((obj._dwFlags == this._dwFlags) && (obj._dwWriterSeqNum == this._dwWriterSeqNum)) && (obj._wReaderAndWriterLevel == this._wReaderAndWriterLevel)) && (obj._dwThreadID == this._dwThreadID));

        public static bool operator ==(LockCookie a, LockCookie b) => 
            a.Equals(b);

        public static bool operator !=(LockCookie a, LockCookie b) => 
            !(a == b);
    }
}

