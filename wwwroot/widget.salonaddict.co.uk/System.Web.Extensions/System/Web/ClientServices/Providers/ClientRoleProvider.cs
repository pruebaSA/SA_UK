namespace System.Web.ClientServices.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Data.Common;
    using System.Globalization;
    using System.Net;
    using System.Security.Principal;
    using System.Threading;
    using System.Web.ClientServices;
    using System.Web.Resources;
    using System.Web.Security;

    public class ClientRoleProvider : RoleProvider
    {
        private DateTime _CacheExpiryDate = DateTime.UtcNow;
        private int _CacheTimeout = 0x5a0;
        private string _ConnectionString;
        private string _ConnectionStringProvider;
        private string _CurrentUser;
        private bool _HonorCookieExpiry;
        private string[] _Roles;
        private string _ServiceUri;
        private bool _UsingFileSystemStore;
        private bool _UsingIsolatedStore;
        private bool _UsingWFCService;

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotSupportedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotSupportedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotSupportedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotSupportedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            lock (this)
            {
                IPrincipal currentPrincipal = Thread.CurrentPrincipal;
                if (((currentPrincipal == null) || (currentPrincipal.Identity == null)) || !currentPrincipal.Identity.IsAuthenticated)
                {
                    return new string[0];
                }
                if (!string.IsNullOrEmpty(username) && (string.Compare(username, currentPrincipal.Identity.Name, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    throw new ArgumentException(AtlasWeb.ArgumentMustBeCurrentUser, "username");
                }
                if ((string.Compare(this._CurrentUser, currentPrincipal.Identity.Name, StringComparison.OrdinalIgnoreCase) != 0) || (DateTime.UtcNow >= this._CacheExpiryDate))
                {
                    if (this.GetRolesFromDBForUser(currentPrincipal.Identity.Name))
                    {
                        return this._Roles;
                    }
                    if (ConnectivityStatus.IsOffline)
                    {
                        return new string[0];
                    }
                    this._Roles = null;
                    this._CacheExpiryDate = DateTime.UtcNow;
                    this._CurrentUser = currentPrincipal.Identity.Name;
                    this.GetRolesForUserCore(currentPrincipal.Identity);
                    if ((!this._HonorCookieExpiry && (this._Roles.Length < 1)) && (currentPrincipal.Identity is ClientFormsIdentity))
                    {
                        ((ClientFormsIdentity) currentPrincipal.Identity).RevalidateUser();
                        this.GetRolesForUserCore(currentPrincipal.Identity);
                    }
                    this.StoreRolesForCurrentUser();
                }
                return this._Roles;
            }
        }

        private void GetRolesForUserCore(IIdentity identity)
        {
            CookieContainer cookies = null;
            if (identity is ClientFormsIdentity)
            {
                cookies = ((ClientFormsIdentity) identity).AuthenticationCookies;
            }
            if (this._UsingWFCService)
            {
                throw new NotImplementedException();
            }
            object obj2 = ProxyHelper.CreateWebRequestAndGetResponse(this.GetServiceUri() + "/GetRolesForCurrentUser", ref cookies, identity.Name, this._ConnectionString, this._ConnectionStringProvider, null, null, typeof(string[]));
            if (obj2 != null)
            {
                this._Roles = (string[]) obj2;
            }
            else
            {
                this._Roles = new string[0];
            }
            this._CacheExpiryDate = DateTime.UtcNow.AddMinutes((double) this._CacheTimeout);
        }

        private bool GetRolesFromDBForUser(string username)
        {
            bool flag;
            this._Roles = null;
            this._CacheExpiryDate = DateTime.UtcNow;
            this._CurrentUser = username;
            if (this._UsingFileSystemStore || this._UsingIsolatedStore)
            {
                ClientData userClientData = ClientDataManager.GetUserClientData(username, this._UsingIsolatedStore);
                if (userClientData.Roles == null)
                {
                    return false;
                }
                this._Roles = userClientData.Roles;
                this._CacheExpiryDate = userClientData.RolesCachedDateUtc.AddMinutes((double) this._CacheTimeout);
                if (!ConnectivityStatus.IsOffline && (this._CacheExpiryDate < DateTime.UtcNow))
                {
                    return false;
                }
                return true;
            }
            using (DbConnection connection = SqlHelper.GetConnection(this._CurrentUser, this._ConnectionString, this._ConnectionStringProvider))
            {
                DbTransaction transaction = null;
                try
                {
                    transaction = connection.BeginTransaction();
                    DbCommand cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = "SELECT PropertyValue FROM UserProperties WHERE PropertyName = @RolesCachedDate";
                    SqlHelper.AddParameter(connection, cmd, "@RolesCachedDate", "RolesCachedDate_" + this._CurrentUser);
                    string s = cmd.ExecuteScalar() as string;
                    if (s == null)
                    {
                        return false;
                    }
                    long fileTime = long.Parse(s, CultureInfo.InvariantCulture);
                    this._CacheExpiryDate = DateTime.FromFileTimeUtc(fileTime).AddMinutes((double) this._CacheTimeout);
                    if (!ConnectivityStatus.IsOffline && (this._CacheExpiryDate < DateTime.UtcNow))
                    {
                        return false;
                    }
                    cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.CommandText = "SELECT RoleName FROM Roles WHERE UserName = @UserName ORDER BY RoleName";
                    SqlHelper.AddParameter(connection, cmd, "@UserName", this._CurrentUser);
                    ArrayList list = new ArrayList();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader.GetString(0));
                        }
                    }
                    this._Roles = new string[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        this._Roles[i] = (string) list[i];
                    }
                    flag = true;
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
            return flag;
        }

        private string GetServiceUri()
        {
            if (string.IsNullOrEmpty(this._ServiceUri))
            {
                throw new ArgumentException(AtlasWeb.ServiceUriNotFound);
            }
            return this._ServiceUri;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotSupportedException();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            base.Initialize(name, config);
            this.ServiceUri = config["serviceUri"];
            string str = config["cacheTimeout"];
            if (!string.IsNullOrEmpty(str))
            {
                this._CacheTimeout = int.Parse(str, CultureInfo.InvariantCulture);
            }
            this._ConnectionString = config["connectionStringName"];
            if (string.IsNullOrEmpty(this._ConnectionString))
            {
                this._ConnectionString = SqlHelper.GetDefaultConnectionString();
            }
            else if (ConfigurationManager.ConnectionStrings[this._ConnectionString] != null)
            {
                this._ConnectionStringProvider = ConfigurationManager.ConnectionStrings[this._ConnectionString].ProviderName;
                this._ConnectionString = ConfigurationManager.ConnectionStrings[this._ConnectionString].ConnectionString;
            }
            switch (SqlHelper.IsSpecialConnectionString(this._ConnectionString))
            {
                case 1:
                    this._UsingFileSystemStore = true;
                    break;

                case 2:
                    this._UsingIsolatedStore = true;
                    break;
            }
            str = config["honorCookieExpiry"];
            if (!string.IsNullOrEmpty(str))
            {
                this._HonorCookieExpiry = string.Compare(str, "true", StringComparison.OrdinalIgnoreCase) == 0;
            }
            config.Remove("name");
            config.Remove("description");
            config.Remove("cacheTimeout");
            config.Remove("connectionStringName");
            config.Remove("serviceUri");
            config.Remove("honorCookieExpiry");
            foreach (string str2 in config.Keys)
            {
                if (!string.IsNullOrEmpty(str2))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.AttributeNotRecognized, new object[] { str2 }));
                }
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            foreach (string str in this.GetRolesForUser(username))
            {
                if (string.Compare(str, roleName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void RemoveRolesFromDB()
        {
            if (!string.IsNullOrEmpty(this._CurrentUser))
            {
                if (this._UsingFileSystemStore || this._UsingIsolatedStore)
                {
                    ClientData userClientData = ClientDataManager.GetUserClientData(this._CurrentUser, this._UsingIsolatedStore);
                    userClientData.Roles = null;
                    userClientData.Save();
                }
                else
                {
                    using (DbConnection connection = SqlHelper.GetConnection(this._CurrentUser, this._ConnectionString, this._ConnectionStringProvider))
                    {
                        DbTransaction transaction = null;
                        try
                        {
                            transaction = connection.BeginTransaction();
                            DbCommand cmd = connection.CreateCommand();
                            cmd.CommandText = "DELETE FROM Roles WHERE UserName = @UserName";
                            SqlHelper.AddParameter(connection, cmd, "@UserName", this._CurrentUser);
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                            cmd = connection.CreateCommand();
                            cmd.CommandText = "DELETE FROM UserProperties WHERE PropertyName = @RolesCachedDate";
                            SqlHelper.AddParameter(connection, cmd, "@RolesCachedDate", "RolesCachedDate_" + this._CurrentUser);
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
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
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotSupportedException();
        }

        public void ResetCache()
        {
            lock (this)
            {
                this._Roles = null;
                this._CacheExpiryDate = DateTime.UtcNow;
                this.RemoveRolesFromDB();
            }
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotSupportedException();
        }

        private void StoreRolesForCurrentUser()
        {
            if (this._UsingFileSystemStore || this._UsingIsolatedStore)
            {
                ClientData userClientData = ClientDataManager.GetUserClientData(this._CurrentUser, this._UsingIsolatedStore);
                userClientData.Roles = this._Roles;
                userClientData.RolesCachedDateUtc = DateTime.UtcNow;
                userClientData.Save();
            }
            else
            {
                this.RemoveRolesFromDB();
                DbTransaction transaction = null;
                using (DbConnection connection = SqlHelper.GetConnection(this._CurrentUser, this._ConnectionString, this._ConnectionStringProvider))
                {
                    try
                    {
                        transaction = connection.BeginTransaction();
                        DbCommand cmd = null;
                        foreach (string str in this._Roles)
                        {
                            cmd = connection.CreateCommand();
                            cmd.CommandText = "INSERT INTO Roles(UserName, RoleName) VALUES(@UserName, @RoleName)";
                            SqlHelper.AddParameter(connection, cmd, "@UserName", this._CurrentUser);
                            SqlHelper.AddParameter(connection, cmd, "@RoleName", str);
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
                        cmd = connection.CreateCommand();
                        cmd.CommandText = "INSERT INTO UserProperties (PropertyName, PropertyValue) VALUES(@RolesCachedDate, @Date)";
                        SqlHelper.AddParameter(connection, cmd, "@RolesCachedDate", "RolesCachedDate_" + this._CurrentUser);
                        SqlHelper.AddParameter(connection, cmd, "@Date", DateTime.UtcNow.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));
                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
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

        public override string ApplicationName
        {
            get => 
                "";
            set
            {
            }
        }

        public string ServiceUri
        {
            get => 
                this._ServiceUri;
            set
            {
                this._ServiceUri = value;
                if (string.IsNullOrEmpty(this._ServiceUri))
                {
                    this._UsingWFCService = false;
                }
                else
                {
                    this._UsingWFCService = this._ServiceUri.EndsWith(".svc", StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }
}

