namespace System.Web.ClientServices.Providers
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Web.Resources;
    using System.Windows.Forms;

    internal static class SqlHelper
    {
        private const string _Isolated_Storage_Tag = "|Isolated_Storage|";
        private const string _SQL_CE_CONN_STRING = "Data Source = |SQL/CE|";
        private const string _SQL_CE_Tag = "|SQL/CE|";
        private const string _SQL_FILES_Tag = "|FILES|";
        private static System.Type _SqlCeConnectionType;
        private static System.Type _SqlCeParamType;

        internal static void AddParameter(DbConnection conn, DbCommand cmd, string paramName, object paramValue)
        {
            if (!(conn is SqlConnection))
            {
                AddSqlCeParameter(cmd, paramName, paramValue);
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter(paramName, paramValue));
            }
        }

        private static void AddSqlCeParameter(DbCommand cmd, string paramName, object paramValue)
        {
            if (_SqlCeParamType == null)
            {
                _SqlCeParamType = GetSqlCeType("SqlCeParameter");
            }
            cmd.Parameters.Add((DbParameter) Activator.CreateInstance(_SqlCeParamType, new object[] { paramName, paramValue }));
        }

        private static DbConnection CreateDBIfRequired(string username, string connectionString)
        {
            if (!connectionString.Contains("|SQL/CE|"))
            {
                return null;
            }
            DbConnection connection = null;
            try
            {
                connection = CreateNewSqlCeConnection(connectionString, false);
                if (string.Compare(connection.Database.Trim(), "|SQL/CE|", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    connection.Open();
                    return connection;
                }
                connection.Dispose();
                connection = null;
            }
            catch (TypeLoadException exception)
            {
                throw new ArgumentException(AtlasWeb.SqlHelper_SqlEverywhereNotInstalled, exception);
            }
            string fullDBFileName = GetFullDBFileName(username, "_DB.spf");
            bool flag = !File.Exists(fullDBFileName);
            connectionString = connectionString.Replace("|SQL/CE|", fullDBFileName);
            if (flag)
            {
                using (IDisposable disposable = (IDisposable) Activator.CreateInstance(GetSqlCeType("SqlCeEngine"), new object[] { connectionString }))
                {
                    disposable.GetType().InvokeMember("CreateDatabase", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, disposable, null, CultureInfo.InvariantCulture);
                }
                using (connection = CreateNewSqlCeConnection(connectionString, true))
                {
                    DbCommand command = connection.CreateCommand();
                    if (username == null)
                    {
                        command.CommandText = "CREATE TABLE ApplicationProperties (PropertyName nvarchar(256), PropertyValue nvarchar(256))";
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        command.CommandText = "CREATE TABLE UserProperties (PropertyName nvarchar(256), PropertyValue nvarchar(256))";
                        command.ExecuteNonQuery();
                        command = connection.CreateCommand();
                        command.CommandText = "CREATE TABLE Roles (UserName nvarchar(256), RoleName nvarchar(256))";
                        command.ExecuteNonQuery();
                        command = connection.CreateCommand();
                        command.CommandText = "CREATE TABLE Settings (PropertyName nvarchar(256), PropertyStoredAs nvarchar(1), PropertyValue nvarchar(2048))";
                        command.ExecuteNonQuery();
                    }
                }
            }
            return CreateNewSqlCeConnection(connectionString, true);
        }

        private static DbConnection CreateNewSqlCeConnection(string connectionString, bool openConn)
        {
            if (_SqlCeConnectionType == null)
            {
                _SqlCeConnectionType = GetSqlCeType("SqlCeConnection");
            }
            DbConnection connection = (DbConnection) Activator.CreateInstance(_SqlCeConnectionType, new object[] { connectionString });
            if (openConn)
            {
                connection.Open();
            }
            return connection;
        }

        internal static void DeleteAllCookies(string username, string connectionString, string sqlProvider)
        {
            if ((connectionString == "|FILES|") || (connectionString == "|Isolated_Storage|"))
            {
                ClientDataManager.DeleteAllCookies(username, connectionString == "|Isolated_Storage|");
            }
            else
            {
                using (DbConnection connection = GetConnection(username, connectionString, sqlProvider))
                {
                    DbTransaction transaction = null;
                    try
                    {
                        transaction = connection.BeginTransaction();
                        DbCommand command = connection.CreateCommand();
                        command.CommandText = "DELETE FROM UserProperties WHERE PropertyName LIKE N'CookieName_%'";
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        if (transaction != null)
                        {
                            transaction.Rollback();
                            transaction = null;
                        }
                        throw;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
        }

        internal static DbConnection GetConnection(string username, string connectionString, string sqlProvider)
        {
            if (connectionString.Contains("|SQL/CE|") || ((sqlProvider != null) && sqlProvider.Contains(".SqlServerCe")))
            {
                try
                {
                    return GetSqlCeConnection(username, connectionString);
                }
                catch (TypeLoadException exception)
                {
                    throw new ArgumentException(AtlasWeb.SqlHelper_SqlEverywhereNotInstalled, exception);
                }
            }
            DbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        internal static string GetCookieFromDB(string name, string username, string connectionString, string sqlProvider)
        {
            if (connectionString == "|FILES|")
            {
                return ClientDataManager.GetCookie(username, name, false);
            }
            if (connectionString == "|Isolated_Storage|")
            {
                return ClientDataManager.GetCookie(username, name, true);
            }
            using (DbConnection connection = GetConnection(username, connectionString, sqlProvider))
            {
                DbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT PropertyValue FROM UserProperties WHERE PropertyName = @PropName";
                AddParameter(connection, cmd, "@PropName", "CookieName_" + name);
                return (cmd.ExecuteScalar() as string);
            }
        }

        internal static string GetDefaultConnectionString() => 
            "|FILES|";

        internal static string GetFullDBFileName(string username, string extension) => 
            Path.Combine(Application.UserAppDataPath, GetPartialDBFileName(username, extension));

        internal static string GetPartialDBFileName(string username, string extension)
        {
            if (string.IsNullOrEmpty(username))
            {
                return ("Application" + extension);
            }
            char[] chArray = username.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                if (!char.IsLetterOrDigit(chArray[i]))
                {
                    chArray[i] = '_';
                }
            }
            return ("User_" + new string(chArray) + extension);
        }

        private static DbConnection GetSqlCeConnection(string username, string connectionString)
        {
            DbConnection connection = CreateDBIfRequired(username, connectionString);
            if (connection == null)
            {
                connection = CreateNewSqlCeConnection(connectionString, true);
            }
            return connection;
        }

        private static System.Type GetSqlCeType(string typeName)
        {
            System.Type type = System.Type.GetType("System.Data.SqlServerCe." + typeName + ", System.Data.SqlServerCe", false, true);
            if (type != null)
            {
                return type;
            }
            type = System.Type.GetType("System.Data.SqlServerCe." + typeName + ", System.Data.SqlServerCe, Version=3.5.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91", false, true);
            if (type != null)
            {
                return type;
            }
            type = System.Type.GetType("System.Data.SqlServerCe." + typeName + ", System.Data.SqlServerCe, Version=3.0.3600.0, Culture=neutral, PublicKeyToken=3be235df1c8d2ad3", false, true);
            if (type != null)
            {
                return type;
            }
            return System.Type.GetType("System.Data.SqlServerCe." + typeName + ", System.Data.SqlServerCe, Version=3.5.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91", true, true);
        }

        internal static int IsSpecialConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return 1;
            }
            if (string.Compare(connectionString, "|FILES|", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return 1;
            }
            return 3;
        }

        internal static string StoreCookieInDB(string cookieName, string cookieValue, string username, string connectionString, string sqlProvider)
        {
            string str2;
            if (connectionString == "|FILES|")
            {
                return ClientDataManager.StoreCookie(username, cookieName, cookieValue, false);
            }
            if (connectionString == "|Isolated_Storage|")
            {
                return ClientDataManager.StoreCookie(username, cookieName, cookieValue, true);
            }
            string str = Guid.NewGuid().ToString("N");
            using (DbConnection connection = GetConnection(username, connectionString, sqlProvider))
            {
                DbTransaction transaction = null;
                try
                {
                    transaction = connection.BeginTransaction();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "DELETE FROM UserProperties WHERE PropertyName LIKE N'CookieName_%' AND PropertyValue LIKE @PropValue";
                    cmd.Transaction = transaction;
                    AddParameter(connection, cmd, "@PropValue", cookieName + "=%");
                    cmd.ExecuteNonQuery();
                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        cmd = connection.CreateCommand();
                        cmd.Transaction = transaction;
                        cmd.CommandText = "INSERT INTO UserProperties (PropertyName, PropertyValue) VALUES (@PropName, @PropValue)";
                        AddParameter(connection, cmd, "@PropName", "CookieName_" + str);
                        AddParameter(connection, cmd, "@PropValue", cookieName + "=" + cookieValue);
                        cmd.ExecuteNonQuery();
                        return str;
                    }
                    str2 = cookieName;
                }
                catch
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                        transaction = null;
                    }
                    throw;
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
            }
            return str2;
        }
    }
}

