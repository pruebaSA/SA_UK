namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ISingleResult<T> : IEnumerable<T>, IEnumerable, IFunctionResult, IDisposable
    {
    }
}

