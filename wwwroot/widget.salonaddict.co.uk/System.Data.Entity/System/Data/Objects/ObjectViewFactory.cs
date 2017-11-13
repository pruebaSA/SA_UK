namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Reflection;

    internal static class ObjectViewFactory
    {
        private static readonly Type genericObjectViewDataInterfaceType = typeof(IObjectViewData<>);
        private static readonly Type genericObjectViewEntityCollectionDataType = typeof(ObjectViewEntityCollectionData<,>);
        private static readonly Type genericObjectViewQueryResultDataType = typeof(ObjectViewQueryResultData<>);
        private static readonly Type genericObjectViewType = typeof(ObjectView<>);

        private static IBindingList CreateObjectView(Type clrElementType, Type objectViewDataType, object viewData, object eventDataSource)
        {
            Type type = genericObjectViewType.MakeGenericType(new Type[] { clrElementType });
            Type[] typeArray = objectViewDataType.FindInterfaces((type, unusedFilter) => type.Name == genericObjectViewDataInterfaceType.Name, null);
            return (IBindingList) type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeArray[0], typeof(object) }, null).Invoke(new object[] { viewData, eventDataSource });
        }

        internal static IBindingList CreateViewForEntityCollection<TElement>(EntityType entityType, EntityCollection<TElement> entityCollection) where TElement: class, IEntityWithRelationships
        {
            Type c = null;
            TypeUsage typeUsage = (entityType == null) ? null : TypeUsage.Create(entityType);
            TypeUsage oSpaceTypeUsage = GetOSpaceTypeUsage(typeUsage, entityCollection.ObjectContext);
            if (oSpaceTypeUsage == null)
            {
                c = typeof(TElement);
            }
            else
            {
                c = GetClrType<TElement>(oSpaceTypeUsage.EdmType);
                if (c == null)
                {
                    c = typeof(TElement);
                }
            }
            if (c == typeof(TElement))
            {
                return new ObjectView<TElement>(new ObjectViewEntityCollectionData<TElement, TElement>(entityCollection), entityCollection);
            }
            if (!typeof(TElement).IsAssignableFrom(c))
            {
                throw EntityUtil.ValueInvalidCast(c, typeof(TElement));
            }
            Type objectViewDataType = genericObjectViewEntityCollectionDataType.MakeGenericType(new Type[] { c, typeof(TElement) });
            object viewData = objectViewDataType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(EntityCollection<TElement>) }, null).Invoke(new object[] { entityCollection });
            return CreateObjectView(c, objectViewDataType, viewData, entityCollection);
        }

        internal static IBindingList CreateViewForQuery<TElement>(TypeUsage elementEdmTypeUsage, IEnumerable<TElement> queryResults, ObjectContext objectContext, bool forceReadOnly, EntitySet singleEntitySet)
        {
            EntityUtil.CheckArgumentNull<IEnumerable<TElement>>(queryResults, "queryResults");
            EntityUtil.CheckArgumentNull<ObjectContext>(objectContext, "objectContext");
            Type c = null;
            TypeUsage oSpaceTypeUsage = GetOSpaceTypeUsage(elementEdmTypeUsage, objectContext);
            if (oSpaceTypeUsage == null)
            {
                c = typeof(TElement);
            }
            c = GetClrType<TElement>(oSpaceTypeUsage.EdmType);
            object objectStateManager = objectContext.ObjectStateManager;
            if (c == typeof(TElement))
            {
                return new ObjectView<TElement>(new ObjectViewQueryResultData<TElement>(queryResults, objectContext, forceReadOnly, singleEntitySet), objectStateManager);
            }
            if (c == null)
            {
                return new DataRecordObjectView(new ObjectViewQueryResultData<DbDataRecord>(queryResults, objectContext, true, null), objectStateManager, (RowType) oSpaceTypeUsage.EdmType, typeof(TElement));
            }
            if (!typeof(TElement).IsAssignableFrom(c))
            {
                throw EntityUtil.ValueInvalidCast(c, typeof(TElement));
            }
            Type objectViewDataType = genericObjectViewQueryResultDataType.MakeGenericType(new Type[] { c });
            object viewData = objectViewDataType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(IEnumerable), typeof(ObjectContext), typeof(bool), typeof(EntitySet) }, null).Invoke(new object[] { queryResults, objectContext, forceReadOnly, singleEntitySet });
            return CreateObjectView(c, objectViewDataType, viewData, objectStateManager);
        }

        private static Type GetClrType<TElement>(EdmType ospaceEdmType)
        {
            if (ospaceEdmType.BuiltInTypeKind == BuiltInTypeKind.RowType)
            {
                RowType type2 = (RowType) ospaceEdmType;
                if ((type2.InitializerMetadata != null) && (type2.InitializerMetadata.ClrType != null))
                {
                    return type2.InitializerMetadata.ClrType;
                }
                Type c = typeof(TElement);
                if (typeof(IDataRecord).IsAssignableFrom(c) || (c == typeof(object)))
                {
                    return null;
                }
                return typeof(TElement);
            }
            Type clrType = ospaceEdmType.ClrType;
            if (clrType == null)
            {
                clrType = typeof(TElement);
            }
            return clrType;
        }

        private static TypeUsage GetOSpaceTypeUsage(TypeUsage typeUsage, ObjectContext objectContext)
        {
            if ((typeUsage == null) || (typeUsage.EdmType == null))
            {
                return null;
            }
            if (typeUsage.EdmType.DataSpace == DataSpace.OSpace)
            {
                return typeUsage;
            }
            if (objectContext == null)
            {
                return null;
            }
            objectContext.EnsureMetadata();
            return objectContext.Perspective.MetadataWorkspace.GetOSpaceTypeUsage(typeUsage);
        }
    }
}

