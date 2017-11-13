namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlFunctionCall : SqlSimpleTypeExpression
    {
        private List<SqlExpression> arguments;
        private string name;

        internal SqlFunctionCall(Type clrType, ProviderType sqlType, string name, IEnumerable<SqlExpression> args, Expression source) : this(SqlNodeType.FunctionCall, clrType, sqlType, name, args, source)
        {
        }

        internal SqlFunctionCall(SqlNodeType nodeType, Type clrType, ProviderType sqlType, string name, IEnumerable<SqlExpression> args, Expression source) : base(nodeType, clrType, sqlType, source)
        {
            this.name = name;
            this.arguments = new List<SqlExpression>(args);
        }

        internal List<SqlExpression> Arguments =>
            this.arguments;

        internal string Name =>
            this.name;
    }
}

