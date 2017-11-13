namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlAliaser
    {
        private Visitor visitor = new Visitor();

        internal SqlAliaser()
        {
        }

        internal SqlNode AssociateColumnsWithAliases(SqlNode node) => 
            this.visitor.Visit(node);

        private class Visitor : SqlVisitor
        {
            private SqlAlias alias;

            internal Visitor()
            {
            }

            internal override SqlAlias VisitAlias(SqlAlias sqlAlias)
            {
                SqlAlias alias = this.alias;
                this.alias = sqlAlias;
                sqlAlias.Node = this.Visit(sqlAlias.Node);
                this.alias = alias;
                return sqlAlias;
            }

            internal override SqlRow VisitRow(SqlRow row)
            {
                foreach (SqlColumn column in row.Columns)
                {
                    column.Alias = this.alias;
                }
                return base.VisitRow(row);
            }

            internal override SqlTable VisitTable(SqlTable tab)
            {
                foreach (SqlColumn column in tab.Columns)
                {
                    column.Alias = this.alias;
                }
                return base.VisitTable(tab);
            }

            internal override SqlExpression VisitTableValuedFunctionCall(SqlTableValuedFunctionCall fc)
            {
                foreach (SqlColumn column in fc.Columns)
                {
                    column.Alias = this.alias;
                }
                return base.VisitTableValuedFunctionCall(fc);
            }
        }
    }
}

