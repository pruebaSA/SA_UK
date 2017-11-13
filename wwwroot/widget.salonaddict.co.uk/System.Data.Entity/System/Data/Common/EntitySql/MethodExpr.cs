namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data.Common.CommandTrees;

    internal sealed class MethodExpr : Expr
    {
        private ExprList<Expr> _args;
        private System.Data.Common.EntitySql.DistinctKind _distinctKind;
        private DbExpression _dummyExpression;
        private string _internalAggregateName;
        private Expr _leftExpr;
        private Identifier _methodIdentifier;
        private ExprList<RelshipNavigationExpr> _relationships;

        internal MethodExpr(Expr left, Identifier methodId, System.Data.Common.EntitySql.DistinctKind distinctKind, ExprList<Expr> args)
        {
            this._leftExpr = left;
            this._methodIdentifier = methodId;
            this._distinctKind = distinctKind;
            this._args = args;
        }

        internal MethodExpr(Expr left, Identifier methodId, System.Data.Common.EntitySql.DistinctKind distinctKind, ExprList<Expr> args, ExprList<RelshipNavigationExpr> relationships)
        {
            this._leftExpr = left;
            this._methodIdentifier = methodId;
            this._distinctKind = distinctKind;
            this._args = args;
            this._relationships = relationships;
        }

        internal void ResetAggregateInfo()
        {
            this._internalAggregateName = null;
            this._dummyExpression = null;
        }

        internal void ResetDummyExpression()
        {
            this._dummyExpression = null;
        }

        internal void SetAggregateInfo(string internalAggregateName, DbExpression dummyExpr)
        {
            this._internalAggregateName = internalAggregateName;
            this._dummyExpression = dummyExpr;
        }

        internal ExprList<Expr> Args =>
            this._args;

        internal System.Data.Common.EntitySql.DistinctKind DistinctKind =>
            this._distinctKind;

        internal DbExpression DummyExpression =>
            this._dummyExpression;

        internal bool HasRelationships =>
            ((this._relationships != null) && (this._relationships.Count > 0));

        internal string InternalAggregateName =>
            this._internalAggregateName;

        internal Expr LeftExpr =>
            this._leftExpr;

        internal Identifier MethodIdentifier =>
            this._methodIdentifier;

        internal string MethodName =>
            this.MethodIdentifier.Name;

        internal DotExpr MethodPrefixExpr =>
            new DotExpr(this.LeftExpr, this.MethodIdentifier);

        internal ExprList<RelshipNavigationExpr> Relationships =>
            this._relationships;

        internal bool WasResolved =>
            (null != this._internalAggregateName);
    }
}

