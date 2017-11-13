namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class JoinClauseItem : Expr
    {
        private System.Data.Common.EntitySql.JoinKind _joinKind;
        private FromClauseItem _joinLeft;
        private FromClauseItem _joinRight;
        private Expr _onExpr;

        internal JoinClauseItem(FromClauseItem joinLeft, FromClauseItem joinRight, System.Data.Common.EntitySql.JoinKind joinKind) : this(joinLeft, joinRight, joinKind, null)
        {
        }

        internal JoinClauseItem(FromClauseItem joinLeft, FromClauseItem joinRight, System.Data.Common.EntitySql.JoinKind joinKind, Expr onExpr)
        {
            this._joinLeft = joinLeft;
            this._joinRight = joinRight;
            this._joinKind = joinKind;
            this._onExpr = onExpr;
        }

        internal System.Data.Common.EntitySql.JoinKind JoinKind
        {
            get => 
                this._joinKind;
            set
            {
                this._joinKind = value;
            }
        }

        internal FromClauseItem LeftExpr =>
            this._joinLeft;

        internal Expr OnExpr =>
            this._onExpr;

        internal FromClauseItem RightExpr =>
            this._joinRight;
    }
}

