namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class OrderByClause : Expr
    {
        private Expr _limitExpr;
        private uint _methodCallCount;
        private ExprList<System.Data.Common.EntitySql.OrderByClauseItem> _orderByClauseItem;
        private Expr _skipExpr;

        internal OrderByClause(ExprList<System.Data.Common.EntitySql.OrderByClauseItem> orderByClauseItem, Expr skipExpr, Expr limitExpr, uint methodCallCount)
        {
            this._orderByClauseItem = orderByClauseItem;
            this._skipExpr = skipExpr;
            this._limitExpr = limitExpr;
            this._methodCallCount = methodCallCount;
        }

        internal bool HasLimitSubClause =>
            (null != this._limitExpr);

        internal bool HasMethodCall =>
            (this._methodCallCount > 0);

        internal bool HasSkipSubClause =>
            (null != this._skipExpr);

        internal Expr LimitSubClause =>
            this._limitExpr;

        internal ExprList<System.Data.Common.EntitySql.OrderByClauseItem> OrderByClauseItem =>
            this._orderByClauseItem;

        internal Expr SkipSubClause =>
            this._skipExpr;
    }
}

