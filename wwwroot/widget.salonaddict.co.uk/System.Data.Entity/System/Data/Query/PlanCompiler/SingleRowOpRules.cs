namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class SingleRowOpRules
    {
        internal static readonly PatternMatchRule Rule_SingleRowOpOverAnything = new PatternMatchRule(new Node(SingleRowOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(SingleRowOpRules.ProcessSingleRowOpOverAnything));
        internal static readonly PatternMatchRule Rule_SingleRowOpOverProject = new PatternMatchRule(new Node(SingleRowOp.Pattern, new Node[] { new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(SingleRowOpRules.ProcessSingleRowOpOverProject));
        internal static readonly Rule[] Rules = new Rule[] { Rule_SingleRowOpOverAnything, Rule_SingleRowOpOverProject };

        private static bool ProcessSingleRowOpOverAnything(RuleProcessingContext context, Node singleRowNode, out Node newNode)
        {
            newNode = singleRowNode;
            TransformationRulesContext context1 = (TransformationRulesContext) context;
            ExtendedNodeInfo extendedNodeInfo = context.Command.GetExtendedNodeInfo(singleRowNode.Child0);
            if (extendedNodeInfo.MaxRows <= RowCount.One)
            {
                newNode = singleRowNode.Child0;
                return true;
            }
            if (singleRowNode.Child0.Op.OpType == OpType.Filter)
            {
                Predicate predicate = new Predicate(context.Command, singleRowNode.Child0.Child1);
                if (predicate.SatisfiesKey(extendedNodeInfo.Keys.KeyVars, extendedNodeInfo.Definitions))
                {
                    extendedNodeInfo.MaxRows = RowCount.One;
                    newNode = singleRowNode.Child0;
                    return true;
                }
            }
            return false;
        }

        private static bool ProcessSingleRowOpOverProject(RuleProcessingContext context, Node singleRowNode, out Node newNode)
        {
            newNode = singleRowNode;
            Node node = singleRowNode.Child0;
            Node node2 = node.Child0;
            singleRowNode.Child0 = node2;
            context.Command.RecomputeNodeInfo(singleRowNode);
            node.Child0 = singleRowNode;
            newNode = node;
            return true;
        }
    }
}

