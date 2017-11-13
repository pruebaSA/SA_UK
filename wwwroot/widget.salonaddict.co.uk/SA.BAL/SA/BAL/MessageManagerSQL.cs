namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;

    public class MessageManagerSQL : IMessageManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly string _connectionString;

        public MessageManagerSQL(string connectionString, ICacheManager cacheManager)
        {
            this._connectionString = connectionString;
            this._cacheManager = cacheManager;
        }

        public void DeleteQueuedMessage(QueuedMessageDB value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.QueuedMessageId == Guid.Empty)
            {
                throw new ArgumentException("queued message identifier cannot be empty.");
            }
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@QueuedMessageId", value.QueuedMessageId) };
            SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_QueuedMessageDeleteById", commandParameters);
        }

        public QueuedMessageDB GetQueuedMessageById(Guid queuedMessageId)
        {
            if (queuedMessageId == Guid.Empty)
            {
                throw new ArgumentException("queued message identifier cannot be empty.");
            }
            QueuedMessageDB edb = null;
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@QueuedMessageId", queuedMessageId) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_QueuedMessageLoadById", parameterValues))
            {
                if (reader.Read())
                {
                    edb = this.QueuedMessageMapping(reader);
                }
            }
            return edb;
        }

        public QueuedMessageDB InsertQueuedMessage(QueuedMessageDB value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.SendTries < 0)
            {
                throw new ArgumentException("sent tries cannot be less than 0.");
            }
            value.Bcc = value.Bcc ?? string.Empty;
            value.Body = value.Body ?? string.Empty;
            value.Cc = value.Cc ?? string.Empty;
            value.Sender = value.Sender ?? string.Empty;
            value.SenderDisplayName = value.SenderDisplayName ?? string.Empty;
            value.MessageType = value.MessageType ?? string.Empty;
            value.Subject = value.Subject ?? string.Empty;
            value.Recipient = value.Recipient ?? string.Empty;
            value.RecipientDisplayName = value.RecipientDisplayName ?? string.Empty;
            SqlParameter parameter = new SqlParameter("@QueuedMessageId", SqlDbType.UniqueIdentifier) {
                Direction = ParameterDirection.Output
            };
            SqlParameter[] commandParameters = new SqlParameter[] { parameter, new SqlParameter("@MessageType", value.MessageType), new SqlParameter("@Sender", value.Sender), new SqlParameter("@SenderDisplayName", value.SenderDisplayName), new SqlParameter("@Recipient", value.Recipient), new SqlParameter("@RecipientDisplayName", value.RecipientDisplayName), new SqlParameter("@Cc", value.Cc), new SqlParameter("@Bcc", value.Bcc), new SqlParameter("@Subject", value.Subject), new SqlParameter("@Body", value.Body), new SqlParameter("@SendTries", value.SendTries), new SqlParameter("@SentOn", value.SentOn ?? SqlDateTime.Null), new SqlParameter("@CreatedOn", value.CreatedOn) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_QueuedMessageInsert", commandParameters) <= 0)
            {
                return null;
            }
            Guid queuedMessageId = new Guid(parameter.Value.ToString());
            value = this.GetQueuedMessageById(queuedMessageId);
            return value;
        }

        private QueuedMessageDB QueuedMessageMapping(SqlDataReader reader) => 
            new QueuedMessageDB { 
                Bcc = reader.GetString(reader.GetOrdinal("Bcc")),
                Body = reader.GetString(reader.GetOrdinal("Body")),
                Cc = reader.GetString(reader.GetOrdinal("Cc")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                MessageType = reader.GetString(reader.GetOrdinal("MessageType")),
                QueuedMessageId = reader.GetGuid(reader.GetOrdinal("QueuedMessageId")),
                Recipient = reader.GetString(reader.GetOrdinal("Recipient")),
                RecipientDisplayName = reader.GetString(reader.GetOrdinal("RecipientDisplayName")),
                Sender = reader.GetString(reader.GetOrdinal("Sender")),
                SenderDisplayName = reader.GetString(reader.GetOrdinal("SenderDisplayName")),
                SentOn = reader.GetValue(reader.GetOrdinal("SentOn")) as DateTime?,
                SendTries = reader.GetInt32(reader.GetOrdinal("SendTries")),
                Subject = reader.GetString(reader.GetOrdinal("Subject"))
            };

        public List<QueuedMessageDB> SearchQueuedMessages(string sender, string recipient, DateTime startDate, DateTime endDate, int queuedEmailCount, bool notSentItemsOnly, int maxSendTries)
        {
            List<QueuedMessageDB> list = new List<QueuedMessageDB>();
            SqlParameter[] parameterValues = new SqlParameter[] { new SqlParameter("@Sender", sender ?? SqlString.Null), new SqlParameter("@Recipient", recipient ?? SqlString.Null), new SqlParameter("@StartDate", startDate ?? SqlDateTime.Null), new SqlParameter("@EndDate", endDate ?? SqlDateTime.Null), new SqlParameter("@QueuedEmailCount", queuedEmailCount ?? SqlInt32.Null), new SqlParameter("@NotSentItemsOnly", notSentItemsOnly ?? SqlInt32.Null), new SqlParameter("@MaxSendTries", maxSendTries ?? SqlInt32.Null) };
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, "SA_Widget_QueuedMessageLoad", parameterValues))
            {
                while (reader.Read())
                {
                    QueuedMessageDB item = this.QueuedMessageMapping(reader);
                    list.Add(item);
                }
            }
            return list;
        }

        public QueuedMessageDB UpdateQueuedMessage(QueuedMessageDB value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.QueuedMessageId == Guid.Empty)
            {
                throw new ArgumentException("queued message identifier cannot be empty.");
            }
            if (value.SendTries < 0)
            {
                throw new ArgumentException("sent tries cannot be less than 0.");
            }
            value.Bcc = value.Bcc ?? string.Empty;
            value.Body = value.Body ?? string.Empty;
            value.Cc = value.Cc ?? string.Empty;
            value.Sender = value.Sender ?? string.Empty;
            value.SenderDisplayName = value.SenderDisplayName ?? string.Empty;
            value.MessageType = value.MessageType ?? string.Empty;
            value.Subject = value.Subject ?? string.Empty;
            value.Recipient = value.Recipient ?? string.Empty;
            value.RecipientDisplayName = value.RecipientDisplayName ?? string.Empty;
            SqlParameter[] commandParameters = new SqlParameter[] { new SqlParameter("@QueuedMessageId", value.QueuedMessageId), new SqlParameter("@MessageType", value.MessageType), new SqlParameter("@Sender", value.Sender), new SqlParameter("@SenderDisplayName", value.SenderDisplayName), new SqlParameter("@Recipient", value.Recipient), new SqlParameter("@RecipientDisplayName", value.RecipientDisplayName), new SqlParameter("@Cc", value.Cc), new SqlParameter("@Bcc", value.Bcc), new SqlParameter("@Subject", value.Subject), new SqlParameter("@Body", value.Body), new SqlParameter("@SendTries", value.SendTries), new SqlParameter("@SentOn", value.SentOn ?? SqlDateTime.Null), new SqlParameter("@CreatedOn", value.CreatedOn) };
            if (SqlHelper.ExecuteNonQuery(this._connectionString, CommandType.StoredProcedure, "SA_Widget_QueuedMessageUpdate", commandParameters) <= 0)
            {
                return null;
            }
            value = this.GetQueuedMessageById(value.QueuedMessageId);
            return value;
        }
    }
}

