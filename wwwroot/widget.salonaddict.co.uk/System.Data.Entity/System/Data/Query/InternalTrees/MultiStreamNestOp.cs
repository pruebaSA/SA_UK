namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class MultiStreamNestOp : NestBaseOp
    {
        internal MultiStreamNestOp(List<SortKey> prefixSortKeys, VarVec outputVars, List<CollectionInfo> collectionInfoList) : base(OpType.MultiStreamNest, prefixSortKeys, outputVars, collectionInfoList)
        {
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);
    }
}

