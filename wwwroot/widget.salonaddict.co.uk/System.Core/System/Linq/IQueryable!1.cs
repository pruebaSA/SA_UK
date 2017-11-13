namespace System.Linq
{
    using System.Collections;
    using System.Collections.Generic;

    public interface IQueryable<T> : IEnumerable<T>, IQueryable, IEnumerable
    {
    }
}

