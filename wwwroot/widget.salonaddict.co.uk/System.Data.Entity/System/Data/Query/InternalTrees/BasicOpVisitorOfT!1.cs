namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Query.PlanCompiler;

    internal abstract class BasicOpVisitorOfT<TResultType>
    {
        protected BasicOpVisitorOfT()
        {
        }

        internal virtual TResultType Unimplemented(Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "Not implemented op type");
            return default(TResultType);
        }

        public virtual TResultType Visit(AggregateOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(ArithmeticOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(CaseOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(CastOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(CollectOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(ComparisonOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(ConditionalOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(ConstantOp op, Node n) => 
            this.VisitConstantOp(op, n);

        public virtual TResultType Visit(ConstantPredicateOp op, Node n) => 
            this.VisitConstantOp(op, n);

        public virtual TResultType Visit(ConstrainedSortOp op, Node n) => 
            this.VisitSortOp(op, n);

        public virtual TResultType Visit(CrossApplyOp op, Node n) => 
            this.VisitApplyOp(op, n);

        public virtual TResultType Visit(CrossJoinOp op, Node n) => 
            this.VisitJoinOp(op, n);

        public virtual TResultType Visit(DerefOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(DiscriminatedNewEntityOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(DistinctOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        public virtual TResultType Visit(ElementOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(ExceptOp op, Node n) => 
            this.VisitSetOp(op, n);

        public virtual TResultType Visit(ExistsOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(FilterOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        public virtual TResultType Visit(FullOuterJoinOp op, Node n) => 
            this.VisitJoinOp(op, n);

        public virtual TResultType Visit(FunctionOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(GetEntityRefOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(GetRefKeyOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(GroupByOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        public virtual TResultType Visit(InnerJoinOp op, Node n) => 
            this.VisitJoinOp(op, n);

        public virtual TResultType Visit(InternalConstantOp op, Node n) => 
            this.VisitConstantOp(op, n);

        public virtual TResultType Visit(IntersectOp op, Node n) => 
            this.VisitSetOp(op, n);

        public virtual TResultType Visit(IsOfOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(LeftOuterJoinOp op, Node n) => 
            this.VisitJoinOp(op, n);

        public virtual TResultType Visit(LikeOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(MultiStreamNestOp op, Node n) => 
            this.VisitNestOp(op, n);

        public virtual TResultType Visit(NavigateOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(NewEntityOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(NewInstanceOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(NewMultisetOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(NewRecordOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(NullOp op, Node n) => 
            this.VisitConstantOp(op, n);

        public virtual TResultType Visit(Op op, Node n) => 
            this.Unimplemented(n);

        public virtual TResultType Visit(OuterApplyOp op, Node n) => 
            this.VisitApplyOp(op, n);

        public virtual TResultType Visit(PhysicalProjectOp op, Node n) => 
            this.VisitPhysicalOpDefault(op, n);

        public virtual TResultType Visit(ProjectOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        public virtual TResultType Visit(PropertyOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(RefOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(RelPropertyOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(ScanTableOp op, Node n) => 
            this.VisitTableOp(op, n);

        public virtual TResultType Visit(ScanViewOp op, Node n) => 
            this.VisitTableOp(op, n);

        public virtual TResultType Visit(SingleRowOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        public virtual TResultType Visit(SingleRowTableOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        public virtual TResultType Visit(SingleStreamNestOp op, Node n) => 
            this.VisitNestOp(op, n);

        public virtual TResultType Visit(SoftCastOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(SortOp op, Node n) => 
            this.VisitSortOp(op, n);

        public virtual TResultType Visit(TreatOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        public virtual TResultType Visit(UnionAllOp op, Node n) => 
            this.VisitSetOp(op, n);

        public virtual TResultType Visit(UnnestOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        public virtual TResultType Visit(VarDefListOp op, Node n) => 
            this.VisitAncillaryOpDefault(op, n);

        public virtual TResultType Visit(VarDefOp op, Node n) => 
            this.VisitAncillaryOpDefault(op, n);

        public virtual TResultType Visit(VarRefOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        protected virtual TResultType VisitAncillaryOpDefault(AncillaryOp op, Node n) => 
            this.VisitDefault(n);

        protected virtual TResultType VisitApplyOp(ApplyBaseOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        protected virtual void VisitChildren(Node n)
        {
            for (int i = 0; i < n.Children.Count; i++)
            {
                this.VisitNode(n.Children[i]);
            }
        }

        protected virtual TResultType VisitConstantOp(ConstantBaseOp op, Node n) => 
            this.VisitScalarOpDefault(op, n);

        protected virtual TResultType VisitDefault(Node n)
        {
            this.VisitChildren(n);
            return default(TResultType);
        }

        protected virtual TResultType VisitJoinOp(JoinBaseOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        protected virtual TResultType VisitNestOp(NestBaseOp op, Node n) => 
            this.VisitPhysicalOpDefault(op, n);

        internal TResultType VisitNode(Node n) => 
            n.Op.Accept<TResultType>((BasicOpVisitorOfT<TResultType>) this, n);

        protected virtual TResultType VisitPhysicalOpDefault(PhysicalOp op, Node n) => 
            this.VisitDefault(n);

        protected virtual TResultType VisitRelOpDefault(RelOp op, Node n) => 
            this.VisitDefault(n);

        protected virtual TResultType VisitScalarOpDefault(ScalarOp op, Node n) => 
            this.VisitDefault(n);

        protected virtual TResultType VisitSetOp(SetOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        protected virtual TResultType VisitSortOp(SortBaseOp op, Node n) => 
            this.VisitRelOpDefault(op, n);

        protected virtual TResultType VisitTableOp(ScanTableBaseOp op, Node n) => 
            this.VisitRelOpDefault(op, n);
    }
}

