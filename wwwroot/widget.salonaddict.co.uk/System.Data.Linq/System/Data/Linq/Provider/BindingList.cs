namespace System.Data.Linq.Provider
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Linq;
    using System.Reflection;

    internal static class BindingList
    {
        internal static IBindingList Create<T>(DataContext context, IEnumerable<T> sequence)
        {
            List<T> list = sequence.ToList<T>();
            MetaTable table = context.Services.Model.GetTable(typeof(T));
            if (table != null)
            {
                ITable table2 = context.GetTable(table.RowType.Type);
                return (IBindingList) Activator.CreateInstance(typeof(DataBindingList<>).MakeGenericType(new Type[] { table.RowType.Type }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { list, table2 }, null);
            }
            return new SortableBindingList<T>(list);
        }
    }
}

