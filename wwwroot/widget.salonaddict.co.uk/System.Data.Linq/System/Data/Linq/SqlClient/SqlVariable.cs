namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlVariable : SqlSimpleTypeExpression
    {
        private string name;

        internal SqlVariable(Type clrType, ProviderType sqlType, string name, Expression sourceExpression) : base(SqlNodeType.Variable, clrType, sqlType, sourceExpression)
        {
            if (name == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("name");
            }
            this.name = name;
        }

        internal string Name =>
            this.name;
    }
}

