namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;

    internal class LeafVisitor<T_Identifier> : Visitor<T_Identifier, bool>
    {
        private readonly List<TermExpr<T_Identifier>> _terms;

        private LeafVisitor()
        {
            this._terms = new List<TermExpr<T_Identifier>>();
        }

        internal static IEnumerable<T_Identifier> GetLeaves(BoolExpr<T_Identifier> expression) => 
            (from term in LeafVisitor<T_Identifier>.GetTerms(expression) select term.Identifier);

        internal static List<TermExpr<T_Identifier>> GetTerms(BoolExpr<T_Identifier> expression)
        {
            LeafVisitor<T_Identifier> visitor = new LeafVisitor<T_Identifier>();
            expression.Accept<bool>(visitor);
            return visitor._terms;
        }

        internal override bool VisitAnd(AndExpr<T_Identifier> expression) => 
            this.VisitTree(expression);

        internal override bool VisitFalse(FalseExpr<T_Identifier> expression) => 
            true;

        internal override bool VisitNot(NotExpr<T_Identifier> expression) => 
            expression.Child.Accept<bool>(this);

        internal override bool VisitOr(OrExpr<T_Identifier> expression) => 
            this.VisitTree(expression);

        internal override bool VisitTerm(TermExpr<T_Identifier> expression)
        {
            this._terms.Add(expression);
            return true;
        }

        private bool VisitTree(TreeExpr<T_Identifier> expression)
        {
            foreach (BoolExpr<T_Identifier> expr in expression.Children)
            {
                expr.Accept<bool>(this);
            }
            return true;
        }

        internal override bool VisitTrue(TrueExpr<T_Identifier> expression) => 
            true;
    }
}

