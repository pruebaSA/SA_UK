namespace System.Data.Linq.SqlClient
{
    using System;

    internal sealed class KnownExpression : InternalExpression
    {
        private SqlNode node;

        internal KnownExpression(SqlNode node, Type type) : base(InternalExpressionType.Known, type)
        {
            this.node = node;
        }

        internal SqlNode Node =>
            this.node;
    }
}

