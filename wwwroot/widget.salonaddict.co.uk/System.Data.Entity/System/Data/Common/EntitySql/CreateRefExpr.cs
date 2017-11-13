namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class CreateRefExpr : Expr
    {
        private Expr _entitySet;
        private Expr _keys;
        private Expr _typeIdentifier;

        internal CreateRefExpr(Expr entitySet, Expr keys)
        {
            this._entitySet = entitySet;
            this._keys = keys;
        }

        internal CreateRefExpr(Expr entitySet, Expr keys, Expr typeIdentifier)
        {
            this._entitySet = entitySet;
            this._keys = keys;
            this._typeIdentifier = typeIdentifier;
        }

        internal Expr EntitySet =>
            this._entitySet;

        internal Expr Keys =>
            this._keys;

        internal Expr TypeIdentifier =>
            this._typeIdentifier;
    }
}

