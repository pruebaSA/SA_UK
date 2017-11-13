namespace MS.Internal.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;

    [StructLayout(LayoutKind.Sequential)]
    internal struct APCustomTypeDescriptor : ICustomTypeDescriptor
    {
        private ICustomTypeDescriptor _parent;
        private DependencyObject _instance;
        private static object _syncLock;
        private static int _dpCacheCount;
        private static DependencyProperty[] _dpCacheArray;
        internal APCustomTypeDescriptor(ICustomTypeDescriptor parent, object instance)
        {
            this._parent = parent;
            this._instance = FromObj(instance);
        }

        public PropertyDescriptorCollection GetProperties() => 
            this.GetProperties(null);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            List<PropertyDescriptor> list;
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
            if (filter == PropertyFilterOptions.None)
            {
                return PropertyDescriptorCollection.Empty;
            }
            DependencyProperty[] registeredProperties = this.GetRegisteredProperties();
            Type targetType = this._instance.GetType();
            if (filter == PropertyFilterOptions.SetValues)
            {
                LocalValueEnumerator localValueEnumerator = this._instance.GetLocalValueEnumerator();
                list = new List<PropertyDescriptor>(localValueEnumerator.Count);
                while (localValueEnumerator.MoveNext())
                {
                    DependencyProperty dp = localValueEnumerator.Current.Property;
                    DependencyPropertyKind dependencyPropertyKind = DependencyObjectProvider.GetDependencyPropertyKind(dp, targetType);
                    if (!dependencyPropertyKind.IsDirect && !dependencyPropertyKind.IsInternal)
                    {
                        DependencyObjectPropertyDescriptor attachedPropertyDescriptor = DependencyObjectProvider.GetAttachedPropertyDescriptor(dp, targetType);
                        list.Add(attachedPropertyDescriptor);
                    }
                }
            }
            else
            {
                list = new List<PropertyDescriptor>(registeredProperties.Length);
                foreach (DependencyProperty property2 in registeredProperties)
                {
                    bool flag = false;
                    DependencyPropertyKind kind2 = DependencyObjectProvider.GetDependencyPropertyKind(property2, targetType);
                    if (kind2.IsAttached)
                    {
                        PropertyFilterOptions options2 = PropertyFilterOptions.UnsetValues | PropertyFilterOptions.SetValues;
                        PropertyFilterOptions options3 = PropertyFilterOptions.Valid | PropertyFilterOptions.Invalid;
                        if (((filter & options2) == options2) || ((filter & options3) == options3))
                        {
                            flag = true;
                        }
                        if (!flag && ((filter & options3) != PropertyFilterOptions.None))
                        {
                            flag = this.CanAttachProperty(property2, this._instance) ^ ((filter & options3) == PropertyFilterOptions.Invalid);
                        }
                        if (!flag && ((filter & options2) != PropertyFilterOptions.None))
                        {
                            flag = this._instance.ContainsValue(property2) ^ ((filter & options2) == PropertyFilterOptions.UnsetValues);
                        }
                    }
                    else if ((((filter & PropertyFilterOptions.SetValues) != PropertyFilterOptions.None) && this._instance.ContainsValue(property2)) && (!kind2.IsDirect && !kind2.IsInternal))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        DependencyObjectPropertyDescriptor item = DependencyObjectProvider.GetAttachedPropertyDescriptor(property2, targetType);
                        list.Add(item);
                    }
                }
            }
            return new PropertyDescriptorCollection(list.ToArray(), true);
        }

        public AttributeCollection GetAttributes() => 
            this._parent.GetAttributes();

        public string GetClassName() => 
            this._parent.GetClassName();

        public string GetComponentName() => 
            this._parent.GetComponentName();

        public TypeConverter GetConverter()
        {
            TypeConverter converter = this._parent.GetConverter();
            if (converter.GetType().IsPublic)
            {
                return converter;
            }
            return null;
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

        private bool CanAttachProperty(DependencyProperty dp, DependencyObject instance) => 
            DependencyObjectProvider.GetAttachInfo(dp).CanAttach(instance);

        private static DependencyObject FromObj(object value) => 
            ((DependencyObject) TypeDescriptor.GetAssociation(typeof(DependencyObject), value));

        private DependencyProperty[] GetRegisteredProperties()
        {
            lock (_syncLock)
            {
                int num = _dpCacheCount;
                int registeredPropertyCount = DependencyProperty.RegisteredPropertyCount;
                if ((_dpCacheArray == null) || (num != registeredPropertyCount))
                {
                    List<DependencyProperty> list = new List<DependencyProperty>(registeredPropertyCount);
                    lock (DependencyProperty.Synchronized)
                    {
                        foreach (DependencyProperty property in DependencyProperty.RegisteredProperties)
                        {
                            list.Add(property);
                        }
                        _dpCacheCount = DependencyProperty.RegisteredPropertyCount;
                        _dpCacheArray = list.ToArray();
                    }
                }
                return _dpCacheArray;
            }
        }

        static APCustomTypeDescriptor()
        {
            _syncLock = new object();
            _dpCacheCount = 0;
        }
    }
}

