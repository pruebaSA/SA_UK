namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;

    internal sealed class DottedIdentifier : Expr
    {
        private string _fullName;
        private List<Identifier> _identifiers;
        private string[] _names;

        internal DottedIdentifier(DotExpr dotExpr)
        {
            this._identifiers = new List<Identifier>();
            this._names = dotExpr.Names;
            this._fullName = dotExpr.FullName;
            Expr left = dotExpr;
            while (left is DotExpr)
            {
                DotExpr expr2 = (DotExpr) left;
                this._identifiers.Add(expr2.Identifier);
                left = expr2.Left;
            }
            this._identifiers.Add((Identifier) left);
            this._identifiers.Reverse();
            base.ErrCtx = left.ErrCtx;
            base.ErrCtx.ErrorContextInfo = "CtxIdentifier";
        }

        internal DottedIdentifier(Identifier id)
        {
            this._identifiers = new List<Identifier>();
            this._identifiers.Add(id);
            this._names = new string[] { id.Name };
            this._fullName = id.Name;
            base.ErrCtx = id.ErrCtx;
        }

        internal string FullName =>
            this._fullName;

        internal string[] Names =>
            this._names;
    }
}

