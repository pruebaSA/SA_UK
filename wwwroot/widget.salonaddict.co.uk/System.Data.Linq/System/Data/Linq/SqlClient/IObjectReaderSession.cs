namespace System.Data.Linq.SqlClient
{
    using System;

    internal interface IObjectReaderSession : IConnectionUser, IDisposable
    {
        void Buffer();

        bool IsBuffered { get; }
    }
}

