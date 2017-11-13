namespace System.ComponentModel
{
    using System;
    using System.Collections;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class TypeConverter
    {
        public bool CanConvertFrom(Type sourceType) => 
            this.CanConvertFrom(null, sourceType);

        public virtual bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            (sourceType == typeof(InstanceDescriptor));

        public bool CanConvertTo(Type destinationType) => 
            this.CanConvertTo(null, destinationType);

        public virtual bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
            (destinationType == typeof(string));

        public object ConvertFrom(object value) => 
            this.ConvertFrom(null, CultureInfo.CurrentCulture, value);

        public virtual object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            InstanceDescriptor descriptor = value as InstanceDescriptor;
            return descriptor?.Invoke();
        }

        public object ConvertFromInvariantString(string text) => 
            this.ConvertFromString(null, CultureInfo.InvariantCulture, text);

        public object ConvertFromInvariantString(ITypeDescriptorContext context, string text) => 
            this.ConvertFromString(context, CultureInfo.InvariantCulture, text);

        public object ConvertFromString(string text) => 
            this.ConvertFrom(null, null, text);

        public object ConvertFromString(ITypeDescriptorContext context, string text) => 
            this.ConvertFrom(context, CultureInfo.CurrentCulture, text);

        public object ConvertFromString(ITypeDescriptorContext context, CultureInfo culture, string text) => 
            this.ConvertFrom(context, culture, text);

        public object ConvertTo(object value, Type destinationType) => 
            this.ConvertTo(null, null, value, destinationType);

        public virtual object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (destinationType != typeof(string))
            {
                throw this.GetConvertToException(value, destinationType);
            }
            if (value == null)
            {
                return string.Empty;
            }
            if ((culture != null) && (culture != CultureInfo.CurrentCulture))
            {
                IFormattable formattable = value as IFormattable;
                if (formattable != null)
                {
                    return formattable.ToString(null, culture);
                }
            }
            return value.ToString();
        }

        public string ConvertToInvariantString(object value) => 
            this.ConvertToString(null, CultureInfo.InvariantCulture, value);

        public string ConvertToInvariantString(ITypeDescriptorContext context, object value) => 
            this.ConvertToString(context, CultureInfo.InvariantCulture, value);

        public string ConvertToString(object value) => 
            ((string) this.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string)));

        public string ConvertToString(ITypeDescriptorContext context, object value) => 
            ((string) this.ConvertTo(context, CultureInfo.CurrentCulture, value, typeof(string)));

        public string ConvertToString(ITypeDescriptorContext context, CultureInfo culture, object value) => 
            ((string) this.ConvertTo(context, culture, value, typeof(string)));

        public object CreateInstance(IDictionary propertyValues) => 
            this.CreateInstance(null, propertyValues);

        public virtual object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) => 
            null;

        protected Exception GetConvertFromException(object value)
        {
            string fullName;
            if (value == null)
            {
                fullName = SR.GetString("ToStringNull");
            }
            else
            {
                fullName = value.GetType().FullName;
            }
            throw new NotSupportedException(SR.GetString("ConvertFromException", new object[] { base.GetType().Name, fullName }));
        }

        protected Exception GetConvertToException(object value, Type destinationType)
        {
            string fullName;
            if (value == null)
            {
                fullName = SR.GetString("ToStringNull");
            }
            else
            {
                fullName = value.GetType().FullName;
            }
            throw new NotSupportedException(SR.GetString("ConvertToException", new object[] { base.GetType().Name, fullName, destinationType.FullName }));
        }

        public bool GetCreateInstanceSupported() => 
            this.GetCreateInstanceSupported(null);

        public virtual bool GetCreateInstanceSupported(ITypeDescriptorContext context) => 
            false;

        public PropertyDescriptorCollection GetProperties(object value) => 
            this.GetProperties(null, value);

        public PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value) => 
            this.GetProperties(context, value, new Attribute[] { BrowsableAttribute.Yes });

        public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) => 
            null;

        public bool GetPropertiesSupported() => 
            this.GetPropertiesSupported(null);

        public virtual bool GetPropertiesSupported(ITypeDescriptorContext context) => 
            false;

        public ICollection GetStandardValues() => 
            this.GetStandardValues(null);

        public virtual StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => 
            null;

        public bool GetStandardValuesExclusive() => 
            this.GetStandardValuesExclusive(null);

        public virtual bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            false;

        public bool GetStandardValuesSupported() => 
            this.GetStandardValuesSupported(null);

        public virtual bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
            false;

        public bool IsValid(object value) => 
            this.IsValid(null, value);

        public virtual bool IsValid(ITypeDescriptorContext context, object value) => 
            true;

        protected PropertyDescriptorCollection SortProperties(PropertyDescriptorCollection props, string[] names)
        {
            props.Sort(names);
            return props;
        }

        protected abstract class SimplePropertyDescriptor : PropertyDescriptor
        {
            private Type componentType;
            private Type propertyType;

            protected SimplePropertyDescriptor(Type componentType, string name, Type propertyType) : this(componentType, name, propertyType, new Attribute[0])
            {
            }

            protected SimplePropertyDescriptor(Type componentType, string name, Type propertyType, Attribute[] attributes) : base(name, attributes)
            {
                this.componentType = componentType;
                this.propertyType = propertyType;
            }

            public override bool CanResetValue(object component)
            {
                DefaultValueAttribute attribute = (DefaultValueAttribute) this.Attributes[typeof(DefaultValueAttribute)];
                return attribute?.Value.Equals(this.GetValue(component));
            }

            public override void ResetValue(object component)
            {
                DefaultValueAttribute attribute = (DefaultValueAttribute) this.Attributes[typeof(DefaultValueAttribute)];
                if (attribute != null)
                {
                    this.SetValue(component, attribute.Value);
                }
            }

            public override bool ShouldSerializeValue(object component) => 
                false;

            public override Type ComponentType =>
                this.componentType;

            public override bool IsReadOnly =>
                this.Attributes.Contains(ReadOnlyAttribute.Yes);

            public override Type PropertyType =>
                this.propertyType;
        }

        public class StandardValuesCollection : ICollection, IEnumerable
        {
            private Array valueArray;
            private ICollection values;

            public StandardValuesCollection(ICollection values)
            {
                if (values == null)
                {
                    values = new object[0];
                }
                Array array = values as Array;
                if (array != null)
                {
                    this.valueArray = array;
                }
                this.values = values;
            }

            public void CopyTo(Array array, int index)
            {
                this.values.CopyTo(array, index);
            }

            public IEnumerator GetEnumerator() => 
                this.values.GetEnumerator();

            void ICollection.CopyTo(Array array, int index)
            {
                this.CopyTo(array, index);
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();

            public int Count
            {
                get
                {
                    if (this.valueArray != null)
                    {
                        return this.valueArray.Length;
                    }
                    return this.values.Count;
                }
            }

            public object this[int index] =>
                this.valueArray?.GetValue(index);

            int ICollection.Count =>
                this.Count;

            bool ICollection.IsSynchronized =>
                false;

            object ICollection.SyncRoot =>
                null;
        }
    }
}

