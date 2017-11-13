namespace System.Data.SqlClient
{
    using System;

    internal class SqlNotification : MarshalByRefObject
    {
        private readonly SqlNotificationInfo _info;
        private readonly string _key;
        private readonly SqlNotificationSource _source;
        private readonly SqlNotificationType _type;

        internal SqlNotification(SqlNotificationInfo info, SqlNotificationSource source, SqlNotificationType type, string key)
        {
            this._info = info;
            this._source = source;
            this._type = type;
            this._key = key;
        }

        internal SqlNotificationInfo Info =>
            this._info;

        internal string Key =>
            this._key;

        internal SqlNotificationSource Source =>
            this._source;

        internal SqlNotificationType Type =>
            this._type;
    }
}

