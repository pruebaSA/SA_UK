namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class PhysicalOp : Op
    {
        internal PhysicalOp(OpType opType) : base(opType)
        {
        }

        internal override bool IsPhysicalOp =>
            true;
    }
}

