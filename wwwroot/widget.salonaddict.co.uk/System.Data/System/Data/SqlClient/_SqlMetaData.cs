﻿namespace System.Data.SqlClient
{
    using System;
    using System.Data;

    internal sealed class _SqlMetaData : SqlMetaDataPriv
    {
        internal string baseColumn;
        internal string column;
        internal bool isColumnSet;
        internal bool isDifferentName;
        internal bool isExpression;
        internal bool isHidden;
        internal bool isIdentity;
        internal bool isKey;
        internal MultiPartTableName multiPartTableName;
        internal byte op;
        internal ushort operand;
        internal readonly int ordinal;
        internal byte tableNum;
        internal byte updatability;

        internal _SqlMetaData(int ordinal)
        {
            this.ordinal = ordinal;
        }

        internal string catalogName =>
            this.multiPartTableName.CatalogName;

        internal bool IsLargeUdt =>
            ((base.type == SqlDbType.Udt) && (base.length == 0x7fffffff));

        internal bool IsNewKatmaiDateTimeType
        {
            get
            {
                if (((SqlDbType.Date != base.type) && (SqlDbType.Time != base.type)) && (SqlDbType.DateTime2 != base.type))
                {
                    return (SqlDbType.DateTimeOffset == base.type);
                }
                return true;
            }
        }

        internal string schemaName =>
            this.multiPartTableName.SchemaName;

        internal string serverName =>
            this.multiPartTableName.ServerName;

        internal string tableName =>
            this.multiPartTableName.TableName;
    }
}

