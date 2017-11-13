namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class HavingClause : Expr
    {
        private Expr _havingExpr;
        private uint _methodCallCount;

        internal HavingClause(Expr havingExpr, uint methodCallCounter)
        {
            this._havingExpr = havingExpr;
            this._methodCallCount = methodCallCounter;
        }

        internal bool HasMethodCall =>
            (this._methodCallCount > 0);

        internal Expr HavingPredicate =>
            this._havingExpr;
    }
}

