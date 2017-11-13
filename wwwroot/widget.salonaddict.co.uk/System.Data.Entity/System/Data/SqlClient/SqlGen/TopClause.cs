namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Data.SqlClient;
    using System.Globalization;

    internal class TopClause : ISqlFragment
    {
        private ISqlFragment topCount;
        private bool withTies;

        internal TopClause(ISqlFragment topCount, bool withTies)
        {
            this.topCount = topCount;
            this.withTies = withTies;
        }

        internal TopClause(int topCount, bool withTies)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append(topCount.ToString(CultureInfo.InvariantCulture));
            this.topCount = builder;
            this.withTies = withTies;
        }

        public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
        {
            writer.Write("TOP ");
            if (sqlGenerator.SqlVersion != SqlVersion.Sql8)
            {
                writer.Write("(");
            }
            this.TopCount.WriteSql(writer, sqlGenerator);
            if (sqlGenerator.SqlVersion != SqlVersion.Sql8)
            {
                writer.Write(")");
            }
            writer.Write(" ");
            if (this.WithTies)
            {
                writer.Write("WITH TIES ");
            }
        }

        internal ISqlFragment TopCount =>
            this.topCount;

        internal bool WithTies =>
            this.withTies;
    }
}

