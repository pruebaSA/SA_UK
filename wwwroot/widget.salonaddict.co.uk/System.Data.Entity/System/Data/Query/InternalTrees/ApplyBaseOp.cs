namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class ApplyBaseOp : RelOp
    {
        internal ApplyBaseOp(OpType opType) : base(opType)
        {
        }

        internal override int Arity =>
            2;
    }
}

