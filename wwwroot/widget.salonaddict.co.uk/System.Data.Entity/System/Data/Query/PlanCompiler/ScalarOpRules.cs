namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class ScalarOpRules
    {
        internal static readonly PatternMatchRule Rule_AndOverConstantPred1 = new PatternMatchRule(new Node(ConditionalOp.PatternAnd, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessAndOverConstantPredicate1));
        internal static readonly PatternMatchRule Rule_AndOverConstantPred2 = new PatternMatchRule(new Node(ConditionalOp.PatternAnd, new Node[] { new Node(ConstantPredicateOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessAndOverConstantPredicate2));
        internal static readonly SimpleRule Rule_Case = new SimpleRule(OpType.Case, new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessCase));
        internal static readonly PatternMatchRule Rule_EqualsOverConstant = new PatternMatchRule(new Node(ComparisonOp.PatternEq, new Node[] { new Node(InternalConstantOp.Pattern, new Node[0]), new Node(InternalConstantOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessComparisonsOverConstant));
        internal static readonly PatternMatchRule Rule_IsNullOverConstant = new PatternMatchRule(new Node(ConditionalOp.PatternIsNull, new Node[] { new Node(InternalConstantOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessIsNullOverConstant));
        internal static readonly PatternMatchRule Rule_IsNullOverNull = new PatternMatchRule(new Node(ConditionalOp.PatternIsNull, new Node[] { new Node(NullOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessIsNullOverNull));
        internal static readonly PatternMatchRule Rule_LikeOverConstants = new PatternMatchRule(new Node(LikeOp.Pattern, new Node[] { new Node(InternalConstantOp.Pattern, new Node[0]), new Node(InternalConstantOp.Pattern, new Node[0]), new Node(NullOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessLikeOverConstant));
        internal static readonly PatternMatchRule Rule_NotOverConstantPred = new PatternMatchRule(new Node(ConditionalOp.PatternNot, new Node[] { new Node(ConstantPredicateOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessNotOverConstantPredicate));
        internal static readonly PatternMatchRule Rule_NullCast = new PatternMatchRule(new Node(CastOp.Pattern, new Node[] { new Node(NullOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessNullCast));
        internal static readonly PatternMatchRule Rule_OrOverConstantPred1 = new PatternMatchRule(new Node(ConditionalOp.PatternOr, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessOrOverConstantPredicate1));
        internal static readonly PatternMatchRule Rule_OrOverConstantPred2 = new PatternMatchRule(new Node(ConditionalOp.PatternOr, new Node[] { new Node(ConstantPredicateOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ScalarOpRules.ProcessOrOverConstantPredicate2));
        internal static readonly Rule[] Rules = new Rule[] { Rule_Case, Rule_LikeOverConstants, Rule_EqualsOverConstant, Rule_AndOverConstantPred1, Rule_AndOverConstantPred2, Rule_OrOverConstantPred1, Rule_OrOverConstantPred2, Rule_NotOverConstantPred, Rule_IsNullOverConstant, Rule_IsNullOverNull, Rule_NullCast };

        private static bool? MatchesPattern(string str, string pattern)
        {
            int index = pattern.IndexOf('%');
            if (((index == -1) || (index != (pattern.Length - 1))) || (pattern.Length > (str.Length + 1)))
            {
                return null;
            }
            bool flag = true;
            int num2 = 0;
            for (num2 = 0; (num2 < str.Length) && (num2 < (pattern.Length - 1)); num2++)
            {
                if (pattern[num2] != str[num2])
                {
                    flag = false;
                    break;
                }
            }
            return new bool?(flag);
        }

        private static bool ProcessAndOverConstantPredicate1(RuleProcessingContext context, Node node, out Node newNode) => 
            ProcessLogOpOverConstant(context, node, node.Child1, node.Child0, out newNode);

        private static bool ProcessAndOverConstantPredicate2(RuleProcessingContext context, Node node, out Node newNode) => 
            ProcessLogOpOverConstant(context, node, node.Child0, node.Child1, out newNode);

        private static bool ProcessCase(RuleProcessingContext context, Node caseOpNode, out Node newNode)
        {
            CaseOp caseOp = (CaseOp) caseOpNode.Op;
            newNode = caseOpNode;
            return (ProcessCase_Collapse(caseOp, caseOpNode, out newNode) || ProcessCase_EliminateWhenClauses(context, caseOp, caseOpNode, out newNode));
        }

        private static bool ProcessCase_Collapse(CaseOp caseOp, Node caseOpNode, out Node newNode)
        {
            newNode = caseOpNode;
            Node other = caseOpNode.Child1;
            Node node2 = caseOpNode.Children[caseOpNode.Children.Count - 1];
            if (!other.IsEquivalent(node2))
            {
                return false;
            }
            for (int i = 3; i < (caseOpNode.Children.Count - 1); i += 2)
            {
                if (!caseOpNode.Children[i].IsEquivalent(other))
                {
                    return false;
                }
            }
            newNode = other;
            return true;
        }

        private static bool ProcessCase_EliminateWhenClauses(RuleProcessingContext context, CaseOp caseOp, Node caseOpNode, out Node newNode)
        {
            List<Node> args = null;
            newNode = caseOpNode;
            int num = 0;
            while (num < caseOpNode.Children.Count)
            {
                if (num == (caseOpNode.Children.Count - 1))
                {
                    if (OpType.SoftCast == caseOpNode.Children[num].Op.OpType)
                    {
                        return false;
                    }
                    if (args != null)
                    {
                        args.Add(caseOpNode.Children[num]);
                    }
                    break;
                }
                if (OpType.SoftCast == caseOpNode.Children[num + 1].Op.OpType)
                {
                    return false;
                }
                if (caseOpNode.Children[num].Op.OpType != OpType.ConstantPredicate)
                {
                    if (args != null)
                    {
                        args.Add(caseOpNode.Children[num]);
                        args.Add(caseOpNode.Children[num + 1]);
                    }
                    num += 2;
                }
                else
                {
                    ConstantPredicateOp op = (ConstantPredicateOp) caseOpNode.Children[num].Op;
                    if (args == null)
                    {
                        args = new List<Node>();
                        for (int i = 0; i < num; i++)
                        {
                            args.Add(caseOpNode.Children[i]);
                        }
                    }
                    if (op.IsTrue)
                    {
                        args.Add(caseOpNode.Children[num + 1]);
                        break;
                    }
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.IsFalse, "constant predicate must be either true or false");
                    num += 2;
                }
            }
            if (args == null)
            {
                return false;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(args.Count > 0, "new args list must not be empty");
            if (args.Count == 1)
            {
                newNode = args[0];
            }
            else
            {
                newNode = context.Command.CreateNode(caseOp, args);
            }
            return true;
        }

        private static bool ProcessComparisonsOverConstant(RuleProcessingContext context, Node node, out Node newNode)
        {
            newNode = node;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((node.Op.OpType == OpType.EQ) || (node.Op.OpType == OpType.NE), "unexpected comparison op type?");
            bool? nullable = node.Child0.Op.IsEquivalent(node.Child1.Op);
            if (!nullable.HasValue)
            {
                return false;
            }
            bool flag = (node.Op.OpType == OpType.EQ) ? nullable.Value : !nullable.Value;
            ConstantPredicateOp op = context.Command.CreateConstantPredicateOp(flag);
            newNode = context.Command.CreateNode(op);
            return true;
        }

        private static bool ProcessIsNullOverConstant(RuleProcessingContext context, Node isNullNode, out Node newNode)
        {
            newNode = context.Command.CreateNode(context.Command.CreateFalseOp());
            return true;
        }

        private static bool ProcessIsNullOverNull(RuleProcessingContext context, Node isNullNode, out Node newNode)
        {
            newNode = context.Command.CreateNode(context.Command.CreateTrueOp());
            return true;
        }

        private static bool ProcessLikeOverConstant(RuleProcessingContext context, Node n, out Node newNode)
        {
            newNode = n;
            InternalConstantOp op = (InternalConstantOp) n.Child1.Op;
            InternalConstantOp op2 = (InternalConstantOp) n.Child0.Op;
            string text1 = (string) op2.Value;
            string text2 = (string) op.Value;
            bool? nullable = MatchesPattern((string) op2.Value, (string) op.Value);
            if (!nullable.HasValue)
            {
                return false;
            }
            ConstantPredicateOp op3 = context.Command.CreateConstantPredicateOp(nullable.Value);
            newNode = context.Command.CreateNode(op3);
            return true;
        }

        private static bool ProcessLogOpOverConstant(RuleProcessingContext context, Node node, Node constantPredicateNode, Node otherNode, out Node newNode)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(constantPredicateNode != null, "null constantPredicateOp?");
            ConstantPredicateOp op = (ConstantPredicateOp) constantPredicateNode.Op;
            switch (node.Op.OpType)
            {
                case OpType.And:
                    newNode = op.IsTrue ? otherNode : constantPredicateNode;
                    break;

                case OpType.Or:
                    newNode = op.IsTrue ? constantPredicateNode : otherNode;
                    break;

                case OpType.Not:
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(otherNode == null, "Not Op with more than 1 child. Gasp!");
                    newNode = context.Command.CreateNode(context.Command.CreateConstantPredicateOp(!op.Value));
                    break;

                default:
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "Unexpected OpType - " + node.Op.OpType);
                    newNode = null;
                    break;
            }
            return true;
        }

        private static bool ProcessNotOverConstantPredicate(RuleProcessingContext context, Node node, out Node newNode) => 
            ProcessLogOpOverConstant(context, node, node.Child0, null, out newNode);

        private static bool ProcessNullCast(RuleProcessingContext context, Node castNullOp, out Node newNode)
        {
            newNode = context.Command.CreateNode(context.Command.CreateNullOp(castNullOp.Op.Type));
            return true;
        }

        private static bool ProcessOrOverConstantPredicate1(RuleProcessingContext context, Node node, out Node newNode) => 
            ProcessLogOpOverConstant(context, node, node.Child1, node.Child0, out newNode);

        private static bool ProcessOrOverConstantPredicate2(RuleProcessingContext context, Node node, out Node newNode) => 
            ProcessLogOpOverConstant(context, node, node.Child0, node.Child1, out newNode);
    }
}

