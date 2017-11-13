namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class DistinctOpRules
    {
        internal static readonly SimpleRule Rule_DistinctOpOfKeys = new SimpleRule(OpType.Distinct, new Rule.ProcessNodeDelegate(DistinctOpRules.ProcessDistinctOpOfKeys));
        internal static readonly Rule[] Rules = new Rule[] { Rule_DistinctOpOfKeys };

        private static bool ProcessDistinctOpOfKeys(RuleProcessingContext context, Node n, out Node newNode)
        {
            Command command = context.Command;
            ExtendedNodeInfo extendedNodeInfo = command.GetExtendedNodeInfo(n.Child0);
            DistinctOp op = (DistinctOp) n.Op;
            if (!extendedNodeInfo.Keys.NoKeys && op.Keys.Subsumes(extendedNodeInfo.Keys.KeyVars))
            {
                ProjectOp op2 = command.CreateProjectOp(op.Keys);
                VarDefListOp op3 = command.CreateVarDefListOp();
                Node node = command.CreateNode(op3);
                newNode = command.CreateNode(op2, n.Child0, node);
                return true;
            }
            newNode = n;
            return false;
        }
    }
}

