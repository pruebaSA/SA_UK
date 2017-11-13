namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class OrderByClauseItem : Expr
    {
        private Identifier _optCollationIdentifier;
        private Expr _orderExpr;
        private System.Data.Common.EntitySql.OrderKind _orderKind;

        internal OrderByClauseItem(Expr orderExpr, System.Data.Common.EntitySql.OrderKind orderKind) : this(orderExpr, orderKind, null)
        {
        }

        internal OrderByClauseItem(Expr orderExpr, System.Data.Common.EntitySql.OrderKind orderKind, Identifier optCollationIdentifier)
        {
            this._orderExpr = orderExpr;
            this._orderKind = orderKind;
            this._optCollationIdentifier = optCollationIdentifier;
        }

        internal Identifier CollateIdentifier =>
            this._optCollationIdentifier;

        internal bool IsCollated =>
            (null != this._optCollationIdentifier);

        internal Expr OrderExpr =>
            this._orderExpr;

        internal System.Data.Common.EntitySql.OrderKind OrderKind =>
            this._orderKind;
    }
}

