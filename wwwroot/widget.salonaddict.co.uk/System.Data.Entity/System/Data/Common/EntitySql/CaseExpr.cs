namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class CaseExpr : Expr
    {
        private Expr _elseExpr;
        private ExprList<WhenThenExpr> _whenThenExpr;

        internal CaseExpr(ExprList<WhenThenExpr> whenThenExpr) : this(whenThenExpr, null)
        {
        }

        internal CaseExpr(ExprList<WhenThenExpr> whenThenExpr, Expr elseExpr)
        {
            this._whenThenExpr = whenThenExpr;
            this._elseExpr = elseExpr;
        }

        internal Expr ElseExpr =>
            this._elseExpr;

        internal ExprList<WhenThenExpr> WhenThenExprList =>
            this._whenThenExpr;
    }
}

