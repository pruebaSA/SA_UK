namespace System.Data.Odbc
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;

    internal static class ODBC
    {
        internal const string Pwd = "pwd";

        internal static Exception CantAllocateEnvironmentHandle(ODBC32.RetCode retcode) => 
            ADP.DataAdapter(Res.GetString("Odbc_CantAllocateEnvironmentHandle", new object[] { ODBC32.RetcodeToString(retcode) }));

        internal static Exception CantEnableConnectionpooling(ODBC32.RetCode retcode) => 
            ADP.DataAdapter(Res.GetString("Odbc_CantEnableConnectionpooling", new object[] { ODBC32.RetcodeToString(retcode) }));

        internal static Exception CantSetPropertyOnOpenConnection() => 
            ADP.InvalidOperation(Res.GetString("Odbc_CantSetPropertyOnOpenConnection"));

        internal static Exception ConnectionStringTooLong() => 
            ADP.Argument(Res.GetString("OdbcConnection_ConnectionStringTooLong", new object[] { 0x400 }));

        internal static Exception FailedToGetDescriptorHandle(ODBC32.RetCode retcode) => 
            ADP.DataAdapter(Res.GetString("Odbc_FailedToGetDescriptorHandle", new object[] { ODBC32.RetcodeToString(retcode) }));

        internal static ArgumentException GetSchemaRestrictionRequired() => 
            ADP.Argument(Res.GetString("ODBC_GetSchemaRestrictionRequired"));

        internal static Exception NegativeArgument() => 
            ADP.Argument(Res.GetString("Odbc_NegativeArgument"));

        internal static InvalidOperationException NoMappingForSqlTransactionLevel(int value) => 
            ADP.DataAdapter(Res.GetString("Odbc_NoMappingForSqlTransactionLevel", new object[] { value.ToString(CultureInfo.InvariantCulture) }));

        internal static Exception NotInTransaction() => 
            ADP.InvalidOperation(Res.GetString("Odbc_NotInTransaction"));

        internal static ArgumentOutOfRangeException NotSupportedCommandType(CommandType value) => 
            NotSupportedEnumerationValue(typeof(CommandType), (int) value);

        internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, int value) => 
            ADP.ArgumentOutOfRange(Res.GetString("ODBC_NotSupportedEnumerationValue", new object[] { type.Name, value.ToString(CultureInfo.InvariantCulture) }), type.Name);

        internal static ArgumentOutOfRangeException NotSupportedIsolationLevel(IsolationLevel value) => 
            NotSupportedEnumerationValue(typeof(IsolationLevel), (int) value);

        internal static short ShortStringLength(string inputString) => 
            ((short) ADP.StringLength(inputString));

        internal static void TraceODBC(int level, string method, ODBC32.RetCode retcode)
        {
            Bid.TraceSqlReturn("<odbc|API|ODBC|RET> %08X{SQLRETURN}, method=%ls\n", retcode, method);
        }

        internal static void TraceODBC(int level, string method, string param, ODBC32.RetCode retcode)
        {
            Bid.TraceSqlReturn("<odbc|API|ODBC|RET> %08X{SQLRETURN}, method=%ls, param=%ls\n", retcode, method, param);
        }

        internal static Exception UnknownOdbcType(OdbcType odbctype) => 
            ADP.InvalidEnumerationValue(typeof(OdbcType), (int) odbctype);

        internal static Exception UnknownSQLType(ODBC32.SQL_TYPE sqltype) => 
            ADP.Argument(Res.GetString("Odbc_UnknownSQLType", new object[] { sqltype.ToString() }));
    }
}

