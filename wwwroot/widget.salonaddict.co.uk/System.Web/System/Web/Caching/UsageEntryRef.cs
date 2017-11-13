namespace System.Web.Caching
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct UsageEntryRef
    {
        private const uint ENTRY_MASK = 0xff;
        private const uint PAGE_MASK = 0xffffff00;
        private const int PAGE_SHIFT = 8;
        internal static readonly UsageEntryRef INVALID;
        private uint _ref;
        internal UsageEntryRef(int pageIndex, int entryIndex)
        {
            this._ref = (uint) ((pageIndex << 8) | (entryIndex & 0xff));
        }

        public override bool Equals(object value) => 
            ((value is UsageEntryRef) && (this._ref == ((UsageEntryRef) value)._ref));

        public static bool operator ==(UsageEntryRef r1, UsageEntryRef r2) => 
            (r1._ref == r2._ref);

        public static bool operator !=(UsageEntryRef r1, UsageEntryRef r2) => 
            (r1._ref != r2._ref);

        public override int GetHashCode() => 
            ((int) this._ref);

        internal int PageIndex =>
            ((int) (this._ref >> 8));
        internal int Ref1Index =>
            ((sbyte) (this._ref & 0xff));
        internal int Ref2Index
        {
            get
            {
                int num = (sbyte) (this._ref & 0xff);
                return -num;
            }
        }
        internal bool IsRef1 =>
            (((sbyte) (this._ref & 0xff)) > 0);
        internal bool IsRef2 =>
            (((sbyte) (this._ref & 0xff)) < 0);
        internal bool IsInvalid =>
            (this._ref == 0);
        static UsageEntryRef()
        {
            INVALID = new UsageEntryRef(0, 0);
        }
    }
}

