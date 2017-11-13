namespace AjaxControlToolkit.Design
{
    using System;
    using System.ComponentModel;

    internal class FilterTypeDescriptionProvider<T> : TypeDescriptionProvider, ICustomTypeDescriptor
    {
        private ICustomTypeDescriptor _baseDescriptor;
        private TypeDescriptionProvider _baseProvider;
        private bool _extended;
        private T _target;

        public FilterTypeDescriptionProvider(T target) : base(TypeDescriptor.GetProvider(target))
        {
            this._target = target;
            this._baseProvider = TypeDescriptor.GetProvider(target);
        }

        public void Dispose()
        {
            this._target = default(T);
            this._baseDescriptor = null;
            this._baseProvider = null;
        }

        private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection props)
        {
            PropertyDescriptor[] array = new PropertyDescriptor[props.Count];
            props.CopyTo(array, 0);
            bool flag = false;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyDescriptor descriptor = this.ProcessProperty(array[i]);
                if (descriptor != array[i])
                {
                    flag = true;
                    array[i] = descriptor;
                }
            }
            if (flag)
            {
                props = new PropertyDescriptorCollection(array);
            }
            return props;
        }

        public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
        {
            if (this.FilterExtendedProperties && (instance == this.Target))
            {
                return this;
            }
            return this._baseProvider.GetExtendedTypeDescriptor(instance);
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            if (!this.FilterExtendedProperties && (instance == this.Target))
            {
                return this;
            }
            return this._baseProvider.GetTypeDescriptor(objectType, instance);
        }

        protected virtual PropertyDescriptor ProcessProperty(PropertyDescriptor baseProp) => 
            baseProp;

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            this.BaseDescriptor.GetAttributes();

        string ICustomTypeDescriptor.GetClassName() => 
            this.BaseDescriptor.GetClassName();

        string ICustomTypeDescriptor.GetComponentName() => 
            this.BaseDescriptor.GetComponentName();

        TypeConverter ICustomTypeDescriptor.GetConverter() => 
            this.BaseDescriptor.GetConverter();

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => 
            this.BaseDescriptor.GetDefaultEvent();

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => 
            this.BaseDescriptor.GetDefaultProperty();

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => 
            this.BaseDescriptor.GetEditor(editorBaseType);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => 
            this.BaseDescriptor.GetEvents();

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => 
            this.BaseDescriptor.GetEvents(attributes);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => 
            this.FilterProperties(this.BaseDescriptor.GetProperties());

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = this.BaseDescriptor.GetProperties(attributes);
            return this.FilterProperties(properties);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => 
            this.BaseDescriptor.GetPropertyOwner(pd);

        private ICustomTypeDescriptor BaseDescriptor
        {
            get
            {
                if (this._baseDescriptor == null)
                {
                    if (this.FilterExtendedProperties)
                    {
                        this._baseDescriptor = this._baseProvider.GetExtendedTypeDescriptor(this.Target);
                    }
                    else
                    {
                        this._baseDescriptor = this._baseProvider.GetTypeDescriptor(this.Target);
                    }
                }
                return this._baseDescriptor;
            }
        }

        protected bool FilterExtendedProperties
        {
            get => 
                this._extended;
            set
            {
                this._extended = value;
            }
        }

        protected T Target =>
            this._target;
    }
}

