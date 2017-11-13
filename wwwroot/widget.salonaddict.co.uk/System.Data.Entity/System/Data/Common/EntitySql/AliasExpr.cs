namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data;
    using System.Data.Entity;

    internal sealed class AliasExpr : System.Data.Common.EntitySql.Expr
    {
        private Identifier _aliasId;
        private System.Data.Common.EntitySql.Expr _expr;

        internal AliasExpr(System.Data.Common.EntitySql.Expr expr)
        {
            this._expr = expr;
        }

        internal AliasExpr(System.Data.Common.EntitySql.Expr expr, Identifier aliasId)
        {
            ValidateAlias(aliasId);
            this._aliasId = aliasId;
            this._expr = expr;
        }

        private static void ValidateAlias(Identifier aliasIdentifier)
        {
            if (string.IsNullOrEmpty(aliasIdentifier.Name))
            {
                throw EntityUtil.EntitySqlError(aliasIdentifier.ErrCtx, Strings.InvalidEmptyIdentifier);
            }
        }

        internal Identifier AliasIdentifier =>
            this._aliasId;

        internal System.Data.Common.EntitySql.Expr Expr =>
            this._expr;

        internal bool HasAlias =>
            (null != this._aliasId);
    }
}

