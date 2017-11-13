namespace System.Data.Linq.Provider
{
    using System;
    using System.Collections;

    internal interface IDeferredSourceFactory
    {
        IEnumerable CreateDeferredSource(object instance);
        IEnumerable CreateDeferredSource(object[] keyValues);
    }
}

