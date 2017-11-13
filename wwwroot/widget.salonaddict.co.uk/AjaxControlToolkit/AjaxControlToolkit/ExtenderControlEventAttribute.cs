namespace AjaxControlToolkit
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited=true)]
    public sealed class ExtenderControlEventAttribute : Attribute
    {
        private bool _isScriptEvent;
        private static ExtenderControlEventAttribute Default = No;
        private static ExtenderControlEventAttribute No = new ExtenderControlEventAttribute(false);
        private static ExtenderControlEventAttribute Yes = new ExtenderControlEventAttribute(true);

        public ExtenderControlEventAttribute() : this(true)
        {
        }

        public ExtenderControlEventAttribute(bool isScriptEvent)
        {
            this._isScriptEvent = isScriptEvent;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, this))
            {
                return true;
            }
            ExtenderControlEventAttribute attribute = obj as ExtenderControlEventAttribute;
            return ((attribute != null) && (attribute._isScriptEvent == this._isScriptEvent));
        }

        public override int GetHashCode() => 
            this._isScriptEvent.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public bool IsScriptEvent =>
            this._isScriptEvent;
    }
}

