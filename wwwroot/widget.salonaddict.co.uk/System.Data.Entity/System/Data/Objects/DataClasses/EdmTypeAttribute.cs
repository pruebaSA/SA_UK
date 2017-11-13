namespace System.Data.Objects.DataClasses
{
    using System;

    public abstract class EdmTypeAttribute : Attribute
    {
        private string _namespaceName;
        private string _typeName;

        internal EdmTypeAttribute()
        {
        }

        public string Name
        {
            get => 
                this._typeName;
            set
            {
                this._typeName = value;
            }
        }

        public string NamespaceName
        {
            get => 
                this._namespaceName;
            set
            {
                this._namespaceName = value;
            }
        }
    }
}

