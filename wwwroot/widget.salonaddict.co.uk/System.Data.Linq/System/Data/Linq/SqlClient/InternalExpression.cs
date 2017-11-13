namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal abstract class InternalExpression : Expression
    {
        internal InternalExpression(InternalExpressionType nt, Type type) : base((ExpressionType) nt, type)
        {
        }

        internal static KnownExpression Known(SqlExpression expr) => 
            new KnownExpression(expr, expr.ClrType);

        internal static KnownExpression Known(SqlNode node, Type type) => 
            new KnownExpression(node, type);
    }
}

