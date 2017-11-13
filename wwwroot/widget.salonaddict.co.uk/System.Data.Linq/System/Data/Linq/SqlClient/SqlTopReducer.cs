namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlTopReducer
    {
        internal static SqlNode Reduce(SqlNode node, SqlNodeAnnotations annotations, SqlFactory sql) => 
            new Visitor(annotations, sql).Visit(node);

        private class Visitor : SqlVisitor
        {
            private SqlNodeAnnotations annotations;
            private SqlFactory sql;

            internal Visitor(SqlNodeAnnotations annotations, SqlFactory sql)
            {
                this.annotations = annotations;
                this.sql = sql;
            }

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                base.VisitSelect(select);
                if (select.Top != null)
                {
                    if (select.Top.NodeType == SqlNodeType.Value)
                    {
                        SqlValue top = (SqlValue) select.Top;
                        if (top.IsClientSpecified)
                        {
                            select.Top = this.sql.Value(top.ClrType, top.SqlType, top.Value, false, top.SourceExpression);
                        }
                        return select;
                    }
                    this.annotations.Add(select.Top, new SqlServerCompatibilityAnnotation(Strings.SourceExpressionAnnotation(select.Top.SourceExpression), new SqlProvider.ProviderMode[] { SqlProvider.ProviderMode.Sql2000 }));
                }
                return select;
            }
        }
    }
}

