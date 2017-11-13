namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class GroupByClause : Expr
    {
        private Identifier _groupIdentifier;
        private ExprList<AliasExpr> _groupItems;

        internal GroupByClause(ExprList<AliasExpr> groupItems, Identifier groupIdentifier)
        {
            this._groupIdentifier = groupIdentifier;
            this._groupItems = groupItems;
        }

        internal ExprList<AliasExpr> GroupItems =>
            this._groupItems;
    }
}

