namespace System.Web.Profile
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ProfileGroupBase
    {
        private string _MyName = null;
        private ProfileBase _Parent = null;

        public object GetPropertyValue(string propertyName) => 
            this._Parent[this._MyName + propertyName];

        public void Init(ProfileBase parent, string myName)
        {
            if (this._Parent == null)
            {
                this._Parent = parent;
                this._MyName = myName + ".";
            }
        }

        public void SetPropertyValue(string propertyName, object propertyValue)
        {
            this._Parent[this._MyName + propertyName] = propertyValue;
        }

        public object this[string propertyName]
        {
            get => 
                this._Parent[this._MyName + propertyName];
            set
            {
                this._Parent[this._MyName + propertyName] = value;
            }
        }
    }
}

