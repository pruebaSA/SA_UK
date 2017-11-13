namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    internal class CompensatingCollection<TElement> : IOrderedQueryable<TElement>, IQueryable<TElement>, IOrderedQueryable, IQueryable, IOrderedEnumerable<TElement>, IEnumerable<TElement>, IEnumerable
    {
        private readonly Expression _expression;
        private readonly IEnumerable<TElement> _source;

        public CompensatingCollection(IEnumerable<TElement> source)
        {
            this._source = EntityUtil.CheckArgumentNull<IEnumerable<TElement>>(source, "source");
            this._expression = Expression.Constant(source);
        }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => 
            this._source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this._source.GetEnumerator();

        IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<K>(Func<TElement, K> keySelector, IComparer<K> comparer, bool descending)
        {
            throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_CreateOrderedEnumerableNotSupported);
        }

        Type IQueryable.ElementType =>
            typeof(TElement);

        Expression IQueryable.Expression =>
            this._expression;

        IQueryProvider IQueryable.Provider
        {
            get
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedQueryableMethod);
            }
        }
    }
}

