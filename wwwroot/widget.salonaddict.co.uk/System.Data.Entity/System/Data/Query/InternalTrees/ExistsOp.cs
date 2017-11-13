namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class ExistsOp : ScalarOp
    {
        internal static readonly ExistsOp Pattern = new ExistsOp();

        private ExistsOp() : base(OpType.Exists)
        {
        }

        internal ExistsOp(TypeUsage type) : base(OpType.Exists, type)
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

        internal override int Arity =>
            1;
    }
}

