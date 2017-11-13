namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;

    internal class RecordState
    {
        private EntityRecordInfo _currentEntityRecordInfo;
        private bool _currentIsNull;
        private EntityRecordInfo _pendingEntityRecordInfo;
        private bool _pendingIsNull;
        internal readonly System.Data.Common.Internal.Materialization.CoordinatorFactory CoordinatorFactory;
        internal object[] CurrentColumnValues;
        internal object[] PendingColumnValues;
        private readonly System.Data.Common.Internal.Materialization.RecordStateFactory RecordStateFactory;

        internal RecordState(System.Data.Common.Internal.Materialization.RecordStateFactory recordStateFactory, System.Data.Common.Internal.Materialization.CoordinatorFactory coordinatorFactory)
        {
            this.RecordStateFactory = recordStateFactory;
            this.CoordinatorFactory = coordinatorFactory;
            this.CurrentColumnValues = new object[this.RecordStateFactory.ColumnCount];
            this.PendingColumnValues = new object[this.RecordStateFactory.ColumnCount];
        }

        internal void AcceptPendingValues()
        {
            object[] currentColumnValues = this.CurrentColumnValues;
            this.CurrentColumnValues = this.PendingColumnValues;
            this.PendingColumnValues = currentColumnValues;
            this._currentEntityRecordInfo = this._pendingEntityRecordInfo;
            this._pendingEntityRecordInfo = null;
            this._currentIsNull = this._pendingIsNull;
            if (this.RecordStateFactory.HasNestedColumns)
            {
                for (int i = 0; i < this.CurrentColumnValues.Length; i++)
                {
                    if (this.RecordStateFactory.IsColumnNested[i])
                    {
                        RecordState state = this.CurrentColumnValues[i] as RecordState;
                        if (state != null)
                        {
                            state.AcceptPendingValues();
                        }
                    }
                }
            }
        }

        internal RecordState GatherData(Shaper shaper)
        {
            this.RecordStateFactory.GatherData(shaper);
            this._pendingIsNull = false;
            return this;
        }

        internal long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            byte[] src = (byte[]) this.CurrentColumnValues[ordinal];
            int num = src.Length;
            int srcOffset = (int) dataOffset;
            int num3 = num - srcOffset;
            if (buffer != null)
            {
                num3 = Math.Min(num3, length);
                if (0 < num3)
                {
                    Buffer.BlockCopy(src, srcOffset, buffer, bufferOffset, num3);
                }
            }
            return (long) Math.Max(0, num3);
        }

        internal long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            char[] chArray;
            string str = this.CurrentColumnValues[ordinal] as string;
            if (str != null)
            {
                chArray = str.ToCharArray();
            }
            else
            {
                chArray = (char[]) this.CurrentColumnValues[ordinal];
            }
            int num = chArray.Length;
            int num2 = (int) dataOffset;
            int num3 = num - num2;
            if (buffer != null)
            {
                num3 = Math.Min(num3, length);
                if (0 < num3)
                {
                    Buffer.BlockCopy(chArray, num2 * 2, buffer, bufferOffset * 2, num3 * 2);
                }
            }
            return (long) Math.Max(0, num3);
        }

        internal string GetName(int ordinal)
        {
            if ((ordinal < 0) || (ordinal >= this.RecordStateFactory.ColumnCount))
            {
                throw EntityUtil.ArgumentOutOfRange("ordinal");
            }
            return this.RecordStateFactory.ColumnNames[ordinal];
        }

        internal int GetOrdinal(string name) => 
            this.RecordStateFactory.FieldNameLookup.GetOrdinal(name);

        internal TypeUsage GetTypeUsage(int ordinal) => 
            this.RecordStateFactory.TypeUsages[ordinal];

        internal bool IsNestedObject(int ordinal) => 
            this.RecordStateFactory.IsColumnNested[ordinal];

        internal void ResetToDefaultState()
        {
            this._currentEntityRecordInfo = null;
        }

        internal bool SetColumnValue(int ordinal, object value)
        {
            this.PendingColumnValues[ordinal] = value;
            return true;
        }

        internal bool SetEntityRecordInfo(EntityKey entityKey, EntitySet entitySet)
        {
            this._pendingEntityRecordInfo = new EntityRecordInfo(this.RecordStateFactory.DataRecordInfo, entityKey, entitySet);
            return true;
        }

        internal RecordState SetNullRecord(Shaper shaper)
        {
            for (int i = 0; i < this.PendingColumnValues.Length; i++)
            {
                this.PendingColumnValues[i] = DBNull.Value;
            }
            this._pendingEntityRecordInfo = null;
            this._pendingIsNull = true;
            return this;
        }

        internal int ColumnCount =>
            this.RecordStateFactory.ColumnCount;

        internal System.Data.Common.DataRecordInfo DataRecordInfo
        {
            get
            {
                System.Data.Common.DataRecordInfo dataRecordInfo = this._currentEntityRecordInfo;
                if (dataRecordInfo == null)
                {
                    dataRecordInfo = this.RecordStateFactory.DataRecordInfo;
                }
                return dataRecordInfo;
            }
        }

        internal bool IsNull =>
            this._currentIsNull;
    }
}

