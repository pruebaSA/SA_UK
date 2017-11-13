namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlDelete : SqlStatement
    {
        private SqlSelect select;

        internal SqlDelete(SqlSelect select, Expression sourceExpression) : base(SqlNodeType.Delete, sourceExpression)
        {
            this.Select = select;
        }

        internal SqlSelect Select
        {
            get => 
                this.select;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                this.select = value;
            }
        }
    }
}

