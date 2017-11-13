namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class SetOpRules
    {
        internal static readonly PatternMatchRule Rule_ExceptOverFilter0 = new PatternMatchRule(new Node(ExceptOp.Pattern, new Node[] { new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverFilter0));
        internal static readonly PatternMatchRule Rule_ExceptOverFilter1 = new PatternMatchRule(new Node(ExceptOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverFilter1));
        internal static readonly PatternMatchRule Rule_IntersectOverFilter0 = new PatternMatchRule(new Node(IntersectOp.Pattern, new Node[] { new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverFilter0));
        internal static readonly PatternMatchRule Rule_IntersectOverFilter1 = new PatternMatchRule(new Node(IntersectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverFilter1));
        internal static readonly PatternMatchRule Rule_UnionAllOverFilter0 = new PatternMatchRule(new Node(UnionAllOp.Pattern, new Node[] { new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverFilter0));
        internal static readonly PatternMatchRule Rule_UnionAllOverFilter1 = new PatternMatchRule(new Node(UnionAllOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(SetOpRules.ProcessSetOpOverFilter1));
        internal static readonly Rule[] Rules = new Rule[] { Rule_UnionAllOverFilter0, Rule_UnionAllOverFilter1, Rule_IntersectOverFilter0, Rule_IntersectOverFilter1, Rule_ExceptOverFilter0, Rule_ExceptOverFilter1 };

        private static bool ProcessSetOpOverFilter(RuleProcessingContext context, Node setOpNode, int filterNodeIndex, out Node newNode)
        {
            newNode = setOpNode;
            Node node = setOpNode.Children[filterNodeIndex];
            ConstantPredicateOp op = (ConstantPredicateOp) node.Child1.Op;
            if (!op.IsFalse)
            {
                return false;
            }
            SetOp op2 = (SetOp) setOpNode.Op;
            int index = (filterNodeIndex == 1) ? 0 : 1;
            Node node2 = setOpNode.Children[index];
            VarMap map = null;
            if (op2.OpType == OpType.UnionAll)
            {
                map = op2.VarMap[index];
                newNode = node2;
            }
            else if (op2.OpType == OpType.Intersect)
            {
                map = op2.VarMap[filterNodeIndex];
                newNode = node;
            }
            else
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op2.OpType == OpType.Except, "unexpected SetOp type?");
                map = op2.VarMap[0];
                newNode = setOpNode.Child0;
            }
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            foreach (KeyValuePair<Var, Var> pair in map)
            {
                context2.AddVarMapping(pair.Key, pair.Value);
            }
            return true;
        }

        private static bool ProcessSetOpOverFilter0(RuleProcessingContext context, Node setOpNode, out Node newNode) => 
            ProcessSetOpOverFilter(context, setOpNode, 0, out newNode);

        private static bool ProcessSetOpOverFilter1(RuleProcessingContext context, Node setOpNode, out Node newNode) => 
            ProcessSetOpOverFilter(context, setOpNode, 1, out newNode);
    }
}

