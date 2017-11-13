namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class AncillaryOp : Op
    {
        internal AncillaryOp(OpType opType) : base(opType)
        {
        }

        internal override bool IsAncillaryOp =>
            true;
    }
}

