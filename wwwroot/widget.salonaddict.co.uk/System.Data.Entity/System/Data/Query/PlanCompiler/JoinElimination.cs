namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal class JoinElimination : BasicOpVisitorOfNode
    {
        private System.Data.Query.PlanCompiler.PlanCompiler m_compilerState;
        private ConstraintManager m_constraintManager = new ConstraintManager();
        private Dictionary<Node, Node> m_joinGraphUnnecessaryMap;
        private bool m_treeModified;
        private VarRefManager m_varRefManager;
        private VarRemapper m_varRemapper;

        private JoinElimination(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            this.m_compilerState = compilerState;
        }

        private void Initialize()
        {
            this.m_joinGraphUnnecessaryMap = new Dictionary<Node, Node>();
            this.m_varRemapper = new VarRemapper(this.m_compilerState.Command);
            this.m_varRefManager = new VarRefManager(this.m_compilerState.Command);
            this.m_treeModified = false;
        }

        private void InitializeAndProcess()
        {
            this.Initialize();
            this.Process();
        }

        private bool NeedsJoinGraph(Node joinNode) => 
            !this.m_joinGraphUnnecessaryMap.ContainsKey(joinNode);

        private void Process()
        {
            this.Command.Root = base.VisitNode(this.Command.Root);
        }

        internal static void Process(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            JoinElimination elimination = new JoinElimination(compilerState);
            elimination.InitializeAndProcess();
            if (elimination.m_treeModified)
            {
                compilerState.MarkPhaseAsNeeded(PlanCompilerPhase.Transformations);
                elimination.InitializeAndProcess();
            }
        }

        private Node ProcessJoinGraph(Node joinNode)
        {
            VarMap map;
            Dictionary<Node, Node> dictionary;
            Node node = new JoinGraph(this.Command, this.m_constraintManager, this.m_varRefManager, joinNode).DoJoinElimination(out map, out dictionary);
            foreach (KeyValuePair<Var, Var> pair in map)
            {
                this.m_varRemapper.AddMapping(pair.Key, pair.Value);
            }
            foreach (Node node2 in dictionary.Keys)
            {
                this.m_joinGraphUnnecessaryMap[node2] = node2;
            }
            return node;
        }

        protected override Node VisitDefault(Node n)
        {
            this.m_varRefManager.AddChildren(n);
            return this.VisitDefaultForAllNodes(n);
        }

        private Node VisitDefaultForAllNodes(Node n)
        {
            this.VisitChildren(n);
            this.m_varRemapper.RemapNode(n);
            this.Command.RecomputeNodeInfo(n);
            return n;
        }

        protected override Node VisitJoinOp(JoinBaseOp op, Node joinNode)
        {
            Node node;
            if (this.NeedsJoinGraph(joinNode))
            {
                node = this.ProcessJoinGraph(joinNode);
                if (node != joinNode)
                {
                    this.m_treeModified = true;
                }
            }
            else
            {
                node = joinNode;
            }
            return this.VisitDefaultForAllNodes(node);
        }

        private System.Data.Query.InternalTrees.Command Command =>
            this.m_compilerState.Command;
    }
}

