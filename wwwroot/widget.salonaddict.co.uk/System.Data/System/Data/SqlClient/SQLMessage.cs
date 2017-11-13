namespace System.Data.SqlClient
{
    using System;
    using System.Data;

    internal sealed class SQLMessage
    {
        private SQLMessage()
        {
        }

        internal static string CultureIdError() => 
            Res.GetString("SQL_CultureIdError");

        internal static string EncryptionNotSupportedByClient() => 
            Res.GetString("SQL_EncryptionNotSupportedByClient");

        internal static string EncryptionNotSupportedByServer() => 
            Res.GetString("SQL_EncryptionNotSupportedByServer");

        internal static string OperationCancelled() => 
            Res.GetString("SQL_OperationCancelled");

        internal static string SevereError() => 
            Res.GetString("SQL_SevereError");

        internal static string SSPIGenerateError() => 
            Res.GetString("SQL_SSPIGenerateError");

        internal static string SSPIInitializeError() => 
            Res.GetString("SQL_SSPIInitializeError");

        internal static string Timeout() => 
            Res.GetString("SQL_Timeout");

        internal static string UserInstanceFailure() => 
            Res.GetString("SQL_UserInstanceFailure");
    }
}

