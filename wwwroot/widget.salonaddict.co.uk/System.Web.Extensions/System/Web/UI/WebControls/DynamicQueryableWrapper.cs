namespace System.Web.UI.WebControls
{
    using System;
    using System.Linq;
    using System.Web.Query.Dynamic;

    internal class DynamicQueryableWrapper : IDynamicQueryable
    {
        public int Count(IQueryable source) => 
            source.Count();

        public IQueryable GroupBy(IQueryable source, string keySelector, string elementSelector, params object[] values) => 
            source.GroupBy(keySelector, elementSelector, values);

        public IQueryable OrderBy(IQueryable source, string ordering, params object[] values) => 
            source.OrderBy(ordering, values);

        public IQueryable Select(IQueryable source, string selector, params object[] values) => 
            source.Select(selector, values);

        public IQueryable Skip(IQueryable source, int count) => 
            source.Skip(count);

        public IQueryable Take(IQueryable source, int count) => 
            source.Take(count);

        public IQueryable Where(IQueryable source, string predicate, params object[] values) => 
            source.Where(predicate, values);
    }
}

