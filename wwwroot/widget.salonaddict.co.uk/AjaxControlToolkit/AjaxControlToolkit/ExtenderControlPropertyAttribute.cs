namespace AjaxControlToolkit
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ExtenderControlPropertyAttribute : Attribute
    {
        private bool _isScriptProperty;
        private bool _useJsonSerialization;
        private static ExtenderControlPropertyAttribute Default = No;
        private static ExtenderControlPropertyAttribute No = new ExtenderControlPropertyAttribute(false);
        private static ExtenderControlPropertyAttribute Yes = new ExtenderControlPropertyAttribute(true);

        public ExtenderControlPropertyAttribute() : this(true)
        {
        }

        public ExtenderControlPropertyAttribute(bool isScriptProperty) : this(isScriptProperty, false)
        {
        }

        public ExtenderControlPropertyAttribute(bool isScriptProperty, bool useJsonSerialization)
        {
            this._isScriptProperty = isScriptProperty;
            this._useJsonSerialization = useJsonSerialization;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }
            ExtenderControlPropertyAttribute attribute = obj as ExtenderControlPropertyAttribute;
            return ((attribute != null) && (attribute._isScriptProperty == this._isScriptProperty));
        }

        public override int GetHashCode() => 
            this._isScriptProperty.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public bool IsScriptProperty =>
            this._isScriptProperty;

        public bool UseJsonSerialization =>
            this._useJsonSerialization;
    }
}

