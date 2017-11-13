namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class RowConstructorExpr : Expr
    {
        private ExprList<AliasExpr> _exprList;

        internal RowConstructorExpr(ExprList<AliasExpr> exprList)
        {
            this._exprList = exprList;
        }

        internal ExprList<AliasExpr> AliasExprList =>
            this._exprList;
    }
}

