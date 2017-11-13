namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections;

    internal interface IObjectReader : IEnumerator, IDisposable
    {
        IObjectReaderSession Session { get; }
    }
}

