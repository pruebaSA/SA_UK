namespace AjaxControlToolkit
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public sealed class ExtenderControlMethodAttribute : Attribute
    {
        private bool _isScriptMethod;
        private static ExtenderControlMethodAttribute Default = No;
        private static ExtenderControlMethodAttribute No = new ExtenderControlMethodAttribute(false);
        private static ExtenderControlMethodAttribute Yes = new ExtenderControlMethodAttribute(true);

        public ExtenderControlMethodAttribute() : this(true)
        {
        }

        public ExtenderControlMethodAttribute(bool isScriptMethod)
        {
            this._isScriptMethod = isScriptMethod;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }
            ExtenderControlMethodAttribute attribute = obj as ExtenderControlMethodAttribute;
            return ((attribute != null) && (attribute._isScriptMethod == this._isScriptMethod));
        }

        public override int GetHashCode() => 
            this._isScriptMethod.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public bool IsScriptMethod =>
            this._isScriptMethod;
    }
}

