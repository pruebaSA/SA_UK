namespace System.Linq
{
    using System.Collections;
    using System.Collections.Generic;

    public interface IOrderedQueryable<T> : IQueryable<T>, IEnumerable<T>, IOrderedQueryable, IQueryable, IEnumerable
    {
    }
}

