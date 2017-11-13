namespace System.Data.Services.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal static class AtomMaterializerInvoker
    {
        internal static object DirectMaterializePlan(object materializer, object entry, Type expectedEntryType) => 
            AtomMaterializer.DirectMaterializePlan((AtomMaterializer) materializer, (AtomEntry) entry, expectedEntryType);

        internal static IEnumerable<T> EnumerateAsElementType<T>(IEnumerable source) => 
            AtomMaterializer.EnumerateAsElementType<T>(source);

        internal static List<TTarget> ListAsElementType<T, TTarget>(object materializer, IEnumerable<T> source) where T: TTarget => 
            AtomMaterializer.ListAsElementType<T, TTarget>((AtomMaterializer) materializer, source);

        internal static bool ProjectionCheckValueForPathIsNull(object entry, Type expectedType, object path) => 
            AtomMaterializer.ProjectionCheckValueForPathIsNull((AtomEntry) entry, expectedType, (ProjectionPath) path);

        internal static AtomEntry ProjectionGetEntry(object entry, string name) => 
            AtomMaterializer.ProjectionGetEntry((AtomEntry) entry, name);

        internal static object ProjectionInitializeEntity(object materializer, object entry, Type expectedType, Type resultType, string[] properties, Func<object, object, Type, object>[] propertyValues) => 
            AtomMaterializer.ProjectionInitializeEntity((AtomMaterializer) materializer, (AtomEntry) entry, expectedType, resultType, properties, propertyValues);

        internal static IEnumerable ProjectionSelect(object materializer, object entry, Type expectedType, Type resultType, object path, Func<object, object, Type, object> selector) => 
            AtomMaterializer.ProjectionSelect((AtomMaterializer) materializer, (AtomEntry) entry, expectedType, resultType, (ProjectionPath) path, selector);

        internal static object ProjectionValueForPath(object materializer, object entry, Type expectedType, object path) => 
            AtomMaterializer.ProjectionValueForPath((AtomMaterializer) materializer, (AtomEntry) entry, expectedType, (ProjectionPath) path);

        internal static object ShallowMaterializePlan(object materializer, object entry, Type expectedEntryType) => 
            AtomMaterializer.ShallowMaterializePlan((AtomMaterializer) materializer, (AtomEntry) entry, expectedEntryType);
    }
}

