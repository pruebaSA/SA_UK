namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Entity;

    internal abstract class BasicOpVisitor
    {
        internal BasicOpVisitor()
        {
        }

        public virtual void Visit(AggregateOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(ArithmeticOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(CaseOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(CastOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(CollectOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(ComparisonOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(ConditionalOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(ConstantOp op, Node n)
        {
            this.VisitConstantOp(op, n);
        }

        public virtual void Visit(ConstantPredicateOp op, Node n)
        {
            this.VisitConstantOp(op, n);
        }

        public virtual void Visit(ConstrainedSortOp op, Node n)
        {
            this.VisitSortOp(op, n);
        }

        public virtual void Visit(CrossApplyOp op, Node n)
        {
            this.VisitApplyOp(op, n);
        }

        public virtual void Visit(CrossJoinOp op, Node n)
        {
            this.VisitJoinOp(op, n);
        }

        public virtual void Visit(DerefOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(DiscriminatedNewEntityOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(DistinctOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        public virtual void Visit(ElementOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(ExceptOp op, Node n)
        {
            this.VisitSetOp(op, n);
        }

        public virtual void Visit(ExistsOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(FilterOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        public virtual void Visit(FullOuterJoinOp op, Node n)
        {
            this.VisitJoinOp(op, n);
        }

        public virtual void Visit(FunctionOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(GetEntityRefOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(GetRefKeyOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(GroupByOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        public virtual void Visit(InnerJoinOp op, Node n)
        {
            this.VisitJoinOp(op, n);
        }

        public virtual void Visit(InternalConstantOp op, Node n)
        {
            this.VisitConstantOp(op, n);
        }

        public virtual void Visit(IntersectOp op, Node n)
        {
            this.VisitSetOp(op, n);
        }

        public virtual void Visit(IsOfOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(LeftOuterJoinOp op, Node n)
        {
            this.VisitJoinOp(op, n);
        }

        public virtual void Visit(LikeOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(MultiStreamNestOp op, Node n)
        {
            this.VisitNestOp(op, n);
        }

        public virtual void Visit(NavigateOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(NewEntityOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(NewInstanceOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(NewMultisetOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(NewRecordOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(NullOp op, Node n)
        {
            this.VisitConstantOp(op, n);
        }

        public virtual void Visit(Op op, Node n)
        {
            throw new NotSupportedException(Strings.Iqt_General_UnsupportedOp(op.GetType().FullName));
        }

        public virtual void Visit(OuterApplyOp op, Node n)
        {
            this.VisitApplyOp(op, n);
        }

        public virtual void Visit(PhysicalProjectOp op, Node n)
        {
            this.VisitPhysicalOpDefault(op, n);
        }

        public virtual void Visit(ProjectOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        public virtual void Visit(PropertyOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(RefOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(RelPropertyOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(ScanTableOp op, Node n)
        {
            this.VisitTableOp(op, n);
        }

        public virtual void Visit(ScanViewOp op, Node n)
        {
            this.VisitTableOp(op, n);
        }

        public virtual void Visit(SingleRowOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        public virtual void Visit(SingleRowTableOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        public virtual void Visit(SingleStreamNestOp op, Node n)
        {
            this.VisitNestOp(op, n);
        }

        public virtual void Visit(SoftCastOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(SortOp op, Node n)
        {
            this.VisitSortOp(op, n);
        }

        public virtual void Visit(TreatOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        public virtual void Visit(UnionAllOp op, Node n)
        {
            this.VisitSetOp(op, n);
        }

        public virtual void Visit(UnnestOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        public virtual void Visit(VarDefListOp op, Node n)
        {
            this.VisitAncillaryOpDefault(op, n);
        }

        public virtual void Visit(VarDefOp op, Node n)
        {
            this.VisitAncillaryOpDefault(op, n);
        }

        public virtual void Visit(VarRefOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        protected virtual void VisitAncillaryOpDefault(AncillaryOp op, Node n)
        {
            this.VisitDefault(n);
        }

        protected virtual void VisitApplyOp(ApplyBaseOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        protected virtual void VisitChildren(Node n)
        {
            foreach (Node node in n.Children)
            {
                this.VisitNode(node);
            }
        }

        protected virtual void VisitConstantOp(ConstantBaseOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
        }

        protected virtual void VisitDefault(Node n)
        {
            this.VisitChildren(n);
        }

        protected virtual void VisitJoinOp(JoinBaseOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        protected virtual void VisitNestOp(NestBaseOp op, Node n)
        {
            this.VisitPhysicalOpDefault(op, n);
        }

        internal virtual void VisitNode(Node n)
        {
            n.Op.Accept(this, n);
        }

        protected virtual void VisitPhysicalOpDefault(PhysicalOp op, Node n)
        {
            this.VisitDefault(n);
        }

        protected virtual void VisitRelOpDefault(RelOp op, Node n)
        {
            this.VisitDefault(n);
        }

        protected virtual void VisitScalarOpDefault(ScalarOp op, Node n)
        {
            this.VisitDefault(n);
        }

        protected virtual void VisitSetOp(SetOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        protected virtual void VisitSortOp(SortBaseOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }

        protected virtual void VisitTableOp(ScanTableBaseOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
        }
    }
}

