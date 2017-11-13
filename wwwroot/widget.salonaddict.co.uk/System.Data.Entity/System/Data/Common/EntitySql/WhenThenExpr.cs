namespace System.Data.Common.EntitySql
{
    using System;

    internal class WhenThenExpr : Expr
    {
        private Expr _thenExpr;
        private Expr _whenExpr;

        internal WhenThenExpr(Expr whenExpr, Expr thenExpr)
        {
            this._whenExpr = whenExpr;
            this._thenExpr = thenExpr;
        }

        internal Expr ThenExpr =>
            this._thenExpr;

        internal Expr WhenExpr =>
            this._whenExpr;
    }
}

