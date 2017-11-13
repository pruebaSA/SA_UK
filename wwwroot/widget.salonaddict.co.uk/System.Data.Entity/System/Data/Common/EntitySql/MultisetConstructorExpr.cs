namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class MultisetConstructorExpr : Expr
    {
        private ExprList<Expr> _exprList;

        internal MultisetConstructorExpr(ExprList<Expr> exprList)
        {
            this._exprList = exprList;
        }

        internal ExprList<Expr> ExprList =>
            this._exprList;
    }
}

