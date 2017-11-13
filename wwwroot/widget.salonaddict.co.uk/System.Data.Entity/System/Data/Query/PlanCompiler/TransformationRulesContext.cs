namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal class TransformationRulesContext : RuleProcessingContext
    {
        private System.Data.Query.PlanCompiler.PlanCompiler m_compilerState;
        private VarVec m_remappedVars;
        private ScopedVarRemapper m_remapper;
        private Dictionary<Node, Node> m_suppressions;

        internal TransformationRulesContext(System.Data.Query.PlanCompiler.PlanCompiler compilerState) : base(compilerState.Command)
        {
            this.m_compilerState = compilerState;
            this.m_remapper = new ScopedVarRemapper(compilerState.Command);
            this.m_suppressions = new Dictionary<Node, Node>();
            this.m_remappedVars = compilerState.Command.CreateVarVec();
        }

        internal void AddVarMapping(Var oldVar, Var newVar)
        {
            this.m_remapper.AddMapping(oldVar, newVar);
            this.m_remappedVars.Set(oldVar);
        }

        internal void AddVarMapping(Var oldVar, Var newVar, Node hidingScopeNode)
        {
            this.m_remapper.AddMapping(oldVar, newVar, hidingScopeNode);
            this.m_remappedVars.Set(oldVar);
        }

        internal Node BuildNullIfExpression(Var conditionVar, Node expr)
        {
            VarRefOp op = base.Command.CreateVarRefOp(conditionVar);
            Node node = base.Command.CreateNode(op);
            Node node2 = base.Command.CreateNode(base.Command.CreateConditionalOp(OpType.IsNull), node);
            Node node3 = expr;
            Node node4 = base.Command.CreateNode(base.Command.CreateNullOp(node3.Op.Type));
            return base.Command.CreateNode(base.Command.CreateCaseOp(node3.Op.Type), node2, node4, node3);
        }

        internal Node Copy(Node node)
        {
            if (node.Op.OpType == OpType.VarRef)
            {
                VarRefOp op = node.Op as VarRefOp;
                return base.Command.CreateNode(base.Command.CreateVarRefOp(op.Var));
            }
            return OpCopier.Copy(base.Command, node);
        }

        internal override int GetHashCode(Node node) => 
            base.Command.GetNodeInfo(node).HashValue;

        internal static Var GetNonNullableVar(Node subTree)
        {
            ScanTableOp op = null;
            if (subTree.Op.OpType == OpType.ScanTable)
            {
                op = (ScanTableOp) subTree.Op;
            }
            else if ((subTree.Op.OpType == OpType.Filter) && (subTree.Child0.Op.OpType == OpType.ScanTable))
            {
                op = (ScanTableOp) subTree.Child0.Op;
            }
            else
            {
                return null;
            }
            foreach (ColumnVar var in op.Table.ReferencedColumns)
            {
                if (!var.ColumnMetadata.IsNullable)
                {
                    return var;
                }
            }
            return null;
        }

        internal Dictionary<Var, Node> GetVarMap(Node varDefListNode, Dictionary<Var, int> varRefMap)
        {
            VarDefListOp op1 = (VarDefListOp) varDefListNode.Op;
            Dictionary<Var, Node> dictionary = new Dictionary<Var, Node>();
            foreach (Node node in varDefListNode.Children)
            {
                Node node2;
                VarDefOp op = (VarDefOp) node.Op;
                int nonLeafNodeCount = 0;
                int num2 = 0;
                if (!this.IsScalarOpTree(node.Child0, null, ref nonLeafNodeCount))
                {
                    return null;
                }
                if (((nonLeafNodeCount > 100) && (varRefMap != null)) && (varRefMap.TryGetValue(op.Var, out num2) && (num2 > 2)))
                {
                    return null;
                }
                if (dictionary.TryGetValue(op.Var, out node2))
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(node2 == node.Child0, "reusing varDef for different Node?");
                }
                else
                {
                    dictionary.Add(op.Var, node.Child0);
                }
            }
            return dictionary;
        }

        internal bool IsFilterPushdownSuppressed(Node n) => 
            this.m_suppressions.ContainsKey(n);

        internal bool IsScalarOpTree(Node node)
        {
            int nonLeafNodeCount = 0;
            return this.IsScalarOpTree(node, null, ref nonLeafNodeCount);
        }

        internal bool IsScalarOpTree(Node node, Dictionary<Var, int> varRefMap)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(varRefMap != null, "Null varRef map");
            int nonLeafNodeCount = 0;
            return this.IsScalarOpTree(node, varRefMap, ref nonLeafNodeCount);
        }

        private bool IsScalarOpTree(Node node, Dictionary<Var, int> varRefMap, ref int nonLeafNodeCount)
        {
            if (!node.Op.IsScalarOp)
            {
                return false;
            }
            if (node.HasChild0)
            {
                nonLeafNodeCount++;
            }
            if ((varRefMap != null) && (node.Op.OpType == OpType.VarRef))
            {
                int num;
                VarRefOp op = (VarRefOp) node.Op;
                if (!varRefMap.TryGetValue(op.Var, out num))
                {
                    num = 1;
                }
                else
                {
                    num++;
                }
                varRefMap[op.Var] = num;
            }
            foreach (Node node2 in node.Children)
            {
                if (!this.IsScalarOpTree(node2, varRefMap, ref nonLeafNodeCount))
                {
                    return false;
                }
            }
            return true;
        }

        internal override void PostProcess(Node n, Rule rule)
        {
            if (rule != null)
            {
                if (TransformationRules.RulesRequiringProjectionPruning.Contains(rule))
                {
                    this.m_compilerState.MarkPhaseAsNeeded(PlanCompilerPhase.ProjectionPruning);
                }
                base.Command.RecomputeNodeInfo(n);
            }
        }

        internal override void PreProcess(Node n)
        {
            this.m_remapper.RemapNode(n);
            base.Command.RecomputeNodeInfo(n);
        }

        internal override void PreProcessSubTree(Node subTree)
        {
            if (!this.m_remappedVars.IsEmpty)
            {
                foreach (Var var in base.Command.GetNodeInfo(subTree).ExternalReferences)
                {
                    if (this.m_remappedVars.IsSet(var))
                    {
                        this.m_remapper.RemapSubtree(subTree);
                        break;
                    }
                }
            }
        }

        internal Node ReMap(Node node, Dictionary<Var, Node> varMap)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.Op.IsScalarOp, "Expected a scalarOp: Found " + Dump.AutoString.ToString(node.Op.OpType));
            if (node.Op.OpType == OpType.VarRef)
            {
                VarRefOp op = node.Op as VarRefOp;
                Node node2 = null;
                if (varMap.TryGetValue(op.Var, out node2))
                {
                    return this.Copy(node2);
                }
                return node;
            }
            for (int i = 0; i < node.Children.Count; i++)
            {
                node.Children[i] = this.ReMap(node.Children[i], varMap);
            }
            base.Command.RecomputeNodeInfo(node);
            return node;
        }

        internal void SuppressFilterPushdown(Node n)
        {
            this.m_suppressions[n] = n;
        }
    }
}

