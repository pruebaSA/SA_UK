namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlMethodTransformer : SqlVisitor
    {
        protected SqlFactory sql;

        internal SqlMethodTransformer(SqlFactory sql)
        {
            this.sql = sql;
        }

        private static bool SkipConversionForDateAdd(string functionName, Type expected, Type actual)
        {
            if (string.Compare(functionName, "DATEADD", StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }
            return ((expected == typeof(DateTime)) && (actual == typeof(DateTimeOffset)));
        }

        internal override SqlExpression VisitFunctionCall(SqlFunctionCall fc)
        {
            SqlExpression first = base.VisitFunctionCall(fc);
            if (first is SqlFunctionCall)
            {
                SqlFunctionCall expr = (SqlFunctionCall) first;
                if (expr.Name == "LEN")
                {
                    SqlExpression expression2 = expr.Arguments[0];
                    if (expression2.SqlType.IsLargeType && !expression2.SqlType.SupportsLength)
                    {
                        first = this.sql.DATALENGTH(expression2);
                        if (expression2.SqlType.IsUnicodeType)
                        {
                            first = this.sql.ConvertToInt(this.sql.Divide(first, this.sql.ValueFromObject(2, expression2.SourceExpression)));
                        }
                    }
                }
                Type closestRuntimeType = expr.SqlType.GetClosestRuntimeType();
                bool flag = SkipConversionForDateAdd(expr.Name, expr.ClrType, closestRuntimeType);
                if ((expr.ClrType != closestRuntimeType) && !flag)
                {
                    first = this.sql.ConvertTo(expr.ClrType, expr);
                }
            }
            return first;
        }

        internal override SqlExpression VisitUnaryOperator(SqlUnary fc)
        {
            SqlExpression first = base.VisitUnaryOperator(fc);
            if (!(first is SqlUnary))
            {
                return first;
            }
            SqlUnary unary = (SqlUnary) first;
            if (unary.NodeType != SqlNodeType.ClrLength)
            {
                return first;
            }
            SqlExpression operand = unary.Operand;
            first = this.sql.DATALENGTH(operand);
            if (operand.SqlType.IsUnicodeType)
            {
                first = this.sql.Divide(first, this.sql.ValueFromObject(2, operand.SourceExpression));
            }
            return this.sql.ConvertToInt(first);
        }
    }
}

