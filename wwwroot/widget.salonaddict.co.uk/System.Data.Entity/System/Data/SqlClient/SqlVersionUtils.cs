namespace System.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.Entity;

    internal static class SqlVersionUtils
    {
        internal static SqlVersion GetSqlVersion(SqlConnection connection)
        {
            if (!connection.IsYukonOrNewer)
            {
                return SqlVersion.Sql8;
            }
            if (connection.IsKatmaiOrNewer)
            {
                return SqlVersion.Sql10;
            }
            return SqlVersion.Sql9;
        }

        internal static SqlVersion GetSqlVersion(string versionHint)
        {
            string str;
            if (!string.IsNullOrEmpty(versionHint) && ((str = versionHint) != null))
            {
                if (str == "2000")
                {
                    return SqlVersion.Sql8;
                }
                if (str == "2005")
                {
                    return SqlVersion.Sql9;
                }
                if (str == "2008")
                {
                    return SqlVersion.Sql10;
                }
            }
            throw EntityUtil.Argument(Strings.UnableToDetermineStoreVersion);
        }

        internal static string GetVersionHint(SqlVersion version)
        {
            SqlVersion version2 = version;
            if (version2 != SqlVersion.Sql8)
            {
                if (version2 == SqlVersion.Sql9)
                {
                    return "2005";
                }
                if (version2 != SqlVersion.Sql10)
                {
                    throw EntityUtil.Argument(Strings.UnableToDetermineStoreVersion);
                }
                return "2008";
            }
            return "2000";
        }

        internal static bool IsPreKatmai(SqlVersion sqlVersion)
        {
            if (sqlVersion != SqlVersion.Sql8)
            {
                return (sqlVersion == SqlVersion.Sql9);
            }
            return true;
        }
    }
}

