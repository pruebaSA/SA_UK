namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class CommandExpr : Expr
    {
        private ExprList<NamespaceExpr> _namespaceDeclarationList;
        private Expr _queryExpr;

        internal CommandExpr(ExprList<NamespaceExpr> nsExpr, Expr queryExpr)
        {
            this._namespaceDeclarationList = nsExpr;
            this._queryExpr = queryExpr;
        }

        internal ExprList<NamespaceExpr> NamespaceDeclList =>
            this._namespaceDeclarationList;

        internal Expr QueryExpr =>
            this._queryExpr;
    }
}

