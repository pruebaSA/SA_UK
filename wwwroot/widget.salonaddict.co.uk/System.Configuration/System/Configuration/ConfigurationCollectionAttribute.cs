namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class ConfigurationCollectionAttribute : Attribute
    {
        private string _addItemName;
        private string _clearItemsName;
        private ConfigurationElementCollectionType _collectionType = ConfigurationElementCollectionType.AddRemoveClearMap;
        private Type _itemType;
        private string _removeItemName;

        public ConfigurationCollectionAttribute(Type itemType)
        {
            if (itemType == null)
            {
                throw new ArgumentNullException("itemType");
            }
            this._itemType = itemType;
        }

        public string AddItemName
        {
            get
            {
                if (this._addItemName == null)
                {
                    return "add";
                }
                return this._addItemName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                this._addItemName = value;
            }
        }

        public string ClearItemsName
        {
            get
            {
                if (this._clearItemsName == null)
                {
                    return "clear";
                }
                return this._clearItemsName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                this._clearItemsName = value;
            }
        }

        public ConfigurationElementCollectionType CollectionType
        {
            get => 
                this._collectionType;
            set
            {
                this._collectionType = value;
            }
        }

        public Type ItemType =>
            this._itemType;

        public string RemoveItemName
        {
            get
            {
                if (this._removeItemName == null)
                {
                    return "remove";
                }
                return this._removeItemName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                this._removeItemName = value;
            }
        }
    }
}

