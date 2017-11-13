namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlIncludeScope : SqlNode
    {
        private SqlNode child;

        internal SqlIncludeScope(SqlNode child, Expression sourceExpression) : base(SqlNodeType.IncludeScope, sourceExpression)
        {
            this.child = child;
        }

        internal SqlNode Child
        {
            get => 
                this.child;
            set
            {
                this.child = value;
            }
        }
    }
}

