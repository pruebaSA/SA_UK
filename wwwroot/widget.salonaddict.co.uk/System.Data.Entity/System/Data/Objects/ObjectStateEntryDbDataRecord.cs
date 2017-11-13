namespace System.Data.Objects
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Reflection;

    internal sealed class ObjectStateEntryDbDataRecord : DbDataRecord, IExtendedDataRecord, IDataRecord
    {
        private readonly ObjectStateEntry _cacheEntry;
        private readonly StateManagerTypeMetadata _metadata;
        private System.Data.Common.DataRecordInfo _recordInfo;
        private readonly object _userObject;

        internal ObjectStateEntryDbDataRecord(ObjectStateEntry cacheEntry)
        {
            EntityUtil.CheckArgumentNull<ObjectStateEntry>(cacheEntry, "cacheEntry");
            EntityState state = cacheEntry.State;
            if (((state != EntityState.Unchanged) && (state != EntityState.Deleted)) && (state != EntityState.Modified))
            {
                throw EntityUtil.CannotCreateObjectStateEntryDbDataRecord();
            }
            this._cacheEntry = cacheEntry;
        }

        internal ObjectStateEntryDbDataRecord(ObjectStateEntry cacheEntry, StateManagerTypeMetadata metadata, object userObject)
        {
            EntityUtil.CheckArgumentNull<ObjectStateEntry>(cacheEntry, "cacheEntry");
            EntityUtil.CheckArgumentNull<object>(userObject, "userObject");
            EntityUtil.CheckArgumentNull<StateManagerTypeMetadata>(metadata, "metadata");
            EntityState state = cacheEntry.State;
            if (((state != EntityState.Unchanged) && (state != EntityState.Deleted)) && (state != EntityState.Modified))
            {
                throw EntityUtil.CannotCreateObjectStateEntryDbDataRecord();
            }
            this._cacheEntry = cacheEntry;
            this._userObject = userObject;
            this._metadata = metadata;
        }

        public override bool GetBoolean(int ordinal) => 
            ((bool) this.GetValue(ordinal));

        public override byte GetByte(int ordinal) => 
            ((byte) this.GetValue(ordinal));

        public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
        {
            byte[] sourceArray = (byte[]) this.GetValue(ordinal);
            if (buffer == null)
            {
                return (long) sourceArray.Length;
            }
            int num = (int) dataIndex;
            int num2 = Math.Min(sourceArray.Length - num, length);
            if (num < 0)
            {
                throw EntityUtil.InvalidSourceBufferIndex(sourceArray.Length, (long) num, "dataIndex");
            }
            if ((bufferIndex < 0) || ((bufferIndex > 0) && (bufferIndex >= buffer.Length)))
            {
                throw EntityUtil.InvalidDestinationBufferIndex(buffer.Length, bufferIndex, "bufferIndex");
            }
            if (0 < num2)
            {
                Array.Copy(sourceArray, dataIndex, buffer, (long) bufferIndex, (long) num2);
            }
            else
            {
                if (length < 0)
                {
                    throw EntityUtil.InvalidDataLength((long) length);
                }
                num2 = 0;
            }
            return (long) num2;
        }

        public override char GetChar(int ordinal) => 
            ((char) this.GetValue(ordinal));

        public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
        {
            char[] sourceArray = (char[]) this.GetValue(ordinal);
            if (buffer == null)
            {
                return (long) sourceArray.Length;
            }
            int num = (int) dataIndex;
            int num2 = Math.Min(sourceArray.Length - num, length);
            if (num < 0)
            {
                throw EntityUtil.InvalidSourceBufferIndex(buffer.Length, (long) bufferIndex, "bufferIndex");
            }
            if ((bufferIndex < 0) || ((bufferIndex > 0) && (bufferIndex >= buffer.Length)))
            {
                throw EntityUtil.InvalidDestinationBufferIndex(buffer.Length, bufferIndex, "bufferIndex");
            }
            if (0 < num2)
            {
                Array.Copy(sourceArray, dataIndex, buffer, (long) bufferIndex, (long) num2);
            }
            else
            {
                if (length < 0)
                {
                    throw EntityUtil.InvalidDataLength((long) length);
                }
                num2 = 0;
            }
            return (long) num2;
        }

        public DbDataReader GetDataReader(int i) => 
            this.GetDbDataReader(i);

        public DbDataRecord GetDataRecord(int ordinal) => 
            ((DbDataRecord) this.GetValue(ordinal));

        public override string GetDataTypeName(int ordinal) => 
            this.GetFieldType(ordinal).Name;

        public override DateTime GetDateTime(int ordinal) => 
            ((DateTime) this.GetValue(ordinal));

        protected override DbDataReader GetDbDataReader(int ordinal)
        {
            throw EntityUtil.NotSupported();
        }

        public override decimal GetDecimal(int ordinal) => 
            ((decimal) this.GetValue(ordinal));

        public override double GetDouble(int ordinal) => 
            ((double) this.GetValue(ordinal));

        public override Type GetFieldType(int ordinal) => 
            this._cacheEntry.GetFieldType(ordinal, this._metadata);

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

        public override string GetName(int ordinal) => 
            this._cacheEntry.GetCLayerName(ordinal, this._metadata);

        public override int GetOrdinal(string name)
        {
            int ordinalforCLayerName = this._cacheEntry.GetOrdinalforCLayerName(name, this._metadata);
            if (ordinalforCLayerName == -1)
            {
                throw EntityUtil.ArgumentOutOfRange("name");
            }
            return ordinalforCLayerName;
        }

        public override string GetString(int ordinal) => 
            ((string) this.GetValue(ordinal));

        public override object GetValue(int ordinal)
        {
            if (this._cacheEntry.IsRelationship)
            {
                return this._cacheEntry.GetOriginalRelationValue(ordinal);
            }
            return this._cacheEntry.GetOriginalEntityValue(this._metadata, ordinal, this._userObject, ObjectStateValueRecord.OriginalReadonly);
        }

        public override int GetValues(object[] values)
        {
            if (values == null)
            {
                throw EntityUtil.ArgumentNull("values");
            }
            int num = Math.Min(values.Length, this.FieldCount);
            for (int i = 0; i < num; i++)
            {
                values[i] = this.GetValue(i);
            }
            return num;
        }

        public override bool IsDBNull(int ordinal) => 
            (this.GetValue(ordinal) == DBNull.Value);

        public System.Data.Common.DataRecordInfo DataRecordInfo
        {
            get
            {
                if (this._recordInfo == null)
                {
                    this._recordInfo = this._cacheEntry.GetDataRecordInfo(this._metadata, this._userObject);
                }
                return this._recordInfo;
            }
        }

        public override int FieldCount =>
            this._cacheEntry.GetFieldCount(this._metadata);

        public override object this[int ordinal] =>
            this.GetValue(ordinal);

        public override object this[string name] =>
            this.GetValue(this.GetOrdinal(name));
    }
}

