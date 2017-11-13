namespace System.Data.Common.EntitySql
{
    using System;

    internal abstract class Expr : AstNode
    {
        internal Expr()
        {
        }

        internal Expr(string query, int inputPos) : base(query, inputPos)
        {
        }

        internal virtual AstExprKind ExprKind =>
            AstExprKind.Generic;
    }
}

