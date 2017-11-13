namespace System.Data.SqlClient
{
    using Microsoft.SqlServer.Server;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlTypes;
    using System.Globalization;
    using System.Transactions;

    internal sealed class SQL
    {
        internal static readonly byte[] AttentionHeader = new byte[] { 6, 1, 0, 8, 0, 0, 0, 0 };
        internal const string Connection = "Connection";
        internal const int SqlDependencyServerTimeout = 0x69780;
        internal const int SqlDependencyTimeoutDefault = 0;
        internal const string SqlNotificationServiceDefault = "SqlQueryNotificationService";
        internal const string SqlNotificationStoredProcedureDefault = "SqlQueryNotificationStoredProcedure";
        internal const string Transaction = "Transaction";
        internal const string WriteToServer = "WriteToServer";

        private SQL()
        {
        }

        internal static Exception ArgumentLengthMismatch(string arg1, string arg2) => 
            ADP.Argument(Res.GetString("SQL_ArgumentLengthMismatch", new object[] { arg1, arg2 }));

        internal static Exception AsyncConnectionRequired() => 
            ADP.InvalidOperation(Res.GetString("SQL_AsyncConnectionRequired"));

        internal static Exception AsyncInProcNotSupported() => 
            ADP.NotSupported(Res.GetString("SQL_AsyncInProcNotSupported"));

        internal static Exception BatchedUpdatesNotAvailableOnContextConnection() => 
            ADP.InvalidOperation(Res.GetString("SQL_BatchedUpdatesNotAvailableOnContextConnection"));

        internal static Exception BulkLoadBulkLoadNotAllowDBNull(string columnName) => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadNotAllowDBNull", new object[] { columnName }));

        internal static Exception BulkLoadCannotConvertValue(Type sourcetype, MetaType metatype, Exception e) => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadCannotConvertValue", new object[] { sourcetype.Name, metatype.TypeName }), e);

        internal static Exception BulkLoadConflictingTransactionOption() => 
            ADP.Argument(Res.GetString("SQL_BulkLoadConflictingTransactionOption"));

        internal static Exception BulkLoadExistingTransaction() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadExistingTransaction"));

        internal static Exception BulkLoadInvalidDestinationTable(string tableName, Exception inner) => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadInvalidDestinationTable", new object[] { tableName }), inner);

        internal static Exception BulkLoadInvalidTimeout(int timeout) => 
            ADP.Argument(Res.GetString("SQL_BulkLoadInvalidTimeout", new object[] { timeout.ToString(CultureInfo.InvariantCulture) }));

        internal static Exception BulkLoadInvalidVariantValue() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadInvalidVariantValue"));

        internal static Exception BulkLoadLcidMismatch(int sourceLcid, string sourceColumnName, int destinationLcid, string destinationColumnName) => 
            ADP.InvalidOperation(Res.GetString("Sql_BulkLoadLcidMismatch", new object[] { sourceLcid, sourceColumnName, destinationLcid, destinationColumnName }));

        internal static Exception BulkLoadMappingInaccessible() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadMappingInaccessible"));

        internal static Exception BulkLoadMappingsNamesOrOrdinalsOnly() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadMappingsNamesOrOrdinalsOnly"));

        internal static Exception BulkLoadMissingDestinationTable() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadMissingDestinationTable"));

        internal static Exception BulkLoadNoCollation() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadNoCollation"));

        internal static Exception BulkLoadNonMatchingColumnMapping() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadNonMatchingColumnMapping"));

        internal static Exception BulkLoadNonMatchingColumnName(string columnName) => 
            BulkLoadNonMatchingColumnName(columnName, null);

        internal static Exception BulkLoadNonMatchingColumnName(string columnName, Exception e) => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadNonMatchingColumnName", new object[] { columnName }), e);

        internal static Exception BulkLoadStringTooLong() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadStringTooLong"));

        internal static Exception CannotCompleteDelegatedTransactionWithOpenResults()
        {
            SqlErrorCollection errorCollection = new SqlErrorCollection {
                new SqlError(-2, 0, 11, null, Res.GetString("ADP_OpenReaderExists"), "", 0)
            };
            return SqlException.CreateException(errorCollection, null);
        }

        internal static Exception CannotGetDTCAddress() => 
            ADP.InvalidOperation(Res.GetString("SQL_CannotGetDTCAddress"));

        internal static Exception CannotModifyPropertyAsyncOperationInProgress(string property) => 
            ADP.InvalidOperation(Res.GetString("SQL_CannotModifyPropertyAsyncOperationInProgress", new object[] { property }));

        internal static Exception ChangePasswordArgumentMissing(string argumentName) => 
            ADP.ArgumentNull(Res.GetString("SQL_ChangePasswordArgumentMissing", new object[] { argumentName }));

        internal static Exception ChangePasswordConflictsWithSSPI() => 
            ADP.Argument(Res.GetString("SQL_ChangePasswordConflictsWithSSPI"));

        internal static Exception ChangePasswordRequiresYukon() => 
            ADP.InvalidOperation(Res.GetString("SQL_ChangePasswordRequiresYukon"));

        internal static Exception ChangePasswordUseOfUnallowedKey(string key) => 
            ADP.InvalidOperation(Res.GetString("SQL_ChangePasswordUseOfUnallowedKey", new object[] { key }));

        internal static Exception ConnectionDoomed() => 
            ADP.InvalidOperation(Res.GetString("SQL_ConnectionDoomed"));

        internal static Exception ConnectionLockedForBcpEvent() => 
            ADP.InvalidOperation(Res.GetString("SQL_ConnectionLockedForBcpEvent"));

        internal static Exception ContextAllowsLimitedKeywords() => 
            ADP.InvalidOperation(Res.GetString("SQL_ContextAllowsLimitedKeywords"));

        internal static Exception ContextAllowsOnlyTypeSystem2005() => 
            ADP.InvalidOperation(Res.GetString("SQL_ContextAllowsOnlyTypeSystem2005"));

        internal static Exception ContextConnectionIsInUse() => 
            ADP.InvalidOperation(Res.GetString("SQL_ContextConnectionIsInUse"));

        internal static Exception ContextUnavailableOutOfProc() => 
            ADP.InvalidOperation(Res.GetString("SQL_ContextUnavailableOutOfProc"));

        internal static Exception ContextUnavailableWhileInProc() => 
            ADP.InvalidOperation(Res.GetString("SQL_ContextUnavailableWhileInProc"));

        internal static Exception DBNullNotSupportedForTVPValues(string paramName) => 
            ADP.NotSupported(Res.GetString("SqlParameter_DBNullNotSupportedForTVP", new object[] { paramName }));

        internal static Exception DuplicateSortOrdinal(int sortOrdinal) => 
            ADP.InvalidOperation(Res.GetString("SqlProvider_DuplicateSortOrdinal", new object[] { sortOrdinal }));

        internal static Exception EnumeratedRecordFieldCountChanged(int recordNumber) => 
            ADP.Argument(Res.GetString("SQL_EnumeratedRecordFieldCountChanged", new object[] { recordNumber }));

        internal static Exception EnumeratedRecordMetaDataChanged(string fieldName, int recordNumber) => 
            ADP.Argument(Res.GetString("SQL_EnumeratedRecordMetaDataChanged", new object[] { fieldName, recordNumber }));

        internal static Exception FatalTimeout() => 
            ADP.InvalidOperation(Res.GetString("SQL_FatalTimeout"));

        internal static string GetSNIErrorMessage(int sniError) => 
            Res.GetString(string.Format(null, "SNI_ERROR_{0}", new object[] { sniError }));

        internal static Exception IEnumerableOfSqlDataRecordHasNoRows() => 
            ADP.Argument(Res.GetString("IEnumerableOfSqlDataRecordHasNoRows"));

        internal static Exception InstanceFailure() => 
            ADP.InvalidOperation(Res.GetString("SQL_InstanceFailure"));

        internal static Exception InvalidColumnMaxLength(string columnName, long maxLength) => 
            ADP.Argument(Res.GetString("SqlProvider_InvalidDataColumnMaxLength", new object[] { columnName, maxLength }));

        internal static Exception InvalidColumnPrecScale() => 
            ADP.Argument(Res.GetString("SqlMisc_InvalidPrecScaleMessage"));

        internal static Exception InvalidInternalPacketSize(string str) => 
            ADP.ArgumentOutOfRange(str);

        internal static Exception InvalidOperationInsideEvent() => 
            ADP.InvalidOperation(Res.GetString("SQL_BulkLoadInvalidOperationInsideEvent"));

        internal static Exception InvalidOptionLength(string key) => 
            ADP.Argument(Res.GetString("SQL_InvalidOptionLength", new object[] { key }));

        internal static Exception InvalidPacketSize() => 
            ADP.ArgumentOutOfRange(Res.GetString("SQL_InvalidTDSPacketSize"));

        internal static Exception InvalidPacketSizeValue() => 
            ADP.Argument(Res.GetString("SQL_InvalidPacketSizeValue"));

        internal static Exception InvalidParameterNameLength(string value) => 
            ADP.Argument(Res.GetString("SQL_InvalidParameterNameLength", new object[] { value }));

        internal static Exception InvalidParameterTypeNameFormat() => 
            ADP.Argument(Res.GetString("SQL_InvalidParameterTypeNameFormat"));

        internal static Exception InvalidPartnerConfiguration(string server, string database) => 
            ADP.InvalidOperation(Res.GetString("SQL_InvalidPartnerConfiguration", new object[] { server, database }));

        internal static Exception InvalidRead() => 
            ADP.InvalidOperation(Res.GetString("SQL_InvalidRead"));

        internal static Exception InvalidSchemaTableOrdinals() => 
            ADP.Argument(Res.GetString("InvalidSchemaTableOrdinals"));

        internal static Exception InvalidSortOrder(SortOrder order) => 
            ADP.InvalidEnumerationValue(typeof(SortOrder), (int) order);

        internal static Exception InvalidSqlDbType(SqlDbType value) => 
            ADP.InvalidEnumerationValue(typeof(SqlDbType), (int) value);

        internal static Exception InvalidSqlDbTypeForConstructor(SqlDbType type) => 
            ADP.Argument(Res.GetString("SqlMetaData_InvalidSqlDbTypeForConstructorFormat", new object[] { type.ToString() }));

        internal static Exception InvalidSqlDbTypeOneAllowedType(SqlDbType invalidType, string method, SqlDbType allowedType) => 
            ADP.Argument(Res.GetString("SQL_InvalidSqlDbTypeWithOneAllowedType", new object[] { invalidType, method, allowedType }));

        internal static ArgumentOutOfRangeException InvalidSqlDependencyTimeout(string param) => 
            ADP.ArgumentOutOfRange(Res.GetString("SqlDependency_InvalidTimeout"), param);

        internal static Exception InvalidSQLServerVersionUnknown() => 
            ADP.DataAdapter(Res.GetString("SQL_InvalidSQLServerVersionUnknown"));

        internal static Exception InvalidSSPIPacketSize() => 
            ADP.Argument(Res.GetString("SQL_InvalidSSPIPacketSize"));

        internal static Exception InvalidTableDerivedPrecisionForTvp(string columnName, byte precision) => 
            ADP.InvalidOperation(Res.GetString("SqlParameter_InvalidTableDerivedPrecisionForTvp", new object[] { precision, columnName, SqlDecimal.MaxPrecision }));

        internal static Exception InvalidTDSVersion() => 
            ADP.InvalidOperation(Res.GetString("SQL_InvalidTDSVersion"));

        internal static Exception InvalidUdt3PartNameFormat() => 
            ADP.Argument(Res.GetString("SQL_InvalidUdt3PartNameFormat"));

        internal static Exception MARSUnspportedOnConnection() => 
            ADP.InvalidOperation(Res.GetString("SQL_MarsUnsupportedOnConnection"));

        internal static Exception MissingSortOrdinal(int sortOrdinal) => 
            ADP.InvalidOperation(Res.GetString("SqlProvider_MissingSortOrdinal", new object[] { sortOrdinal }));

        internal static Exception MoneyOverflow(string moneyValue) => 
            ADP.Overflow(Res.GetString("SQL_MoneyOverflow", new object[] { moneyValue }));

        internal static Exception MultiSubnetFailoverWithFailoverPartner(bool serverProvidedFailoverPartner)
        {
            string error = Res.GetString("SQLMSF_FailoverPartnerNotSupported");
            if (serverProvidedFailoverPartner)
            {
                return ADP.InvalidOperation(error);
            }
            return ADP.Argument(error);
        }

        internal static Exception MultiSubnetFailoverWithInstanceSpecified() => 
            ADP.Argument(GetSNIErrorMessage(0x30));

        internal static Exception MultiSubnetFailoverWithMoreThan64IPs() => 
            ADP.InvalidOperation(GetSNIErrorMessage(0x2f));

        internal static Exception MultiSubnetFailoverWithNonTcpProtocol() => 
            ADP.Argument(GetSNIErrorMessage(0x31));

        internal static Exception MustSetTypeNameForParam(string paramType, string paramName) => 
            ADP.Argument(Res.GetString("SQL_ParameterTypeNameRequired", new object[] { paramType, paramName }));

        internal static Exception MustSetUdtTypeNameForUdtParams() => 
            ADP.Argument(Res.GetString("SQLUDT_InvalidUdtTypeName"));

        internal static Exception MustSpecifyBothSortOrderAndOrdinal(SortOrder order, int ordinal) => 
            ADP.InvalidOperation(Res.GetString("SqlMetaData_SpecifyBothSortOrderAndOrdinal", new object[] { order.ToString(), ordinal }));

        internal static Exception NameTooLong(string parameterName) => 
            ADP.Argument(Res.GetString("SqlMetaData_NameTooLong"), parameterName);

        internal static Exception NestedTransactionScopesNotSupported() => 
            ADP.InvalidOperation(Res.GetString("SQL_NestedTransactionScopesNotSupported"));

        internal static Exception NonBlobColumn(string columnName) => 
            ADP.InvalidCast(Res.GetString("SQL_NonBlobColumn", new object[] { columnName }));

        internal static Exception NonCharColumn(string columnName) => 
            ADP.InvalidCast(Res.GetString("SQL_NonCharColumn", new object[] { columnName }));

        internal static Exception NonLocalSSEInstance() => 
            ADP.NotSupported(Res.GetString("SQL_NonLocalSSEInstance"));

        internal static Exception NonXmlResult() => 
            ADP.InvalidOperation(Res.GetString("SQL_NonXmlResult"));

        internal static Exception NotAvailableOnContextConnection() => 
            ADP.InvalidOperation(Res.GetString("SQL_NotAvailableOnContextConnection"));

        internal static Exception NotEnoughColumnsInStructuredType() => 
            ADP.Argument(Res.GetString("SqlProvider_NotEnoughColumnsInStructuredType"));

        internal static Exception NotificationsNotAvailableOnContextConnection() => 
            ADP.InvalidOperation(Res.GetString("SQL_NotificationsNotAvailableOnContextConnection"));

        internal static Exception NotificationsRequireYukon() => 
            ADP.NotSupported(Res.GetString("SQL_NotificationsRequireYukon"));

        internal static ArgumentOutOfRangeException NotSupportedCommandType(CommandType value) => 
            NotSupportedEnumerationValue(typeof(CommandType), (int) value);

        internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, int value) => 
            ADP.ArgumentOutOfRange(Res.GetString("SQL_NotSupportedEnumerationValue", new object[] { type.Name, value.ToString(CultureInfo.InvariantCulture) }), type.Name);

        internal static ArgumentOutOfRangeException NotSupportedIsolationLevel(System.Data.IsolationLevel value) => 
            NotSupportedEnumerationValue(typeof(System.Data.IsolationLevel), (int) value);

        internal static Exception NullEmptyTransactionName() => 
            ADP.Argument(Res.GetString("SQL_NullEmptyTransactionName"));

        internal static Exception NullSchemaTableDataTypeNotSupported(string columnName) => 
            ADP.Argument(Res.GetString("NullSchemaTableDataTypeNotSupported", new object[] { columnName }));

        internal static Exception OperationCancelled() => 
            ADP.InvalidOperation(Res.GetString("SQL_OperationCancelled"));

        internal static Exception ParameterInvalidVariant(string paramName) => 
            ADP.InvalidOperation(Res.GetString("SQL_ParameterInvalidVariant", new object[] { paramName }));

        internal static Exception ParameterSizeRestrictionFailure(int index) => 
            ADP.InvalidOperation(Res.GetString("OleDb_CommandParameterError", new object[] { index.ToString(CultureInfo.InvariantCulture), "SqlParameter.Size" }));

        internal static Exception ParsingError() => 
            ADP.InvalidOperation(Res.GetString("SQL_ParsingError"));

        internal static Exception PendingBeginXXXExists() => 
            ADP.InvalidOperation(Res.GetString("SQL_PendingBeginXXXExists"));

        internal static Exception PrecisionValueOutOfRange(byte precision) => 
            ADP.Argument(Res.GetString("SQL_PrecisionValueOutOfRange", new object[] { precision.ToString(CultureInfo.InvariantCulture) }));

        internal static TransactionPromotionException PromotionFailed(Exception inner)
        {
            TransactionPromotionException e = new TransactionPromotionException(Res.GetString("SqlDelegatedTransaction_PromotionFailed"), inner);
            ADP.TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static Exception ROR_FailoverNotSupportedConnString() => 
            ADP.Argument(Res.GetString("SQLROR_FailoverNotSupported"));

        internal static Exception ROR_FailoverNotSupportedServer()
        {
            SqlErrorCollection errorCollection = new SqlErrorCollection {
                new SqlError(0, 0, 20, null, Res.GetString("SQLROR_FailoverNotSupported"), "", 0)
            };
            SqlException exception = SqlException.CreateException(errorCollection, null);
            exception._doNotReconnect = true;
            return exception;
        }

        internal static Exception ROR_InvalidRoutingInfo()
        {
            SqlErrorCollection errorCollection = new SqlErrorCollection {
                new SqlError(0, 0, 20, null, Res.GetString("SQLROR_InvalidRoutingInfo"), "", 0)
            };
            SqlException exception = SqlException.CreateException(errorCollection, null);
            exception._doNotReconnect = true;
            return exception;
        }

        internal static Exception ROR_RecursiveRoutingNotSupported()
        {
            SqlErrorCollection errorCollection = new SqlErrorCollection {
                new SqlError(0, 0, 20, null, Res.GetString("SQLROR_RecursiveRoutingNotSupported"), "", 0)
            };
            SqlException exception = SqlException.CreateException(errorCollection, null);
            exception._doNotReconnect = true;
            return exception;
        }

        internal static Exception ROR_TimeoutAfterRoutingInfo()
        {
            SqlErrorCollection errorCollection = new SqlErrorCollection {
                new SqlError(0, 0, 20, null, Res.GetString("SQLROR_TimeoutAfterRoutingInfo"), "", 0)
            };
            SqlException exception = SqlException.CreateException(errorCollection, null);
            exception._doNotReconnect = true;
            return exception;
        }

        internal static Exception ROR_UnexpectedRoutingInfo()
        {
            SqlErrorCollection errorCollection = new SqlErrorCollection {
                new SqlError(0, 0, 20, null, Res.GetString("SQLROR_UnexpectedRoutingInfo"), "", 0)
            };
            SqlException exception = SqlException.CreateException(errorCollection, null);
            exception._doNotReconnect = true;
            return exception;
        }

        internal static Exception ScaleValueOutOfRange(byte scale) => 
            ADP.Argument(Res.GetString("SQL_ScaleValueOutOfRange", new object[] { scale.ToString(CultureInfo.InvariantCulture) }));

        internal static Exception SingleValuedStructNotSupported() => 
            ADP.NotSupported(Res.GetString("MetaType_SingleValuedStructNotSupported"));

        internal static Exception SmallDateTimeOverflow(string datetime) => 
            ADP.Overflow(Res.GetString("SQL_SmallDateTimeOverflow", new object[] { datetime }));

        internal static Exception SnapshotNotSupported(System.Data.IsolationLevel level) => 
            ADP.Argument(Res.GetString("SQL_SnapshotNotSupported", new object[] { typeof(System.Data.IsolationLevel), level.ToString() }));

        internal static Exception SNIPacketAllocationFailure() => 
            ADP.InvalidOperation(Res.GetString("SQL_SNIPacketAllocationFailure"));

        internal static Exception SortOrdinalGreaterThanFieldCount(int columnOrdinal, int sortOrdinal) => 
            ADP.InvalidOperation(Res.GetString("SqlProvider_SortOrdinalGreaterThanFieldCount", new object[] { sortOrdinal, columnOrdinal }));

        internal static Exception SqlCommandHasExistingSqlNotificationRequest() => 
            ADP.InvalidOperation(Res.GetString("SQLNotify_AlreadyHasCommand"));

        internal static Exception SqlDepCannotBeCreatedInProc() => 
            ADP.InvalidOperation(Res.GetString("SqlNotify_SqlDepCannotBeCreatedInProc"));

        internal static Exception SqlDepDefaultOptionsButNoStart() => 
            ADP.InvalidOperation(Res.GetString("SqlDependency_DefaultOptionsButNoStart"));

        internal static Exception SqlDependencyDatabaseBrokerDisabled() => 
            ADP.InvalidOperation(Res.GetString("SqlDependency_DatabaseBrokerDisabled"));

        internal static Exception SqlDependencyDuplicateStart() => 
            ADP.InvalidOperation(Res.GetString("SqlDependency_DuplicateStart"));

        internal static Exception SqlDependencyEventNoDuplicate() => 
            ADP.InvalidOperation(Res.GetString("SqlDependency_EventNoDuplicate"));

        internal static Exception SqlDependencyIdMismatch() => 
            ADP.InvalidOperation(Res.GetString("SqlDependency_IdMismatch"));

        internal static Exception SqlDependencyNoMatchingServerDatabaseStart() => 
            ADP.InvalidOperation(Res.GetString("SqlDependency_NoMatchingServerDatabaseStart"));

        internal static Exception SqlDependencyNoMatchingServerStart() => 
            ADP.InvalidOperation(Res.GetString("SqlDependency_NoMatchingServerStart"));

        internal static Exception SqlMetaDataNoMetaData() => 
            ADP.InvalidOperation(Res.GetString("SqlMetaData_NoMetadata"));

        internal static Exception SqlNotificationException(SqlNotificationEventArgs notify) => 
            ADP.InvalidOperation(Res.GetString("SQLNotify_ErrorFormat", new object[] { notify.Type, notify.Info, notify.Source }));

        internal static SqlNullValueException SqlNullValue()
        {
            SqlNullValueException e = new SqlNullValueException();
            ADP.TraceExceptionAsReturnValue(e);
            return e;
        }

        internal static Exception SqlPipeAlreadyHasAnOpenResultSet(string methodName) => 
            ADP.InvalidOperation(Res.GetString("SqlPipe_AlreadyHasAnOpenResultSet", new object[] { methodName }));

        internal static Exception SqlPipeCommandHookedUpToNonContextConnection() => 
            ADP.InvalidOperation(Res.GetString("SqlPipe_CommandHookedUpToNonContextConnection"));

        internal static Exception SqlPipeDoesNotHaveAnOpenResultSet(string methodName) => 
            ADP.InvalidOperation(Res.GetString("SqlPipe_DoesNotHaveAnOpenResultSet", new object[] { methodName }));

        internal static Exception SqlPipeErrorRequiresSendEnd() => 
            ADP.InvalidOperation(Res.GetString("SQL_PipeErrorRequiresSendEnd"));

        internal static Exception SqlPipeIsBusy() => 
            ADP.InvalidOperation(Res.GetString("SqlPipe_IsBusy"));

        internal static Exception SqlPipeMessageTooLong(int messageLength) => 
            ADP.Argument(Res.GetString("SqlPipe_MessageTooLong", new object[] { messageLength }));

        internal static Exception SqlRecordReadOnly(string methodname)
        {
            if (methodname == null)
            {
                return ADP.InvalidOperation(Res.GetString("SQL_SqlRecordReadOnly2"));
            }
            return ADP.InvalidOperation(Res.GetString("SQL_SqlRecordReadOnly", new object[] { methodname }));
        }

        internal static Exception SqlResultSetClosed(string methodname)
        {
            if (methodname == null)
            {
                return ADP.InvalidOperation(Res.GetString("SQL_SqlResultSetClosed2"));
            }
            return ADP.InvalidOperation(Res.GetString("SQL_SqlResultSetClosed", new object[] { methodname }));
        }

        internal static Exception SqlResultSetCommandNotInSameConnection() => 
            ADP.InvalidOperation(Res.GetString("SQL_SqlResultSetCommandNotInSameConnection"));

        internal static Exception SqlResultSetNoAcceptableCursor() => 
            ADP.InvalidOperation(Res.GetString("SQL_SqlResultSetNoAcceptableCursor"));

        internal static Exception SqlResultSetNoData(string methodname) => 
            ADP.InvalidOperation(Res.GetString("ADP_DataReaderNoData", new object[] { methodname }));

        internal static Exception SqlResultSetRowDeleted(string methodname)
        {
            if (methodname == null)
            {
                return ADP.InvalidOperation(Res.GetString("SQL_SqlResultSetRowDeleted2"));
            }
            return ADP.InvalidOperation(Res.GetString("SQL_SqlResultSetRowDeleted", new object[] { methodname }));
        }

        internal static Exception StreamReadNotSupported() => 
            ADP.NotSupported(Res.GetString("SQL_StreamReadNotSupported"));

        internal static Exception StreamSeekNotSupported() => 
            ADP.NotSupported(Res.GetString("SQL_StreamSeekNotSupported"));

        internal static Exception StreamWriteNotSupported() => 
            ADP.NotSupported(Res.GetString("SQL_StreamWriteNotSupported"));

        internal static Exception SubclassMustOverride() => 
            ADP.InvalidOperation(Res.GetString("SqlMisc_SubclassMustOverride"));

        internal static Exception TableTypeCanOnlyBeParameter() => 
            ADP.Argument(Res.GetString("SQLTVP_TableTypeCanOnlyBeParameter"));

        internal static Exception TimeOverflow(string time) => 
            ADP.Overflow(Res.GetString("SQL_TimeOverflow", new object[] { time }));

        internal static Exception TimeScaleValueOutOfRange(byte scale) => 
            ADP.Argument(Res.GetString("SQL_TimeScaleValueOutOfRange", new object[] { scale.ToString(CultureInfo.InvariantCulture) }));

        internal static Exception TooManyValues(string arg) => 
            ADP.Argument(Res.GetString("SQL_TooManyValues"), arg);

        internal static Exception UDTInvalidSqlType(string typeName) => 
            ADP.Argument(Res.GetString("SQLUDT_InvalidSqlType", new object[] { typeName }));

        internal static Exception UDTUnexpectedResult(string exceptionText) => 
            ADP.TypeLoad(Res.GetString("SQLUDT_Unexpected", new object[] { exceptionText }));

        internal static Exception UnexpectedSmiEvent(SmiEventSink_Default.UnexpectedEventType eventType) => 
            ADP.InvalidOperation(Res.GetString("SQL_UnexpectedSmiEvent", new object[] { (int) eventType }));

        internal static Exception UnexpectedTypeNameForNonStructParams(string paramName) => 
            ADP.NotSupported(Res.GetString("SqlParameter_UnexpectedTypeNameForNonStruct", new object[] { paramName }));

        internal static Exception UnexpectedUdtTypeNameForNonUdtParams() => 
            ADP.Argument(Res.GetString("SQLUDT_UnexpectedUdtTypeName"));

        internal static Exception UnknownSysTxIsolationLevel(System.Transactions.IsolationLevel isolationLevel) => 
            ADP.InvalidOperation(Res.GetString("SQL_UnknownSysTxIsolationLevel", new object[] { isolationLevel.ToString() }));

        internal static Exception UnsupportedColumnTypeForSqlProvider(string columnName, string typeName) => 
            ADP.Argument(Res.GetString("SqlProvider_InvalidDataColumnType", new object[] { columnName, typeName }));

        internal static Exception UnsupportedTVPOutputParameter(ParameterDirection direction, string paramName) => 
            ADP.NotSupported(Res.GetString("SqlParameter_UnsupportedTVPOutputParameter", new object[] { direction.ToString(CultureInfo.InvariantCulture), paramName }));

        internal static Exception UserInstanceFailoverNotCompatible() => 
            ADP.Argument(Res.GetString("SQL_UserInstanceFailoverNotCompatible"));

        internal static Exception UserInstanceNotAvailableInProc() => 
            ADP.InvalidOperation(Res.GetString("SQL_UserInstanceNotAvailableInProc"));
    }
}

