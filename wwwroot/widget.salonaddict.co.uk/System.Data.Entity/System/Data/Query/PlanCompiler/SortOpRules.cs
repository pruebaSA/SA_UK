namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class SortOpRules
    {
        internal static readonly SimpleRule Rule_SortOpOverAtMostOneRow = new SimpleRule(OpType.Sort, new Rule.ProcessNodeDelegate(SortOpRules.ProcessSortOpOverAtMostOneRow));
        internal static readonly Rule[] Rules = new Rule[] { Rule_SortOpOverAtMostOneRow };

        private static bool ProcessSortOpOverAtMostOneRow(RuleProcessingContext context, Node n, out Node newNode)
        {
            ExtendedNodeInfo extendedNodeInfo = ((TransformationRulesContext) context).Command.GetExtendedNodeInfo(n.Child0);
            if ((extendedNodeInfo.MaxRows == RowCount.Zero) || (extendedNodeInfo.MaxRows == RowCount.One))
            {
                newNode = n.Child0;
                return true;
            }
            newNode = n;
            return false;
        }
    }
}

