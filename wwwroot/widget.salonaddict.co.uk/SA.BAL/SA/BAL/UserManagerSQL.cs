namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;

    public class UserManagerSQL : IUserManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public UserManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public void DeleteSalonUser(SalonUserDB user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.UserId == Guid.Empty)
            {
                throw new ArgumentException("User identifier cannot be empty.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@UserId", user.UserId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonUserDeleteById", commandParameters);
            this._cacheManager.Remove($"SALON-USER-{user.Username}");
        }

        public void DeleteWidgetApiKey(WidgetApiKeyDB apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException("apiKey");
            }
            if (apiKey.KeyId == Guid.Empty)
            {
                throw new ArgumentException("Api key identifier cannot be empty.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@KeyId", apiKey.KeyId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_WidgetApiKeyDeleteById", commandParameters);
            this._cacheManager.Remove($"APIKEY-{apiKey}");
        }

        public SalonUserDB GetSalonUserById(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User identifier cannot be empty.");
            }
            SalonUserDB rdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@UserId", userId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonUserLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    rdb = this.SalonUserMappingDB(reader);
                }
            }
            return rdb;
        }

        public SalonUserDB GetSalonUserByUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }
            string key = $"SALON-USER-{username}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (SalonUserDB) obj2;
            }
            SalonUserDB rdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@Username", username) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonUserLoadByUsername", parameterValues))
            {
                if (reader.Read())
                {
                    rdb = this.SalonUserMappingDB(reader);
                }
            }
            this._cacheManager.Add(key, rdb);
            return rdb;
        }

        public List<SalonUserDB> GetSalonUsersBySalonId(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            List<SalonUserDB> list = new List<SalonUserDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_SalonUserLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    SalonUserDB item = this.SalonUserMappingDB(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public WidgetApiKeyDB GetWidgetApiKeyById(Guid keyId)
        {
            if (keyId == Guid.Empty)
            {
                throw new ArgumentException("Api key identifier cannot be empty.");
            }
            WidgetApiKeyDB ydb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@KeyId", keyId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_WidgetApiKeyLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    ydb = this.WApiKeyMappingDB(reader);
                }
            }
            return ydb;
        }

        public List<WidgetApiKeyDB> GetWidgetApiKeyBySalonId(Guid salonId)
        {
            if (salonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            List<WidgetApiKeyDB> list = new List<WidgetApiKeyDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@SalonId", salonId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_WidgetApiKeyLoadBySalonId", parameterValues))
            {
                while (reader.Read())
                {
                    WidgetApiKeyDB item = this.WApiKeyMappingDB(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public WidgetApiKeyDB GetWidgetApiKeyByVerificationToken(string verificationToken)
        {
            if (verificationToken == null)
            {
                throw new ArgumentException("Verification token cannot be empty.");
            }
            string key = $"APIKEY-{verificationToken}";
            object obj2 = this._cacheManager.Get(key);
            if (obj2 != null)
            {
                return (WidgetApiKeyDB) obj2;
            }
            WidgetApiKeyDB ydb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@VerificationToken", verificationToken) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_WidgetApiKeyLoadByVerificationToken", parameterValues))
            {
                if (reader.Read())
                {
                    ydb = this.WApiKeyMappingDB(reader);
                }
            }
            this._cacheManager.Add(key, ydb);
            return ydb;
        }

        public SalonUserDB InsertSalonUser(SalonUserDB user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.Username == null)
            {
                throw new ArgumentException("Username cannot be null");
            }
            if (user.Password == null)
            {
                throw new ArgumentException("Password cannot be null");
            }
            if (user.PasswordFormat == null)
            {
                throw new ArgumentException("Password format cannot be null");
            }
            user.AdminComment = user.AdminComment ?? string.Empty;
            user.ConfirmationToken = user.ConfirmationToken ?? string.Empty;
            user.Currency = user.Currency ?? string.Empty;
            user.DisplayText = user.DisplayText ?? string.Empty;
            user.Email = user.Email ?? string.Empty;
            user.FirstName = user.FirstName ?? string.Empty;
            user.Language = user.Language ?? string.Empty;
            user.LastName = user.LastName ?? string.Empty;
            user.LastIPAddress = user.LastIPAddress ?? string.Empty;
            user.Mobile = user.Mobile ?? string.Empty;
            user.MobilePIN = user.MobilePIN ?? string.Empty;
            user.PasswordSalt = user.PasswordSalt ?? string.Empty;
            user.PasswordVerificationToken = user.PasswordVerificationToken ?? string.Empty;
            user.PhoneNumber = user.PhoneNumber ?? string.Empty;
            user.TimeZone = user.TimeZone ?? string.Empty;
            user.VatNumber = user.VatNumber ?? string.Empty;
            user.VatNumberStatus = user.VatNumberStatus ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] parameterArray2 = new SqlParameter[40];
            parameterArray2[0] = parameter;
            parameterArray2[1] = new SqlParameter("@Active", user.Active);
            parameterArray2[2] = new SqlParameter("@AdminComment", user.AdminComment);
            parameterArray2[3] = new SqlParameter("@ConfirmationToken", user.ConfirmationToken);
            DateTime? confirmationTokenExpirationUtc = user.ConfirmationTokenExpirationUtc;
            parameterArray2[4] = new SqlParameter("@ConfirmationTokenExpirationUtc", confirmationTokenExpirationUtc.HasValue ? ((SqlDateTime) confirmationTokenExpirationUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[5] = new SqlParameter("@CreatedOnUtc", user.CreatedOnUtc);
            parameterArray2[6] = new SqlParameter("@Currency", user.Currency);
            parameterArray2[7] = new SqlParameter("@Deleted", user.Deleted);
            parameterArray2[8] = new SqlParameter("@DisplayText", user.DisplayText);
            parameterArray2[9] = new SqlParameter("@Email", user.Email);
            parameterArray2[10] = new SqlParameter("@FailedPasswordAttemptCount", user.FailedPasswordAttemptCount);
            DateTime? failedPasswordWindowStartDateUtc = user.FailedPasswordWindowStartDateUtc;
            parameterArray2[11] = new SqlParameter("@FailedPasswordWindowStartDateUtc", failedPasswordWindowStartDateUtc.HasValue ? ((SqlDateTime) failedPasswordWindowStartDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[12] = new SqlParameter("@FirstName", user.FirstName);
            parameterArray2[13] = new SqlParameter("@IsAdmin", user.IsAdmin);
            parameterArray2[14] = new SqlParameter("@IsApproved", user.IsApproved);
            parameterArray2[15] = new SqlParameter("@IsConfirmed", user.IsConfirmed);
            parameterArray2[0x10] = new SqlParameter("@IsGuest", user.IsGuest);
            parameterArray2[0x11] = new SqlParameter("@IsLockedOut", user.IsLockedOut);
            parameterArray2[0x12] = new SqlParameter("@IsTaxExempt", user.IsTaxExempt);
            parameterArray2[0x13] = new SqlParameter("@Language", user.Language);
            DateTime? lastActivityDateUtc = user.LastActivityDateUtc;
            parameterArray2[20] = new SqlParameter("@LastActivityDateUtc", lastActivityDateUtc.HasValue ? ((SqlDateTime) lastActivityDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x15] = new SqlParameter("@LastIPAddress", user.LastIPAddress);
            DateTime? lastLockoutDateUtc = user.LastLockoutDateUtc;
            parameterArray2[0x16] = new SqlParameter("@LastLockoutDateUtc", lastLockoutDateUtc.HasValue ? ((SqlDateTime) lastLockoutDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            DateTime? lastLoginDateUtc = user.LastLoginDateUtc;
            parameterArray2[0x17] = new SqlParameter("@LastLoginDateUtc", lastLoginDateUtc.HasValue ? ((SqlDateTime) lastLoginDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x18] = new SqlParameter("@LastName", user.LastName);
            DateTime? lastPasswordChangeDateUtc = user.LastPasswordChangeDateUtc;
            parameterArray2[0x19] = new SqlParameter("@LastPasswordChangeDateUtc", lastPasswordChangeDateUtc.HasValue ? ((SqlDateTime) lastPasswordChangeDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            DateTime? lastPasswordFailureDateUtc = user.LastPasswordFailureDateUtc;
            parameterArray2[0x1a] = new SqlParameter("@LastPasswordFailureDateUtc", lastPasswordFailureDateUtc.HasValue ? ((SqlDateTime) lastPasswordFailureDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x1b] = new SqlParameter("@Mobile", user.Mobile);
            parameterArray2[0x1c] = new SqlParameter("@MobilePIN", user.MobilePIN);
            parameterArray2[0x1d] = new SqlParameter("@Password", user.Password);
            parameterArray2[30] = new SqlParameter("@PasswordFormat", user.PasswordFormat);
            parameterArray2[0x1f] = new SqlParameter("@PasswordSalt", user.PasswordSalt);
            parameterArray2[0x20] = new SqlParameter("@PasswordVerificationToken", user.PasswordVerificationToken);
            DateTime? passwordVerificationTokenExpirationUtc = user.PasswordVerificationTokenExpirationUtc;
            parameterArray2[0x21] = new SqlParameter("@PasswordVerificationTokenExpirationUtc", passwordVerificationTokenExpirationUtc.HasValue ? ((SqlDateTime) passwordVerificationTokenExpirationUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x22] = new SqlParameter("@PhoneNumber", user.PhoneNumber);
            parameterArray2[0x23] = new SqlParameter("@SalonId", user.SalonId);
            parameterArray2[0x24] = new SqlParameter("@TimeZone", user.TimeZone);
            parameterArray2[0x25] = new SqlParameter("@Username", user.Username);
            parameterArray2[0x26] = new SqlParameter("@VatNumber", user.VatNumber);
            parameterArray2[0x27] = new SqlParameter("@VatNumberStatus", user.VatNumberStatus);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonUserInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid userId = new Guid(parameter.Value.ToString());
            user = this.GetSalonUserById(userId);
            return user;
        }

        public WidgetApiKeyDB InsertWidgetApiKey(WidgetApiKeyDB apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException("apiKey");
            }
            if (apiKey.SalonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            if (apiKey.VerificationToken == null)
            {
                throw new ArgumentNullException("Verification token cannot be null.");
            }
            SqlParameter parameter = new SqlParameter("@KeyId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@Active", apiKey.Active), new SqlParameter("@CreatedOnUtc", apiKey.CreatedOnUtc), new SqlParameter("@Deleted", apiKey.Deleted), new SqlParameter("@HttpReferer", apiKey.HttpReferer), new SqlParameter("@SalonId", apiKey.SalonId), new SqlParameter("@VerificationToken", apiKey.VerificationToken) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_WidgetApiKeyInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid keyId = new Guid(parameter.Value.ToString());
            apiKey = this.GetWidgetApiKeyById(keyId);
            return apiKey;
        }

        private SalonUserDB SalonUserMappingDB(SqlDataReader reader) => 
            new SalonUserDB { 
                AdminComment = reader.GetString(reader.GetOrdinal("AdminComment")),
                ConfirmationToken = reader.GetString(reader.GetOrdinal("ConfirmationToken")),
                ConfirmationTokenExpirationUtc = reader.GetValue(reader.GetOrdinal("ConfirmationTokenExpirationUtc")) as DateTime?,
                CreatedOnUtc = reader.GetDateTime(reader.GetOrdinal("CreatedOnUtc")),
                Currency = reader.GetString(reader.GetOrdinal("Currency")),
                DisplayText = reader.GetString(reader.GetOrdinal("DisplayText")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                FailedPasswordAttemptCount = reader.GetInt32(reader.GetOrdinal("FailedPasswordAttemptCount")),
                FailedPasswordWindowStartDateUtc = reader.GetValue(reader.GetOrdinal("FailedPasswordWindowStartDateUtc")) as DateTime?,
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin")),
                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                IsConfirmed = reader.GetBoolean(reader.GetOrdinal("IsConfirmed")),
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                IsGuest = reader.GetBoolean(reader.GetOrdinal("IsGuest")),
                IsLockedOut = reader.GetBoolean(reader.GetOrdinal("IsLockedOut")),
                IsTaxExempt = reader.GetBoolean(reader.GetOrdinal("IsTaxExempt")),
                Language = reader.GetString(reader.GetOrdinal("Language")),
                LastActivityDateUtc = reader.GetValue(reader.GetOrdinal("LastActivityDateUtc")) as DateTime?,
                LastIPAddress = reader.GetString(reader.GetOrdinal("LastIPAddress")),
                LastLockoutDateUtc = reader.GetValue(reader.GetOrdinal("LastLockoutDateUtc")) as DateTime?,
                LastLoginDateUtc = reader.GetValue(reader.GetOrdinal("LastLoginDateUtc")) as DateTime?,
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                LastPasswordChangeDateUtc = reader.GetValue(reader.GetOrdinal("LastPasswordChangeDateUtc")) as DateTime?,
                LastPasswordFailureDateUtc = reader.GetValue(reader.GetOrdinal("LastPasswordFailureDateUtc")) as DateTime?,
                Mobile = reader.GetString(reader.GetOrdinal("Mobile")),
                MobilePIN = reader.GetString(reader.GetOrdinal("MobilePIN")),
                Password = reader.GetString(reader.GetOrdinal("Password")),
                PasswordFormat = reader.GetString(reader.GetOrdinal("PasswordFormat")),
                PasswordSalt = reader.GetString(reader.GetOrdinal("PasswordSalt")),
                PasswordVerificationToken = reader.GetString(reader.GetOrdinal("PasswordVerificationToken")),
                PasswordVerificationTokenExpirationUtc = reader.GetValue(reader.GetOrdinal("PasswordVerificationTokenExpirationUtc")) as DateTime?,
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                TimeZone = reader.GetString(reader.GetOrdinal("TimeZone")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                VatNumber = reader.GetString(reader.GetOrdinal("VatNumber")),
                VatNumberStatus = reader.GetString(reader.GetOrdinal("VatNumberStatus"))
            };

        public SalonUserDB UpdateSalonUser(SalonUserDB user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.UserId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException("User identifier cannot be empty.");
            }
            if (user.Username == null)
            {
                throw new ArgumentException("Username cannot be null");
            }
            if (user.Password == null)
            {
                throw new ArgumentException("Password cannot be null");
            }
            if (user.PasswordFormat == null)
            {
                throw new ArgumentException("Password format cannot be null");
            }
            user.AdminComment = user.AdminComment ?? string.Empty;
            user.ConfirmationToken = user.ConfirmationToken ?? string.Empty;
            user.Currency = user.Currency ?? string.Empty;
            user.DisplayText = user.DisplayText ?? string.Empty;
            user.Email = user.Email ?? string.Empty;
            user.FirstName = user.FirstName ?? string.Empty;
            user.Language = user.Language ?? string.Empty;
            user.LastName = user.LastName ?? string.Empty;
            user.LastIPAddress = user.LastIPAddress ?? string.Empty;
            user.Mobile = user.Mobile ?? string.Empty;
            user.PasswordSalt = user.PasswordSalt ?? string.Empty;
            user.PasswordVerificationToken = user.PasswordVerificationToken ?? string.Empty;
            user.PhoneNumber = user.PhoneNumber ?? string.Empty;
            user.TimeZone = user.TimeZone ?? string.Empty;
            user.VatNumber = user.VatNumber ?? string.Empty;
            user.VatNumberStatus = user.VatNumberStatus ?? string.Empty;
            SqlParameter[] parameterArray2 = new SqlParameter[40];
            parameterArray2[0] = new SqlParameter("@UserId", user.UserId);
            parameterArray2[1] = new SqlParameter("@Active", user.Active);
            parameterArray2[2] = new SqlParameter("@AdminComment", user.AdminComment);
            parameterArray2[3] = new SqlParameter("@ConfirmationToken", user.ConfirmationToken);
            DateTime? confirmationTokenExpirationUtc = user.ConfirmationTokenExpirationUtc;
            parameterArray2[4] = new SqlParameter("@ConfirmationTokenExpirationUtc", confirmationTokenExpirationUtc.HasValue ? ((SqlDateTime) confirmationTokenExpirationUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[5] = new SqlParameter("@CreatedOnUtc", user.CreatedOnUtc);
            parameterArray2[6] = new SqlParameter("@Currency", user.Currency);
            parameterArray2[7] = new SqlParameter("@Deleted", user.Deleted);
            parameterArray2[8] = new SqlParameter("@DisplayText", user.DisplayText);
            parameterArray2[9] = new SqlParameter("@Email", user.Email);
            parameterArray2[10] = new SqlParameter("@FailedPasswordAttemptCount", user.FailedPasswordAttemptCount);
            DateTime? failedPasswordWindowStartDateUtc = user.FailedPasswordWindowStartDateUtc;
            parameterArray2[11] = new SqlParameter("@FailedPasswordWindowStartDateUtc", failedPasswordWindowStartDateUtc.HasValue ? ((SqlDateTime) failedPasswordWindowStartDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[12] = new SqlParameter("@FirstName", user.FirstName);
            parameterArray2[13] = new SqlParameter("@IsAdmin", user.IsAdmin);
            parameterArray2[14] = new SqlParameter("@IsApproved", user.IsApproved);
            parameterArray2[15] = new SqlParameter("@IsConfirmed", user.IsConfirmed);
            parameterArray2[0x10] = new SqlParameter("@IsGuest", user.IsGuest);
            parameterArray2[0x11] = new SqlParameter("@IsLockedOut", user.IsLockedOut);
            parameterArray2[0x12] = new SqlParameter("@IsTaxExempt", user.IsTaxExempt);
            parameterArray2[0x13] = new SqlParameter("@Language", user.Language);
            DateTime? lastActivityDateUtc = user.LastActivityDateUtc;
            parameterArray2[20] = new SqlParameter("@LastActivityDateUtc", lastActivityDateUtc.HasValue ? ((SqlDateTime) lastActivityDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x15] = new SqlParameter("@LastIPAddress", user.LastIPAddress);
            DateTime? lastLockoutDateUtc = user.LastLockoutDateUtc;
            parameterArray2[0x16] = new SqlParameter("@LastLockoutDateUtc", lastLockoutDateUtc.HasValue ? ((SqlDateTime) lastLockoutDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            DateTime? lastLoginDateUtc = user.LastLoginDateUtc;
            parameterArray2[0x17] = new SqlParameter("@LastLoginDateUtc", lastLoginDateUtc.HasValue ? ((SqlDateTime) lastLoginDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x18] = new SqlParameter("@LastName", user.LastName);
            DateTime? lastPasswordChangeDateUtc = user.LastPasswordChangeDateUtc;
            parameterArray2[0x19] = new SqlParameter("@LastPasswordChangeDateUtc", lastPasswordChangeDateUtc.HasValue ? ((SqlDateTime) lastPasswordChangeDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            DateTime? lastPasswordFailureDateUtc = user.LastPasswordFailureDateUtc;
            parameterArray2[0x1a] = new SqlParameter("@LastPasswordFailureDateUtc", lastPasswordFailureDateUtc.HasValue ? ((SqlDateTime) lastPasswordFailureDateUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x1b] = new SqlParameter("@Mobile", user.Mobile);
            parameterArray2[0x1c] = new SqlParameter("@MobilePIN", user.MobilePIN);
            parameterArray2[0x1d] = new SqlParameter("@Password", user.Password);
            parameterArray2[30] = new SqlParameter("@PasswordFormat", user.PasswordFormat);
            parameterArray2[0x1f] = new SqlParameter("@PasswordSalt", user.PasswordSalt);
            parameterArray2[0x20] = new SqlParameter("@PasswordVerificationToken", user.PasswordVerificationToken);
            DateTime? passwordVerificationTokenExpirationUtc = user.PasswordVerificationTokenExpirationUtc;
            parameterArray2[0x21] = new SqlParameter("@PasswordVerificationTokenExpirationUtc", passwordVerificationTokenExpirationUtc.HasValue ? ((SqlDateTime) passwordVerificationTokenExpirationUtc.GetValueOrDefault()) : SqlDateTime.Null);
            parameterArray2[0x22] = new SqlParameter("@PhoneNumber", user.PhoneNumber);
            parameterArray2[0x23] = new SqlParameter("@SalonId", user.SalonId);
            parameterArray2[0x24] = new SqlParameter("@TimeZone", user.TimeZone);
            parameterArray2[0x25] = new SqlParameter("@Username", user.Username);
            parameterArray2[0x26] = new SqlParameter("@VatNumber", user.VatNumber);
            parameterArray2[0x27] = new SqlParameter("@VatNumberStatus", user.VatNumberStatus);
            SqlParameter[] commandParameters = parameterArray2;
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_SalonUserUpdate", commandParameters) <= 0)
            {
                return null;
            }
            user = this.GetSalonUserById(user.UserId);
            return user;
        }

        public WidgetApiKeyDB UpdateWidgetApiKey(WidgetApiKeyDB apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException("apiKey");
            }
            if (apiKey.KeyId == Guid.Empty)
            {
                throw new ArgumentException("Api key identifier cannot be empty.");
            }
            if (apiKey.SalonId == Guid.Empty)
            {
                throw new ArgumentException("Salon identifier cannot be empty.");
            }
            if (apiKey.VerificationToken == null)
            {
                throw new ArgumentNullException("Verification token cannot be null.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@KeyId", apiKey.KeyId), new SqlParameter("@Active", apiKey.Active), new SqlParameter("@CreatedOnUtc", apiKey.CreatedOnUtc), new SqlParameter("@Deleted", apiKey.Deleted), new SqlParameter("@HttpReferer", apiKey.HttpReferer), new SqlParameter("@SalonId", apiKey.SalonId), new SqlParameter("@VerificationToken", apiKey.VerificationToken) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_WidgetApiKeyUpdate", commandParameters) <= 0)
            {
                return null;
            }
            string key = $"APIKEY-{apiKey.VerificationToken}";
            this._cacheManager.Remove(key);
            apiKey = this.GetWidgetApiKeyById(apiKey.KeyId);
            return apiKey;
        }

        private WidgetApiKeyDB WApiKeyMappingDB(SqlDataReader reader) => 
            new WidgetApiKeyDB { 
                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                KeyId = reader.GetGuid(reader.GetOrdinal("KeyId")),
                CreatedOnUtc = reader.GetDateTime(reader.GetOrdinal("CreatedOnUtc")),
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                HttpReferer = reader.GetString(reader.GetOrdinal("HttpReferer")),
                SalonId = reader.GetGuid(reader.GetOrdinal("SalonId")),
                VerificationToken = reader.GetString(reader.GetOrdinal("VerificationToken"))
            };
    }
}

