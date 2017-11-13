namespace System.Data.Query.ResultAssembly
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Internal.Materialization;
    using System.Data.Metadata.Edm;
    using System.Reflection;

    internal sealed class BridgeDataRecord : DbDataRecord, IExtendedDataRecord, IDataRecord
    {
        private BridgeDataReader _currentNestedReader;
        private BridgeDataRecord _currentNestedRecord;
        private int _lastColumnRead;
        private long _lastDataOffsetRead;
        private int _lastOrdinalCheckedForNull;
        private object _lastValueCheckedForNull;
        private RecordState _source;
        private Status _status;
        internal readonly int Depth;
        private readonly Shaper<RecordState> Shaper;

        internal BridgeDataRecord(Shaper<RecordState> shaper, int depth)
        {
            this.Shaper = shaper;
            this.Depth = depth;
        }

        private void AssertReaderIsOpen()
        {
            if (this.IsExplicitlyClosed)
            {
                throw EntityUtil.ClosedDataReaderError();
            }
            if (this.IsImplicitlyClosed)
            {
                throw EntityUtil.ImplicitlyClosedDataReaderError();
            }
        }

        private void AssertReaderIsOpenWithData()
        {
            this.AssertReaderIsOpen();
            if (!this.HasData)
            {
                throw EntityUtil.NoData();
            }
        }

        private void AssertSequentialAccess(int ordinal)
        {
            if ((ordinal < 0) || (ordinal >= this._source.ColumnCount))
            {
                throw EntityUtil.ArgumentOutOfRange("ordinal");
            }
            if (this._lastColumnRead >= ordinal)
            {
                throw EntityUtil.NonSequentialColumnAccess(ordinal, this._lastColumnRead + 1);
            }
            this._lastColumnRead = ordinal;
            this._lastDataOffsetRead = 0x7fffffffffffffffL;
        }

        private void AssertSequentialAccess(int ordinal, long dataOffset, string methodName)
        {
            if ((ordinal < 0) || (ordinal >= this._source.ColumnCount))
            {
                throw EntityUtil.ArgumentOutOfRange("ordinal");
            }
            if ((this._lastColumnRead > ordinal) || ((this._lastColumnRead == ordinal) && (this._lastDataOffsetRead == 0x7fffffffffffffffL)))
            {
                throw EntityUtil.NonSequentialColumnAccess(ordinal, this._lastColumnRead + 1);
            }
            if (this._lastColumnRead == ordinal)
            {
                if (this._lastDataOffsetRead >= dataOffset)
                {
                    throw EntityUtil.NonSequentialChunkAccess(dataOffset, this._lastDataOffsetRead + 1L, methodName);
                }
            }
            else
            {
                this._lastColumnRead = ordinal;
                this._lastDataOffsetRead = -1L;
            }
        }

        internal void CloseExplicitly()
        {
            this._status = Status.ClosedExplicitly;
            this._source = null;
            this.CloseNestedObjectImplicitly();
        }

        internal void CloseImplicitly()
        {
            this._status = Status.ClosedImplicitly;
            this._source = null;
            this.CloseNestedObjectImplicitly();
        }

        private void CloseNestedObjectImplicitly()
        {
            BridgeDataRecord record = this._currentNestedRecord;
            if (record != null)
            {
                this._currentNestedRecord = null;
                record.CloseImplicitly();
            }
            BridgeDataReader reader = this._currentNestedReader;
            if (reader != null)
            {
                this._currentNestedReader = null;
                reader.CloseImplicitly();
            }
        }

        public override bool GetBoolean(int ordinal) => 
            ((bool) this.GetValue(ordinal));

        public override byte GetByte(int ordinal) => 
            ((byte) this.GetValue(ordinal));

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            this.AssertReaderIsOpenWithData();
            this.AssertSequentialAccess(ordinal, dataOffset, "GetBytes");
            long num = this._source.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
            if (buffer != null)
            {
                this._lastDataOffsetRead = (dataOffset + num) - 1L;
            }
            return num;
        }

        public override char GetChar(int ordinal) => 
            ((char) this.GetValue(ordinal));

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            this.AssertReaderIsOpenWithData();
            this.AssertSequentialAccess(ordinal, dataOffset, "GetChars");
            long num = this._source.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
            if (buffer != null)
            {
                this._lastDataOffsetRead = (dataOffset + num) - 1L;
            }
            return num;
        }

        public DbDataReader GetDataReader(int ordinal) => 
            this.GetDbDataReader(ordinal);

        public DbDataRecord GetDataRecord(int ordinal) => 
            ((DbDataRecord) this.GetValue(ordinal));

        public override string GetDataTypeName(int ordinal)
        {
            this.AssertReaderIsOpenWithData();
            return TypeHelpers.GetFullName(this.GetTypeUsage(ordinal));
        }

        public override DateTime GetDateTime(int ordinal) => 
            ((DateTime) this.GetValue(ordinal));

        protected override DbDataReader GetDbDataReader(int ordinal) => 
            ((DbDataReader) this.GetValue(ordinal));

        public override decimal GetDecimal(int ordinal) => 
            ((decimal) this.GetValue(ordinal));

        public override double GetDouble(int ordinal) => 
            ((double) this.GetValue(ordinal));

        public override Type GetFieldType(int ordinal)
        {
            this.AssertReaderIsOpenWithData();
            return BridgeDataReader.GetClrTypeFromTypeMetadata(this.GetTypeUsage(ordinal));
        }

        public override float GetFloat(int ordinal) => 
            ((float) this.GetValue(ordinal));

        public override Guid GetGuid(int ordinal) => 
            ((Guid) this.GetValue(ordinal));

        public override short GetInt16(int ordinal) => 
            ((short) this.GetValue(ordinal));

        public override int GetInt32(int ordinal) => 
            ((int) this.GetValue(ordinal));

        public override long GetInt64(int ordinal) => 
            ((long) this.GetValue(ordinal));

        public override string GetName(int ordinal)
        {
            this.AssertReaderIsOpen();
            return this._source.GetName(ordinal);
        }

        private object GetNestedObjectValue(object result)
        {
            if (result != DBNull.Value)
            {
                RecordState newSource = result as RecordState;
                if (newSource != null)
                {
                    if (newSource.IsNull)
                    {
                        result = DBNull.Value;
                        return result;
                    }
                    BridgeDataRecord record = new BridgeDataRecord(this.Shaper, this.Depth + 1);
                    record.SetRecordSource(newSource, true);
                    result = record;
                    this._currentNestedRecord = record;
                    this._currentNestedReader = null;
                    return result;
                }
                Coordinator<RecordState> coordinator = result as Coordinator<RecordState>;
                if (coordinator != null)
                {
                    BridgeDataReader reader = new BridgeDataReader(this.Shaper, coordinator.TypedCoordinatorFactory, this.Depth + 1);
                    result = reader;
                    this._currentNestedRecord = null;
                    this._currentNestedReader = reader;
                }
            }
            return result;
        }

        public override int GetOrdinal(string name)
        {
            this.AssertReaderIsOpen();
            return this._source.GetOrdinal(name);
        }

        public override string GetString(int ordinal) => 
            ((string) this.GetValue(ordinal));

        private TypeUsage GetTypeUsage(int ordinal)
        {
            if ((ordinal < 0) || (ordinal >= this._source.ColumnCount))
            {
                throw EntityUtil.ArgumentOutOfRange("ordinal");
            }
            RecordState state = this._source.CurrentColumnValues[ordinal] as RecordState;
            if (state != null)
            {
                return state.DataRecordInfo.RecordType;
            }
            return this._source.GetTypeUsage(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            this.AssertReaderIsOpenWithData();
            this.AssertSequentialAccess(ordinal);
            object result = null;
            if (ordinal == this._lastOrdinalCheckedForNull)
            {
                return this._lastValueCheckedForNull;
            }
            this._lastOrdinalCheckedForNull = -1;
            this._lastValueCheckedForNull = null;
            this.CloseNestedObjectImplicitly();
            result = this._source.CurrentColumnValues[ordinal];
            if (this._source.IsNestedObject(ordinal))
            {
                result = this.GetNestedObjectValue(result);
            }
            return result;
        }

        public override int GetValues(object[] values)
        {
            EntityUtil.CheckArgumentNull<object[]>(values, "values");
            int num = Math.Min(values.Length, this.FieldCount);
            for (int i = 0; i < num; i++)
            {
                values[i] = this.GetValue(i);
            }
            return num;
        }

        public override bool IsDBNull(int ordinal)
        {
            object obj2 = this.GetValue(ordinal);
            this._lastColumnRead--;
            this._lastDataOffsetRead = -1L;
            this._lastValueCheckedForNull = obj2;
            this._lastOrdinalCheckedForNull = ordinal;
            return (DBNull.Value == obj2);
        }

        internal void SetRecordSource(RecordState newSource, bool hasData)
        {
            if (hasData)
            {
                this._source = newSource;
            }
            else
            {
                this._source = null;
            }
            this._status = Status.Open;
            this._lastColumnRead = -1;
            this._lastDataOffsetRead = -1L;
            this._lastOrdinalCheckedForNull = -1;
            this._lastValueCheckedForNull = null;
        }

        public System.Data.Common.DataRecordInfo DataRecordInfo
        {
            get
            {
                this.AssertReaderIsOpen();
                return this._source.DataRecordInfo;
            }
        }

        public override int FieldCount
        {
            get
            {
                this.AssertReaderIsOpen();
                return this._source.ColumnCount;
            }
        }

        internal bool HasData =>
            (this._source != null);

        internal bool IsClosed =>
            (this._status != Status.Open);

        internal bool IsExplicitlyClosed =>
            (this._status == Status.ClosedExplicitly);

        internal bool IsImplicitlyClosed =>
            (this._status == Status.ClosedImplicitly);

        public override object this[int ordinal] =>
            this.GetValue(ordinal);

        public override object this[string name] =>
            this.GetValue(this.GetOrdinal(name));

        private enum Status
        {
            Open,
            ClosedImplicitly,
            ClosedExplicitly
        }
    }
}

