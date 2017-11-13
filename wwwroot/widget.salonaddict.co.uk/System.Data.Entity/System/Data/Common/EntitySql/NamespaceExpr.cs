namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data;
    using System.Data.Entity;

    internal sealed class NamespaceExpr : Expr
    {
        private Identifier _namespaceAlias;
        private DottedIdentifier _namespaceName;

        internal NamespaceExpr(BuiltInExpr bltInExpr)
        {
            this._namespaceAlias = null;
            Identifier identifier = bltInExpr.Arg1 as Identifier;
            if (identifier == null)
            {
                throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.InvalidNamespaceAlias);
            }
            if (identifier.IsEscaped)
            {
                throw EntityUtil.EntitySqlError(identifier.ErrCtx.QueryText, Strings.InvalidEscapedNamespaceAlias, identifier.ErrCtx.InputPosition);
            }
            this._namespaceAlias = identifier;
            if (bltInExpr.Arg2 is Identifier)
            {
                this._namespaceName = new DottedIdentifier((Identifier) bltInExpr.Arg2);
            }
            else
            {
                if (!(bltInExpr.Arg2 is DotExpr))
                {
                    throw EntityUtil.EntitySqlError(bltInExpr.ErrCtx, Strings.InvalidNamespace);
                }
                DotExpr dotExpr = (DotExpr) bltInExpr.Arg2;
                if (!dotExpr.IsDottedIdentifier)
                {
                    throw EntityUtil.EntitySqlError(dotExpr.ErrCtx, Strings.InvalidNamespace);
                }
                this._namespaceName = new DottedIdentifier(dotExpr);
            }
        }

        internal NamespaceExpr(DotExpr dotExpr)
        {
            if (!dotExpr.IsDottedIdentifier)
            {
                throw EntityUtil.EntitySqlError(dotExpr.ErrCtx, Strings.InvalidNamespace);
            }
            this._namespaceName = new DottedIdentifier(dotExpr);
        }

        internal NamespaceExpr(Identifier identifier)
        {
            this._namespaceName = new DottedIdentifier(identifier);
        }

        internal Identifier AliasIdentifier =>
            this._namespaceAlias;

        internal bool IsAliased =>
            (null != this._namespaceAlias);

        internal DottedIdentifier NamespaceName =>
            this._namespaceName;
    }
}

