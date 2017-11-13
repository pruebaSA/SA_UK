namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class CaseOp : ScalarOp
    {
        internal static readonly CaseOp Pattern = new CaseOp();

        private CaseOp() : base(OpType.Case)
        {
        }

        internal CaseOp(TypeUsage type) : base(OpType.Case, type)
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

