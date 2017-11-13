namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal static class NegationPusher
    {
        internal static BoolExpr<DomainConstraint<T_Variable, T_Element>> EliminateNot<T_Variable, T_Element>(BoolExpr<DomainConstraint<T_Variable, T_Element>> expression) => 
            expression.Accept<BoolExpr<DomainConstraint<T_Variable, T_Element>>>(NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element>.Instance);

        private class NegatedDomainConstraintTreeVisitor<T_Variable, T_Element> : NegationPusher.NegatedTreeVisitor<DomainConstraint<T_Variable, T_Element>>
        {
            internal static readonly NegationPusher.NegatedDomainConstraintTreeVisitor<T_Variable, T_Element> Instance;

            static NegatedDomainConstraintTreeVisitor()
            {
                NegationPusher.NegatedDomainConstraintTreeVisitor<T_Variable, T_Element>.Instance = new NegationPusher.NegatedDomainConstraintTreeVisitor<T_Variable, T_Element>();
            }

            private NegatedDomainConstraintTreeVisitor()
            {
            }

            internal override BoolExpr<DomainConstraint<T_Variable, T_Element>> VisitNot(NotExpr<DomainConstraint<T_Variable, T_Element>> expression) => 
                expression.Child.Accept<BoolExpr<DomainConstraint<T_Variable, T_Element>>>(NegationPusher.NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element>.Instance);

            internal override BoolExpr<DomainConstraint<T_Variable, T_Element>> VisitTerm(TermExpr<DomainConstraint<T_Variable, T_Element>> expression) => 
                new TermExpr<DomainConstraint<T_Variable, T_Element>>(expression.Identifier.InvertDomainConstraint());
        }

        private class NegatedTreeVisitor<T_Identifier> : Visitor<T_Identifier, BoolExpr<T_Identifier>>
        {
            internal static readonly NegationPusher.NegatedTreeVisitor<T_Identifier> Instance;

            static NegatedTreeVisitor()
            {
                NegationPusher.NegatedTreeVisitor<T_Identifier>.Instance = new NegationPusher.NegatedTreeVisitor<T_Identifier>();
            }

            protected NegatedTreeVisitor()
            {
            }

            internal override BoolExpr<T_Identifier> VisitAnd(AndExpr<T_Identifier> expression) => 
                new OrExpr<T_Identifier>(from child in expression.Children select child.Accept<BoolExpr<T_Identifier>>(this));

            internal override BoolExpr<T_Identifier> VisitFalse(FalseExpr<T_Identifier> expression) => 
                TrueExpr<T_Identifier>.Value;

            internal override BoolExpr<T_Identifier> VisitNot(NotExpr<T_Identifier> expression) => 
                expression.Child.Accept<BoolExpr<T_Identifier>>(NegationPusher.NonNegatedTreeVisitor<T_Identifier>.Instance);

            internal override BoolExpr<T_Identifier> VisitOr(OrExpr<T_Identifier> expression) => 
                new AndExpr<T_Identifier>(from child in expression.Children select child.Accept<BoolExpr<T_Identifier>>(this));

            internal override BoolExpr<T_Identifier> VisitTerm(TermExpr<T_Identifier> expression) => 
                new NotExpr<T_Identifier>(expression);

            internal override BoolExpr<T_Identifier> VisitTrue(TrueExpr<T_Identifier> expression) => 
                FalseExpr<T_Identifier>.Value;
        }

        private class NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element> : NegationPusher.NonNegatedTreeVisitor<DomainConstraint<T_Variable, T_Element>>
        {
            internal static readonly NegationPusher.NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element> Instance;

            static NonNegatedDomainConstraintTreeVisitor()
            {
                NegationPusher.NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element>.Instance = new NegationPusher.NonNegatedDomainConstraintTreeVisitor<T_Variable, T_Element>();
            }

            private NonNegatedDomainConstraintTreeVisitor()
            {
            }

            internal override BoolExpr<DomainConstraint<T_Variable, T_Element>> VisitNot(NotExpr<DomainConstraint<T_Variable, T_Element>> expression) => 
                expression.Child.Accept<BoolExpr<DomainConstraint<T_Variable, T_Element>>>(NegationPusher.NegatedDomainConstraintTreeVisitor<T_Variable, T_Element>.Instance);
        }

        private class NonNegatedTreeVisitor<T_Identifier> : BasicVisitor<T_Identifier>
        {
            internal static readonly NegationPusher.NonNegatedTreeVisitor<T_Identifier> Instance;

            static NonNegatedTreeVisitor()
            {
                NegationPusher.NonNegatedTreeVisitor<T_Identifier>.Instance = new NegationPusher.NonNegatedTreeVisitor<T_Identifier>();
            }

            protected NonNegatedTreeVisitor()
            {
            }

            internal override BoolExpr<T_Identifier> VisitNot(NotExpr<T_Identifier> expression) => 
                expression.Child.Accept<BoolExpr<T_Identifier>>(NegationPusher.NegatedTreeVisitor<T_Identifier>.Instance);
        }
    }
}

