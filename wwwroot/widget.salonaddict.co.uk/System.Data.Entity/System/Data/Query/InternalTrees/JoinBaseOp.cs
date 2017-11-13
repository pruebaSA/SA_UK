namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class JoinBaseOp : RelOp
    {
        internal JoinBaseOp(OpType opType) : base(opType)
        {
        }

        internal override int Arity =>
            3;
    }
}

