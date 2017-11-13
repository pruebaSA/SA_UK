namespace System.Data.Query.InternalTrees
{
    using System;

    internal sealed class LeafOp : RulePatternOp
    {
        internal static readonly LeafOp Instance = new LeafOp();
        internal static readonly LeafOp Pattern = Instance;

        private LeafOp() : base(OpType.Leaf)
        {
        }

        internal override int Arity =>
            0;
    }
}

