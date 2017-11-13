namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;

    internal abstract class NestBaseOp : PhysicalOp
    {
        private List<System.Data.Query.InternalTrees.CollectionInfo> m_collectionInfoList;
        private VarVec m_outputs;
        private List<SortKey> m_prefixSortKeys;

        internal NestBaseOp(OpType opType, List<SortKey> prefixSortKeys, VarVec outputVars, List<System.Data.Query.InternalTrees.CollectionInfo> collectionInfoList) : base(opType)
        {
            this.m_outputs = outputVars;
            this.m_collectionInfoList = collectionInfoList;
            this.m_prefixSortKeys = prefixSortKeys;
        }

        internal List<System.Data.Query.InternalTrees.CollectionInfo> CollectionInfo =>
            this.m_collectionInfoList;

        internal VarVec Outputs =>
            this.m_outputs;

        internal List<SortKey> PrefixSortKeys =>
            this.m_prefixSortKeys;
    }
}

