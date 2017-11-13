namespace System.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ConfigurationPropertyAttribute : Attribute
    {
        private object _DefaultValue = ConfigurationElement.s_nullPropertyValue;
        private ConfigurationPropertyOptions _Flags;
        private string _Name;
        internal static readonly string DefaultCollectionPropertyName = "";

        public ConfigurationPropertyAttribute(string name)
        {
            this._Name = name;
        }

        public object DefaultValue
        {
            get => 
                this._DefaultValue;
            set
            {
                this._DefaultValue = value;
            }
        }

        public bool IsDefaultCollection
        {
            get => 
                ((this.Options & ConfigurationPropertyOptions.IsDefaultCollection) != ConfigurationPropertyOptions.None);
            set
            {
                if (value)
                {
                    this.Options |= ConfigurationPropertyOptions.IsDefaultCollection;
                }
                else
                {
                    this.Options &= ~ConfigurationPropertyOptions.IsDefaultCollection;
                }
            }
        }

        public bool IsKey
        {
            get => 
                ((this.Options & ConfigurationPropertyOptions.IsKey) != ConfigurationPropertyOptions.None);
            set
            {
                if (value)
                {
                    this.Options |= ConfigurationPropertyOptions.IsKey;
                }
                else
                {
                    this.Options &= ~ConfigurationPropertyOptions.IsKey;
                }
            }
        }

        public bool IsRequired
        {
            get => 
                ((this.Options & ConfigurationPropertyOptions.IsRequired) != ConfigurationPropertyOptions.None);
            set
            {
                if (value)
                {
                    this.Options |= ConfigurationPropertyOptions.IsRequired;
                }
                else
                {
                    this.Options &= ~ConfigurationPropertyOptions.IsRequired;
                }
            }
        }

        public string Name =>
            this._Name;

        public ConfigurationPropertyOptions Options
        {
            get => 
                this._Flags;
            set
            {
                this._Flags = value;
            }
        }
    }
}

