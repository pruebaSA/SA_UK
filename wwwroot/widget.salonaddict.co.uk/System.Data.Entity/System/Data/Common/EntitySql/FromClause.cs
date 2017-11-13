namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class FromClause : Expr
    {
        private ExprList<FromClauseItem> _fromClauseItems;

        internal FromClause(ExprList<FromClauseItem> fromClauseItems)
        {
            this._fromClauseItems = fromClauseItems;
        }

        internal ExprList<FromClauseItem> FromClauseItems =>
            this._fromClauseItems;
    }
}

