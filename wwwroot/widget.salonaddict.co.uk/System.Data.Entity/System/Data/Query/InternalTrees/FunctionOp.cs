namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class FunctionOp : ScalarOp
    {
        private EdmFunction m_function;
        internal static readonly FunctionOp Pattern = new FunctionOp();

        private FunctionOp() : base(OpType.Function)
        {
        }

        internal FunctionOp(EdmFunction function) : base(OpType.Function, function.ReturnParameter.TypeUsage)
        {
            this.m_function = function;
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal EdmFunction Function =>
            this.m_function;
    }
}

