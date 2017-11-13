namespace System.Data
{
    using System;
    using System.Data.Common;

    public interface IExtendedDataRecord : IDataRecord
    {
        DbDataReader GetDataReader(int i);
        DbDataRecord GetDataRecord(int i);

        System.Data.Common.DataRecordInfo DataRecordInfo { get; }
    }
}

