namespace System.Data.Metadata.Edm
{
    using System;
    using System.Reflection;

    internal sealed class MetadataPropertyValue
    {
        private MetadataItem _item;
        private PropertyInfo _propertyInfo;

        internal MetadataPropertyValue(PropertyInfo propertyInfo, MetadataItem item)
        {
            this._propertyInfo = propertyInfo;
            this._item = item;
        }

        internal object GetValue() => 
            this._propertyInfo.GetValue(this._item, new object[0]);
    }
}

