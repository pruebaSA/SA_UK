namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq.Provider;

    internal interface IReaderProvider : IProvider, IDisposable
    {
        IConnectionManager ConnectionManager { get; }

        IDataServices Services { get; }
    }
}

