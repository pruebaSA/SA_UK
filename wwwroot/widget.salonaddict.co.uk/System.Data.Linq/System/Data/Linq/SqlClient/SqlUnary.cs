namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class SqlUnary : SqlSimpleTypeExpression
    {
        private MethodInfo method;
        private SqlExpression operand;

        internal SqlUnary(SqlNodeType nt, Type clrType, ProviderType sqlType, SqlExpression expr, Expression sourceExpression) : this(nt, clrType, sqlType, expr, null, sourceExpression)
        {
        }

        internal SqlUnary(SqlNodeType nt, Type clrType, ProviderType sqlType, SqlExpression expr, MethodInfo method, Expression sourceExpression) : base(nt, clrType, sqlType, sourceExpression)
        {
            switch (nt)
            {
                case SqlNodeType.Avg:
                case SqlNodeType.BitNot:
                case SqlNodeType.Cast:
                case SqlNodeType.IsNotNull:
                case SqlNodeType.IsNull:
                case SqlNodeType.LongCount:
                case SqlNodeType.Convert:
                case SqlNodeType.Count:
                case SqlNodeType.Covar:
                case SqlNodeType.ClrLength:
                case SqlNodeType.Negate:
                case SqlNodeType.Not:
                case SqlNodeType.Not2V:
                case SqlNodeType.OuterJoinedValue:
                case SqlNodeType.Max:
                case SqlNodeType.Min:
                case SqlNodeType.Stddev:
                case SqlNodeType.Sum:
                case SqlNodeType.Treat:
                case SqlNodeType.ValueOf:
                    this.Operand = expr;
                    this.method = method;
                    return;
            }
            throw System.Data.Linq.SqlClient.Error.UnexpectedNode(nt);
        }

        internal MethodInfo Method =>
            this.method;

        internal SqlExpression Operand
        {
            get => 
                this.operand;
            set
            {
                if (((value == null) && (base.NodeType != SqlNodeType.Count)) && (base.NodeType != SqlNodeType.LongCount))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                this.operand = value;
            }
        }
    }
}

