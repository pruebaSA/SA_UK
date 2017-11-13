namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class GroupByOpRules
    {
        internal static readonly SimpleRule Rule_GroupByOpWithSimpleVarRedefinitions = new SimpleRule(OpType.GroupBy, new Rule.ProcessNodeDelegate(GroupByOpRules.ProcessGroupByWithSimpleVarRedefinitions));
        internal static readonly PatternMatchRule Rule_GroupByOverProject = new PatternMatchRule(new Node(GroupByOp.Pattern, new Node[] { new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(GroupByOpRules.ProcessGroupByOverProject));
        internal static readonly Rule[] Rules = new Rule[] { Rule_GroupByOpWithSimpleVarRedefinitions, Rule_GroupByOverProject };

        private static bool ProcessGroupByOverProject(RuleProcessingContext context, Node n, out Node newNode)
        {
            newNode = n;
            GroupByOp op = (GroupByOp) n.Op;
            Command command = ((TransformationRulesContext) context).Command;
            Node node = n.Child0;
            Node node2 = node.Child1;
            Node node3 = n.Child1;
            Node root = n.Child2;
            if (node3.Children.Count > 0)
            {
                return false;
            }
            VarVec localDefinitions = command.GetExtendedNodeInfo(node).LocalDefinitions;
            if (op.Outputs.Overlaps(localDefinitions))
            {
                return false;
            }
            bool flag = false;
            for (int i = 0; i < node2.Children.Count; i++)
            {
                Node node5 = node2.Children[i];
                if ((node5.Child0.Op.OpType == OpType.Constant) || (node5.Child0.Op.OpType == OpType.InternalConstant))
                {
                    if (!flag)
                    {
                        localDefinitions = command.CreateVarVec(localDefinitions);
                        flag = true;
                    }
                    localDefinitions.Clear(((VarDefOp) node5.Op).Var);
                }
            }
            if (VarRefUsageFinder.AnyVarUsedMoreThanOnce(localDefinitions, root, command))
            {
                return false;
            }
            Dictionary<Var, Node> varReplacementTable = new Dictionary<Var, Node>(node2.Children.Count);
            for (int j = 0; j < node2.Children.Count; j++)
            {
                Node node6 = node2.Children[j];
                Var key = ((VarDefOp) node6.Op).Var;
                varReplacementTable.Add(key, node6.Child0);
            }
            newNode.Child2 = VarRefReplacer.Replace(varReplacementTable, root, command);
            newNode.Child0 = node.Child0;
            return true;
        }

        private static bool ProcessGroupByWithSimpleVarRedefinitions(RuleProcessingContext context, Node n, out Node newNode)
        {
            newNode = n;
            GroupByOp op = (GroupByOp) n.Op;
            if (n.Child1.Children.Count == 0)
            {
                return false;
            }
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Command command = context2.Command;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(n);
            bool flag = false;
            foreach (Node node in n.Child1.Children)
            {
                Node node2 = node.Child0;
                if (node2.Op.OpType == OpType.VarRef)
                {
                    VarRefOp op2 = (VarRefOp) node2.Op;
                    if (!extendedNodeInfo.ExternalReferences.IsSet(op2.Var))
                    {
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                return false;
            }
            List<Node> args = new List<Node>();
            foreach (Node node3 in n.Child1.Children)
            {
                VarDefOp op3 = (VarDefOp) node3.Op;
                VarRefOp op4 = node3.Child0.Op as VarRefOp;
                if ((op4 != null) && !extendedNodeInfo.ExternalReferences.IsSet(op4.Var))
                {
                    op.Outputs.Clear(op3.Var);
                    op.Outputs.Set(op4.Var);
                    op.Keys.Clear(op3.Var);
                    op.Keys.Set(op4.Var);
                    context2.AddVarMapping(op3.Var, op4.Var);
                }
                else
                {
                    args.Add(node3);
                }
            }
            Node node4 = command.CreateNode(command.CreateVarDefListOp(), args);
            n.Child1 = node4;
            return true;
        }

        internal class VarRefReplacer : BasicOpVisitorOfNode
        {
            private Command m_command;
            private Dictionary<Var, Node> m_varReplacementTable;

            private VarRefReplacer(Dictionary<Var, Node> varReplacementTable, Command command)
            {
                this.m_varReplacementTable = varReplacementTable;
                this.m_command = command;
            }

            internal static Node Replace(Dictionary<Var, Node> varReplacementTable, Node root, Command command)
            {
                GroupByOpRules.VarRefReplacer replacer = new GroupByOpRules.VarRefReplacer(varReplacementTable, command);
                return replacer.VisitNode(root);
            }

            public override Node Visit(VarRefOp op, Node n)
            {
                Node node;
                if (this.m_varReplacementTable.TryGetValue(op.Var, out node))
                {
                    return node;
                }
                return n;
            }

            protected override Node VisitDefault(Node n)
            {
                Node node = base.VisitDefault(n);
                this.m_command.RecomputeNodeInfo(node);
                return node;
            }
        }

        internal class VarRefUsageFinder : BasicOpVisitor
        {
            private bool m_anyUsedMoreThenOnce;
            private VarVec m_usedVars;
            private VarVec m_varVec;

            private VarRefUsageFinder(VarVec varVec, Command command)
            {
                this.m_varVec = varVec;
                this.m_usedVars = command.CreateVarVec();
            }

            internal static bool AnyVarUsedMoreThanOnce(VarVec varVec, Node root, Command command)
            {
                GroupByOpRules.VarRefUsageFinder finder = new GroupByOpRules.VarRefUsageFinder(varVec, command);
                finder.VisitNode(root);
                return finder.m_anyUsedMoreThenOnce;
            }

            public override void Visit(VarRefOp op, Node n)
            {
                Var v = op.Var;
                if (this.m_varVec.IsSet(v))
                {
                    if (this.m_usedVars.IsSet(v))
                    {
                        this.m_anyUsedMoreThenOnce = true;
                    }
                    else
                    {
                        this.m_usedVars.Set(v);
                    }
                }
            }

            protected override void VisitChildren(Node n)
            {
                if (!this.m_anyUsedMoreThenOnce)
                {
                    base.VisitChildren(n);
                }
            }
        }
    }
}

