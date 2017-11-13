namespace System.Data.SqlClient.SqlGen
{
    using System;

    internal class SymbolPair : ISqlFragment
    {
        public Symbol Column;
        public Symbol Source;

        public SymbolPair(Symbol source, Symbol column)
        {
            this.Source = source;
            this.Column = column;
        }

        public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
        {
        }
    }
}

