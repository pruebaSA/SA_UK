namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class FromClauseItem : Expr
    {
        private Expr _fromClauseItemExpr;
        private System.Data.Common.EntitySql.FromClauseItemKind _fromClauseItemKind;

        internal FromClauseItem(AliasExpr aliasExpr)
        {
            this._fromClauseItemExpr = aliasExpr;
            this._fromClauseItemKind = System.Data.Common.EntitySql.FromClauseItemKind.AliasedFromClause;
        }

        internal FromClauseItem(ApplyClauseItem applyClauseItem)
        {
            this._fromClauseItemExpr = applyClauseItem;
            this._fromClauseItemKind = System.Data.Common.EntitySql.FromClauseItemKind.ApplyFromClause;
        }

        internal FromClauseItem(JoinClauseItem joinClauseItem)
        {
            this._fromClauseItemExpr = joinClauseItem;
            this._fromClauseItemKind = System.Data.Common.EntitySql.FromClauseItemKind.JoinFromClause;
        }

        internal System.Data.Common.EntitySql.FromClauseItemKind FromClauseItemKind =>
            this._fromClauseItemKind;

        internal Expr FromExpr =>
            this._fromClauseItemExpr;
    }
}

