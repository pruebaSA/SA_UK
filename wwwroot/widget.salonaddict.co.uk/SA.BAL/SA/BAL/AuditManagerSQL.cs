namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;

    public class AuditManagerSQL : ILogManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public AuditManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public void ClearLog()
        {
            SqlParameter[] commandParameters = new SqlParameter[0];
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_LogClear", commandParameters);
        }

        public void DeleteLog(LogDB log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            if (log.LogId == Guid.Empty)
            {
                throw new ArgumentException("log identifier cannot be empty.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@LogId", log.LogId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_LogDeleteById", commandParameters);
        }

        public List<LogDB> GetAllErrors()
        {
            List<LogDB> list = new List<LogDB>();
            SqlParameter[] parameterValues = new SqlParameter[0];
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_LogLoad", parameterValues))
            {
                while (reader.Read())
                {
                    LogDB item = this.LogMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public LogDB GetErrorById(Guid logId)
        {
            if (logId == Guid.Empty)
            {
                throw new ArgumentException("log identifier cannot be empty.");
            }
            LogDB gdb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@LogId", logId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_LogLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    gdb = this.LogMapping(reader);
                }
            }
            return gdb;
        }

        public LogDB InsertError(LogDB log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            log.Exception = log.Exception ?? string.Empty;
            log.LogType = log.LogType ?? string.Empty;
            log.Message = log.Message ?? string.Empty;
            log.PageURL = log.PageURL ?? string.Empty;
            log.Params = log.Params ?? string.Empty;
            log.ReferrerURL = log.ReferrerURL ?? string.Empty;
            log.UserHostAddress = log.UserHostAddress ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@LogId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@LogType", log.LogType), new SqlParameter("@Message", log.Message), new SqlParameter("@Exception", log.Exception), new SqlParameter("@UserId", log.UserId ?? SqlGuid.Null), new SqlParameter("@PageURL", log.PageURL), new SqlParameter("@ReferrerURL", log.ReferrerURL), new SqlParameter("@UserHostAddress", log.UserHostAddress), new SqlParameter("@Params", log.Params), new SqlParameter("@CreatedOn", log.CreatedOn) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_LogInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid logId = new Guid(parameter.Value.ToString());
            log = this.GetErrorById(logId);
            return log;
        }

        private LogDB LogMapping(SqlDataReader reader) => 
            new LogDB { 
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                Exception = reader.GetString(reader.GetOrdinal("Exception")),
                LogId = reader.GetGuid(reader.GetOrdinal("LogId")),
                LogType = reader.GetString(reader.GetOrdinal("LogType")),
                Message = reader.GetString(reader.GetOrdinal("Message")),
                PageURL = reader.GetString(reader.GetOrdinal("PageURL")),
                Params = reader.GetString(reader.GetOrdinal("Params")),
                ReferrerURL = reader.GetString(reader.GetOrdinal("ReferrerURL")),
                UserId = reader.GetValue(reader.GetOrdinal("UserId")) as Guid?,
                UserHostAddress = reader.GetString(reader.GetOrdinal("UserHostAddress"))
            };
    }
}

