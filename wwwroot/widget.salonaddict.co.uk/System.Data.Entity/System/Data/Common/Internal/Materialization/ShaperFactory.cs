namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Data.Common.QueryCache;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.Internal;
    using System.Data.Query.InternalTrees;

    internal abstract class ShaperFactory
    {
        protected ShaperFactory()
        {
        }

        internal static ShaperFactory Create(Type elementType, QueryCacheManager cacheManager, ColumnMap columnMap, MetadataWorkspace metadata, SpanIndex spanInfo, MergeOption mergeOption, bool valueLayer)
        {
            ShaperFactoryCreator creator = (ShaperFactoryCreator) Activator.CreateInstance(typeof(TypedShaperFactoryCreator).MakeGenericType(new Type[] { elementType }));
            return creator.TypedCreate(cacheManager, columnMap, metadata, spanInfo, mergeOption, valueLayer);
        }

        private abstract class ShaperFactoryCreator
        {
            protected ShaperFactoryCreator()
            {
            }

            internal abstract ShaperFactory TypedCreate(QueryCacheManager cacheManager, ColumnMap columnMap, MetadataWorkspace metadata, SpanIndex spanInfo, MergeOption mergeOption, bool valueLayer);
        }

        private sealed class TypedShaperFactoryCreator<T> : ShaperFactory.ShaperFactoryCreator
        {
            internal override ShaperFactory TypedCreate(QueryCacheManager cacheManager, ColumnMap columnMap, MetadataWorkspace metadata, SpanIndex spanInfo, MergeOption mergeOption, bool valueLayer) => 
                Translator.TranslateColumnMap<T>(cacheManager, columnMap, metadata, spanInfo, mergeOption, valueLayer);
        }
    }
}

