namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class SqlGatherProducedAliases
    {
        internal static HashSet<SqlAlias> Gather(SqlNode node)
        {
            Gatherer gatherer = new Gatherer();
            gatherer.Visit(node);
            return gatherer.Produced;
        }

        private class Gatherer : SqlVisitor
        {
            internal HashSet<SqlAlias> Produced = new HashSet<SqlAlias>();

            internal override SqlAlias VisitAlias(SqlAlias a)
            {
                this.Produced.Add(a);
                return base.VisitAlias(a);
            }
        }
    }
}

