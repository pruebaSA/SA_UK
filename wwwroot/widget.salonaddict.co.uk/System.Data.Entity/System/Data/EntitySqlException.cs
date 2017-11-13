namespace System.Data
{
    using System;
    using System.Data.Common.EntitySql;
    using System.Data.Entity;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;

    [Serializable]
    public sealed class EntitySqlException : EntityException
    {
        private int _column;
        private string _errorContext;
        private string _errorDescription;
        private int _line;
        private string _message;

        public EntitySqlException() : this(Strings.GeneralQueryError)
        {
            base.HResult = -2146232006;
        }

        public EntitySqlException(string message)
        {
            this._message = message;
            base.HResult = -2146232006;
        }

        private EntitySqlException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
            base.HResult = -2146232006;
        }

        public EntitySqlException(string message, Exception innerException) : base(message, innerException)
        {
            this._message = message;
            base.HResult = -2146232006;
        }

        private EntitySqlException(string message, string errorDescription, string errorContext, int line, int column, Exception innerException) : base(message, innerException)
        {
            this._message = message;
            this._errorDescription = errorDescription;
            this._errorContext = errorContext;
            this._line = line;
            this._column = column;
            base.HResult = -2146232006;
        }

        internal static EntitySqlException Create(System.Data.Common.EntitySql.ErrorContext errCtx, string errorMessage, Exception innerException) => 
            Create(errCtx.QueryText, errorMessage, errCtx.InputPosition, errCtx.ErrorContextInfo, errCtx.UseContextInfoAsResourceIdentifier, innerException);

        internal static EntitySqlException Create(string queryText, string errorDescription, int errorPosition, string errorContextInfo, bool loadErrorContextFromResource, Exception innerException)
        {
            string str;
            int num;
            int num2;
            if (loadErrorContextFromResource)
            {
                str = (errorContextInfo != null) ? EntityRes.GetString(errorContextInfo) : string.Empty;
            }
            else
            {
                str = errorContextInfo;
            }
            return new EntitySqlException(FormatQueryError(queryText, errorDescription, errorPosition, str, out num, out num2), errorDescription, str, num, num2, innerException);
        }

        private static string FormatQueryError(string queryText, string errorMessage, int errPos, string additionalInfo, out int lineNumber, out int columnNumber)
        {
            lineNumber = columnNumber = 0;
            if (queryText == null)
            {
                return (errorMessage ?? string.Empty);
            }
            StringBuilder builder = new StringBuilder(queryText.Length);
            for (int i = 0; i < queryText.Length; i++)
            {
                char c = queryText[i];
                if (CqlLexer.IsNewLine(c))
                {
                    c = '\n';
                }
                else if ((char.IsControl(c) || char.IsWhiteSpace(c)) && ('\r' != c))
                {
                    c = ' ';
                }
                builder.Append(c);
            }
            queryText = builder.ToString().TrimEnd(new char[] { '\n' });
            int num2 = errPos;
            int num3 = 1;
            string[] strArray = queryText.Split(new char[] { '\n' }, StringSplitOptions.None);
            for (int j = 0; j < strArray.Length; j++)
            {
                if (num2 < strArray[j].Length)
                {
                    break;
                }
                num2 -= strArray[j].Length + 1;
                num3++;
            }
            num2++;
            builder = new StringBuilder();
            builder.Append(errorMessage);
            bool flag = false;
            if (!string.IsNullOrEmpty(additionalInfo))
            {
                flag = true;
                builder.AppendFormat(CultureInfo.CurrentCulture, ", {0}", new object[] { Strings.LocalizedNear });
                builder.AppendFormat(CultureInfo.CurrentCulture, " {0}", new object[] { additionalInfo });
            }
            if (0 < errPos)
            {
                if (flag)
                {
                    builder.Append(",");
                }
                else
                {
                    builder.AppendFormat(CultureInfo.CurrentCulture, ", {0}", new object[] { Strings.LocalizedNear });
                }
                builder.AppendFormat(CultureInfo.CurrentCulture, " {0} {1}, {2} {3}", new object[] { Strings.LocalizedLine, num3, Strings.LocalizedColumn, num2 });
            }
            lineNumber = num3;
            columnNumber = num2;
            return builder.Append(".").ToString();
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Message", this._message);
            info.AddValue("ErrorDescription", this._errorDescription);
            info.AddValue("ErrorContext", this._errorContext);
            info.AddValue("Line", this._line);
            info.AddValue("Column", this._column);
        }

        public int Column =>
            this._column;

        public string ErrorContext =>
            (this._errorContext ?? string.Empty);

        public string ErrorDescription =>
            (this._errorDescription ?? string.Empty);

        public int Line =>
            this._line;

        public override string Message =>
            (this._message ?? string.Empty);
    }
}

