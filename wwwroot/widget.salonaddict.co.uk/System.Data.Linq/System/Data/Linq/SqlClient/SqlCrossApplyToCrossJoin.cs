namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class SqlCrossApplyToCrossJoin
    {
        internal static SqlNode Reduce(SqlNode node, SqlNodeAnnotations annotations)
        {
            Reducer reducer = new Reducer {
                Annotations = annotations
            };
            return reducer.Visit(node);
        }

        private class Reducer : SqlVisitor
        {
            internal SqlNodeAnnotations Annotations;

            internal override SqlSource VisitJoin(SqlJoin join)
            {
                if (join.JoinType != SqlJoinType.CrossApply)
                {
                    return base.VisitJoin(join);
                }
                HashSet<SqlAlias> set = SqlGatherProducedAliases.Gather(join.Left);
                HashSet<SqlAlias> other = SqlGatherConsumedAliases.Gather(join.Right);
                if (set.Overlaps(other))
                {
                    this.Annotations.Add(join, new SqlServerCompatibilityAnnotation(Strings.SourceExpressionAnnotation(join.SourceExpression), new SqlProvider.ProviderMode[] { SqlProvider.ProviderMode.Sql2000 }));
                    return base.VisitJoin(join);
                }
                join.JoinType = SqlJoinType.Cross;
                return this.VisitJoin(join);
            }
        }
    }
}

