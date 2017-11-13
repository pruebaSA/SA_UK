namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal class ProjectionPruner : BasicOpVisitorOfNode
    {
        private System.Data.Query.PlanCompiler.PlanCompiler m_compilerState;
        private VarVec m_referencedVars;

        private ProjectionPruner(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            this.m_compilerState = compilerState;
            this.m_referencedVars = compilerState.Command.CreateVarVec();
        }

        private void AddReference(IEnumerable<Var> varSet)
        {
            foreach (Var var in varSet)
            {
                this.AddReference(var);
            }
        }

        private void AddReference(Var v)
        {
            this.m_referencedVars.Set(v);
        }

        private bool IsReferenced(Var v) => 
            this.m_referencedVars.IsSet(v);

        private bool IsUnreferenced(Var v) => 
            !this.IsReferenced(v);

        private Node Process(Node node) => 
            base.VisitNode(node);

        internal static void Process(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            compilerState.Command.Root = Process(compilerState, compilerState.Command.Root);
        }

        internal static Node Process(System.Data.Query.PlanCompiler.PlanCompiler compilerState, Node node)
        {
            ProjectionPruner pruner = new ProjectionPruner(compilerState);
            return pruner.Process(node);
        }

        private void PruneVarMap(VarMap varMap)
        {
            List<Var> list = new List<Var>();
            foreach (Var var in varMap.Keys)
            {
                if (!this.IsReferenced(var))
                {
                    list.Add(var);
                }
                else
                {
                    this.AddReference(varMap[var]);
                }
            }
            foreach (Var var2 in list)
            {
                varMap.Remove(var2);
            }
        }

        private void PruneVarSet(VarVec varSet)
        {
            varSet.And(this.m_referencedVars);
        }

        public override Node Visit(DistinctOp op, Node n)
        {
            this.AddReference(op.Keys);
            this.VisitChildren(n);
            return n;
        }

        public override Node Visit(ElementOp op, Node n)
        {
            ExtendedNodeInfo extendedNodeInfo = this.m_command.GetExtendedNodeInfo(n.Child0);
            this.AddReference(extendedNodeInfo.Definitions);
            n.Child0 = base.VisitNode(n.Child0);
            return n;
        }

        public override Node Visit(ExistsOp op, Node n)
        {
            ProjectOp op2 = (ProjectOp) n.Child0.Op;
            this.AddReference(op2.Outputs.First);
            this.VisitChildren(n);
            return n;
        }

        public override Node Visit(FilterOp op, Node n)
        {
            n.Child1 = base.VisitNode(n.Child1);
            n.Child0 = base.VisitNode(n.Child0);
            return n;
        }

        public override Node Visit(GroupByOp op, Node n)
        {
            this.AddReference(op.Keys);
            n.Child2 = base.VisitNode(n.Child2);
            n.Child1 = base.VisitNode(n.Child1);
            n.Child0 = base.VisitNode(n.Child0);
            this.PruneVarSet(op.Outputs);
            if ((op.Keys.Count == 0) && (op.Outputs.Count == 0))
            {
                return this.m_command.CreateNode(this.m_command.CreateSingleRowTableOp());
            }
            return n;
        }

        public override Node Visit(MultiStreamNestOp op, Node n) => 
            this.VisitNestOp(op, n);

        public override Node Visit(PhysicalProjectOp op, Node n)
        {
            if (n == this.m_command.Root)
            {
                ColumnMapVarTracker.FindVars(op.ColumnMap, this.m_referencedVars);
                op.Outputs.RemoveAll(new Predicate<Var>(this.IsUnreferenced));
            }
            else
            {
                this.AddReference(op.Outputs);
            }
            this.VisitChildren(n);
            return n;
        }

        public override Node Visit(ProjectOp op, Node n)
        {
            this.PruneVarSet(op.Outputs);
            n.Child1 = base.VisitNode(n.Child1);
            n.Child0 = base.VisitNode(n.Child0);
            if (!op.Outputs.IsEmpty)
            {
                return n;
            }
            return n.Child0;
        }

        public override Node Visit(ScanTableOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!n.HasChild0, "scanTable with an input?");
            op.Table.ReferencedColumns.And(this.m_referencedVars);
            return n;
        }

        public override Node Visit(SingleStreamNestOp op, Node n)
        {
            this.AddReference(op.Discriminator);
            return this.VisitNestOp(op, n);
        }

        public override Node Visit(UnnestOp op, Node n)
        {
            this.AddReference(op.Var);
            this.VisitChildren(n);
            return n;
        }

        public override Node Visit(VarDefListOp op, Node n)
        {
            List<Node> args = new List<Node>();
            foreach (Node node in n.Children)
            {
                VarDefOp op2 = node.Op as VarDefOp;
                if (this.IsReferenced(op2.Var))
                {
                    args.Add(base.VisitNode(node));
                }
            }
            return this.m_command.CreateNode(op, args);
        }

        public override Node Visit(VarRefOp op, Node n)
        {
            this.AddReference(op.Var);
            return n;
        }

        protected override Node VisitApplyOp(ApplyBaseOp op, Node n)
        {
            n.Child1 = base.VisitNode(n.Child1);
            n.Child0 = base.VisitNode(n.Child0);
            return n;
        }

        protected override Node VisitJoinOp(JoinBaseOp op, Node n)
        {
            if (n.Op.OpType == OpType.CrossJoin)
            {
                this.VisitChildren(n);
                return n;
            }
            n.Child2 = base.VisitNode(n.Child2);
            n.Child0 = base.VisitNode(n.Child0);
            n.Child1 = base.VisitNode(n.Child1);
            return n;
        }

        protected override Node VisitNestOp(NestBaseOp op, Node n)
        {
            this.AddReference(op.Outputs);
            this.VisitChildren(n);
            return n;
        }

        protected override Node VisitSetOp(SetOp op, Node n)
        {
            if ((OpType.Intersect == op.OpType) || (OpType.Except == op.OpType))
            {
                this.AddReference(op.Outputs);
            }
            this.PruneVarSet(op.Outputs);
            foreach (VarMap map in op.VarMap)
            {
                this.PruneVarMap(map);
            }
            this.VisitChildren(n);
            return n;
        }

        protected override Node VisitSortOp(SortBaseOp op, Node n)
        {
            foreach (SortKey key in op.Keys)
            {
                this.AddReference(key.Var);
            }
            if (n.HasChild1)
            {
                n.Child1 = base.VisitNode(n.Child1);
            }
            n.Child0 = base.VisitNode(n.Child0);
            return n;
        }

        private Command m_command =>
            this.m_compilerState.Command;

        private class ColumnMapVarTracker : ColumnMapVisitor<VarVec>
        {
            private ColumnMapVarTracker()
            {
            }

            internal static void FindVars(ColumnMap columnMap, VarVec vec)
            {
                ProjectionPruner.ColumnMapVarTracker visitor = new ProjectionPruner.ColumnMapVarTracker();
                columnMap.Accept<VarVec>(visitor, vec);
            }

            internal override void Visit(VarRefColumnMap columnMap, VarVec arg)
            {
                arg.Set(columnMap.Var);
                base.Visit(columnMap, arg);
            }
        }
    }
}

