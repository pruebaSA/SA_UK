namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class SingleStreamNestOp : NestBaseOp
    {
        private Var m_discriminator;
        private VarVec m_keys;
        private List<SortKey> m_postfixSortKeys;

        internal SingleStreamNestOp(VarVec keys, List<SortKey> prefixSortKeys, List<SortKey> postfixSortKeys, VarVec outputVars, List<CollectionInfo> collectionInfoList, Var discriminatorVar) : base(OpType.SingleStreamNest, prefixSortKeys, outputVars, collectionInfoList)
        {
            this.m_keys = keys;
            this.m_postfixSortKeys = postfixSortKeys;
            this.m_discriminator = discriminatorVar;
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal override int Arity =>
            1;

        internal Var Discriminator =>
            this.m_discriminator;

        internal VarVec Keys =>
            this.m_keys;

        internal List<SortKey> PostfixSortKeys =>
            this.m_postfixSortKeys;
    }
}

