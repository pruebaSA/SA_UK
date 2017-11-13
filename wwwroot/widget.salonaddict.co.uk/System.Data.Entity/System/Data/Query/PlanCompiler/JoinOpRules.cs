namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class JoinOpRules
    {
        internal static readonly PatternMatchRule Rule_CrossJoinOverFilter1 = new PatternMatchRule(new Node(CrossJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
        internal static readonly PatternMatchRule Rule_CrossJoinOverFilter2 = new PatternMatchRule(new Node(CrossJoinOp.Pattern, new Node[] { new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
        internal static readonly PatternMatchRule Rule_CrossJoinOverProject1 = new PatternMatchRule(new Node(CrossJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
        internal static readonly PatternMatchRule Rule_CrossJoinOverProject2 = new PatternMatchRule(new Node(CrossJoinOp.Pattern, new Node[] { new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
        internal static readonly PatternMatchRule Rule_CrossJoinOverSingleRowTable1 = new PatternMatchRule(new Node(CrossJoinOp.Pattern, new Node[] { new Node(SingleRowTableOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverSingleRowTable));
        internal static readonly PatternMatchRule Rule_CrossJoinOverSingleRowTable2 = new PatternMatchRule(new Node(CrossJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(SingleRowTableOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverSingleRowTable));
        internal static readonly PatternMatchRule Rule_InnerJoinOverFilter1 = new PatternMatchRule(new Node(InnerJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
        internal static readonly PatternMatchRule Rule_InnerJoinOverFilter2 = new PatternMatchRule(new Node(InnerJoinOp.Pattern, new Node[] { new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
        internal static readonly PatternMatchRule Rule_InnerJoinOverProject1 = new PatternMatchRule(new Node(InnerJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
        internal static readonly PatternMatchRule Rule_InnerJoinOverProject2 = new PatternMatchRule(new Node(InnerJoinOp.Pattern, new Node[] { new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
        internal static readonly PatternMatchRule Rule_LeftOuterJoinOverSingleRowTable = new PatternMatchRule(new Node(LeftOuterJoinOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(SingleRowTableOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverSingleRowTable));
        internal static readonly PatternMatchRule Rule_OuterJoinOverFilter2 = new PatternMatchRule(new Node(LeftOuterJoinOp.Pattern, new Node[] { new Node(FilterOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverFilter));
        internal static readonly PatternMatchRule Rule_OuterJoinOverProject2 = new PatternMatchRule(new Node(LeftOuterJoinOp.Pattern, new Node[] { new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(JoinOpRules.ProcessJoinOverProject));
        internal static readonly Rule[] Rules = new Rule[] { Rule_CrossJoinOverProject1, Rule_CrossJoinOverProject2, Rule_InnerJoinOverProject1, Rule_InnerJoinOverProject2, Rule_OuterJoinOverProject2, Rule_CrossJoinOverFilter1, Rule_CrossJoinOverFilter2, Rule_InnerJoinOverFilter1, Rule_InnerJoinOverFilter2, Rule_OuterJoinOverFilter2, Rule_CrossJoinOverSingleRowTable1, Rule_CrossJoinOverSingleRowTable2, Rule_LeftOuterJoinOverSingleRowTable };

        private static bool ProcessJoinOverFilter(RuleProcessingContext context, Node joinNode, out Node newNode)
        {
            Node node4;
            newNode = joinNode;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Command command = context2.Command;
            Node node = null;
            Node node2 = joinNode.Child0;
            if (joinNode.Child0.Op.OpType == OpType.Filter)
            {
                node = joinNode.Child0.Child1;
                node2 = joinNode.Child0.Child0;
            }
            Node node3 = joinNode.Child1;
            if ((joinNode.Child1.Op.OpType == OpType.Filter) && (joinNode.Op.OpType != OpType.LeftOuterJoin))
            {
                if (node == null)
                {
                    node = joinNode.Child1.Child1;
                }
                else
                {
                    node = command.CreateNode(command.CreateConditionalOp(OpType.And), node, joinNode.Child1.Child1);
                }
                node3 = joinNode.Child1.Child0;
            }
            if (node == null)
            {
                return false;
            }
            if (joinNode.Op.OpType == OpType.CrossJoin)
            {
                node4 = command.CreateNode(joinNode.Op, node2, node3);
            }
            else
            {
                node4 = command.CreateNode(joinNode.Op, node2, node3, joinNode.Child2);
            }
            FilterOp op = command.CreateFilterOp();
            newNode = command.CreateNode(op, node4, node);
            context2.SuppressFilterPushdown(newNode);
            return true;
        }

        private static bool ProcessJoinOverProject(RuleProcessingContext context, Node joinNode, out Node newNode)
        {
            newNode = joinNode;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Command command = context2.Command;
            Node node = joinNode.HasChild2 ? joinNode.Child2 : null;
            Dictionary<Var, int> varRefMap = new Dictionary<Var, int>();
            if ((node != null) && !context2.IsScalarOpTree(node, varRefMap))
            {
                return false;
            }
            VarVec vars = command.CreateVarVec();
            List<Node> args = new List<Node>();
            if (((joinNode.Op.OpType != OpType.LeftOuterJoin) && (joinNode.Child0.Op.OpType == OpType.Project)) && (joinNode.Child1.Op.OpType == OpType.Project))
            {
                Node node2;
                ProjectOp op = (ProjectOp) joinNode.Child0.Op;
                ProjectOp op2 = (ProjectOp) joinNode.Child1.Op;
                Dictionary<Var, Node> dictionary2 = context2.GetVarMap(joinNode.Child0.Child1, varRefMap);
                Dictionary<Var, Node> dictionary3 = context2.GetVarMap(joinNode.Child1.Child1, varRefMap);
                if ((dictionary2 == null) || (dictionary3 == null))
                {
                    return false;
                }
                if (node != null)
                {
                    node = context2.ReMap(node, dictionary2);
                    node = context2.ReMap(node, dictionary3);
                    node2 = context.Command.CreateNode(joinNode.Op, joinNode.Child0.Child0, joinNode.Child1.Child0, node);
                }
                else
                {
                    node2 = context.Command.CreateNode(joinNode.Op, joinNode.Child0.Child0, joinNode.Child1.Child0);
                }
                vars.InitFrom(op.Outputs);
                foreach (Var var in op2.Outputs)
                {
                    vars.Set(var);
                }
                ProjectOp op3 = command.CreateProjectOp(vars);
                args.AddRange(joinNode.Child0.Child1.Children);
                args.AddRange(joinNode.Child1.Child1.Children);
                Node node4 = command.CreateNode(command.CreateVarDefListOp(), args);
                Node node3 = command.CreateNode(op3, node2, node4);
                newNode = node3;
                return true;
            }
            int num = -1;
            int num2 = -1;
            if (joinNode.Child0.Op.OpType == OpType.Project)
            {
                num = 0;
                num2 = 1;
            }
            else
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(joinNode.Op.OpType != OpType.LeftOuterJoin, "unexpected non-LeftOuterJoin");
                num = 1;
                num2 = 0;
            }
            Node node5 = joinNode.Children[num];
            ProjectOp op4 = node5.Op as ProjectOp;
            Dictionary<Var, Node> varMap = context2.GetVarMap(node5.Child1, varRefMap);
            if (varMap == null)
            {
                return false;
            }
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(joinNode.Children[num2]);
            VarVec other = command.CreateVarVec(op4.Outputs);
            other.Or(extendedNodeInfo.Definitions);
            op4.Outputs.InitFrom(other);
            if (node != null)
            {
                node = context2.ReMap(node, varMap);
                joinNode.Child2 = node;
            }
            joinNode.Children[num] = node5.Child0;
            context.Command.RecomputeNodeInfo(joinNode);
            newNode = context.Command.CreateNode(op4, joinNode, node5.Child1);
            return true;
        }

        private static bool ProcessJoinOverSingleRowTable(RuleProcessingContext context, Node joinNode, out Node newNode)
        {
            newNode = joinNode;
            if (joinNode.Child0.Op.OpType == OpType.SingleRowTable)
            {
                newNode = joinNode.Child1;
            }
            else
            {
                newNode = joinNode.Child0;
            }
            return true;
        }
    }
}

