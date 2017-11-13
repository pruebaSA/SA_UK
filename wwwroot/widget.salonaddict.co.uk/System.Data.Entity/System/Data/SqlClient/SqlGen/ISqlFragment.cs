namespace System.Data.SqlClient.SqlGen
{
    using System;

    internal interface ISqlFragment
    {
        void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator);
    }
}

