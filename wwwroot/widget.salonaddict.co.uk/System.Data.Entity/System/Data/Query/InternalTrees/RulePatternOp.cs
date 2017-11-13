namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class RulePatternOp : Op
    {
        internal RulePatternOp(OpType opType) : base(opType)
        {
        }

        internal override bool IsRulePatternOp =>
            true;
    }
}

