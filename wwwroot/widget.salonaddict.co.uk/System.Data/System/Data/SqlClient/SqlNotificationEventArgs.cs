﻿namespace System.Data.SqlClient
{
    using System;

    public class SqlNotificationEventArgs : EventArgs
    {
        private SqlNotificationInfo _info;
        private SqlNotificationSource _source;
        private SqlNotificationType _type;
        internal static SqlNotificationEventArgs NotifyError = new SqlNotificationEventArgs(SqlNotificationType.Subscribe, SqlNotificationInfo.Error, SqlNotificationSource.Object);

        public SqlNotificationEventArgs(SqlNotificationType type, SqlNotificationInfo info, SqlNotificationSource source)
        {
            this._info = info;
            this._source = source;
            this._type = type;
        }

        public SqlNotificationInfo Info =>
            this._info;

        public SqlNotificationSource Source =>
            this._source;

        public SqlNotificationType Type =>
            this._type;
    }
}

