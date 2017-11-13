namespace System.Data.Design
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal abstract class DataSourceComponent : Component, ICustomTypeDescriptor, IObjectWithParent, IDataSourceCollectionMember, IDataSourceRenamableObject
    {
        private DataSourceCollectionBase collectionParent;

        protected DataSourceComponent()
        {
        }

        private PropertyDescriptorCollection GetProperties(Attribute[] attributes) => 
            TypeDescriptor.GetProperties(base.GetType(), attributes);

        protected override object GetService(Type service)
        {
            DataSourceComponent collectionHost = this;
            while ((collectionHost != null) && (collectionHost.Site == null))
            {
                if (collectionHost.CollectionParent != null)
                {
                    collectionHost = collectionHost.CollectionParent.CollectionHost;
                }
                else
                {
                    if ((collectionHost.Parent != null) && (collectionHost.Parent is DataSourceComponent))
                    {
                        collectionHost = collectionHost.Parent as DataSourceComponent;
                        continue;
                    }
                    collectionHost = null;
                }
            }
            if ((collectionHost != null) && (collectionHost.Site != null))
            {
                return collectionHost.Site.GetService(service);
            }
            return null;
        }

        public virtual void SetCollection(DataSourceCollectionBase collection)
        {
            this.CollectionParent = collection;
        }

        internal void SetPropertyValue(string propertyName, object value)
        {
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            TypeDescriptor.GetAttributes(base.GetType());

        string ICustomTypeDescriptor.GetClassName()
        {
            if (this is IDataSourceNamedObject)
            {
                return ((IDataSourceNamedObject) this).PublicTypeName;
            }
            return null;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            INamedObject obj2 = this as INamedObject;
            return obj2?.Name;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter() => 
            TypeDescriptor.GetConverter(base.GetType());

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => 
            TypeDescriptor.GetDefaultEvent(base.GetType());

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => 
            TypeDescriptor.GetDefaultProperty(base.GetType());

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => 
            TypeDescriptor.GetEditor(base.GetType(), editorBaseType);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => 
            TypeDescriptor.GetEvents(base.GetType());

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => 
            TypeDescriptor.GetEvents(base.GetType(), attributes);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => 
            this.GetProperties(null);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) => 
            this.GetProperties(attributes);

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => 
            this;

        protected internal virtual DataSourceCollectionBase CollectionParent
        {
            get => 
                this.collectionParent;
            set
            {
                this.collectionParent = value;
            }
        }

        protected virtual object ExternalPropertyHost =>
            null;

        [Browsable(false)]
        public virtual string GeneratorName =>
            null;

        internal virtual StringCollection NamingPropertyNames =>
            null;

        [Browsable(false)]
        public virtual object Parent =>
            this.collectionParent;
    }
}

