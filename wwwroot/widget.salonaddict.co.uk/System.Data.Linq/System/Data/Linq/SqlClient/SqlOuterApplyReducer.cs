namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class SqlOuterApplyReducer
    {
        internal static SqlNode Reduce(SqlNode node, SqlFactory factory, SqlNodeAnnotations annotations)
        {
            Visitor visitor = new Visitor(factory, annotations);
            return visitor.Visit(node);
        }

        private static class SqlAliasDependencyChecker
        {
            internal static bool IsDependent(SqlNode node, HashSet<SqlAlias> aliasesToCheck, HashSet<SqlExpression> ignoreExpressions)
            {
                Visitor visitor = new Visitor(aliasesToCheck, ignoreExpressions);
                visitor.Visit(node);
                return visitor.hasDependency;
            }

            private class Visitor : SqlVisitor
            {
                private HashSet<SqlAlias> aliasesToCheck;
                internal bool hasDependency;
                private HashSet<SqlExpression> ignoreExpressions;

                internal Visitor(HashSet<SqlAlias> aliasesToCheck, HashSet<SqlExpression> ignoreExpressions)
                {
                    this.aliasesToCheck = aliasesToCheck;
                    this.ignoreExpressions = ignoreExpressions;
                }

                internal override SqlNode Visit(SqlNode node)
                {
                    SqlExpression item = node as SqlExpression;
                    if (this.hasDependency)
                    {
                        return node;
                    }
                    if ((item != null) && this.ignoreExpressions.Contains(item))
                    {
                        return node;
                    }
                    return base.Visit(node);
                }

                internal override SqlExpression VisitColumn(SqlColumn col)
                {
                    if (col.Expression != null)
                    {
                        this.Visit(col.Expression);
                    }
                    return col;
                }

                internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
                {
                    if (this.aliasesToCheck.Contains(cref.Column.Alias))
                    {
                        this.hasDependency = true;
                        return cref;
                    }
                    if (cref.Column.Expression != null)
                    {
                        this.Visit(cref.Column.Expression);
                    }
                    return cref;
                }
            }
        }

        private class SqlAliasesReferenced
        {
            private HashSet<SqlAlias> aliases;
            private bool referencesAny;
            private Visitor visitor;

            internal SqlAliasesReferenced(HashSet<SqlAlias> aliases)
            {
                this.aliases = aliases;
                this.visitor = new Visitor(this);
            }

            internal bool ReferencesAny(SqlExpression expression)
            {
                this.referencesAny = false;
                this.visitor.Visit(expression);
                return this.referencesAny;
            }

            private class Visitor : SqlVisitor
            {
                private SqlOuterApplyReducer.SqlAliasesReferenced parent;

                internal Visitor(SqlOuterApplyReducer.SqlAliasesReferenced parent)
                {
                    this.parent = parent;
                }

                internal override SqlExpression VisitColumn(SqlColumn col)
                {
                    if (col.Expression != null)
                    {
                        this.Visit(col.Expression);
                    }
                    return col;
                }

                internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
                {
                    if (this.parent.aliases.Contains(cref.Column.Alias))
                    {
                        this.parent.referencesAny = true;
                        return cref;
                    }
                    if (cref.Column.Expression != null)
                    {
                        this.Visit(cref.Column.Expression);
                    }
                    return cref;
                }
            }
        }

        private class SqlGatherReferencedColumns
        {
            private SqlGatherReferencedColumns()
            {
            }

            internal static HashSet<SqlColumn> Gather(SqlNode node, HashSet<SqlColumn> columns)
            {
                new Visitor(columns).Visit(node);
                return columns;
            }

            private class Visitor : SqlVisitor
            {
                private HashSet<SqlColumn> columns;

                internal Visitor(HashSet<SqlColumn> columns)
                {
                    this.columns = columns;
                }

                internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
                {
                    if (!this.columns.Contains(cref.Column))
                    {
                        this.columns.Add(cref.Column);
                        if (cref.Column.Expression != null)
                        {
                            this.Visit(cref.Column.Expression);
                        }
                    }
                    return cref;
                }
            }
        }

        private static class SqlPredicateLifter
        {
            internal static bool CanLift(SqlSource source, HashSet<SqlAlias> aliasesForLifting, HashSet<SqlExpression> liftedExpressions)
            {
                Visitor visitor = new Visitor(false, aliasesForLifting, liftedExpressions);
                visitor.VisitSource(source);
                return visitor.canLiftAll;
            }

            internal static SqlExpression Lift(SqlSource source, HashSet<SqlAlias> aliasesForLifting)
            {
                Visitor visitor = new Visitor(true, aliasesForLifting, null);
                visitor.VisitSource(source);
                return visitor.lifted;
            }

            private class Visitor : SqlVisitor
            {
                private SqlAggregateChecker aggregateChecker;
                private SqlOuterApplyReducer.SqlAliasesReferenced aliases;
                internal bool canLiftAll;
                private bool doLifting;
                internal SqlExpression lifted;
                private HashSet<SqlExpression> liftedExpressions;

                internal Visitor(bool doLifting, HashSet<SqlAlias> aliasesForLifting, HashSet<SqlExpression> liftedExpressions)
                {
                    this.doLifting = doLifting;
                    this.aliases = new SqlOuterApplyReducer.SqlAliasesReferenced(aliasesForLifting);
                    this.liftedExpressions = liftedExpressions;
                    this.canLiftAll = true;
                    this.aggregateChecker = new SqlAggregateChecker();
                }

                internal override SqlSelect VisitSelect(SqlSelect select)
                {
                    this.VisitSource(select.From);
                    if (((select.Top != null) || (select.GroupBy.Count > 0)) || (this.aggregateChecker.HasAggregates(select) || select.IsDistinct))
                    {
                        this.canLiftAll = false;
                    }
                    if ((this.canLiftAll && (select.Where != null)) && this.aliases.ReferencesAny(select.Where))
                    {
                        if (this.liftedExpressions != null)
                        {
                            this.liftedExpressions.Add(select.Where);
                        }
                        if (!this.doLifting)
                        {
                            return select;
                        }
                        if (this.lifted != null)
                        {
                            this.lifted = new SqlBinary(SqlNodeType.And, this.lifted.ClrType, this.lifted.SqlType, this.lifted, select.Where);
                        }
                        else
                        {
                            this.lifted = select.Where;
                        }
                        select.Where = null;
                    }
                    return select;
                }
            }
        }

        private static class SqlSelectionLifter
        {
            internal static bool CanLift(SqlSource source, HashSet<SqlAlias> aliasesForLifting, HashSet<SqlExpression> liftedExpressions)
            {
                Visitor visitor = new Visitor(false, aliasesForLifting, liftedExpressions);
                visitor.VisitSource(source);
                return visitor.canLiftAll;
            }

            internal static List<List<SqlColumn>> Lift(SqlSource source, HashSet<SqlAlias> aliasesForLifting, HashSet<SqlExpression> liftedExpressions)
            {
                Visitor visitor = new Visitor(true, aliasesForLifting, liftedExpressions);
                visitor.VisitSource(source);
                return visitor.lifted;
            }

            private class Visitor : SqlVisitor
            {
                private SqlAggregateChecker aggregateChecker;
                private SqlOuterApplyReducer.SqlAliasesReferenced aliases;
                internal bool canLiftAll;
                private bool doLifting;
                private bool hasLifted;
                internal List<List<SqlColumn>> lifted;
                private HashSet<SqlExpression> liftedExpressions;
                private HashSet<SqlColumn> referencedColumns;

                internal Visitor(bool doLifting, HashSet<SqlAlias> aliasesForLifting, HashSet<SqlExpression> liftedExpressions)
                {
                    this.doLifting = doLifting;
                    this.aliases = new SqlOuterApplyReducer.SqlAliasesReferenced(aliasesForLifting);
                    this.referencedColumns = new HashSet<SqlColumn>();
                    this.liftedExpressions = liftedExpressions;
                    this.canLiftAll = true;
                    if (doLifting)
                    {
                        this.lifted = new List<List<SqlColumn>>();
                    }
                    this.aggregateChecker = new SqlAggregateChecker();
                }

                private void ReferenceColumns(SqlExpression expression)
                {
                    if ((expression != null) && ((this.liftedExpressions == null) || !this.liftedExpressions.Contains(expression)))
                    {
                        SqlOuterApplyReducer.SqlGatherReferencedColumns.Gather(expression, this.referencedColumns);
                    }
                }

                internal override SqlSource VisitJoin(SqlJoin join)
                {
                    this.ReferenceColumns(join.Condition);
                    return base.VisitJoin(join);
                }

                internal override SqlSelect VisitSelect(SqlSelect select)
                {
                    this.ReferenceColumns(select.Where);
                    foreach (SqlOrderExpression expression in select.OrderBy)
                    {
                        this.ReferenceColumns(expression.Expression);
                    }
                    foreach (SqlExpression expression2 in select.GroupBy)
                    {
                        this.ReferenceColumns(expression2);
                    }
                    this.ReferenceColumns(select.Having);
                    List<SqlColumn> item = null;
                    List<SqlColumn> collection = null;
                    foreach (SqlColumn column in select.Row.Columns)
                    {
                        bool flag = this.aliases.ReferencesAny(column.Expression);
                        bool flag2 = this.referencedColumns.Contains(column);
                        if (flag)
                        {
                            if (flag2)
                            {
                                this.canLiftAll = false;
                                this.ReferenceColumns(column);
                            }
                            else
                            {
                                this.hasLifted = true;
                                if (this.doLifting)
                                {
                                    if (item == null)
                                    {
                                        item = new List<SqlColumn>();
                                    }
                                    item.Add(column);
                                }
                            }
                        }
                        else
                        {
                            if (this.doLifting)
                            {
                                if (collection == null)
                                {
                                    collection = new List<SqlColumn>();
                                }
                                collection.Add(column);
                            }
                            this.ReferenceColumns(column);
                        }
                    }
                    if (this.canLiftAll)
                    {
                        this.VisitSource(select.From);
                    }
                    if ((((select.Top != null) || (select.GroupBy.Count > 0)) || (this.aggregateChecker.HasAggregates(select) || select.IsDistinct)) && this.hasLifted)
                    {
                        this.canLiftAll = false;
                    }
                    if (this.doLifting && this.canLiftAll)
                    {
                        select.Row.Columns.Clear();
                        if (collection != null)
                        {
                            select.Row.Columns.AddRange(collection);
                        }
                        if (item != null)
                        {
                            this.lifted.Add(item);
                        }
                    }
                    return select;
                }
            }
        }

        private class Visitor : SqlVisitor
        {
            private SqlNodeAnnotations annotations;
            private SqlFactory factory;

            internal Visitor(SqlFactory factory, SqlNodeAnnotations annotations)
            {
                this.factory = factory;
                this.annotations = annotations;
            }

            private void AnnotateSqlIncompatibility(SqlNode node, params SqlProvider.ProviderMode[] providers)
            {
                this.annotations.Add(node, new SqlServerCompatibilityAnnotation(Strings.SourceExpressionAnnotation(node.SourceExpression), providers));
            }

            private SqlJoin GetLeftOuterWithUnreferencedSingletonOnLeft(SqlSource source)
            {
                SqlAlias alias = source as SqlAlias;
                if (alias != null)
                {
                    SqlSelect node = alias.Node as SqlSelect;
                    if ((((node != null) && (node.Where == null)) && ((node.Top == null) && (node.GroupBy.Count == 0))) && (node.OrderBy.Count == 0))
                    {
                        return this.GetLeftOuterWithUnreferencedSingletonOnLeft(node.From);
                    }
                }
                SqlJoin join = source as SqlJoin;
                if ((join == null) || (join.JoinType != SqlJoinType.LeftOuter))
                {
                    return null;
                }
                if (!this.IsSingletonSelect(join.Left))
                {
                    return null;
                }
                HashSet<SqlAlias> set = SqlGatherProducedAliases.Gather(join.Left);
                HashSet<SqlAlias> other = SqlGatherConsumedAliases.Gather(join.Right);
                if (set.Overlaps(other))
                {
                    return null;
                }
                return join;
            }

            private void GetSelectionsBeforeJoin(SqlSource source, List<List<SqlColumn>> selections)
            {
                if (!(source is SqlJoin))
                {
                    SqlAlias alias = source as SqlAlias;
                    if (alias != null)
                    {
                        SqlSelect node = alias.Node as SqlSelect;
                        if (node != null)
                        {
                            this.GetSelectionsBeforeJoin(node.From, selections);
                            selections.Add(node.Row.Columns);
                        }
                    }
                }
            }

            private bool IsSingletonSelect(SqlSource source)
            {
                SqlAlias alias = source as SqlAlias;
                if (alias == null)
                {
                    return false;
                }
                SqlSelect node = alias.Node as SqlSelect;
                if (node == null)
                {
                    return false;
                }
                if (node.From != null)
                {
                    return false;
                }
                return true;
            }

            private SqlSource PushSourceDown(SqlSource sqlSource, List<SqlColumn> cols)
            {
                SqlSelect node = new SqlSelect(new SqlNop(cols[0].ClrType, cols[0].SqlType, sqlSource.SourceExpression), sqlSource, sqlSource.SourceExpression);
                node.Row.Columns.AddRange(cols);
                return new SqlAlias(node);
            }

            internal override SqlSource VisitSource(SqlSource source)
            {
                source = base.VisitSource(source);
                SqlJoin node = source as SqlJoin;
                if (node != null)
                {
                    if (node.JoinType == SqlJoinType.OuterApply)
                    {
                        HashSet<SqlAlias> aliasesForLifting = SqlGatherProducedAliases.Gather(node.Left);
                        HashSet<SqlExpression> liftedExpressions = new HashSet<SqlExpression>();
                        if ((SqlOuterApplyReducer.SqlPredicateLifter.CanLift(node.Right, aliasesForLifting, liftedExpressions) && SqlOuterApplyReducer.SqlSelectionLifter.CanLift(node.Right, aliasesForLifting, liftedExpressions)) && !SqlOuterApplyReducer.SqlAliasDependencyChecker.IsDependent(node.Right, aliasesForLifting, liftedExpressions))
                        {
                            SqlExpression expression = SqlOuterApplyReducer.SqlPredicateLifter.Lift(node.Right, aliasesForLifting);
                            List<List<SqlColumn>> list = SqlOuterApplyReducer.SqlSelectionLifter.Lift(node.Right, aliasesForLifting, liftedExpressions);
                            node.JoinType = SqlJoinType.LeftOuter;
                            node.Condition = expression;
                            if (list != null)
                            {
                                foreach (List<SqlColumn> list2 in list)
                                {
                                    source = this.PushSourceDown(source, list2);
                                }
                            }
                        }
                        else
                        {
                            this.AnnotateSqlIncompatibility(node, new SqlProvider.ProviderMode[] { SqlProvider.ProviderMode.Sql2000 });
                        }
                    }
                    else if (node.JoinType == SqlJoinType.CrossApply)
                    {
                        SqlJoin leftOuterWithUnreferencedSingletonOnLeft = this.GetLeftOuterWithUnreferencedSingletonOnLeft(node.Right);
                        if (leftOuterWithUnreferencedSingletonOnLeft != null)
                        {
                            HashSet<SqlAlias> set3 = SqlGatherProducedAliases.Gather(node.Left);
                            HashSet<SqlExpression> set4 = new HashSet<SqlExpression>();
                            if ((SqlOuterApplyReducer.SqlPredicateLifter.CanLift(leftOuterWithUnreferencedSingletonOnLeft.Right, set3, set4) && SqlOuterApplyReducer.SqlSelectionLifter.CanLift(leftOuterWithUnreferencedSingletonOnLeft.Right, set3, set4)) && !SqlOuterApplyReducer.SqlAliasDependencyChecker.IsDependent(leftOuterWithUnreferencedSingletonOnLeft.Right, set3, set4))
                            {
                                SqlExpression right = SqlOuterApplyReducer.SqlPredicateLifter.Lift(leftOuterWithUnreferencedSingletonOnLeft.Right, set3);
                                List<List<SqlColumn>> selections = SqlOuterApplyReducer.SqlSelectionLifter.Lift(leftOuterWithUnreferencedSingletonOnLeft.Right, set3, set4);
                                this.GetSelectionsBeforeJoin(node.Right, selections);
                                foreach (List<SqlColumn> list4 in from s in selections
                                    where s.Count > 0
                                    select s)
                                {
                                    source = this.PushSourceDown(source, list4);
                                }
                                node.JoinType = SqlJoinType.LeftOuter;
                                node.Condition = this.factory.AndAccumulate(leftOuterWithUnreferencedSingletonOnLeft.Condition, right);
                                node.Right = leftOuterWithUnreferencedSingletonOnLeft.Right;
                            }
                            else
                            {
                                this.AnnotateSqlIncompatibility(node, new SqlProvider.ProviderMode[] { SqlProvider.ProviderMode.Sql2000 });
                            }
                        }
                    }
                    while (node.JoinType == SqlJoinType.LeftOuter)
                    {
                        SqlJoin join3 = this.GetLeftOuterWithUnreferencedSingletonOnLeft(node.Left);
                        if (join3 == null)
                        {
                            return source;
                        }
                        List<List<SqlColumn>> list5 = new List<List<SqlColumn>>();
                        this.GetSelectionsBeforeJoin(node.Left, list5);
                        foreach (List<SqlColumn> list6 in list5)
                        {
                            source = this.PushSourceDown(source, list6);
                        }
                        SqlSource source2 = node.Right;
                        SqlExpression condition = node.Condition;
                        node.Left = join3.Left;
                        node.Right = join3;
                        node.Condition = join3.Condition;
                        join3.Left = join3.Right;
                        join3.Right = source2;
                        join3.Condition = condition;
                    }
                }
                return source;
            }
        }
    }
}

