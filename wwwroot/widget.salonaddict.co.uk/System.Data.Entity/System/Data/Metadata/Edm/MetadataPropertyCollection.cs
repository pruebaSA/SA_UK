namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal sealed class MetadataPropertyCollection : MetadataCollection<MetadataProperty>
    {
        private static readonly Memoizer<Type, ItemTypeInformation> s_itemTypeMemoizer = new Memoizer<Type, ItemTypeInformation>(clrType => new ItemTypeInformation(clrType), null);

        internal MetadataPropertyCollection(MetadataItem item) : base(GetSystemMetadataProperties(item))
        {
        }

        [CompilerGenerated]
        private static ItemTypeInformation <.cctor>b__0(Type clrType) => 
            new ItemTypeInformation(clrType);

        private static ItemTypeInformation GetItemTypeInformation(Type clrType) => 
            s_itemTypeMemoizer.Evaluate(clrType);

        private static IEnumerable<MetadataProperty> GetSystemMetadataProperties(MetadataItem item)
        {
            EntityUtil.CheckArgumentNull<MetadataItem>(item, "item");
            return GetItemTypeInformation(item.GetType()).GetItemAttributes(item);
        }

        private class ItemPropertyInfo
        {
            private readonly MetadataPropertyAttribute _attribute;
            private readonly PropertyInfo _propertyInfo;

            internal ItemPropertyInfo(PropertyInfo propertyInfo, MetadataPropertyAttribute attribute)
            {
                this._propertyInfo = propertyInfo;
                this._attribute = attribute;
            }

            internal MetadataProperty GetMetadataProperty(MetadataItem item) => 
                new MetadataProperty(this._propertyInfo.Name, this._attribute.Type, this._attribute.IsCollectionType, new MetadataPropertyValue(this._propertyInfo, item));
        }

        private class ItemTypeInformation
        {
            private readonly List<MetadataPropertyCollection.ItemPropertyInfo> _itemProperties;

            internal ItemTypeInformation(Type clrType)
            {
                this._itemProperties = GetItemProperties(clrType);
            }

            internal IEnumerable<MetadataProperty> GetItemAttributes(MetadataItem item)
            {
                foreach (MetadataPropertyCollection.ItemPropertyInfo iteratorVariable0 in this._itemProperties)
                {
                    yield return iteratorVariable0.GetMetadataProperty(item);
                }
            }

            private static List<MetadataPropertyCollection.ItemPropertyInfo> GetItemProperties(Type clrType)
            {
                List<MetadataPropertyCollection.ItemPropertyInfo> list = new List<MetadataPropertyCollection.ItemPropertyInfo>();
                foreach (PropertyInfo info in clrType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    foreach (MetadataPropertyAttribute attribute in info.GetCustomAttributes(typeof(MetadataPropertyAttribute), false))
                    {
                        list.Add(new MetadataPropertyCollection.ItemPropertyInfo(info, attribute));
                    }
                }
                return list;
            }

        }
    }
}

