namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct EntryIndex
    {
        private uint _store;
        public EntryIndex(uint index)
        {
            this._store = index | 0x80000000;
        }

        public EntryIndex(uint index, bool found)
        {
            this._store = index & 0x7fffffff;
            if (found)
            {
                this._store |= 0x80000000;
            }
        }

        public bool Found =>
            ((this._store & 0x80000000) != 0);
        public uint Index =>
            (this._store & 0x7fffffff);
    }
}

