namespace MS.Internal.ComponentModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Markup;

    [StructLayout(LayoutKind.Sequential)]
    internal struct DPCustomTypeDescriptor : ICustomTypeDescriptor
    {
        private const PropertyFilterOptions _anySet = (PropertyFilterOptions.UnsetValues | PropertyFilterOptions.SetValues);
        private const PropertyFilterOptions _anyValid = PropertyFilterOptions.Valid;
        private ICustomTypeDescriptor _parent;
        private Type _objectType;
        private object _instance;
        private static Dictionary<PropertyDescriptor, DependencyObjectPropertyDescriptor> _propertyMap;
        private static Hashtable _typeProperties;
        internal DPCustomTypeDescriptor(ICustomTypeDescriptor parent, Type objectType, object instance)
        {
            this._parent = parent;
            this._objectType = objectType;
            this._instance = instance;
        }

        public string GetComponentName()
        {
            if (this._instance != null)
            {
                RuntimeNamePropertyAttribute attribute = this.GetAttributes()[typeof(RuntimeNamePropertyAttribute)] as RuntimeNamePropertyAttribute;
                if ((attribute != null) && (attribute.Name != null))
                {
                    PropertyDescriptor descriptor = this.GetProperties()[attribute.Name];
                    if (descriptor != null)
                    {
                        return (descriptor.GetValue(this._instance) as string);
                    }
                }
            }
            return this._parent.GetComponentName();
        }

        public PropertyDescriptorCollection GetProperties() => 
            this.GetProperties(null);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            DependencyObject association;
            PropertyFilterOptions filter = PropertyFilterOptions.Valid | PropertyFilterOptions.SetValues;
            if (attributes != null)
            {
                foreach (Attribute attribute in attributes)
                {
                    PropertyFilterAttribute attribute2 = attribute as PropertyFilterAttribute;
                    if (attribute2 != null)
                    {
                        filter = attribute2.Filter;
                        break;
                    }
                }
            }
            if ((filter == PropertyFilterOptions.None) || (filter == PropertyFilterOptions.Invalid))
            {
                return PropertyDescriptorCollection.Empty;
            }
            if (filter == PropertyFilterOptions.SetValues)
            {
                if (this._instance == null)
                {
                    return PropertyDescriptorCollection.Empty;
                }
                association = (DependencyObject) TypeDescriptor.GetAssociation(this._objectType, this._instance);
            }
            else
            {
                association = null;
            }
            PropertyDescriptorCollection descriptors = (PropertyDescriptorCollection) _typeProperties[this._objectType];
            if (descriptors == null)
            {
                descriptors = this.CreateProperties();
                lock (_typeProperties)
                {
                    _typeProperties[this._objectType] = descriptors;
                }
            }
            if (((filter & (PropertyFilterOptions.UnsetValues | PropertyFilterOptions.SetValues)) != (PropertyFilterOptions.UnsetValues | PropertyFilterOptions.SetValues)) && ((filter & PropertyFilterOptions.Valid) != PropertyFilterOptions.Valid))
            {
                List<PropertyDescriptor> list = null;
                int count = descriptors.Count;
                for (int i = 0; i < count; i++)
                {
                    PropertyDescriptor item = descriptors[i];
                    if (!(item.ShouldSerializeValue(association) ^ ((filter & (PropertyFilterOptions.UnsetValues | PropertyFilterOptions.SetValues)) == PropertyFilterOptions.UnsetValues)))
                    {
                        if (list == null)
                        {
                            list = new List<PropertyDescriptor>(count);
                            for (int j = 0; j < i; j++)
                            {
                                list.Add(descriptors[j]);
                            }
                        }
                    }
                    else if (list != null)
                    {
                        list.Add(item);
                    }
                }
                if (list != null)
                {
                    descriptors = new PropertyDescriptorCollection(list.ToArray(), true);
                }
            }
            return descriptors;
        }

        public AttributeCollection GetAttributes() => 
            this._parent.GetAttributes();

        public string GetClassName() => 
            this._parent.GetClassName();

        public TypeConverter GetConverter()
        {
            TypeConverter converter = this._parent.GetConverter();
            if (converter.GetType().IsPublic)
            {
                return converter;
            }
            return new TypeConverter();
        }

        public EventDescriptor GetDefaultEvent() => 
            this._parent.GetDefaultEvent();

        public PropertyDescriptor GetDefaultProperty() => 
            this._parent.GetDefaultProperty();

        public object GetEditor(Type editorBaseType) => 
            this._parent.GetEditor(editorBaseType);

        public EventDescriptorCollection GetEvents() => 
            this._parent.GetEvents();

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => 
            this._parent.GetEvents(attributes);

        public object GetPropertyOwner(PropertyDescriptor property) => 
            this._parent.GetPropertyOwner(property);

        internal static void ClearCache()
        {
            lock (_propertyMap)
            {
                _propertyMap.Clear();
            }
            lock (_typeProperties)
            {
                _typeProperties.Clear();
            }
        }

        private PropertyDescriptorCollection CreateProperties()
        {
            PropertyDescriptorCollection properties = this._parent.GetProperties();
            List<PropertyDescriptor> list = new List<PropertyDescriptor>(properties.Count);
            for (int i = 0; i < properties.Count; i++)
            {
                DependencyObjectPropertyDescriptor descriptor2;
                bool flag;
                PropertyDescriptor key = properties[i];
                DependencyProperty dp = null;
                lock (_propertyMap)
                {
                    flag = _propertyMap.TryGetValue(key, out descriptor2);
                }
                if (flag && (descriptor2 != null))
                {
                    dp = DependencyProperty.FromName(key.Name, this._objectType);
                    if (dp != descriptor2.DependencyProperty)
                    {
                        descriptor2 = null;
                    }
                    else if (descriptor2.Metadata != dp.GetMetadata(this._objectType))
                    {
                        descriptor2 = null;
                    }
                }
                if (descriptor2 == null)
                {
                    if ((dp != null) || typeof(DependencyObject).IsAssignableFrom(key.ComponentType))
                    {
                        if (dp == null)
                        {
                            dp = DependencyProperty.FromName(key.Name, this._objectType);
                        }
                        if (dp != null)
                        {
                            descriptor2 = new DependencyObjectPropertyDescriptor(key, dp, this._objectType);
                        }
                    }
                    if (!flag)
                    {
                        lock (_propertyMap)
                        {
                            _propertyMap[key] = descriptor2;
                        }
                    }
                }
                if (descriptor2 != null)
                {
                    key = descriptor2;
                }
                list.Add(key);
            }
            return new PropertyDescriptorCollection(list.ToArray(), true);
        }

        static DPCustomTypeDescriptor()
        {
            _propertyMap = new Dictionary<PropertyDescriptor, DependencyObjectPropertyDescriptor>(new PropertyDescriptorComparer());
            _typeProperties = new Hashtable();
        }
    }
}

