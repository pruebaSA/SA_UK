namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;

    internal abstract class SortBaseOp : RelOp
    {
        private List<SortKey> m_keys;

        internal SortBaseOp(OpType opType) : base(opType)
        {
        }

        internal SortBaseOp(OpType opType, List<SortKey> sortKeys) : this(opType)
        {
            this.m_keys = sortKeys;
        }

        internal List<SortKey> Keys =>
            this.m_keys;
    }
}

