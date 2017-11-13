namespace AjaxControlToolkit
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class ClientScriptResourceAttribute : Attribute
    {
        private string _componentType;
        private int _loadOrder;
        private string _resourcePath;

        public ClientScriptResourceAttribute()
        {
        }

        public ClientScriptResourceAttribute(string componentType)
        {
            this._componentType = componentType;
        }

        public ClientScriptResourceAttribute(string componentType, string fullResourceName) : this(componentType)
        {
            if (fullResourceName == null)
            {
                throw new ArgumentNullException("fullResourceName");
            }
            this.ResourcePath = fullResourceName;
        }

        public ClientScriptResourceAttribute(string componentType, Type baseType, string resourceName)
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
            this.ResourcePath = fullName + "." + resourceName;
            this._componentType = componentType;
        }

        public override bool IsDefaultAttribute() => 
            ((this.ComponentType == null) && (this.ResourcePath == null));

        public string ComponentType
        {
            get => 
                this._componentType;
            set
            {
                this._componentType = value;
            }
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

        public string ResourcePath
        {
            get => 
                this._resourcePath;
            set
            {
                this._resourcePath = value;
            }
        }
    }
}

