namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Globalization;

    internal class Symbol : ISqlFragment
    {
        private Dictionary<string, Symbol> columns;
        private string name;
        private bool needsRenaming;
        private string newName;
        private bool outputColumnsRenamed;
        private TypeUsage type;

        public Symbol(string name, TypeUsage type)
        {
            this.name = name;
            this.newName = name;
            this.Type = type;
        }

        public Symbol(string name, TypeUsage type, Dictionary<string, Symbol> columns)
        {
            this.name = name;
            this.newName = name;
            this.Type = type;
            this.columns = columns;
            this.OutputColumnsRenamed = true;
        }

        public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
        {
            if (this.NeedsRenaming)
            {
                int num;
                if (sqlGenerator.AllColumnNames.TryGetValue(this.NewName, out num))
                {
                    string str;
                    do
                    {
                        num++;
                        str = this.NewName + num.ToString(CultureInfo.InvariantCulture);
                    }
                    while (sqlGenerator.AllColumnNames.ContainsKey(str));
                    sqlGenerator.AllColumnNames[this.NewName] = num;
                    this.NewName = str;
                }
                sqlGenerator.AllColumnNames[this.NewName] = 0;
                this.NeedsRenaming = false;
            }
            writer.Write(SqlGenerator.QuoteIdentifier(this.NewName));
        }

        internal Dictionary<string, Symbol> Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new Dictionary<string, Symbol>(StringComparer.OrdinalIgnoreCase);
                }
                return this.columns;
            }
        }

        public string Name =>
            this.name;

        internal bool NeedsRenaming
        {
            get => 
                this.needsRenaming;
            set
            {
                this.needsRenaming = value;
            }
        }

        public string NewName
        {
            get => 
                this.newName;
            set
            {
                this.newName = value;
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

        internal TypeUsage Type
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
        }
    }
}

