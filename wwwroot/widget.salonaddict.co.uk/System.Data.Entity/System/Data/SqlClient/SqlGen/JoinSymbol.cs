namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    internal sealed class JoinSymbol : Symbol
    {
        private List<Symbol> columnList;
        private List<Symbol> extentList;
        private List<Symbol> flattenedExtentList;
        private bool isNestedJoin;
        private Dictionary<string, Symbol> nameToExtent;

        public JoinSymbol(string name, TypeUsage type, List<Symbol> extents) : base(name, type)
        {
            this.extentList = new List<Symbol>(extents.Count);
            this.nameToExtent = new Dictionary<string, Symbol>(extents.Count, StringComparer.OrdinalIgnoreCase);
            foreach (Symbol symbol in extents)
            {
                this.nameToExtent[symbol.Name] = symbol;
                this.ExtentList.Add(symbol);
            }
        }

        internal List<Symbol> ColumnList
        {
            get
            {
                if (this.columnList == null)
                {
                    this.columnList = new List<Symbol>();
                }
                return this.columnList;
            }
            set
            {
                this.columnList = value;
            }
        }

        internal List<Symbol> ExtentList =>
            this.extentList;

        internal List<Symbol> FlattenedExtentList
        {
            get
            {
                if (this.flattenedExtentList == null)
                {
                    this.flattenedExtentList = new List<Symbol>();
                }
                return this.flattenedExtentList;
            }
            set
            {
                this.flattenedExtentList = value;
            }
        }

        internal bool IsNestedJoin
        {
            get => 
                this.isNestedJoin;
            set
            {
                this.isNestedJoin = value;
            }
        }

        internal Dictionary<string, Symbol> NameToExtent =>
            this.nameToExtent;
    }
}

