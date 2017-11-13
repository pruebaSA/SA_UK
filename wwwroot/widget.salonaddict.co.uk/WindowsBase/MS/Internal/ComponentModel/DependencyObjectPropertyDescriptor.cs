namespace MS.Internal.ComponentModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;

    internal sealed class DependencyObjectPropertyDescriptor : PropertyDescriptor
    {
        private static object _attributeSyncLock = new object();
        private Type _componentType;
        private System.Windows.DependencyProperty _dp;
        private static Binder _dpBinder = new AttachedPropertyMethodSelector();
        private static Type[] _dpType = new Type[] { typeof(DependencyObject) };
        private static Type[] _emptyType = new Type[0];
        private static Hashtable _getMethodCache = new Hashtable();
        private PropertyMetadata _metadata;
        private static object _nullMethodSentinel = new object();
        private PropertyDescriptor _property;
        private bool _queriedResetMethod;
        private bool _queriedShouldSerializeMethod;
        private MethodInfo _resetMethod;
        private static Hashtable _setMethodCache = new Hashtable();
        private MethodInfo _shouldSerializeMethod;
        private Dictionary<DependencyObject, PropertyChangeTracker> _trackers;

        internal DependencyObjectPropertyDescriptor(System.Windows.DependencyProperty dp, Type ownerType) : base(dp.OwnerType.Name + "." + dp.Name, null)
        {
            this._dp = dp;
            this._componentType = ownerType;
            this._metadata = this._dp.GetMetadata(ownerType);
        }

        internal DependencyObjectPropertyDescriptor(PropertyDescriptor property, System.Windows.DependencyProperty dp, Type objectType) : base(dp.Name, null)
        {
            this._property = property;
            this._dp = dp;
            this._componentType = property.ComponentType;
            this._metadata = this._dp.GetMetadata(objectType);
        }

        public override void AddValueChanged(object component, EventHandler handler)
        {
            PropertyChangeTracker tracker;
            DependencyObject key = FromObj(component);
            if (this._trackers == null)
            {
                this._trackers = new Dictionary<DependencyObject, PropertyChangeTracker>();
            }
            if (!this._trackers.TryGetValue(key, out tracker))
            {
                tracker = new PropertyChangeTracker(key, this._dp);
                this._trackers.Add(key, tracker);
            }
            tracker.Changed = (EventHandler) Delegate.Combine(tracker.Changed, handler);
        }

        public override bool CanResetValue(object component) => 
            this.ShouldSerializeValue(component);

        internal static void ClearCache()
        {
            lock (_getMethodCache)
            {
                _getMethodCache.Clear();
            }
            lock (_setMethodCache)
            {
                _setMethodCache.Clear();
            }
        }

        protected override AttributeCollection CreateAttributeCollection()
        {
            this.MergeAttributes();
            return base.CreateAttributeCollection();
        }

        private static DependencyObject FromObj(object value) => 
            ((DependencyObject) TypeDescriptor.GetAssociation(typeof(DependencyObject), value));

        private AttributeCollection GetAttachedPropertyAttributes()
        {
            MethodInfo attachedPropertyMethod = GetAttachedPropertyMethod(this._dp);
            if (attachedPropertyMethod == null)
            {
                return AttributeCollection.Empty;
            }
            Attribute[] customAttributes = (Attribute[]) attachedPropertyMethod.GetCustomAttributes(typeof(Attribute), true);
            Attribute[] sourceArray = (Attribute[]) TypeDescriptor.GetReflectionType(this._dp.PropertyType).GetCustomAttributes(typeof(Attribute), true);
            if ((sourceArray != null) && (sourceArray.Length > 0))
            {
                Attribute[] destinationArray = new Attribute[customAttributes.Length + sourceArray.Length];
                Array.Copy(customAttributes, destinationArray, customAttributes.Length);
                Array.Copy(sourceArray, 0, destinationArray, customAttributes.Length, sourceArray.Length);
                customAttributes = destinationArray;
            }
            Attribute[] attributeArray4 = null;
            foreach (Attribute attribute in customAttributes)
            {
                AttributeProviderAttribute attribute2 = attribute as AttributeProviderAttribute;
                if (attribute2 != null)
                {
                    Type type = Type.GetType(attribute2.TypeName);
                    if (type != null)
                    {
                        Attribute[] attributeArray5 = null;
                        if (!string.IsNullOrEmpty(attribute2.PropertyName))
                        {
                            MemberInfo[] member = type.GetMember(attribute2.PropertyName);
                            if ((member.Length > 0) && (member[0] != null))
                            {
                                attributeArray5 = (Attribute[]) member[0].GetCustomAttributes(typeof(Attribute), true);
                            }
                        }
                        else
                        {
                            attributeArray5 = (Attribute[]) type.GetCustomAttributes(typeof(Attribute), true);
                        }
                        if (attributeArray5 != null)
                        {
                            if (attributeArray4 == null)
                            {
                                attributeArray4 = attributeArray5;
                            }
                            else
                            {
                                Attribute[] array = new Attribute[attributeArray4.Length + attributeArray5.Length];
                                attributeArray4.CopyTo(array, 0);
                                attributeArray5.CopyTo(array, attributeArray4.Length);
                                attributeArray4 = array;
                            }
                        }
                    }
                }
            }
            if (attributeArray4 != null)
            {
                Attribute[] attributeArray7 = new Attribute[attributeArray4.Length + customAttributes.Length];
                customAttributes.CopyTo(attributeArray7, 0);
                attributeArray4.CopyTo(attributeArray7, customAttributes.Length);
                customAttributes = attributeArray7;
            }
            return new AttributeCollection(customAttributes);
        }

        internal static MethodInfo GetAttachedPropertyMethod(System.Windows.DependencyProperty dp)
        {
            Type reflectionType = TypeDescriptor.GetReflectionType(dp.OwnerType);
            object obj2 = _getMethodCache[dp];
            MethodInfo info = obj2 as MethodInfo;
            if ((obj2 == null) || ((info != null) && !object.ReferenceEquals(info.DeclaringType, reflectionType)))
            {
                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
                string name = "Get" + dp.Name;
                info = reflectionType.GetMethod(name, bindingAttr, _dpBinder, _dpType, null);
                lock (_getMethodCache)
                {
                    _getMethodCache[dp] = (info == null) ? _nullMethodSentinel : info;
                }
            }
            return info;
        }

        private static MethodInfo GetAttachedPropertySetMethod(System.Windows.DependencyProperty dp)
        {
            Type reflectionType = TypeDescriptor.GetReflectionType(dp.OwnerType);
            object obj2 = _setMethodCache[dp];
            MethodInfo info = obj2 as MethodInfo;
            if ((obj2 == null) || ((info != null) && !object.ReferenceEquals(info.DeclaringType, reflectionType)))
            {
                BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
                string name = "Set" + dp.Name;
                Type[] types = new Type[] { typeof(DependencyObject), dp.PropertyType };
                info = reflectionType.GetMethod(name, bindingAttr, _dpBinder, types, null);
                lock (_setMethodCache)
                {
                    _setMethodCache[dp] = (info == null) ? _nullMethodSentinel : info;
                }
            }
            return info;
        }

        private MethodInfo GetSpecialMethod(string methodPrefix)
        {
            Type[] typeArray;
            Type reflectionType;
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public;
            if (this._property == null)
            {
                typeArray = _dpType;
                bindingAttr |= BindingFlags.Static;
                reflectionType = TypeDescriptor.GetReflectionType(this._dp.OwnerType);
            }
            else
            {
                typeArray = _emptyType;
                bindingAttr |= BindingFlags.Instance;
                reflectionType = TypeDescriptor.GetReflectionType(this._property.ComponentType);
            }
            string name = methodPrefix + this._dp.Name;
            MethodInfo info = reflectionType.GetMethod(name, bindingAttr, _dpBinder, typeArray, null);
            if ((info != null) && !info.IsPublic)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("SpecialMethodMustBePublic", new object[] { info.Name }));
            }
            return info;
        }

        public override object GetValue(object component) => 
            FromObj(component).GetValue(this._dp);

        private void MergeAttributes()
        {
            AttributeCollection attachedPropertyAttributes;
            if (this._property != null)
            {
                attachedPropertyAttributes = this._property.Attributes;
            }
            else
            {
                attachedPropertyAttributes = this.GetAttachedPropertyAttributes();
            }
            List<Attribute> list = new List<Attribute>(attachedPropertyAttributes.Count + 1);
            bool isReadOnly = false;
            foreach (Attribute attribute in attachedPropertyAttributes)
            {
                Attribute attribute2 = attribute;
                if (attribute is DefaultValueAttribute)
                {
                    attribute2 = null;
                }
                else
                {
                    ReadOnlyAttribute attribute4 = attribute as ReadOnlyAttribute;
                    if (attribute4 != null)
                    {
                        isReadOnly = attribute4.IsReadOnly;
                        attribute2 = null;
                    }
                }
                if (attribute2 != null)
                {
                    list.Add(attribute2);
                }
            }
            isReadOnly |= this._dp.ReadOnly;
            if (((this._property == null) && !isReadOnly) && (GetAttachedPropertySetMethod(this._dp) == null))
            {
                isReadOnly = true;
            }
            DependencyPropertyAttribute item = new DependencyPropertyAttribute(this._dp, this._property == null);
            list.Add(item);
            if (this._metadata.DefaultValue != System.Windows.DependencyProperty.UnsetValue)
            {
                list.Add(new DefaultValueAttribute(this._metadata.DefaultValue));
            }
            if (isReadOnly)
            {
                list.Add(new ReadOnlyAttribute(true));
            }
            Attribute[] attributeArray = list.ToArray();
            for (int i = 0; i < (attributeArray.Length / 2); i++)
            {
                int index = (attributeArray.Length - i) - 1;
                Attribute attribute6 = attributeArray[i];
                attributeArray[i] = attributeArray[index];
                attributeArray[index] = attribute6;
            }
            this.AttributeArray = attributeArray;
        }

        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            if (this._trackers != null)
            {
                PropertyChangeTracker tracker;
                DependencyObject key = FromObj(component);
                if (this._trackers.TryGetValue(key, out tracker))
                {
                    tracker.Changed = (EventHandler) Delegate.Remove(tracker.Changed, handler);
                    if (tracker.CanClose)
                    {
                        tracker.Close();
                        this._trackers.Remove(key);
                    }
                }
            }
        }

        public override void ResetValue(object component)
        {
            if (!this._queriedResetMethod)
            {
                this._resetMethod = this.GetSpecialMethod("Reset");
                this._queriedResetMethod = true;
            }
            DependencyObject obj2 = FromObj(component);
            if (this._resetMethod != null)
            {
                if (this._property == null)
                {
                    this._resetMethod.Invoke(null, new object[] { obj2 });
                }
                else
                {
                    this._resetMethod.Invoke(obj2, null);
                }
            }
            else
            {
                obj2.ClearValue(this._dp);
            }
        }

        public override void SetValue(object component, object value)
        {
            FromObj(component).SetValue(this._dp, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            DependencyObject obj2 = FromObj(component);
            bool flag = obj2.ShouldSerializeProperty(this._dp);
            if (!flag)
            {
                return flag;
            }
            if (!this._queriedShouldSerializeMethod)
            {
                MethodInfo specialMethod = this.GetSpecialMethod("ShouldSerialize");
                if ((specialMethod != null) && (specialMethod.ReturnType == typeof(bool)))
                {
                    this._shouldSerializeMethod = specialMethod;
                }
                this._queriedShouldSerializeMethod = true;
            }
            if (this._shouldSerializeMethod == null)
            {
                return flag;
            }
            if (this._property == null)
            {
                return (bool) this._shouldSerializeMethod.Invoke(null, new object[] { obj2 });
            }
            return (bool) this._shouldSerializeMethod.Invoke(obj2, null);
        }

        public override AttributeCollection Attributes
        {
            get
            {
                AttributeCollection attributes = base.Attributes;
                if (attributes != null)
                {
                    return attributes;
                }
                lock (_attributeSyncLock)
                {
                    return base.Attributes;
                }
            }
        }

        public override Type ComponentType =>
            this._componentType;

        internal System.Windows.DependencyProperty DependencyProperty =>
            this._dp;

        internal bool IsAttached =>
            (this._property == null);

        public override bool IsReadOnly
        {
            get
            {
                bool readOnly = this._dp.ReadOnly;
                if (!readOnly)
                {
                    readOnly = this.Attributes.Contains(ReadOnlyAttribute.Yes);
                }
                return readOnly;
            }
        }

        internal PropertyMetadata Metadata =>
            this._metadata;

        public override Type PropertyType =>
            this._dp.PropertyType;

        public override bool SupportsChangeEvents =>
            true;
    }
}

