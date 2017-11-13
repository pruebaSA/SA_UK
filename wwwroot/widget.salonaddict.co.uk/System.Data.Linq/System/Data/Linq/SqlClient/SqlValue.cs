namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlValue : SqlSimpleTypeExpression
    {
        private bool isClient;
        private object value;

        internal SqlValue(Type clrType, ProviderType sqlType, object value, bool isClientSpecified, Expression sourceExpression) : base(SqlNodeType.Value, clrType, sqlType, sourceExpression)
        {
            this.value = value;
            this.isClient = isClientSpecified;
        }

        internal bool IsClientSpecified =>
            this.isClient;

        internal object Value =>
            this.value;
    }
}

