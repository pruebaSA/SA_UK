namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal abstract class Visitor<T_Identifier, T_Return>
    {
        protected Visitor()
        {
        }

        internal abstract T_Return VisitAnd(AndExpr<T_Identifier> expression);
        internal abstract T_Return VisitFalse(FalseExpr<T_Identifier> expression);
        internal abstract T_Return VisitNot(NotExpr<T_Identifier> expression);
        internal abstract T_Return VisitOr(OrExpr<T_Identifier> expression);
        internal abstract T_Return VisitTerm(TermExpr<T_Identifier> expression);
        internal abstract T_Return VisitTrue(TrueExpr<T_Identifier> expression);
    }
}

