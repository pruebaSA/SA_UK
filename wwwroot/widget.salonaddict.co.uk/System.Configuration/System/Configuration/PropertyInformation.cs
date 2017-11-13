namespace System.Configuration
{
    using System;
    using System.ComponentModel;

    public sealed class PropertyInformation
    {
        private ConfigurationProperty _Prop;
        private const string LockAll = "*";
        private string PropertyName;
        private ConfigurationElement ThisElement;

        internal PropertyInformation(ConfigurationElement thisElement, string propertyName)
        {
            this.PropertyName = propertyName;
            this.ThisElement = thisElement;
        }

        public TypeConverter Converter =>
            this.Prop.Converter;

        public object DefaultValue =>
            this.Prop.DefaultValue;

        public string Description =>
            this.Prop.Description;

        public bool IsKey =>
            this.Prop.IsKey;

        public bool IsLocked
        {
            get
            {
                if (((this.ThisElement.LockedAllExceptAttributesList == null) || this.ThisElement.LockedAllExceptAttributesList.DefinedInParent(this.PropertyName)) && ((this.ThisElement.LockedAttributesList == null) || (!this.ThisElement.LockedAttributesList.DefinedInParent(this.PropertyName) && !this.ThisElement.LockedAttributesList.DefinedInParent("*"))))
                {
                    return (((this.ThisElement.ItemLocked & ConfigurationValueFlags.Locked) != ConfigurationValueFlags.Default) && ((this.ThisElement.ItemLocked & ConfigurationValueFlags.Inherited) != ConfigurationValueFlags.Default));
                }
                return true;
            }
        }

        public bool IsModified
        {
            get
            {
                if (this.ThisElement.Values[this.PropertyName] == null)
                {
                    return false;
                }
                return this.ThisElement.Values.IsModified(this.PropertyName);
            }
        }

        public bool IsRequired =>
            this.Prop.IsRequired;

        public int LineNumber
        {
            get
            {
                PropertySourceInfo sourceInfo = this.ThisElement.Values.GetSourceInfo(this.PropertyName);
                return sourceInfo?.LineNumber;
            }
        }

        public string Name =>
            this.PropertyName;

        private ConfigurationProperty Prop
        {
            get
            {
                if (this._Prop == null)
                {
                    this._Prop = this.ThisElement.Properties[this.PropertyName];
                }
                return this._Prop;
            }
        }

        internal string ProvidedName =>
            this.Prop.ProvidedName;

        public string Source
        {
            get
            {
                PropertySourceInfo sourceInfo = this.ThisElement.Values.GetSourceInfo(this.PropertyName);
                return sourceInfo?.FileName;
            }
        }

        public System.Type Type =>
            this.Prop.Type;

        public ConfigurationValidatorBase Validator =>
            this.Prop.Validator;

        public object Value
        {
            get => 
                this.ThisElement[this.PropertyName];
            set
            {
                this.ThisElement[this.PropertyName] = value;
            }
        }

        public PropertyValueOrigin ValueOrigin
        {
            get
            {
                if (this.ThisElement.Values[this.PropertyName] == null)
                {
                    return PropertyValueOrigin.Default;
                }
                if (this.ThisElement.Values.IsInherited(this.PropertyName))
                {
                    return PropertyValueOrigin.Inherited;
                }
                return PropertyValueOrigin.SetHere;
            }
        }
    }
}

