namespace System.Web.UI.WebControls
{
    using System;
    using System.Linq;

    internal interface IDynamicQueryable
    {
        int Count(IQueryable source);
        IQueryable GroupBy(IQueryable source, string keySelector, string elementSelector, params object[] values);
        IQueryable OrderBy(IQueryable source, string ordering, params object[] values);
        IQueryable Select(IQueryable source, string selector, params object[] values);
        IQueryable Skip(IQueryable source, int count);
        IQueryable Take(IQueryable source, int count);
        IQueryable Where(IQueryable source, string predicate, params object[] values);
    }
}

