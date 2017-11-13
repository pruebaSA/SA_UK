namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class SqlNamer
    {
        private Visitor visitor = new Visitor();

        internal SqlNamer()
        {
        }

        internal SqlNode AssignNames(SqlNode node) => 
            this.visitor.Visit(node);

        internal static string DiscoverName(SqlExpression e)
        {
            if (e != null)
            {
                switch (e.NodeType)
                {
                    case SqlNodeType.Column:
                        return DiscoverName(((SqlColumn) e).Expression);

                    case SqlNodeType.ColumnRef:
                    {
                        SqlColumnRef ref2 = (SqlColumnRef) e;
                        if (ref2.Column.Name == null)
                        {
                            return DiscoverName(ref2.Column);
                        }
                        return ref2.Column.Name;
                    }
                    case SqlNodeType.ExprSet:
                    {
                        SqlExprSet set = (SqlExprSet) e;
                        return DiscoverName(set.Expressions[0]);
                    }
                }
            }
            return "value";
        }

        private class ColumnNameGatherer : SqlVisitor
        {
            public ColumnNameGatherer()
            {
                this.Names = new HashSet<string>();
            }

            internal override SqlExpression VisitColumn(SqlColumn col)
            {
                if (!string.IsNullOrEmpty(col.Name))
                {
                    this.Names.Add(col.Name);
                }
                return base.VisitColumn(col);
            }

            internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
            {
                this.Visit(cref.Column);
                return base.VisitColumnRef(cref);
            }

            public HashSet<string> Names { get; set; }
        }

        private class Visitor : SqlVisitor
        {
            private SqlAlias alias;
            private int aliasCount;
            private string lastName;
            private bool makeUnique = true;
            private bool useMappedNames = false;

            internal Visitor()
            {
            }

            private ICollection<string> GetColumnNames(IEnumerable<SqlOrderExpression> orderList)
            {
                SqlNamer.ColumnNameGatherer gatherer = new SqlNamer.ColumnNameGatherer();
                foreach (SqlOrderExpression expression in orderList)
                {
                    gatherer.Visit(expression.Expression);
                }
                return gatherer.Names;
            }

            internal string GetNextAlias() => 
                ("t" + this.aliasCount++);

            private static bool IsSimpleColumn(SqlColumn c, string name)
            {
                if (c.Expression != null)
                {
                    if (c.Expression.NodeType != SqlNodeType.ColumnRef)
                    {
                        return false;
                    }
                    SqlColumnRef expression = c.Expression as SqlColumnRef;
                    if (!string.IsNullOrEmpty(name))
                    {
                        return (string.Compare(name, expression.Column.Name, StringComparison.OrdinalIgnoreCase) == 0);
                    }
                }
                return true;
            }

            private bool IsUniqueName(List<SqlColumn> columns, ICollection<string> reservedNames, SqlColumn c, string name)
            {
                foreach (SqlColumn column in columns)
                {
                    if ((column != c) && (string.Compare(column.Name, name, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        return false;
                    }
                }
                if (!IsSimpleColumn(c, name))
                {
                    return !reservedNames.Contains(name);
                }
                return true;
            }

            internal override SqlAlias VisitAlias(SqlAlias sqlAlias)
            {
                SqlAlias alias = this.alias;
                this.alias = sqlAlias;
                sqlAlias.Node = this.Visit(sqlAlias.Node);
                sqlAlias.Name = this.GetNextAlias();
                this.alias = alias;
                return sqlAlias;
            }

            internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
            {
                if ((cref.Column.Name == null) && (this.lastName != null))
                {
                    cref.Column.Name = this.lastName;
                }
                return cref;
            }

            internal override SqlExpression VisitExpression(SqlExpression expr)
            {
                SqlExpression expression;
                string lastName = this.lastName;
                this.lastName = null;
                try
                {
                    expression = (SqlExpression) this.Visit(expr);
                }
                finally
                {
                    this.lastName = lastName;
                }
                return expression;
            }

            internal override SqlExpression VisitGrouping(SqlGrouping g)
            {
                g.Key = this.VisitNamedExpression(g.Key, "Key");
                g.Group = this.VisitNamedExpression(g.Group, "Group");
                return g;
            }

            internal override SqlStatement VisitInsert(SqlInsert insert)
            {
                bool makeUnique = this.makeUnique;
                this.makeUnique = false;
                bool useMappedNames = this.useMappedNames;
                this.useMappedNames = true;
                SqlStatement statement = base.VisitInsert(insert);
                this.makeUnique = makeUnique;
                this.useMappedNames = useMappedNames;
                return statement;
            }

            internal override SqlExpression VisitMethodCall(SqlMethodCall mc)
            {
                mc.Object = this.VisitExpression(mc.Object);
                ParameterInfo[] parameters = mc.Method.GetParameters();
                int index = 0;
                int count = mc.Arguments.Count;
                while (index < count)
                {
                    mc.Arguments[index] = this.VisitNamedExpression(mc.Arguments[index], parameters[index].Name);
                    index++;
                }
                return mc;
            }

            private SqlExpression VisitNamedExpression(SqlExpression expr, string name)
            {
                SqlExpression expression;
                string lastName = this.lastName;
                this.lastName = name;
                try
                {
                    expression = (SqlExpression) this.Visit(expr);
                }
                finally
                {
                    this.lastName = lastName;
                }
                return expression;
            }

            internal override SqlExpression VisitNew(SqlNew sox)
            {
                if (sox.Constructor != null)
                {
                    ParameterInfo[] parameters = sox.Constructor.GetParameters();
                    int index = 0;
                    int count = sox.Args.Count;
                    while (index < count)
                    {
                        sox.Args[index] = this.VisitNamedExpression(sox.Args[index], parameters[index].Name);
                        index++;
                    }
                }
                else
                {
                    int num3 = 0;
                    int num4 = sox.Args.Count;
                    while (num3 < num4)
                    {
                        sox.Args[num3] = this.VisitExpression(sox.Args[num3]);
                        num3++;
                    }
                }
                foreach (SqlMemberAssign assign in sox.Members)
                {
                    string name = assign.Member.Name;
                    if (this.useMappedNames)
                    {
                        name = sox.MetaType.GetDataMember(assign.Member).MappedName;
                    }
                    assign.Expression = this.VisitNamedExpression(assign.Expression, name);
                }
                return sox;
            }

            internal override SqlExpression VisitOptionalValue(SqlOptionalValue sov)
            {
                sov.HasValue = this.VisitNamedExpression(sov.HasValue, "test");
                sov.Value = this.VisitExpression(sov.Value);
                return sov;
            }

            internal override SqlExpression VisitScalarSubSelect(SqlSubSelect ss)
            {
                base.VisitScalarSubSelect(ss);
                if (ss.Select.Row.Columns.Count > 0)
                {
                    ss.Select.Row.Columns[0].Name = "";
                }
                return ss;
            }

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                select = base.VisitSelect(select);
                string[] strArray = new string[select.Row.Columns.Count];
                int index = 0;
                int length = strArray.Length;
                while (index < length)
                {
                    SqlColumn e = select.Row.Columns[index];
                    string name = e.Name;
                    if (name == null)
                    {
                        name = SqlNamer.DiscoverName(e);
                    }
                    strArray[index] = name;
                    e.Name = null;
                    index++;
                }
                ICollection<string> columnNames = this.GetColumnNames(select.OrderBy);
                int num3 = 0;
                int count = select.Row.Columns.Count;
                while (num3 < count)
                {
                    SqlColumn c = select.Row.Columns[num3];
                    string str2 = strArray[num3];
                    string str3 = str2;
                    if (this.makeUnique)
                    {
                        int num5 = 1;
                        while (!this.IsUniqueName(select.Row.Columns, columnNames, c, str3))
                        {
                            num5++;
                            str3 = str2 + num5;
                        }
                    }
                    c.Name = str3;
                    c.Ordinal = num3;
                    num3++;
                }
                return select;
            }

            internal override SqlStatement VisitUpdate(SqlUpdate update)
            {
                bool makeUnique = this.makeUnique;
                this.makeUnique = false;
                bool useMappedNames = this.useMappedNames;
                this.useMappedNames = true;
                SqlStatement statement = base.VisitUpdate(update);
                this.makeUnique = makeUnique;
                this.useMappedNames = useMappedNames;
                return statement;
            }
        }
    }
}

