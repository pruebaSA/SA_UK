namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class SqlGatherConsumedAliases
    {
        internal static HashSet<SqlAlias> Gather(SqlNode node)
        {
            Gatherer gatherer = new Gatherer();
            gatherer.Visit(node);
            return gatherer.Consumed;
        }

        private class Gatherer : SqlVisitor
        {
            internal HashSet<SqlAlias> Consumed = new HashSet<SqlAlias>();

            internal void VisitAliasConsumed(SqlAlias a)
            {
                this.Consumed.Add(a);
            }

            internal override SqlExpression VisitColumn(SqlColumn col)
            {
                this.VisitAliasConsumed(col.Alias);
                this.VisitExpression(col.Expression);
                return col;
            }

            internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
            {
                this.VisitAliasConsumed(cref.Column.Alias);
                return cref;
            }
        }
    }
}

