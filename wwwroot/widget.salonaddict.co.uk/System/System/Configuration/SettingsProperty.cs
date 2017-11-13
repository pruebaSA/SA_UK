namespace System.Configuration
{
    using System;

    public class SettingsProperty
    {
        private SettingsAttributeDictionary _Attributes;
        private object _DefaultValue;
        private bool _IsReadOnly;
        private string _Name;
        private Type _PropertyType;
        private SettingsProvider _Provider;
        private SettingsSerializeAs _SerializeAs;
        private bool _ThrowOnErrorDeserializing;
        private bool _ThrowOnErrorSerializing;

        public SettingsProperty(SettingsProperty propertyToCopy)
        {
            this._Name = propertyToCopy.Name;
            this._IsReadOnly = propertyToCopy.IsReadOnly;
            this._DefaultValue = propertyToCopy.DefaultValue;
            this._SerializeAs = propertyToCopy.SerializeAs;
            this._Provider = propertyToCopy.Provider;
            this._PropertyType = propertyToCopy.PropertyType;
            this._ThrowOnErrorDeserializing = propertyToCopy.ThrowOnErrorDeserializing;
            this._ThrowOnErrorSerializing = propertyToCopy.ThrowOnErrorSerializing;
            this._Attributes = new SettingsAttributeDictionary(propertyToCopy.Attributes);
        }

        public SettingsProperty(string name)
        {
            this._Name = name;
            this._Attributes = new SettingsAttributeDictionary();
        }

        public SettingsProperty(string name, Type propertyType, SettingsProvider provider, bool isReadOnly, object defaultValue, SettingsSerializeAs serializeAs, SettingsAttributeDictionary attributes, bool throwOnErrorDeserializing, bool throwOnErrorSerializing)
        {
            this._Name = name;
            this._PropertyType = propertyType;
            this._Provider = provider;
            this._IsReadOnly = isReadOnly;
            this._DefaultValue = defaultValue;
            this._SerializeAs = serializeAs;
            this._Attributes = attributes;
            this._ThrowOnErrorDeserializing = throwOnErrorDeserializing;
            this._ThrowOnErrorSerializing = throwOnErrorSerializing;
        }

        public virtual SettingsAttributeDictionary Attributes =>
            this._Attributes;

        public virtual object DefaultValue
        {
            get => 
                this._DefaultValue;
            set
            {
                this._DefaultValue = value;
            }
        }

        public virtual bool IsReadOnly
        {
            get => 
                this._IsReadOnly;
            set
            {
                this._IsReadOnly = value;
            }
        }

        public virtual string Name
        {
            get => 
                this._Name;
            set
            {
                this._Name = value;
            }
        }

        public virtual Type PropertyType
        {
            get => 
                this._PropertyType;
            set
            {
                this._PropertyType = value;
            }
        }

        public virtual SettingsProvider Provider
        {
            get => 
                this._Provider;
            set
            {
                this._Provider = value;
            }
        }

        public virtual SettingsSerializeAs SerializeAs
        {
            get => 
                this._SerializeAs;
            set
            {
                this._SerializeAs = value;
            }
        }

        public bool ThrowOnErrorDeserializing
        {
            get => 
                this._ThrowOnErrorDeserializing;
            set
            {
                this._ThrowOnErrorDeserializing = value;
            }
        }

        public bool ThrowOnErrorSerializing
        {
            get => 
                this._ThrowOnErrorSerializing;
            set
            {
                this._ThrowOnErrorSerializing = value;
            }
        }
    }
}

