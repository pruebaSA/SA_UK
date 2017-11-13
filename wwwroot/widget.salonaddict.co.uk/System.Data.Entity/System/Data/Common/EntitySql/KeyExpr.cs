namespace System.Data.Common.EntitySql
{
    using System;

    internal class KeyExpr : Expr
    {
        private Expr _refExpr;

        internal KeyExpr(Expr refExpr)
        {
            this._refExpr = refExpr;
        }

        internal Expr RefExpr =>
            this._refExpr;
    }
}

