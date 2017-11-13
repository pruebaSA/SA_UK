namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Diagnostics;

    internal sealed class VarDefListOp : AncillaryOp
    {
        internal static readonly VarDefListOp Instance = new VarDefListOp();
        internal static readonly VarDefListOp Pattern = Instance;

        private VarDefListOp() : base(OpType.VarDefList)
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

