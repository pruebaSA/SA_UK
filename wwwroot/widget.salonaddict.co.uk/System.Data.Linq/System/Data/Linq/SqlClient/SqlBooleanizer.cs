namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq.Mapping;

    internal class SqlBooleanizer
    {
        internal static SqlNode Rationalize(SqlNode node, TypeSystemProvider typeProvider, MetaModel model) => 
            new Booleanizer(typeProvider, model).Visit(node);

        private class Booleanizer : SqlBooleanMismatchVisitor
        {
            private SqlFactory sql;

            internal Booleanizer(TypeSystemProvider typeProvider, MetaModel model)
            {
                this.sql = new SqlFactory(typeProvider, model);
            }

            internal override SqlExpression ConvertPredicateToValue(SqlExpression predicateExpression)
            {
                SqlExpression expression = this.sql.ValueFromObject(true, false, predicateExpression.SourceExpression);
                SqlExpression expression2 = this.sql.ValueFromObject(false, false, predicateExpression.SourceExpression);
                if (SqlExpressionNullability.CanBeNull(predicateExpression) != false)
                {
                    return new SqlSearchedCase(predicateExpression.ClrType, new SqlWhen[] { new SqlWhen(predicateExpression, expression), new SqlWhen(new SqlUnary(SqlNodeType.Not, predicateExpression.ClrType, predicateExpression.SqlType, predicateExpression, predicateExpression.SourceExpression), expression2) }, this.sql.Value(expression.ClrType, expression.SqlType, null, false, predicateExpression.SourceExpression), predicateExpression.SourceExpression);
                }
                return new SqlSearchedCase(predicateExpression.ClrType, new SqlWhen[] { new SqlWhen(predicateExpression, expression) }, expression2, predicateExpression.SourceExpression);
            }

            internal override SqlExpression ConvertValueToPredicate(SqlExpression valueExpression) => 
                new SqlBinary(SqlNodeType.EQ, valueExpression.ClrType, this.sql.TypeProvider.From(typeof(bool)), valueExpression, this.sql.Value(typeof(bool), valueExpression.SqlType, true, false, valueExpression.SourceExpression));

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                if ((select.Where != null) && (select.Where.NodeType == SqlNodeType.Coalesce))
                {
                    SqlBinary where = (SqlBinary) select.Where;
                    if (where.Right.NodeType == SqlNodeType.Value)
                    {
                        SqlValue right = (SqlValue) where.Right;
                        if (((right.Value != null) && (right.Value.GetType() == typeof(bool))) && !((bool) right.Value))
                        {
                            select.Where = where.Left;
                        }
                    }
                }
                return base.VisitSelect(select);
            }
        }
    }
}

