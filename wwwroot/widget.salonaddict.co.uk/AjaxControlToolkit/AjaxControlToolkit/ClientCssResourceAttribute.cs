namespace AjaxControlToolkit
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class ClientCssResourceAttribute : Attribute
    {
        private int _loadOrder;
        private string _resourcePath;

        public ClientCssResourceAttribute(string fullResourceName)
        {
            if (fullResourceName == null)
            {
                throw new ArgumentNullException("fullResourceName");
            }
            this._resourcePath = fullResourceName;
        }

        public ClientCssResourceAttribute(Type baseType, string resourceName)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException("baseType");
            }
            if (resourceName == null)
            {
                throw new ArgumentNullException("resourceName");
            }
            string fullName = baseType.FullName;
            int length = fullName.LastIndexOf('.');
            if (length != -1)
            {
                fullName = fullName.Substring(0, length);
            }
            this._resourcePath = fullName + '.' + resourceName;
        }

        public int LoadOrder
        {
            get => 
                this._loadOrder;
            set
            {
                this._loadOrder = value;
            }
        }

        public string ResourcePath =>
            this._resourcePath;
    }
}

