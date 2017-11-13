namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class QueryExpr : Expr
    {
        private System.Data.Common.EntitySql.FromClause _fromClause;
        private System.Data.Common.EntitySql.GroupByClause _groupByClause;
        private System.Data.Common.EntitySql.HavingClause _havingClause;
        private System.Data.Common.EntitySql.OrderByClause _orderByClause;
        private System.Data.Common.EntitySql.SelectClause _selectClause;
        private Expr _whereClause;

        internal QueryExpr(System.Data.Common.EntitySql.SelectClause selectClause, System.Data.Common.EntitySql.FromClause fromClause, Expr whereClause, System.Data.Common.EntitySql.GroupByClause groupByClause, System.Data.Common.EntitySql.HavingClause havingClause, System.Data.Common.EntitySql.OrderByClause orderByClause)
        {
            this._selectClause = selectClause;
            this._fromClause = fromClause;
            this._whereClause = whereClause;
            this._groupByClause = groupByClause;
            this._havingClause = havingClause;
            this._orderByClause = orderByClause;
        }

        internal override AstExprKind ExprKind =>
            AstExprKind.Query;

        internal System.Data.Common.EntitySql.FromClause FromClause =>
            this._fromClause;

        internal System.Data.Common.EntitySql.GroupByClause GroupByClause =>
            this._groupByClause;

        internal bool HasMethodCall =>
            ((this._selectClause.HasMethodCall || ((this._havingClause != null) && this._havingClause.HasMethodCall)) || ((this._orderByClause != null) && this._orderByClause.HasMethodCall));

        internal System.Data.Common.EntitySql.HavingClause HavingClause =>
            this._havingClause;

        internal System.Data.Common.EntitySql.OrderByClause OrderByClause =>
            this._orderByClause;

        internal System.Data.Common.EntitySql.SelectClause SelectClause =>
            this._selectClause;

        internal Expr WhereClause =>
            this._whereClause;
    }
}

