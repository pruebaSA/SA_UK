namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbSortClause
    {
        private bool _asc;
        private string _coll;
        private ExpressionLink _expr;

        internal DbSortClause(DbExpression key, bool asc, string collation)
        {
            if (key == null)
            {
                throw EntityUtil.ArgumentNull("key");
            }
            this._expr = new ExpressionLink("Expression", key.CommandTree, key);
            if (!TypeHelpers.IsValidSortOpKeyType(this.Expression.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Sort_OrderComparable, "key");
            }
            if (!string.IsNullOrEmpty(collation) && !TypeSemantics.IsPrimitiveType(this.Expression.ResultType, PrimitiveTypeKind.String))
            {
                throw EntityUtil.Argument(Strings.Cqt_Sort_NonStringCollationInvalid, "collation");
            }
            this._asc = asc;
            this._coll = collation;
        }

        public bool Ascending =>
            this._asc;

        public string Collation =>
            this._coll;

        public DbExpression Expression
        {
            get => 
                this._expr.Expression;
            internal set
            {
                this._expr.Expression = value;
            }
        }
    }
}

