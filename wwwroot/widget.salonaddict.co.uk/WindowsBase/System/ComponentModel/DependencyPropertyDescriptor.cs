namespace System.ComponentModel
{
    using MS.Internal.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Windows;

    public sealed class DependencyPropertyDescriptor : PropertyDescriptor
    {
        private static Dictionary<object, DependencyPropertyDescriptor> _cache = new Dictionary<object, DependencyPropertyDescriptor>(new ReferenceEqualityComparer());
        private Type _componentType;
        private System.Windows.DependencyProperty _dp;
        private bool _isAttached;
        private PropertyMetadata _metadata;
        private PropertyDescriptor _property;

        private DependencyPropertyDescriptor(PropertyDescriptor property, string name, Type componentType, System.Windows.DependencyProperty dp, bool isAttached) : base(name, null)
        {
            this._property = property;
            this._componentType = componentType;
            this._dp = dp;
            this._isAttached = isAttached;
            this._metadata = this._dp.GetMetadata(componentType);
        }

        public override void AddValueChanged(object component, EventHandler handler)
        {
            this.Property.AddValueChanged(component, handler);
        }

        public override bool CanResetValue(object component) => 
            this.Property.CanResetValue(component);

        internal static void ClearCache()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
        }

        public override bool Equals(object obj)
        {
            DependencyPropertyDescriptor descriptor = obj as DependencyPropertyDescriptor;
            return (((descriptor != null) && (descriptor._dp == this._dp)) && (descriptor._componentType == this._componentType));
        }

        public static DependencyPropertyDescriptor FromName(string name, Type ownerType, Type targetType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (ownerType == null)
            {
                throw new ArgumentNullException("ownerType");
            }
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            System.Windows.DependencyProperty dependencyProperty = System.Windows.DependencyProperty.FromName(name, ownerType);
            if (dependencyProperty != null)
            {
                return FromProperty(dependencyProperty, targetType);
            }
            return null;
        }

        public static DependencyPropertyDescriptor FromProperty(PropertyDescriptor property)
        {
            DependencyPropertyDescriptor descriptor;
            bool flag;
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            lock (_cache)
            {
                flag = _cache.TryGetValue(property, out descriptor);
            }
            if (!flag)
            {
                System.Windows.DependencyProperty dp = null;
                bool isAttached = false;
                DependencyObjectPropertyDescriptor descriptor2 = property as DependencyObjectPropertyDescriptor;
                if (descriptor2 != null)
                {
                    dp = descriptor2.DependencyProperty;
                    isAttached = descriptor2.IsAttached;
                }
                else
                {
                    DependencyPropertyAttribute attribute = property.Attributes[typeof(DependencyPropertyAttribute)] as DependencyPropertyAttribute;
                    if (attribute != null)
                    {
                        dp = attribute.DependencyProperty;
                        isAttached = attribute.IsAttached;
                    }
                }
                if (dp == null)
                {
                    return descriptor;
                }
                descriptor = new DependencyPropertyDescriptor(property, property.Name, property.ComponentType, dp, isAttached);
                lock (_cache)
                {
                    _cache[property] = descriptor;
                }
            }
            return descriptor;
        }

        public static DependencyPropertyDescriptor FromProperty(System.Windows.DependencyProperty dependencyProperty, Type targetType)
        {
            if (dependencyProperty == null)
            {
                throw new ArgumentNullException("dependencyProperty");
            }
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            DependencyPropertyDescriptor descriptor = null;
            DependencyPropertyKind dependencyPropertyKind = DependencyObjectProvider.GetDependencyPropertyKind(dependencyProperty, targetType);
            if (dependencyPropertyKind.IsDirect)
            {
                lock (_cache)
                {
                    _cache.TryGetValue(dependencyProperty, out descriptor);
                }
                if (descriptor != null)
                {
                    return descriptor;
                }
                descriptor = new DependencyPropertyDescriptor(null, dependencyProperty.Name, targetType, dependencyProperty, false);
                lock (_cache)
                {
                    _cache[dependencyProperty] = descriptor;
                    return descriptor;
                }
            }
            if (!dependencyPropertyKind.IsInternal)
            {
                PropertyDescriptor attachedPropertyDescriptor = DependencyObjectProvider.GetAttachedPropertyDescriptor(dependencyProperty, targetType);
                if (attachedPropertyDescriptor != null)
                {
                    descriptor = FromProperty(attachedPropertyDescriptor);
                }
            }
            return descriptor;
        }

        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter) => 
            this.Property.GetChildProperties(instance, filter);

        public override object GetEditor(Type editorBaseType) => 
            this.Property.GetEditor(editorBaseType);

        public override int GetHashCode() => 
            (this._dp.GetHashCode() ^ this._componentType.GetHashCode());

        public override object GetValue(object component) => 
            this.Property.GetValue(component);

        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            this.Property.RemoveValueChanged(component, handler);
        }

        public override void ResetValue(object component)
        {
            this.Property.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            this.Property.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component) => 
            this.Property.ShouldSerializeValue(component);

        public override string ToString() => 
            this.Name;

        public override AttributeCollection Attributes =>
            this.Property.Attributes;

        public override string Category =>
            this.Property.Category;

        public override Type ComponentType =>
            this._componentType;

        public override TypeConverter Converter
        {
            get
            {
                TypeConverter converter = this.Property.Converter;
                if (converter.GetType().IsPublic)
                {
                    return converter;
                }
                return null;
            }
        }

        public System.Windows.DependencyProperty DependencyProperty =>
            this._dp;

        public override string Description =>
            this.Property.Description;

        public CoerceValueCallback DesignerCoerceValueCallback
        {
            get => 
                this.DependencyProperty.DesignerCoerceValueCallback;
            set
            {
                this.DependencyProperty.DesignerCoerceValueCallback = value;
            }
        }

        public override bool DesignTimeOnly =>
            this.Property.DesignTimeOnly;

        public override string DisplayName =>
            this.Property.DisplayName;

        public bool IsAttached =>
            this._isAttached;

        public override bool IsBrowsable =>
            this.Property.IsBrowsable;

        public override bool IsLocalizable =>
            this.Property.IsLocalizable;

        public override bool IsReadOnly =>
            this.Property.IsReadOnly;

        public PropertyMetadata Metadata =>
            this._metadata;

        private PropertyDescriptor Property
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (this._property == null)
                {
                    this._property = TypeDescriptor.GetProperties(this._componentType)[this.Name];
                    if (this._property == null)
                    {
                        this._property = TypeDescriptor.CreateProperty(this._componentType, this.Name, this._dp.PropertyType, new Attribute[0]);
                    }
                }
                return this._property;
            }
        }

        public override Type PropertyType =>
            this._dp.PropertyType;

        public override bool SupportsChangeEvents =>
            this.Property.SupportsChangeEvents;
    }
}

