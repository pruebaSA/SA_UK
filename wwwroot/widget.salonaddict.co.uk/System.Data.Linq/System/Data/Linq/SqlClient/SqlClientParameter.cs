namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlClientParameter : SqlSimpleTypeExpression
    {
        private LambdaExpression accessor;

        internal SqlClientParameter(Type clrType, ProviderType sqlType, LambdaExpression accessor, Expression sourceExpression) : base(SqlNodeType.ClientParameter, clrType, sqlType, sourceExpression)
        {
            this.accessor = accessor;
        }

        internal LambdaExpression Accessor =>
            this.accessor;
    }
}

