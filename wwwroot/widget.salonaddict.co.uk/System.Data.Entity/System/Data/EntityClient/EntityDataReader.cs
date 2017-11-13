namespace System.Data.EntityClient
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Reflection;

    public class EntityDataReader : DbDataReader, IExtendedDataRecord, IDataRecord
    {
        private CommandBehavior _behavior;
        private EntityCommand _command;
        private DbDataReader _storeDataReader;
        private IExtendedDataRecord _storeExtendedDataRecord;

        internal EntityDataReader(EntityCommand command, DbDataReader storeDataReader, CommandBehavior behavior)
        {
            this._command = command;
            this._storeDataReader = storeDataReader;
            this._storeExtendedDataRecord = storeDataReader as IExtendedDataRecord;
            this._behavior = behavior;
        }

        public override void Close()
        {
            if (this._command != null)
            {
                this._storeDataReader.Close();
                this._command.NotifyDataReaderClosing();
                if ((this._behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
                {
                    this._command.Connection.Close();
                }
                this._command = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this._storeDataReader.Dispose();
            }
        }

        public override bool GetBoolean(int ordinal) => 
            this._storeDataReader.GetBoolean(ordinal);

        public override byte GetByte(int ordinal) => 
            this._storeDataReader.GetByte(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => 
            this._storeDataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

        public override char GetChar(int ordinal) => 
            this._storeDataReader.GetChar(ordinal);

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => 
            this._storeDataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

        public DbDataReader GetDataReader(int i) => 
            this.GetDbDataReader(i);

        public DbDataRecord GetDataRecord(int i) => 
            this._storeExtendedDataRecord?.GetDataRecord(i);

        public override string GetDataTypeName(int ordinal) => 
            this._storeDataReader.GetDataTypeName(ordinal);

        public override DateTime GetDateTime(int ordinal) => 
            this._storeDataReader.GetDateTime(ordinal);

        protected override DbDataReader GetDbDataReader(int ordinal) => 
            this._storeDataReader.GetData(ordinal);

        public override decimal GetDecimal(int ordinal) => 
            this._storeDataReader.GetDecimal(ordinal);

        public override double GetDouble(int ordinal) => 
            this._storeDataReader.GetDouble(ordinal);

        public override IEnumerator GetEnumerator() => 
            this._storeDataReader.GetEnumerator();

        public override Type GetFieldType(int ordinal) => 
            this._storeDataReader.GetFieldType(ordinal);

        public override float GetFloat(int ordinal) => 
            this._storeDataReader.GetFloat(ordinal);

        public override Guid GetGuid(int ordinal) => 
            this._storeDataReader.GetGuid(ordinal);

        public override short GetInt16(int ordinal) => 
            this._storeDataReader.GetInt16(ordinal);

        public override int GetInt32(int ordinal) => 
            this._storeDataReader.GetInt32(ordinal);

        public override long GetInt64(int ordinal) => 
            this._storeDataReader.GetInt64(ordinal);

        public override string GetName(int ordinal) => 
            this._storeDataReader.GetName(ordinal);

        public override int GetOrdinal(string name)
        {
            EntityUtil.CheckArgumentNull<string>(name, "name");
            return this._storeDataReader.GetOrdinal(name);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Type GetProviderSpecificFieldType(int ordinal) => 
            this._storeDataReader.GetProviderSpecificFieldType(ordinal);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object GetProviderSpecificValue(int ordinal) => 
            this._storeDataReader.GetProviderSpecificValue(ordinal);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetProviderSpecificValues(object[] values) => 
            this._storeDataReader.GetProviderSpecificValues(values);

        public override DataTable GetSchemaTable() => 
            this._storeDataReader.GetSchemaTable();

        public override string GetString(int ordinal) => 
            this._storeDataReader.GetString(ordinal);

        public override object GetValue(int ordinal) => 
            this._storeDataReader.GetValue(ordinal);

        public override int GetValues(object[] values) => 
            this._storeDataReader.GetValues(values);

        public override bool IsDBNull(int ordinal) => 
            this._storeDataReader.IsDBNull(ordinal);

        public override bool NextResult()
        {
            bool flag;
            try
            {
                flag = this._storeDataReader.NextResult();
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.CommandExecution(Strings.EntityClient_StoreReaderFailed, exception);
                }
                throw;
            }
            return flag;
        }

        public override bool Read() => 
            this._storeDataReader.Read();

        public System.Data.Common.DataRecordInfo DataRecordInfo =>
            this._storeExtendedDataRecord?.DataRecordInfo;

        public override int Depth =>
            this._storeDataReader.Depth;

        public override int FieldCount =>
            this._storeDataReader.FieldCount;

        public override bool HasRows =>
            this._storeDataReader.HasRows;

        public override bool IsClosed =>
            this._storeDataReader.IsClosed;

        public override object this[int ordinal] =>
            this._storeDataReader[ordinal];

        public override object this[string name]
        {
            get
            {
                EntityUtil.CheckArgumentNull<string>(name, "name");
                return this._storeDataReader[name];
            }
        }

        public override int RecordsAffected =>
            this._storeDataReader.RecordsAffected;

        public override int VisibleFieldCount =>
            this._storeDataReader.VisibleFieldCount;
    }
}

