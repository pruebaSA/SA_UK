namespace System.Data.Linq.Provider
{
    using System;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal interface IDataServices
    {
        object GetCachedObject(Expression query);
        IDeferredSourceFactory GetDeferredSourceFactory(MetaDataMember member);
        object InsertLookupCachedObject(MetaType type, object instance);
        bool IsCachedObject(MetaType type, object instance);
        void OnEntityMaterialized(MetaType type, object instance);

        DataContext Context { get; }

        MetaModel Model { get; }
    }
}

