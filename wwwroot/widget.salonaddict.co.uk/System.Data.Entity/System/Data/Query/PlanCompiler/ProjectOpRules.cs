namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class ProjectOpRules
    {
        internal static readonly SimpleRule Rule_ProjectOpWithSimpleVarRedefinitions = new SimpleRule(OpType.Project, new Rule.ProcessNodeDelegate(ProjectOpRules.ProcessProjectWithSimpleVarRedefinitions));
        internal static readonly PatternMatchRule Rule_ProjectOverProject = new PatternMatchRule(new Node(ProjectOp.Pattern, new Node[] { new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(LeafOp.Pattern, new Node[0]) }), new Node(LeafOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ProjectOpRules.ProcessProjectOverProject));
        internal static readonly PatternMatchRule Rule_ProjectWithNoLocalDefs = new PatternMatchRule(new Node(ProjectOp.Pattern, new Node[] { new Node(LeafOp.Pattern, new Node[0]), new Node(VarDefListOp.Pattern, new Node[0]) }), new Rule.ProcessNodeDelegate(ProjectOpRules.ProcessProjectWithNoLocalDefinitions));
        internal static readonly Rule[] Rules = new Rule[] { Rule_ProjectOpWithSimpleVarRedefinitions, Rule_ProjectOverProject, Rule_ProjectWithNoLocalDefs };

        private static bool ProcessProjectOverProject(RuleProcessingContext context, Node projectNode, out Node newNode)
        {
            newNode = projectNode;
            ProjectOp op1 = (ProjectOp) projectNode.Op;
            Node node = projectNode.Child1;
            Node node2 = projectNode.Child0;
            ProjectOp op2 = (ProjectOp) node2.Op;
            TransformationRulesContext context2 = (TransformationRulesContext) context;
            Dictionary<Var, int> varRefMap = new Dictionary<Var, int>();
            foreach (Node node3 in node.Children)
            {
                if (!context2.IsScalarOpTree(node3.Child0, varRefMap))
                {
                    return false;
                }
            }
            Dictionary<Var, Node> varMap = context2.GetVarMap(node2.Child1, varRefMap);
            if (varMap == null)
            {
                return false;
            }
            Node node4 = context2.Command.CreateNode(context2.Command.CreateVarDefListOp());
            foreach (Node node5 in node.Children)
            {
                node5.Child0 = context2.ReMap(node5.Child0, varMap);
                context2.Command.RecomputeNodeInfo(node5);
                node4.Children.Add(node5);
            }
            ExtendedNodeInfo extendedNodeInfo = context2.Command.GetExtendedNodeInfo(projectNode);
            foreach (Node node6 in node2.Child1.Children)
            {
                VarDefOp op = (VarDefOp) node6.Op;
                if (extendedNodeInfo.Definitions.IsSet(op.Var))
                {
                    node4.Children.Add(node6);
                }
            }
            projectNode.Child0 = node2.Child0;
            projectNode.Child1 = node4;
            return true;
        }

        private static bool ProcessProjectWithNoLocalDefinitions(RuleProcessingContext context, Node n, out Node newNode)
        {
            newNode = n;
            if (!context.Command.GetNodeInfo(n).ExternalReferences.IsEmpty)
            {
                return false;
            }
            newNode = n.Child0;
            return true;
        }

        private static bool ProcessProjectWithSimpleVarRedefinitions(RuleProcessingContext context, Node n, out Node newNode)
        {
            newNode = n;
            ProjectOp op = (ProjectOp) n.Op;
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
                        break;
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
    }
}

