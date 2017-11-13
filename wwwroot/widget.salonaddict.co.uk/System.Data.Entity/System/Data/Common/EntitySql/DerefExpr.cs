namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class DerefExpr : Expr
    {
        private Expr _refExpr;

        internal DerefExpr(Expr refExpr)
        {
            this._refExpr = refExpr;
        }

        internal Expr RefExpr =>
            this._refExpr;
    }
}

