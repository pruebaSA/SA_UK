namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class SqlMethodCall : SqlSimpleTypeExpression
    {
        private List<SqlExpression> arguments;
        private MethodInfo method;
        private SqlExpression obj;

        internal SqlMethodCall(Type clrType, ProviderType sqlType, MethodInfo method, SqlExpression obj, IEnumerable<SqlExpression> args, Expression sourceExpression) : base(SqlNodeType.MethodCall, clrType, sqlType, sourceExpression)
        {
            if (method == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("method");
            }
            this.method = method;
            this.Object = obj;
            this.arguments = new List<SqlExpression>();
            if (args != null)
            {
                this.arguments.AddRange(args);
            }
        }

        internal List<SqlExpression> Arguments =>
            this.arguments;

        internal MethodInfo Method =>
            this.method;

        internal SqlExpression Object
        {
            get => 
                this.obj;
            set
            {
                if ((value == null) && !this.method.IsStatic)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if ((value != null) && !this.method.DeclaringType.IsAssignableFrom(value.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.method.DeclaringType, value.ClrType);
                }
                this.obj = value;
            }
        }
    }
}

