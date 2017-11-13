namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlRowNumber : SqlSimpleTypeExpression
    {
        private List<SqlOrderExpression> orderBy;

        internal SqlRowNumber(Type clrType, ProviderType sqlType, List<SqlOrderExpression> orderByList, Expression sourceExpression) : base(SqlNodeType.RowNumber, clrType, sqlType, sourceExpression)
        {
            if (orderByList == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("orderByList");
            }
            this.orderBy = orderByList;
        }

        internal List<SqlOrderExpression> OrderBy =>
            this.orderBy;
    }
}

