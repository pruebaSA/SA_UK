namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class RefExpr : Expr
    {
        private Expr _refArgExpression;

        internal RefExpr(Expr refArgExpr)
        {
            this._refArgExpression = refArgExpr;
        }

        internal Expr RefArgExpr =>
            this._refArgExpression;
    }
}

