namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class FilterOpRules
    {
        internal static readonly PatternMatchRule Rule_FilterOverCrossJoin = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(CrossJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverJoin));
        internal static readonly PatternMatchRule Rule_FilterOverDistinct = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(DistinctOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverDistinct));
        internal static readonly PatternMatchRule Rule_FilterOverExcept = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(ExceptOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverSetOp));
        internal static readonly PatternMatchRule Rule_FilterOverFilter = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverFilter));
        internal static readonly PatternMatchRule Rule_FilterOverGroupBy = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(GroupByOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverGroupBy));
        internal static readonly PatternMatchRule Rule_FilterOverInnerJoin = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(InnerJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverJoin));
        internal static readonly PatternMatchRule Rule_FilterOverIntersect = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(IntersectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverSetOp));
        internal static readonly PatternMatchRule Rule_FilterOverLeftOuterJoin = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(LeftOuterJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverJoin));
        internal static readonly PatternMatchRule Rule_FilterOverOuterApply = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(OuterApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverOuterApply));
        internal static readonly PatternMatchRule Rule_FilterOverProject = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverProject));
        internal static readonly PatternMatchRule Rule_FilterOverUnionAll = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(UnionAllOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterOverSetOp));
        internal static readonly PatternMatchRule Rule_FilterWithConstantPredicate = new PatternMatchRule(new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(FilterOpRules.ProcessFilterWithConstantPredicate));
        internal static readonly Rule[] Rules = new Rule[] { Rule_FilterWithConstantPredicate, Rule_FilterOverCrossJoin, Rule_FilterOverDistinct, Rule_FilterOverExcept, Rule_FilterOverFilter, Rule_FilterOverGroupBy, Rule_FilterOverInnerJoin, Rule_FilterOverIntersect, Rule_FilterOverLeftOuterJoin, Rule_FilterOverProject, Rule_FilterOverUnionAll, Rule_FilterOverOuterApply };

        private static Node GetPushdownPredicate(Command command, Node filterNode, VarVec columns, out Node nonPushdownPredicateNode)
        {
            Node andTree = filterNode.Child1;
            nonPushdownPredicateNode = null;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(filterNode);
            if ((columns != null) || !extendedNodeInfo.ExternalReferences.IsEmpty)
            {
                Predicate predicate2;
                if (columns == null)
                {
                    columns = command.GetExtendedNodeInfo(filterNode.Child0).Definitions;
                }
                Predicate predicate = new Predicate(command, andTree);
                andTree = predicate.GetSingleTablePredicates(columns, out predicate2).BuildAndTree();
                nonPushdownPredicateNode = predicate2.BuildAndTree();
            }
            return andTree;
        }

        private static bool ProcessFilterOverDistinct(RuleProcessingContext context, Node filterNode, out Node newNode)
        {
            Node node;
            newNode = filterNode;
            Node node2 = GetPushdownPredicate(context.Command, filterNode, null, out node);
            if (node2 == null)
            {
                return false;
            }
            Node node3 = filterNode.Child0;
            Node node4 = context.Command.CreateNode(context.Command.CreateFilterOp(), node3.Child0, node2);
            Node node5 = context.Command.CreateNode(node3.Op, node4);
            if (node != null)
            {
                newNode = context.Command.CreateNode(context.Command.CreateFilterOp(), node5, node);
            }
            else
            {
                newNode = node5;
            }
            return true;
        }

        private static bool ProcessFilterOverFilter(RuleProcessingContext context, Node filterNode, out Node newNode)
        {
            Node node = context.Command.CreateNode(context.Command.CreateConditionalOp(OpType.And), filterNode.Child0.Child1, filterNode.Child1);
            newNode = context.Command.CreateNode(context.Command.CreateFilterOp(), filterNode.Child0.Child0, node);
            return true;
        }

        private static bool ProcessFilterOverGroupBy(RuleProcessingContext context, Node filterNode, out Node newNode)
        {
            Node node2;
            newNode = filterNode;
            Node node = filterNode.Child0;
            GroupByOp op = (GroupByOp) node.Op;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Dictionary<Var, int> varRefMap = new Dictionary<Var, int>();
            if (!context2.IsScalarOpTree(filterNode.Child1, varRefMap))
            {
                return false;
            }
            Node node3 = GetPushdownPredicate(context.Command, filterNode, op.Keys, out node2);
            if (node3 == null)
            {
                return false;
            }
            Dictionary<Var, Node> varMap = context2.GetVarMap(node.Child1, varRefMap);
            if (varMap == null)
            {
                return false;
            }
            Node node4 = context2.ReMap(node3, varMap);
            Node node5 = context2.Command.CreateNode(context2.Command.CreateFilterOp(), node.Child0, node4);
            Node node6 = context2.Command.CreateNode(node.Op, node5, node.Child1, node.Child2);
            if (node2 == null)
            {
                newNode = node6;
            }
            else
            {
                newNode = context2.Command.CreateNode(context2.Command.CreateFilterOp(), node6, node2);
            }
            return true;
        }

        private static bool ProcessFilterOverJoin(RuleProcessingContext context, Node filterNode, out Node newNode)
        {
            Node node7;
            newNode = filterNode;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            if (context2.IsFilterPushdownSuppressed(filterNode))
            {
                return false;
            }
            Node node = filterNode.Child0;
            Op op = node.Op;
            Node n = node.Child0;
            Node node3 = node.Child1;
            Command command = context2.Command;
            bool flag = false;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(node3);
            Predicate otherPredicates = new Predicate(command, filterNode.Child1);
            if ((op.OpType == OpType.LeftOuterJoin) && !otherPredicates.PreservesNulls(extendedNodeInfo.Definitions, true))
            {
                op = command.CreateInnerJoinOp();
                flag = true;
            }
            ExtendedNodeInfo info2 = command.GetExtendedNodeInfo(n);
            Node node4 = null;
            if (n.Op.OpType != OpType.ScanTable)
            {
                node4 = otherPredicates.GetSingleTablePredicates(info2.Definitions, out otherPredicates).BuildAndTree();
            }
            Node node5 = null;
            if ((node3.Op.OpType != OpType.ScanTable) && (op.OpType != OpType.LeftOuterJoin))
            {
                node5 = otherPredicates.GetSingleTablePredicates(extendedNodeInfo.Definitions, out otherPredicates).BuildAndTree();
            }
            Node node6 = null;
            if ((op.OpType == OpType.CrossJoin) || (op.OpType == OpType.InnerJoin))
            {
                node6 = otherPredicates.GetJoinPredicates(info2.Definitions, extendedNodeInfo.Definitions, out otherPredicates).BuildAndTree();
            }
            if (node4 != null)
            {
                n = command.CreateNode(command.CreateFilterOp(), n, node4);
                flag = true;
            }
            if (node5 != null)
            {
                node3 = command.CreateNode(command.CreateFilterOp(), node3, node5);
                flag = true;
            }
            if (node6 != null)
            {
                flag = true;
                if (op.OpType == OpType.CrossJoin)
                {
                    op = command.CreateInnerJoinOp();
                }
                else
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.OpType == OpType.InnerJoin, "unexpected non-InnerJoin?");
                    node6 = command.CreateNode(command.CreateConditionalOp(OpType.And), node.Child2, node6);
                }
            }
            else
            {
                node6 = (op.OpType == OpType.CrossJoin) ? null : node.Child2;
            }
            if (!flag)
            {
                return false;
            }
            if (op.OpType == OpType.CrossJoin)
            {
                node7 = command.CreateNode(op, n, node3);
            }
            else
            {
                node7 = command.CreateNode(op, n, node3, node6);
            }
            Node node8 = otherPredicates.BuildAndTree();
            if (node8 == null)
            {
                newNode = node7;
            }
            else
            {
                newNode = command.CreateNode(command.CreateFilterOp(), node7, node8);
            }
            return true;
        }

        private static bool ProcessFilterOverOuterApply(RuleProcessingContext context, Node filterNode, out Node newNode)
        {
            newNode = filterNode;
            Node node = filterNode.Child0;
            Op op = node.Op;
            Node n = node.Child1;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Command command = context2.Command;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(n);
            Predicate predicate = new Predicate(command, filterNode.Child1);
            if (!predicate.PreservesNulls(extendedNodeInfo.Definitions, true))
            {
                Node node3 = command.CreateNode(command.CreateCrossApplyOp(), node.Child0, n);
                Node node4 = command.CreateNode(command.CreateFilterOp(), node3, filterNode.Child1);
                newNode = node4;
                return true;
            }
            return false;
        }

        private static bool ProcessFilterOverProject(RuleProcessingContext context, Node filterNode, out Node newNode)
        {
            newNode = filterNode;
            Node node = filterNode.Child1;
            if (node.Op.OpType == OpType.ConstantPredicate)
            {
                return false;
            }
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Dictionary<Var, int> varRefMap = new Dictionary<Var, int>();
            if (!context2.IsScalarOpTree(node, varRefMap))
            {
                return false;
            }
            Node node2 = filterNode.Child0;
            Dictionary<Var, Node> varMap = context2.GetVarMap(node2.Child1, varRefMap);
            if (varMap == null)
            {
                return false;
            }
            Node node3 = context2.ReMap(node, varMap);
            Node node4 = context2.Command.CreateNode(context2.Command.CreateFilterOp(), node2.Child0, node3);
            Node node5 = context2.Command.CreateNode(node2.Op, node4, node2.Child1);
            newNode = node5;
            return true;
        }

        private static bool ProcessFilterOverSetOp(RuleProcessingContext context, Node filterNode, out Node newNode)
        {
            Node node;
            newNode = filterNode;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Node node2 = GetPushdownPredicate(context2.Command, filterNode, null, out node);
            if (node2 == null)
            {
                return false;
            }
            if (!context2.IsScalarOpTree(node2))
            {
                return false;
            }
            Node node3 = filterNode.Child0;
            SetOp op = (SetOp) node3.Op;
            List<Node> args = new List<Node>();
            int num = 0;
            foreach (VarMap map in op.VarMap)
            {
                if ((op.OpType == OpType.Except) && (num == 1))
                {
                    args.Add(node3.Child1);
                    break;
                }
                Dictionary<Var, Node> varMap = new Dictionary<Var, Node>();
                foreach (KeyValuePair<Var, Var> pair in map)
                {
                    Node node4 = context2.Command.CreateNode(context2.Command.CreateVarRefOp(pair.Value));
                    varMap.Add(pair.Key, node4);
                }
                Node node5 = node2;
                if ((num == 0) && (filterNode.Op.OpType != OpType.Except))
                {
                    node5 = context2.Copy(node5);
                }
                Node n = context2.ReMap(node5, varMap);
                context2.Command.RecomputeNodeInfo(n);
                Node item = context2.Command.CreateNode(context2.Command.CreateFilterOp(), node3.Children[num], n);
                args.Add(item);
                num++;
            }
            Node node8 = context2.Command.CreateNode(node3.Op, args);
            if (node != null)
            {
                newNode = context2.Command.CreateNode(context2.Command.CreateFilterOp(), node8, node);
            }
            else
            {
                newNode = node8;
            }
            return true;
        }

        private static bool ProcessFilterWithConstantPredicate(RuleProcessingContext context, Node n, out Node newNode)
        {
            newNode = n;
            ConstantPredicateOp op = (ConstantPredicateOp) n.Child1.Op;
            if (op.IsTrue)
            {
                newNode = n.Child0;
                return true;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.IsFalse, "unexpected non-false predicate?");
            if ((n.Child0.Op.OpType == OpType.SingleRowTable) || ((n.Child0.Op.OpType == OpType.Project) && (n.Child0.Child0.Op.OpType == OpType.SingleRowTable)))
            {
                return false;
            }
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            ExtendedNodeInfo extendedNodeInfo = context2.Command.GetExtendedNodeInfo(n.Child0);
            List<Node> args = new List<Node>();
            VarVec vars = context2.Command.CreateVarVec();
            foreach (Var var in extendedNodeInfo.Definitions)
            {
                Var var2;
                NullOp op2 = context2.Command.CreateNullOp(var.Type);
                Node definingExpr = context2.Command.CreateNode(op2);
                Node item = context2.Command.CreateVarDefNode(definingExpr, out var2);
                context2.AddVarMapping(var, var2);
                vars.Set(var2);
                args.Add(item);
            }
            if (vars.IsEmpty)
            {
                Var var3;
                NullOp op3 = context2.Command.CreateNullOp(context2.Command.BooleanType);
                Node node3 = context2.Command.CreateNode(op3);
                Node node4 = context2.Command.CreateVarDefNode(node3, out var3);
                vars.Set(var3);
                args.Add(node4);
            }
            Node node5 = context2.Command.CreateNode(context2.Command.CreateVarDefListOp(), args);
            ProjectOp op4 = context2.Command.CreateProjectOp(vars);
            Node node6 = context2.Command.CreateNode(context2.Command.CreateSingleRowTableOp());
            Node node7 = context2.Command.CreateNode(op4, node6, node5);
            n.Child0 = node7;
            return true;
        }
    }
}

