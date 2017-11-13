namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class RelOp : Op
    {
        internal RelOp(OpType opType) : base(opType)
        {
        }

        internal override bool IsRelOp =>
            true;
    }
}

