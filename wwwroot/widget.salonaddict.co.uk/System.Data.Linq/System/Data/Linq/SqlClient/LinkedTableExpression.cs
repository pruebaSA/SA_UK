namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq;

    internal sealed class LinkedTableExpression : InternalExpression
    {
        private SqlLink link;
        private ITable table;

        internal LinkedTableExpression(SqlLink link, ITable table, Type type) : base(InternalExpressionType.LinkedTable, type)
        {
            this.link = link;
            this.table = table;
        }

        internal SqlLink Link =>
            this.link;

        internal ITable Table =>
            this.table;
    }
}

