namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    internal class SqlParameter : SqlSimpleTypeExpression
    {
        private ParameterDirection direction;
        private string name;

        internal SqlParameter(Type clrType, ProviderType sqlType, string name, Expression sourceExpression) : base(SqlNodeType.Parameter, clrType, sqlType, sourceExpression)
        {
            if (name == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("name");
            }
            if (typeof(Type).IsAssignableFrom(clrType))
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentWrongValue("clrType");
            }
            this.name = name;
            this.direction = ParameterDirection.Input;
        }

        internal ParameterDirection Direction
        {
            get => 
                this.direction;
            set
            {
                this.direction = value;
            }
        }

        internal string Name =>
            this.name;
    }
}

