namespace AjaxControlToolkit.Design
{
    using System;

    [AttributeUsage(AttributeTargets.Delegate, AllowMultiple=false, Inherited=true)]
    public sealed class PageMethodSignatureAttribute : Attribute
    {
        private string _friendlyName;
        private string _serviceMethodProperty;
        private string _servicePathProperty;
        private string _useContextKeyProperty;

        public PageMethodSignatureAttribute(string friendlyName, string servicePathProperty, string serviceMethodProperty) : this(friendlyName, servicePathProperty, serviceMethodProperty, null)
        {
        }

        public PageMethodSignatureAttribute(string friendlyName, string servicePathProperty, string serviceMethodProperty, string useContextKeyProperty)
        {
            this._friendlyName = friendlyName;
            this._servicePathProperty = servicePathProperty;
            this._serviceMethodProperty = serviceMethodProperty;
            this._useContextKeyProperty = useContextKeyProperty;
        }

        public string FriendlyName =>
            this._friendlyName;

        public bool IncludeContextParameter =>
            !string.IsNullOrEmpty(this._useContextKeyProperty);

        public string ServiceMethodProperty =>
            this._serviceMethodProperty;

        public string ServicePathProperty =>
            this._servicePathProperty;

        public string UseContextKeyProperty =>
            this._useContextKeyProperty;
    }
}

