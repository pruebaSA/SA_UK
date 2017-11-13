namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class ApplyClauseItem : Expr
    {
        private System.Data.Common.EntitySql.ApplyKind _applyKind;
        private FromClauseItem _applyLeft;
        private FromClauseItem _applyRight;

        internal ApplyClauseItem(FromClauseItem applyLeft, FromClauseItem applyRight, System.Data.Common.EntitySql.ApplyKind applyKind)
        {
            this._applyLeft = applyLeft;
            this._applyRight = applyRight;
            this._applyKind = applyKind;
        }

        internal System.Data.Common.EntitySql.ApplyKind ApplyKind =>
            this._applyKind;

        internal FromClauseItem LeftExpr =>
            this._applyLeft;

        internal FromClauseItem RightExpr =>
            this._applyRight;
    }
}

