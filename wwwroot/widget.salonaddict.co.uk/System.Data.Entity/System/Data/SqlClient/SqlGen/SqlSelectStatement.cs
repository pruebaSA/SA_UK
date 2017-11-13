namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal sealed class SqlSelectStatement : ISqlFragment
    {
        private List<Symbol> allJoinExtents;
        private SqlBuilder from = new SqlBuilder();
        private List<Symbol> fromExtents;
        private SqlBuilder groupBy;
        private bool isDistinct;
        private bool isTopMost;
        private SqlBuilder orderBy;
        private Dictionary<Symbol, bool> outerExtents;
        private Dictionary<string, Symbol> outputColumns;
        private bool outputColumnsRenamed;
        private SqlBuilder select = new SqlBuilder();
        private TopClause top;
        private SqlBuilder where;

        public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
        {
            List<string> list = null;
            if ((this.outerExtents != null) && (0 < this.outerExtents.Count))
            {
                foreach (Symbol symbol in this.outerExtents.Keys)
                {
                    JoinSymbol symbol2 = symbol as JoinSymbol;
                    if (symbol2 != null)
                    {
                        foreach (Symbol symbol3 in symbol2.FlattenedExtentList)
                        {
                            if (list == null)
                            {
                                list = new List<string>();
                            }
                            list.Add(symbol3.NewName);
                        }
                    }
                    else
                    {
                        if (list == null)
                        {
                            list = new List<string>();
                        }
                        list.Add(symbol.NewName);
                    }
                }
            }
            List<Symbol> list2 = this.AllJoinExtents ?? this.fromExtents;
            if (list2 != null)
            {
                foreach (Symbol symbol4 in list2)
                {
                    if ((list != null) && list.Contains(symbol4.Name))
                    {
                        string str;
                        int num = sqlGenerator.AllExtentNames[symbol4.Name];
                        do
                        {
                            num++;
                            str = symbol4.Name + num.ToString(CultureInfo.InvariantCulture);
                        }
                        while (sqlGenerator.AllExtentNames.ContainsKey(str));
                        sqlGenerator.AllExtentNames[symbol4.Name] = num;
                        symbol4.NewName = str;
                        sqlGenerator.AllExtentNames[str] = 0;
                    }
                    if (list == null)
                    {
                        list = new List<string>();
                    }
                    list.Add(symbol4.NewName);
                }
            }
            writer.Indent++;
            writer.Write("SELECT ");
            if (this.IsDistinct)
            {
                writer.Write("DISTINCT ");
            }
            if (this.Top != null)
            {
                this.Top.WriteSql(writer, sqlGenerator);
            }
            if ((this.select == null) || this.Select.IsEmpty)
            {
                writer.Write("*");
            }
            else
            {
                this.Select.WriteSql(writer, sqlGenerator);
            }
            writer.WriteLine();
            writer.Write("FROM ");
            this.From.WriteSql(writer, sqlGenerator);
            if ((this.where != null) && !this.Where.IsEmpty)
            {
                writer.WriteLine();
                writer.Write("WHERE ");
                this.Where.WriteSql(writer, sqlGenerator);
            }
            if ((this.groupBy != null) && !this.GroupBy.IsEmpty)
            {
                writer.WriteLine();
                writer.Write("GROUP BY ");
                this.GroupBy.WriteSql(writer, sqlGenerator);
            }
            if (((this.orderBy != null) && !this.OrderBy.IsEmpty) && (this.IsTopMost || (this.Top != null)))
            {
                writer.WriteLine();
                writer.Write("ORDER BY ");
                this.OrderBy.WriteSql(writer, sqlGenerator);
            }
            writer.Indent--;
        }

        internal List<Symbol> AllJoinExtents
        {
            get => 
                this.allJoinExtents;
            set
            {
                this.allJoinExtents = value;
            }
        }

        internal SqlBuilder From =>
            this.from;

        internal List<Symbol> FromExtents
        {
            get
            {
                if (this.fromExtents == null)
                {
                    this.fromExtents = new List<Symbol>();
                }
                return this.fromExtents;
            }
        }

        internal SqlBuilder GroupBy
        {
            get
            {
                if (this.groupBy == null)
                {
                    this.groupBy = new SqlBuilder();
                }
                return this.groupBy;
            }
        }

        internal bool IsDistinct
        {
            get => 
                this.isDistinct;
            set
            {
                this.isDistinct = value;
            }
        }

        internal bool IsTopMost
        {
            get => 
                this.isTopMost;
            set
            {
                this.isTopMost = value;
            }
        }

        public SqlBuilder OrderBy
        {
            get
            {
                if (this.orderBy == null)
                {
                    this.orderBy = new SqlBuilder();
                }
                return this.orderBy;
            }
        }

        internal Dictionary<Symbol, bool> OuterExtents
        {
            get
            {
                if (this.outerExtents == null)
                {
                    this.outerExtents = new Dictionary<Symbol, bool>();
                }
                return this.outerExtents;
            }
        }

        internal Dictionary<string, Symbol> OutputColumns
        {
            get => 
                this.outputColumns;
            set
            {
                this.outputColumns = value;
            }
        }

        internal bool OutputColumnsRenamed
        {
            get => 
                this.outputColumnsRenamed;
            set
            {
                this.outputColumnsRenamed = value;
            }
        }

        internal SqlBuilder Select =>
            this.select;

        internal TopClause Top
        {
            get => 
                this.top;
            set
            {
                this.top = value;
            }
        }

        internal SqlBuilder Where
        {
            get
            {
                if (this.where == null)
                {
                    this.where = new SqlBuilder();
                }
                return this.where;
            }
        }
    }
}

