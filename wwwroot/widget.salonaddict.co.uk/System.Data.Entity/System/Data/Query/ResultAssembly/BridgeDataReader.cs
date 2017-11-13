namespace System.Data.Query.ResultAssembly
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Internal.Materialization;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Query.InternalTrees;
    using System.Data.Query.PlanCompiler;
    using System.Reflection;

    internal sealed class BridgeDataReader : DbDataReader, IExtendedDataRecord, IDataRecord
    {
        private readonly bool _hasRows;
        private bool _isClosed;
        private readonly CoordinatorFactory<RecordState> CoordinatorFactory;
        private readonly BridgeDataRecord DataRecord;
        private readonly RecordState DefaultRecordState;
        private readonly Shaper<RecordState> Shaper;

        internal BridgeDataReader(Shaper<RecordState> shaper, CoordinatorFactory<RecordState> coordinatorFactory, int depth)
        {
            this.Shaper = shaper;
            this.CoordinatorFactory = coordinatorFactory;
            this.DataRecord = new BridgeDataRecord(shaper, depth);
            this._hasRows = false;
            if (!this.Shaper.DataWaiting)
            {
                this.Shaper.DataWaiting = this.Shaper.RootEnumerator.MoveNext();
            }
            if (this.Shaper.DataWaiting)
            {
                RecordState current = this.Shaper.RootEnumerator.Current;
                if (current != null)
                {
                    this._hasRows = current.CoordinatorFactory == this.CoordinatorFactory;
                }
            }
            this.DefaultRecordState = coordinatorFactory.GetDefaultRecordState(this.Shaper);
        }

        private void AssertReaderIsOpen(string methodName)
        {
            if (this.IsClosed)
            {
                if (this.DataRecord.IsImplicitlyClosed)
                {
                    throw EntityUtil.ImplicitlyClosedDataReaderError();
                }
                if (this.DataRecord.IsExplicitlyClosed)
                {
                    throw EntityUtil.DataReaderClosed(methodName);
                }
            }
        }

        public override void Close()
        {
            this.DataRecord.CloseExplicitly();
            if (!this._isClosed)
            {
                this._isClosed = true;
                if (this.DataRecord.Depth == 0)
                {
                    this.Shaper.Reader.Close();
                }
                else
                {
                    this.Consume();
                }
            }
        }

        internal void CloseImplicitly()
        {
            this.Consume();
            this.DataRecord.CloseImplicitly();
        }

        private void Consume()
        {
            while (this.ReadInternal())
            {
            }
        }

        internal static DbDataReader Create(DbDataReader storeDataReader, ColumnMap columnMap, MetadataWorkspace workspace)
        {
            Shaper<RecordState> shaper = Translator.TranslateColumnMap<RecordState>(workspace.GetQueryCacheManager(), columnMap, workspace, null, MergeOption.NoTracking, true).Create(storeDataReader, workspace);
            return new BridgeDataReader(shaper, shaper.RootCoordinator.TypedCoordinatorFactory, 0);
        }

        public override bool GetBoolean(int ordinal) => 
            this.DataRecord.GetBoolean(ordinal);

        public override byte GetByte(int ordinal) => 
            this.DataRecord.GetByte(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => 
            this.DataRecord.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

        public override char GetChar(int ordinal) => 
            this.DataRecord.GetChar(ordinal);

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => 
            this.DataRecord.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

        internal static Type GetClrTypeFromTypeMetadata(TypeUsage typeUsage)
        {
            PrimitiveType type2;
            if (TypeHelpers.TryGetEdmType<PrimitiveType>(typeUsage, out type2))
            {
                return type2.ClrEquivalentType;
            }
            if (TypeSemantics.IsReferenceType(typeUsage))
            {
                return typeof(EntityKey);
            }
            if (TypeUtils.IsStructuredType(typeUsage))
            {
                return typeof(DbDataRecord);
            }
            if (TypeUtils.IsCollectionType(typeUsage))
            {
                return typeof(DbDataReader);
            }
            return typeof(object);
        }

        public DbDataReader GetDataReader(int ordinal) => 
            this.GetDbDataReader(ordinal);

        public DbDataRecord GetDataRecord(int ordinal) => 
            this.DataRecord.GetDataRecord(ordinal);

        public override string GetDataTypeName(int ordinal)
        {
            this.AssertReaderIsOpen("GetDataTypeName");
            if (this.DataRecord.HasData)
            {
                return this.DataRecord.GetDataTypeName(ordinal);
            }
            return TypeHelpers.GetFullName(this.DefaultRecordState.GetTypeUsage(ordinal));
        }

        public override DateTime GetDateTime(int ordinal) => 
            this.DataRecord.GetDateTime(ordinal);

        protected override DbDataReader GetDbDataReader(int ordinal) => 
            ((DbDataReader) this.DataRecord.GetData(ordinal));

        public override decimal GetDecimal(int ordinal) => 
            this.DataRecord.GetDecimal(ordinal);

        public override double GetDouble(int ordinal) => 
            this.DataRecord.GetDouble(ordinal);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IEnumerator GetEnumerator() => 
            new DbEnumerator(this, true);

        public override Type GetFieldType(int ordinal)
        {
            this.AssertReaderIsOpen("GetFieldType");
            if (this.DataRecord.HasData)
            {
                return this.DataRecord.GetFieldType(ordinal);
            }
            return GetClrTypeFromTypeMetadata(this.DefaultRecordState.GetTypeUsage(ordinal));
        }

        public override float GetFloat(int ordinal) => 
            this.DataRecord.GetFloat(ordinal);

        public override Guid GetGuid(int ordinal) => 
            this.DataRecord.GetGuid(ordinal);

        public override short GetInt16(int ordinal) => 
            this.DataRecord.GetInt16(ordinal);

        public override int GetInt32(int ordinal) => 
            this.DataRecord.GetInt32(ordinal);

        public override long GetInt64(int ordinal) => 
            this.DataRecord.GetInt64(ordinal);

        public override string GetName(int ordinal)
        {
            this.AssertReaderIsOpen("GetName");
            if (this.DataRecord.HasData)
            {
                return this.DataRecord.GetName(ordinal);
            }
            return this.DefaultRecordState.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            this.AssertReaderIsOpen("GetOrdinal");
            if (this.DataRecord.HasData)
            {
                return this.DataRecord.GetOrdinal(name);
            }
            return this.DefaultRecordState.GetOrdinal(name);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            throw EntityUtil.NotSupported();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object GetProviderSpecificValue(int ordinal)
        {
            throw EntityUtil.NotSupported();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetProviderSpecificValues(object[] values)
        {
            throw EntityUtil.NotSupported();
        }

        public override DataTable GetSchemaTable()
        {
            throw EntityUtil.NotSupported(Strings.ADP_GetSchemaTableIsNotSupported);
        }

        public override string GetString(int ordinal) => 
            this.DataRecord.GetString(ordinal);

        public override object GetValue(int ordinal) => 
            this.DataRecord.GetValue(ordinal);

        public override int GetValues(object[] values) => 
            this.DataRecord.GetValues(values);

        public override bool IsDBNull(int ordinal) => 
            this.DataRecord.IsDBNull(ordinal);

        public override bool NextResult()
        {
            this.AssertReaderIsOpen("NextResult");
            this.CloseImplicitly();
            if (this.DataRecord.Depth == 0)
            {
                CommandHelper.ConsumeReader(this.Shaper.Reader);
            }
            else
            {
                this.Consume();
            }
            this.DataRecord.SetRecordSource(null, false);
            return false;
        }

        public override bool Read()
        {
            this.AssertReaderIsOpen("Read");
            this.DataRecord.CloseImplicitly();
            bool hasData = this.ReadInternal();
            this.DataRecord.SetRecordSource(this.Shaper.RootEnumerator.Current, hasData);
            return hasData;
        }

        private bool ReadInternal()
        {
            bool flag = false;
            if (!this.Shaper.DataWaiting)
            {
                this.Shaper.DataWaiting = this.Shaper.RootEnumerator.MoveNext();
            }
            while ((this.Shaper.DataWaiting && (this.Shaper.RootEnumerator.Current.CoordinatorFactory != this.CoordinatorFactory)) && (this.Shaper.RootEnumerator.Current.CoordinatorFactory.Depth > this.CoordinatorFactory.Depth))
            {
                this.Shaper.DataWaiting = this.Shaper.RootEnumerator.MoveNext();
            }
            if (this.Shaper.DataWaiting && (this.Shaper.RootEnumerator.Current.CoordinatorFactory == this.CoordinatorFactory))
            {
                this.Shaper.DataWaiting = false;
                this.Shaper.RootEnumerator.Current.AcceptPendingValues();
                flag = true;
            }
            return flag;
        }

        public System.Data.Common.DataRecordInfo DataRecordInfo
        {
            get
            {
                this.AssertReaderIsOpen("DataRecordInfo");
                if (this.DataRecord.HasData)
                {
                    return this.DataRecord.DataRecordInfo;
                }
                return this.DefaultRecordState.DataRecordInfo;
            }
        }

        public override int Depth
        {
            get
            {
                this.AssertReaderIsOpen("Depth");
                return this.DataRecord.Depth;
            }
        }

        public override int FieldCount
        {
            get
            {
                this.AssertReaderIsOpen("FieldCount");
                return this.DefaultRecordState.ColumnCount;
            }
        }

        public override bool HasRows
        {
            get
            {
                this.AssertReaderIsOpen("HasRows");
                return this._hasRows;
            }
        }

        public override bool IsClosed
        {
            get
            {
                if (!this._isClosed)
                {
                    return this.DataRecord.IsClosed;
                }
                return true;
            }
        }

        public override object this[int ordinal] =>
            this.DataRecord[ordinal];

        public override object this[string name]
        {
            get
            {
                int ordinal = this.GetOrdinal(name);
                return this.DataRecord[ordinal];
            }
        }

        public override int RecordsAffected
        {
            get
            {
                int recordsAffected = -1;
                if (this.DataRecord.Depth == 0)
                {
                    recordsAffected = this.Shaper.Reader.RecordsAffected;
                }
                return recordsAffected;
            }
        }
    }
}

