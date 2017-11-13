namespace System.Web.ClientServices.Providers
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Data.Common;
    using System.Globalization;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Web.ClientServices;
    using System.Web.Resources;
    using System.Web.Security;

    public class ClientFormsAuthenticationMembershipProvider : MembershipProvider
    {
        private string _ConnectionString;
        private string _ConnectionStringProvider;
        private Type _GetCredentialsType;
        private string _GetCredentialsTypeName;
        private bool _SavePasswordHash = true;
        private string _ServiceUri;
        private bool _UsingFileSystemStore;
        private bool _UsingIsolatedStore;
        private bool _UsingWFCService;

        public event EventHandler<UserValidatedEventArgs> UserValidated;

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotSupportedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotSupportedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotSupportedException();
        }

        private static string EncodePassword(string password, byte[] salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] array = new byte[salt.Length + bytes.Length];
            salt.CopyTo(array, 0);
            bytes.CopyTo(array, salt.Length);
            byte[] inArray = null;
            using (SHA1 sha = SHA1.Create())
            {
                inArray = sha.ComputeHash(array);
            }
            return Convert.ToBase64String(inArray);
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        private bool GetCredsFromUI(ref string username, ref string password, ref bool rememberMe)
        {
            if (this._GetCredentialsType == null)
            {
                if (string.IsNullOrEmpty(this._GetCredentialsTypeName))
                {
                    return false;
                }
                this._GetCredentialsType = Type.GetType(this._GetCredentialsTypeName, true, true);
            }
            ClientFormsAuthenticationCredentials credentials = ((IClientFormsAuthenticationCredentialsProvider) Activator.CreateInstance(this._GetCredentialsType)).GetCredentials();
            if (credentials == null)
            {
                return false;
            }
            username = credentials.UserName;
            password = credentials.Password;
            rememberMe = credentials.RememberMe;
            return true;
        }

        private string GetLastUserNameFromOffileStore()
        {
            string str;
            if (this._UsingFileSystemStore || this._UsingIsolatedStore)
            {
                return ClientDataManager.GetAppClientData(this._UsingIsolatedStore).LastLoggedInUserName;
            }
            using (DbConnection connection = SqlHelper.GetConnection(null, this._ConnectionString, this._ConnectionStringProvider))
            {
                DbTransaction transaction = null;
                try
                {
                    transaction = connection.BeginTransaction();
                    DbCommand command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = "SELECT PropertyValue FROM ApplicationProperties WHERE PropertyName = N'LastLoggedInUserName'";
                    str = command.ExecuteScalar()?.ToString();
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
            return str;
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        private string GetServiceUri()
        {
            if (string.IsNullOrEmpty(this._ServiceUri))
            {
                throw new ArgumentException(AtlasWeb.ServiceUriNotFound);
            }
            return this._ServiceUri;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotSupportedException();
        }

        public override string GetUserNameByEmail(string email)
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
            this._GetCredentialsTypeName = config["credentialsProvider"];
            this._ConnectionString = config["connectionStringName"];
            this.ServiceUri = config["serviceUri"];
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
            string str = config["savePasswordHashLocally"];
            if (!string.IsNullOrEmpty(str))
            {
                this._SavePasswordHash = string.Compare(str, "true", StringComparison.OrdinalIgnoreCase) == 0;
            }
            config.Remove("savePasswordHashLocally");
            config.Remove("name");
            config.Remove("description");
            config.Remove("credentialsProvider");
            config.Remove("connectionStringName");
            config.Remove("serviceUri");
            foreach (string str2 in config.Keys)
            {
                if (!string.IsNullOrEmpty(str2))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.AttributeNotRecognized, new object[] { str2 }));
                }
            }
        }

        public void Logout()
        {
            IPrincipal currentPrincipal = Thread.CurrentPrincipal;
            if ((currentPrincipal != null) && (currentPrincipal.Identity is ClientFormsIdentity))
            {
                lock (this)
                {
                    if (!ConnectivityStatus.IsOffline)
                    {
                        CookieContainer authenticationCookies = ((ClientFormsIdentity) currentPrincipal.Identity).AuthenticationCookies;
                        if (this._UsingWFCService)
                        {
                            throw new NotImplementedException();
                        }
                        ProxyHelper.CreateWebRequestAndGetResponse(this.GetServiceUri() + "/Logout", ref authenticationCookies, currentPrincipal.Identity.Name, this._ConnectionString, this._ConnectionStringProvider, null, null, null);
                    }
                    SqlHelper.DeleteAllCookies(currentPrincipal.Identity.Name, this._ConnectionString, this._ConnectionStringProvider);
                    Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                }
                this.StoreLastUserNameInOffileStore(null);
                if (this.UserValidated != null)
                {
                    this.UserValidated(this, new UserValidatedEventArgs(""));
                }
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        private void StoreHashedPasswordInDB(string username, string password)
        {
            if (this._SavePasswordHash)
            {
                byte[] buffer = new byte[0x10];
                new RNGCryptoServiceProvider().GetBytes(buffer);
                string paramValue = Convert.ToBase64String(buffer);
                string str2 = EncodePassword(password, buffer);
                if (this._UsingFileSystemStore || this._UsingIsolatedStore)
                {
                    ClientData userClientData = ClientDataManager.GetUserClientData(username, this._UsingIsolatedStore);
                    userClientData.PasswordHash = str2;
                    userClientData.PasswordSalt = paramValue;
                    userClientData.Save();
                }
                else
                {
                    using (DbConnection connection = SqlHelper.GetConnection(username, this._ConnectionString, this._ConnectionStringProvider))
                    {
                        DbTransaction transaction = null;
                        DbCommand cmd = null;
                        try
                        {
                            transaction = connection.BeginTransaction();
                            cmd = connection.CreateCommand();
                            cmd.CommandText = "DELETE FROM UserProperties WHERE PropertyName = @PasswordHashName";
                            SqlHelper.AddParameter(connection, cmd, "@PasswordHashName", "PasswordHash_" + username);
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                            cmd = connection.CreateCommand();
                            cmd.CommandText = "DELETE FROM UserProperties WHERE PropertyName = @PasswordSaltName";
                            SqlHelper.AddParameter(connection, cmd, "@PasswordSaltName", "PasswordSalt_" + username);
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                            cmd = connection.CreateCommand();
                            cmd.CommandText = "INSERT INTO UserProperties(PropertyName, PropertyValue) VALUES (@PasswordHashName, @PasswordHashValue)";
                            SqlHelper.AddParameter(connection, cmd, "@PasswordHashName", "PasswordHash_" + username);
                            SqlHelper.AddParameter(connection, cmd, "@PasswordHashValue", str2);
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                            cmd = connection.CreateCommand();
                            cmd.CommandText = "INSERT INTO UserProperties(PropertyName, PropertyValue) VALUES (@PasswordSaltName, @PasswordSaltValue)";
                            SqlHelper.AddParameter(connection, cmd, "@PasswordSaltName", "PasswordSalt_" + username);
                            SqlHelper.AddParameter(connection, cmd, "@PasswordSaltValue", paramValue);
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

        private void StoreLastUserNameInOffileStore(string username)
        {
            if (this._UsingFileSystemStore || this._UsingIsolatedStore)
            {
                ClientData appClientData = ClientDataManager.GetAppClientData(this._UsingIsolatedStore);
                appClientData.LastLoggedInUserName = username;
                appClientData.LastLoggedInDateUtc = DateTime.UtcNow;
                appClientData.Save();
            }
            else
            {
                using (DbConnection connection = SqlHelper.GetConnection(null, this._ConnectionString, this._ConnectionStringProvider))
                {
                    DbTransaction transaction = null;
                    try
                    {
                        transaction = connection.BeginTransaction();
                        DbCommand cmd = connection.CreateCommand();
                        cmd.Transaction = transaction;
                        cmd.CommandText = "DELETE FROM ApplicationProperties WHERE PropertyName = N'LastLoggedInUserName'";
                        cmd.ExecuteNonQuery();
                        if (!string.IsNullOrEmpty(username))
                        {
                            cmd = connection.CreateCommand();
                            cmd.Transaction = transaction;
                            cmd.CommandText = "INSERT INTO ApplicationProperties(PropertyName, PropertyValue) VALUES (N'LastLoggedInUserName', @UserName)";
                            SqlHelper.AddParameter(connection, cmd, "@UserName", username);
                            cmd.ExecuteNonQuery();
                            cmd = connection.CreateCommand();
                            cmd.Transaction = transaction;
                            cmd.CommandText = "INSERT INTO ApplicationProperties(PropertyName, PropertyValue) VALUES (N'LastLoggedInDate', @Date)";
                            SqlHelper.AddParameter(connection, cmd, "@Date", DateTime.Now.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture));
                            cmd.Transaction = transaction;
                            cmd.ExecuteNonQuery();
                        }
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

        public override bool UnlockUser(string username)
        {
            throw new NotSupportedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }

        private bool ValidateByCallingIsLoggedIn(string username, ref CookieContainer cookies)
        {
            if (this._UsingWFCService)
            {
                throw new NotImplementedException();
            }
            object obj2 = ProxyHelper.CreateWebRequestAndGetResponse(this.GetServiceUri() + "/IsLoggedIn", ref cookies, username, this._ConnectionString, this._ConnectionStringProvider, null, null, typeof(bool));
            return (((obj2 != null) && (obj2 is bool)) && ((bool) obj2));
        }

        public override bool ValidateUser(string username, string password) => 
            this.ValidateUserCore(username, password, 2);

        public bool ValidateUser(string username, string password, bool rememberMe) => 
            this.ValidateUserCore(username, password, rememberMe ? 1 : 0);

        public static bool ValidateUser(string username, string password, string serviceUri)
        {
            CookieContainer cookies = null;
            bool useWFCService = serviceUri.EndsWith(".svc", StringComparison.OrdinalIgnoreCase);
            bool flag2 = ValidateUserByCallingLogin(username, password, false, serviceUri, useWFCService, ref cookies, null, null);
            if (flag2)
            {
                Thread.CurrentPrincipal = new ClientRolePrincipal(new ClientFormsIdentity(username, password, new ClientFormsAuthenticationMembershipProvider(), "ClientForms", true, cookies));
            }
            return flag2;
        }

        private static bool ValidateUserByCallingLogin(string username, string password, bool rememberMe, string serviceUri, bool useWFCService, ref CookieContainer cookies, string connectionString, string connectionStringProvider)
        {
            if (useWFCService)
            {
                throw new NotImplementedException();
            }
            serviceUri = serviceUri + "/Login";
            string[] paramNames = new string[] { "userName", "password", "createPersistentCookie" };
            object[] paramValues = new object[] { username, password, rememberMe };
            object obj2 = ProxyHelper.CreateWebRequestAndGetResponse(serviceUri, ref cookies, username, connectionString, connectionStringProvider, paramNames, paramValues, typeof(bool));
            return (((obj2 != null) && (obj2 is bool)) && ((bool) obj2));
        }

        private bool ValidateUserCore(string username, string password, int rememberMeInt)
        {
            lock (this)
            {
                int promptCount = string.IsNullOrEmpty(username) ? 0 : 3;
                if (this.ValidateUserCore(username, password, rememberMeInt, ref promptCount, true))
                {
                    if (this.UserValidated != null)
                    {
                        this.UserValidated(this, new UserValidatedEventArgs(Thread.CurrentPrincipal.Identity.Name));
                    }
                    return true;
                }
                if (!string.IsNullOrEmpty(this._GetCredentialsTypeName))
                {
                    while (promptCount < 3)
                    {
                        if (this.ValidateUserCore(null, password, rememberMeInt, ref promptCount, false))
                        {
                            if (this.UserValidated != null)
                            {
                                this.UserValidated(this, new UserValidatedEventArgs(Thread.CurrentPrincipal.Identity.Name));
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool ValidateUserCore(string username, string password, int rememberMeInt, ref int promptCount, bool tryToUseLastLoggedInUser)
        {
            string strB = null;
            bool flag = false;
            string str2 = tryToUseLastLoggedInUser ? this.GetLastUserNameFromOffileStore() : null;
            bool flag2 = string.IsNullOrEmpty(username);
            CookieContainer cookies = null;
            bool flag3 = false;
            bool rememberMe = rememberMeInt == 1;
            bool flag5 = rememberMeInt != 2;
            if ((Thread.CurrentPrincipal != null) && (Thread.CurrentPrincipal.Identity is ClientFormsIdentity))
            {
                strB = Thread.CurrentPrincipal.Identity.Name;
            }
            if (string.IsNullOrEmpty(str2) && (strB != null))
            {
                str2 = strB;
            }
            if (flag2)
            {
                username = str2;
            }
            if (((Thread.CurrentPrincipal is ClientRolePrincipal) && (Thread.CurrentPrincipal.Identity is ClientFormsIdentity)) && (Thread.CurrentPrincipal.Identity.Name == username))
            {
                cookies = ((ClientFormsIdentity) Thread.CurrentPrincipal.Identity).AuthenticationCookies;
            }
            if (!string.IsNullOrEmpty(str2) && (string.Compare(str2, username, StringComparison.OrdinalIgnoreCase) == 0))
            {
                if (!ConnectivityStatus.IsOffline)
                {
                    flag = this.ValidateByCallingIsLoggedIn(str2, ref cookies);
                }
                else
                {
                    flag = ProxyHelper.DoAnyCookiesExist(this.GetServiceUri(), str2, this._ConnectionString, this._ConnectionStringProvider);
                }
                flag3 = true;
            }
            if (!flag)
            {
                if (flag2)
                {
                    promptCount++;
                    if (!this.GetCredsFromUI(ref username, ref password, ref rememberMe))
                    {
                        promptCount += 100;
                        return false;
                    }
                    flag5 = true;
                }
                if (!ConnectivityStatus.IsOffline)
                {
                    if (!ValidateUserByCallingLogin(username, password, rememberMe, this.GetServiceUri(), this._UsingWFCService, ref cookies, this._ConnectionString, this._ConnectionStringProvider))
                    {
                        return false;
                    }
                    this.StoreHashedPasswordInDB(username, password);
                }
                else if (!this.ValidateUserWithOfflineStore(username, password))
                {
                    return false;
                }
            }
            if (!flag3 || flag5)
            {
                this.StoreLastUserNameInOffileStore(rememberMe ? username : null);
            }
            if ((!(Thread.CurrentPrincipal is ClientRolePrincipal) || !(Thread.CurrentPrincipal.Identity is ClientFormsIdentity)) || (Thread.CurrentPrincipal.Identity.Name != username))
            {
                if (cookies == null)
                {
                    cookies = ProxyHelper.ConstructCookieContainer(this.GetServiceUri(), username, this._ConnectionString, this._ConnectionStringProvider);
                }
                Thread.CurrentPrincipal = new ClientRolePrincipal(new ClientFormsIdentity(username, password, this, "ClientForms", true, cookies));
            }
            if ((strB != null) && (string.Compare(username, strB, StringComparison.OrdinalIgnoreCase) != 0))
            {
                SqlHelper.DeleteAllCookies(strB, this._ConnectionString, this._ConnectionStringProvider);
            }
            return true;
        }

        private bool ValidateUserWithOfflineStore(string username, string password)
        {
            if (!this._SavePasswordHash)
            {
                return false;
            }
            string passwordHash = null;
            string passwordSalt = null;
            if (this._UsingFileSystemStore || this._UsingIsolatedStore)
            {
                ClientData userClientData = ClientDataManager.GetUserClientData(username, this._UsingIsolatedStore);
                passwordHash = userClientData.PasswordHash;
                passwordSalt = userClientData.PasswordSalt;
            }
            else
            {
                DbTransaction transaction = null;
                using (DbConnection connection = SqlHelper.GetConnection(username, this._ConnectionString, this._ConnectionStringProvider))
                {
                    try
                    {
                        DbCommand cmd = connection.CreateCommand();
                        cmd.Transaction = transaction;
                        cmd.CommandText = "SELECT PropertyValue FROM UserProperties WHERE PropertyName = @PasswordHashName";
                        SqlHelper.AddParameter(connection, cmd, "@PasswordHashName", "PasswordHash_" + username);
                        passwordHash = cmd.ExecuteScalar() as string;
                        cmd = connection.CreateCommand();
                        cmd.Transaction = transaction;
                        cmd.CommandText = "SELECT PropertyValue FROM UserProperties WHERE PropertyName = @PasswordSaltName";
                        SqlHelper.AddParameter(connection, cmd, "@PasswordSaltName", "PasswordSalt_" + username);
                        passwordSalt = cmd.ExecuteScalar() as string;
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
            if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(passwordSalt))
            {
                return false;
            }
            byte[] salt = Convert.FromBase64String(passwordSalt);
            return (passwordHash == EncodePassword(password, salt));
        }

        public override string ApplicationName
        {
            get => 
                "";
            set
            {
            }
        }

        public override bool EnablePasswordReset =>
            false;

        public override bool EnablePasswordRetrieval =>
            false;

        public override int MaxInvalidPasswordAttempts =>
            0x7fffffff;

        public override int MinRequiredNonAlphanumericCharacters =>
            0;

        public override int MinRequiredPasswordLength =>
            1;

        public override int PasswordAttemptWindow =>
            0x7fffffff;

        public override MembershipPasswordFormat PasswordFormat =>
            MembershipPasswordFormat.Hashed;

        public override string PasswordStrengthRegularExpression =>
            "*";

        public override bool RequiresQuestionAndAnswer =>
            false;

        public override bool RequiresUniqueEmail =>
            false;

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

