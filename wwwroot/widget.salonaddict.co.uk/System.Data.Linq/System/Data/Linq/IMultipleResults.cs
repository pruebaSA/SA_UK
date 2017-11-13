namespace System.Data.Linq
{
    using System;
    using System.Collections.Generic;

    public interface IMultipleResults : IFunctionResult, IDisposable
    {
        IEnumerable<TElement> GetResult<TElement>();
    }
}

