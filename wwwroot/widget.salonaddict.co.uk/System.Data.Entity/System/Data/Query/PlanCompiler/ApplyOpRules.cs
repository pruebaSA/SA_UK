namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class ApplyOpRules
    {
        internal static readonly PatternMatchRule Rule_CrossApplyIntoScalarSubquery = new PatternMatchRule(new Node(CrossApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyIntoScalarSubquery));
        internal static readonly PatternMatchRule Rule_CrossApplyOverAnything = new PatternMatchRule(new Node(CrossApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyOverAnything));
        internal static readonly PatternMatchRule Rule_CrossApplyOverFilter = new PatternMatchRule(new Node(CrossApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyOverFilter));
        internal static readonly PatternMatchRule Rule_CrossApplyOverLeftOuterJoinOverSingleRowTable = new PatternMatchRule(new Node(CrossApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeftOuterJoinOp.Pattern, new Node[] { new Node(SingleRowTableOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]), new Node(ConstantPredicateOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessCrossApplyOverLeftOuterJoinOverSingleRowTable));
        internal static readonly PatternMatchRule Rule_CrossApplyOverProject = new PatternMatchRule(new Node(CrossApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessCrossApplyOverProject));
        internal static readonly PatternMatchRule Rule_OuterApplyIntoScalarSubquery = new PatternMatchRule(new Node(OuterApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyIntoScalarSubquery));
        internal static readonly PatternMatchRule Rule_OuterApplyOverAnything = new PatternMatchRule(new Node(OuterApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyOverAnything));
        internal static readonly PatternMatchRule Rule_OuterApplyOverDummyProjectOverFilter = new PatternMatchRule(new Node(OuterApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ProjectOp.Pattern, new Node[] { new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(VarDefListOp.Pattern, new Node[] { new Node(VarDefOp.Pattern, new Node[] { new Node(InternalConstantOp.Pattern, new Node[0]) }) }) }) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessOuterApplyOverDummyProjectOverFilter));
        internal static readonly PatternMatchRule Rule_OuterApplyOverFilter = new PatternMatchRule(new Node(OuterApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessApplyOverFilter));
        internal static readonly PatternMatchRule Rule_OuterApplyOverProject = new PatternMatchRule(new Node(OuterApplyOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(ApplyOpRules.ProcessOuterApplyOverProject));
        internal static readonly Rule[] Rules = new Rule[] { Rule_CrossApplyOverAnything, Rule_CrossApplyOverFilter, Rule_CrossApplyOverProject, Rule_OuterApplyOverAnything, Rule_OuterApplyOverDummyProjectOverFilter, Rule_OuterApplyOverProject, Rule_OuterApplyOverFilter, Rule_CrossApplyOverLeftOuterJoinOverSingleRowTable, Rule_CrossApplyIntoScalarSubquery, Rule_OuterApplyIntoScalarSubquery };

        private static bool CanRewriteApply(ExtendedNodeInfo applyRightChildNodeInfo, OpType applyKind)
        {
            if (applyRightChildNodeInfo.Definitions.Count != 1)
            {
                return false;
            }
            if (applyRightChildNodeInfo.MaxRows != RowCount.One)
            {
                return false;
            }
            if ((applyKind == OpType.CrossApply) && (applyRightChildNodeInfo.MinRows != RowCount.One))
            {
                return false;
            }
            return true;
        }

        private static bool ProcessApplyIntoScalarSubquery(RuleProcessingContext context, Node applyNode, out Node newNode)
        {
            Var var2;
            Command command = context.Command;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(applyNode.Child1);
            OpType opType = applyNode.Op.OpType;
            if (!CanRewriteApply(extendedNodeInfo, opType))
            {
                newNode = applyNode;
                return false;
            }
            ExtendedNodeInfo info2 = command.GetExtendedNodeInfo(applyNode.Child0);
            Var first = extendedNodeInfo.Definitions.First;
            VarVec vars = command.CreateVarVec(info2.Definitions);
            Node definingExpr = command.CreateNode(command.CreateElementOp(first.Type), applyNode.Child1);
            Node node2 = command.CreateVarDefListNode(definingExpr, out var2);
            vars.Set(var2);
            newNode = command.CreateNode(command.CreateProjectOp(vars), applyNode.Child0, node2);
            ((TransformationRulesContext) context).AddVarMapping(first, var2, applyNode.Child1);
            return true;
        }

        private static bool ProcessApplyOverAnything(RuleProcessingContext context, Node applyNode, out Node newNode)
        {
            newNode = applyNode;
            Node n = applyNode.Child0;
            Node node2 = applyNode.Child1;
            ApplyBaseOp op = (ApplyBaseOp) applyNode.Op;
            Command command = context.Command;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(node2);
            ExtendedNodeInfo info2 = command.GetExtendedNodeInfo(n);
            bool flag = false;
            if ((op.OpType == OpType.OuterApply) && (extendedNodeInfo.MinRows >= RowCount.One))
            {
                op = command.CreateCrossApplyOp();
                flag = true;
            }
            if (extendedNodeInfo.ExternalReferences.Overlaps(info2.Definitions))
            {
                if (flag)
                {
                    newNode = command.CreateNode(op, n, node2);
                    return true;
                }
                return false;
            }
            if (op.OpType == OpType.CrossApply)
            {
                newNode = command.CreateNode(command.CreateCrossJoinOp(), n, node2);
            }
            else
            {
                LeftOuterJoinOp op2 = command.CreateLeftOuterJoinOp();
                ConstantPredicateOp op3 = command.CreateTrueOp();
                Node node3 = command.CreateNode(op3);
                newNode = command.CreateNode(op2, n, node2, node3);
            }
            return true;
        }

        private static bool ProcessApplyOverFilter(RuleProcessingContext context, Node applyNode, out Node newNode)
        {
            newNode = applyNode;
            Node node = applyNode.Child1;
            Command command = context.Command;
            NodeInfo nodeInfo = command.GetNodeInfo(node.Child0);
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(applyNode.Child0);
            if (nodeInfo.ExternalReferences.Overlaps(extendedNodeInfo.Definitions))
            {
                return false;
            }
            JoinBaseOp op = null;
            if (applyNode.Op.OpType == OpType.CrossApply)
            {
                op = command.CreateInnerJoinOp();
            }
            else
            {
                op = command.CreateLeftOuterJoinOp();
            }
            newNode = command.CreateNode(op, applyNode.Child0, node.Child0, node.Child1);
            return true;
        }

        private static bool ProcessCrossApplyOverLeftOuterJoinOverSingleRowTable(RuleProcessingContext context, Node applyNode, out Node newNode)
        {
            newNode = applyNode;
            Node node = applyNode.Child1;
            ConstantPredicateOp op = (ConstantPredicateOp) node.Child2.Op;
            if (op.IsFalse)
            {
                return false;
            }
            applyNode.Op = context.Command.CreateOuterApplyOp();
            applyNode.Child1 = node.Child1;
            return true;
        }

        private static bool ProcessCrossApplyOverProject(RuleProcessingContext context, Node applyNode, out Node newNode)
        {
            newNode = applyNode;
            Node node = applyNode.Child1;
            ProjectOp op = (ProjectOp) node.Op;
            Command command = context.Command;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(applyNode);
            VarVec other = command.CreateVarVec(op.Outputs);
            other.Or(extendedNodeInfo.Definitions);
            op.Outputs.InitFrom(other);
            applyNode.Child1 = node.Child0;
            context.Command.RecomputeNodeInfo(applyNode);
            node.Child0 = applyNode;
            newNode = node;
            return true;
        }

        private static bool ProcessOuterApplyOverDummyProjectOverFilter(RuleProcessingContext context, Node applyNode, out Node newNode)
        {
            newNode = applyNode;
            Node n = applyNode.Child1;
            Node node2 = n.Child0;
            Node node3 = node2.Child0;
            Command command = context.Command;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(node3);
            ExtendedNodeInfo info2 = command.GetExtendedNodeInfo(applyNode.Child0);
            if (extendedNodeInfo.ExternalReferences.Overlaps(info2.Definitions))
            {
                return false;
            }
            ProjectOp op = (ProjectOp) n.Op;
            bool flag = false;
            Node node4 = null;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Var nonNullableVar = TransformationRulesContext.GetNonNullableVar(node3);
            if (nonNullableVar != null)
            {
                flag = true;
                Node node5 = n.Child1.Child0;
                node5.Child0 = context2.BuildNullIfExpression(nonNullableVar, node5.Child0);
                node4 = node3;
            }
            else
            {
                node4 = n;
                foreach (Var var2 in command.GetNodeInfo(node2.Child1).ExternalReferences)
                {
                    if (extendedNodeInfo.Definitions.IsSet(var2))
                    {
                        op.Outputs.Set(var2);
                    }
                }
                n.Child0 = node3;
                context.Command.RecomputeNodeInfo(n);
            }
            Node node6 = command.CreateNode(command.CreateLeftOuterJoinOp(), applyNode.Child0, node4, node2.Child1);
            if (flag)
            {
                ExtendedNodeInfo info4 = command.GetExtendedNodeInfo(node6);
                n.Child0 = node6;
                op.Outputs.Or(info4.Definitions);
                newNode = n;
            }
            else
            {
                newNode = node6;
            }
            return true;
        }

        private static bool ProcessOuterApplyOverProject(RuleProcessingContext context, Node applyNode, out Node newNode)
        {
            newNode = applyNode;
            Node node = applyNode.Child1;
            Node node2 = node.Child1;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Var nonNullableVar = TransformationRulesContext.GetNonNullableVar(node.Child0);
            if (((nonNullableVar == null) && (node2.Children.Count == 1)) && (node2.Child0.Child0.Op.OpType == OpType.InternalConstant))
            {
                return false;
            }
            Command command = context.Command;
            Node node3 = null;
            InternalConstantOp op = null;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(node.Child0);
            foreach (Node node4 in node2.Children)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(node4.Op.OpType == OpType.VarDef, "Expected VarDefOp. Found " + node4.Op.OpType + " instead");
                VarRefOp op2 = node4.Child0.Op as VarRefOp;
                if ((op2 == null) || !extendedNodeInfo.Definitions.IsSet(op2.Var))
                {
                    Node node7;
                    if (nonNullableVar == null)
                    {
                        op = command.CreateInternalConstantOp(command.IntegerType, 1);
                        Node definingExpr = command.CreateNode(op);
                        Node node6 = command.CreateVarDefListNode(definingExpr, out nonNullableVar);
                        ProjectOp op3 = command.CreateProjectOp(nonNullableVar);
                        op3.Outputs.Or(extendedNodeInfo.Definitions);
                        node3 = command.CreateNode(op3, node.Child0, node6);
                    }
                    if ((op != null) && (op.IsEquivalent(node4.Child0.Op) == true))
                    {
                        node7 = command.CreateNode(command.CreateVarRefOp(nonNullableVar));
                    }
                    else
                    {
                        node7 = context2.BuildNullIfExpression(nonNullableVar, node4.Child0);
                    }
                    node4.Child0 = node7;
                }
            }
            applyNode.Child1 = (node3 != null) ? node3 : node.Child0;
            command.RecomputeNodeInfo(applyNode);
            node.Child0 = applyNode;
            ExtendedNodeInfo info2 = command.GetExtendedNodeInfo(applyNode.Child0);
            ProjectOp op4 = (ProjectOp) node.Op;
            op4.Outputs.Or(info2.Definitions);
            newNode = node;
            return true;
        }
    }
}

