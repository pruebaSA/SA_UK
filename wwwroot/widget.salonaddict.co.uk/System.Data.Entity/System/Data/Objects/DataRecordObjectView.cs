namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Reflection;

    internal sealed class DataRecordObjectView : ObjectView<DbDataRecord>, ITypedList
    {
        private PropertyDescriptorCollection _propertyDescriptorsCache;
        private RowType _rowType;

        internal DataRecordObjectView(IObjectViewData<DbDataRecord> viewData, object eventDataSource, RowType rowType, Type propertyComponentType) : base(viewData, eventDataSource)
        {
            if (!typeof(IDataRecord).IsAssignableFrom(propertyComponentType))
            {
                propertyComponentType = typeof(IDataRecord);
            }
            this._rowType = rowType;
            this._propertyDescriptorsCache = MaterializedDataRecord.CreatePropertyDescriptorCollection(this._rowType, propertyComponentType, true);
        }

        private static Type GetListItemType(Type type)
        {
            if (typeof(Array).IsAssignableFrom(type))
            {
                return type.GetElementType();
            }
            PropertyInfo typedIndexer = GetTypedIndexer(type);
            if (typedIndexer != null)
            {
                return typedIndexer.PropertyType;
            }
            return type;
        }

        private static PropertyInfo GetTypedIndexer(Type type)
        {
            PropertyInfo info = null;
            if ((typeof(IList).IsAssignableFrom(type) || typeof(ITypedList).IsAssignableFrom(type)) || typeof(IListSource).IsAssignableFrom(type))
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < properties.Length; i++)
                {
                    if ((properties[i].GetIndexParameters().Length > 0) && (properties[i].PropertyType != typeof(object)))
                    {
                        info = properties[i];
                        if (info.Name == "Item")
                        {
                            return info;
                        }
                    }
                }
            }
            return info;
        }

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if ((listAccessors == null) || (listAccessors.Length == 0))
            {
                return this._propertyDescriptorsCache;
            }
            PropertyDescriptor descriptor = listAccessors[listAccessors.Length - 1];
            FieldDescriptor descriptor2 = descriptor as FieldDescriptor;
            if (((descriptor2 != null) && (descriptor2.EdmProperty != null)) && (descriptor2.EdmProperty.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.RowType))
            {
                return MaterializedDataRecord.CreatePropertyDescriptorCollection((RowType) descriptor2.EdmProperty.TypeUsage.EdmType, typeof(IDataRecord), true);
            }
            return TypeDescriptor.GetProperties(GetListItemType(descriptor.PropertyType));
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors) => 
            this._rowType.Name;
    }
}

