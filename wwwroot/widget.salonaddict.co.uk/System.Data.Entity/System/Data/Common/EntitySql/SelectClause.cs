namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class SelectClause : Expr
    {
        private System.Data.Common.EntitySql.DistinctKind _distinctKind;
        private uint _methodCallCount;
        private ExprList<AliasExpr> _selectClauseItems;
        private System.Data.Common.EntitySql.SelectKind _selectKind;
        private Expr _topExpr;

        internal SelectClause(Expr item, System.Data.Common.EntitySql.DistinctKind distinctKind, Expr topExpr, uint methodCallCount)
        {
            this._selectKind = System.Data.Common.EntitySql.SelectKind.SelectValue;
            this._selectClauseItems = new ExprList<AliasExpr>(new AliasExpr(item));
            this._distinctKind = distinctKind;
            this._topExpr = topExpr;
            this._methodCallCount = methodCallCount;
        }

        internal SelectClause(ExprList<AliasExpr> items, System.Data.Common.EntitySql.DistinctKind distinctKind, Expr topExpr, uint methodCallCount)
        {
            this._selectKind = System.Data.Common.EntitySql.SelectKind.SelectRow;
            this._selectClauseItems = items;
            this._distinctKind = distinctKind;
            this._topExpr = topExpr;
            this._methodCallCount = methodCallCount;
        }

        internal System.Data.Common.EntitySql.DistinctKind DistinctKind =>
            this._distinctKind;

        internal bool HasMethodCall =>
            (this._methodCallCount > 0);

        internal bool HasTopClause =>
            (null != this._topExpr);

        internal ExprList<AliasExpr> Items =>
            this._selectClauseItems;

        internal System.Data.Common.EntitySql.SelectKind SelectKind =>
            this._selectKind;

        internal Expr TopExpr =>
            this._topExpr;
    }
}

