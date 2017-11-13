namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class QueryExtractor
    {
        internal static SqlClientQuery Extract(SqlSubSelect subquery, IEnumerable<SqlParameter> parentParameters)
        {
            SqlClientQuery query = new SqlClientQuery(subquery);
            if (parentParameters != null)
            {
                query.Parameters.AddRange(parentParameters);
            }
            Visitor visitor = new Visitor(query.Arguments, query.Parameters);
            query.Query = (SqlSubSelect) visitor.Visit(subquery);
            return query;
        }

        private class Visitor : SqlDuplicator.DuplicatingVisitor
        {
            private List<SqlExpression> externals;
            private List<SqlParameter> parameters;

            internal Visitor(List<SqlExpression> externals, List<SqlParameter> parameters) : base(true)
            {
                this.externals = externals;
                this.parameters = parameters;
            }

            private SqlExpression ExtractParameter(SqlExpression expr)
            {
                Type clrType = expr.ClrType;
                if (expr.ClrType.IsValueType && !TypeSystem.IsNullableType(expr.ClrType))
                {
                    clrType = typeof(Nullable<>).MakeGenericType(new Type[] { expr.ClrType });
                }
                this.externals.Add(expr);
                SqlParameter item = new SqlParameter(clrType, expr.SqlType, "@x" + (this.parameters.Count + 1), expr.SourceExpression);
                this.parameters.Add(item);
                return item;
            }

            internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
            {
                SqlExpression expr = base.VisitColumnRef(cref);
                if (expr == cref)
                {
                    return this.ExtractParameter(expr);
                }
                return expr;
            }

            internal override SqlNode VisitLink(SqlLink link)
            {
                SqlExpression[] keyExpressions = new SqlExpression[link.KeyExpressions.Count];
                int index = 0;
                int length = keyExpressions.Length;
                while (index < length)
                {
                    keyExpressions[index] = this.VisitExpression(link.KeyExpressions[index]);
                    index++;
                }
                return new SqlLink(new object(), link.RowType, link.ClrType, link.SqlType, null, link.Member, keyExpressions, null, link.SourceExpression);
            }

            internal override SqlExpression VisitUserColumn(SqlUserColumn suc)
            {
                SqlExpression expr = base.VisitUserColumn(suc);
                if (expr == suc)
                {
                    return this.ExtractParameter(expr);
                }
                return expr;
            }
        }
    }
}

