namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class ElementOp : ScalarOp
    {
        internal static readonly ElementOp Pattern = new ElementOp();

        private ElementOp() : base(OpType.Element)
        {
        }

        internal ElementOp(TypeUsage type) : base(OpType.Element, type)
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

