namespace System.Data.SqlClient
{
    using System;
    using System.Data.SqlTypes;

    internal sealed class SqlCollation
    {
        private const uint BinarySort = 0x1000000;
        private const uint IgnoreCase = 0x100000;
        private const uint IgnoreKanaType = 0x800000;
        private const uint IgnoreNonSpace = 0x200000;
        private const uint IgnoreWidth = 0x400000;
        internal uint info;
        private const uint MaskCompareOpt = 0x1f00000;
        internal const uint MaskLcid = 0xfffff;
        internal byte sortId;

        internal string TraceString() => 
            string.Format(null, "(LCID={0}, Opts={1})", new object[] { this.LCID, (int) this.SqlCompareOptions });

        internal int LCID
        {
            get => 
                (((int) this.info) & 0xfffff);
            set
            {
                this.info = (this.info & 0x1f00000) | ((uint) (value & 0xfffff));
            }
        }

        internal System.Data.SqlTypes.SqlCompareOptions SqlCompareOptions
        {
            get
            {
                System.Data.SqlTypes.SqlCompareOptions none = System.Data.SqlTypes.SqlCompareOptions.None;
                if ((this.info & 0x100000) != 0)
                {
                    none |= System.Data.SqlTypes.SqlCompareOptions.IgnoreCase;
                }
                if ((this.info & 0x200000) != 0)
                {
                    none |= System.Data.SqlTypes.SqlCompareOptions.IgnoreNonSpace;
                }
                if ((this.info & 0x400000) != 0)
                {
                    none |= System.Data.SqlTypes.SqlCompareOptions.IgnoreWidth;
                }
                if ((this.info & 0x800000) != 0)
                {
                    none |= System.Data.SqlTypes.SqlCompareOptions.IgnoreKanaType;
                }
                if ((this.info & 0x1000000) != 0)
                {
                    none |= System.Data.SqlTypes.SqlCompareOptions.BinarySort;
                }
                return none;
            }
            set
            {
                uint num = 0;
                if ((value & System.Data.SqlTypes.SqlCompareOptions.IgnoreCase) != System.Data.SqlTypes.SqlCompareOptions.None)
                {
                    num |= 0x100000;
                }
                if ((value & System.Data.SqlTypes.SqlCompareOptions.IgnoreNonSpace) != System.Data.SqlTypes.SqlCompareOptions.None)
                {
                    num |= 0x200000;
                }
                if ((value & System.Data.SqlTypes.SqlCompareOptions.IgnoreWidth) != System.Data.SqlTypes.SqlCompareOptions.None)
                {
                    num |= 0x400000;
                }
                if ((value & System.Data.SqlTypes.SqlCompareOptions.IgnoreKanaType) != System.Data.SqlTypes.SqlCompareOptions.None)
                {
                    num |= 0x800000;
                }
                if ((value & System.Data.SqlTypes.SqlCompareOptions.BinarySort) != System.Data.SqlTypes.SqlCompareOptions.None)
                {
                    num |= 0x1000000;
                }
                this.info = (this.info & 0xfffff) | num;
            }
        }
    }
}

