namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal static class SqlAliasesReferenced
    {
        internal static bool ReferencesAny(SqlNode node, IEnumerable<SqlAlias> aliases)
        {
            Visitor visitor = new Visitor {
                aliases = aliases
            };
            visitor.Visit(node);
            return visitor.referencesAnyMatchingAliases;
        }

        private class Visitor : SqlVisitor
        {
            internal IEnumerable<SqlAlias> aliases;
            internal bool referencesAnyMatchingAliases;

            internal override SqlNode Visit(SqlNode node)
            {
                if (this.referencesAnyMatchingAliases)
                {
                    return node;
                }
                return base.Visit(node);
            }

            internal SqlAlias VisitAliasConsumed(SqlAlias a)
            {
                if (a != null)
                {
                    bool flag = false;
                    foreach (SqlAlias alias in this.aliases)
                    {
                        if (alias == a)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        this.referencesAnyMatchingAliases = true;
                    }
                }
                return a;
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
                this.VisitExpression(cref.Column.Expression);
                return cref;
            }
        }
    }
}

