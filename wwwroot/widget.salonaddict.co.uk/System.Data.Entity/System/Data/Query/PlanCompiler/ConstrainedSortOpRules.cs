namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class ConstrainedSortOpRules
    {
        internal static readonly SimpleRule Rule_ConstrainedSortOpOverEmptySet = new SimpleRule(OpType.ConstrainedSort, new Rule.ProcessNodeDelegate(ConstrainedSortOpRules.ProcessConstrainedSortOpOverEmptySet));
        internal static readonly Rule[] Rules = new Rule[] { Rule_ConstrainedSortOpOverEmptySet };

        private static bool ProcessConstrainedSortOpOverEmptySet(RuleProcessingContext context, Node n, out Node newNode)
        {
            if (((TransformationRulesContext) context).Command.GetExtendedNodeInfo(n.Child0).MaxRows == RowCount.Zero)
            {
                newNode = n.Child0;
                return true;
            }
            newNode = n;
            return false;
        }
    }
}

