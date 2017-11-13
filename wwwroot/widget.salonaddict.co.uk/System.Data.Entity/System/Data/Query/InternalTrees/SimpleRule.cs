namespace System.Data.Query.InternalTrees
{
    using System;

    internal sealed class SimpleRule : Rule
    {
        internal SimpleRule(OpType opType, Rule.ProcessNodeDelegate processDelegate) : base(opType, processDelegate)
        {
        }

        internal override bool Match(Node node) => 
            (node.Op.OpType == base.RuleOpType);
    }
}

