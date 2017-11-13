namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Web.Resources;

    [AttributeUsage(AttributeTargets.Class, Inherited=true, AllowMultiple=false)]
    internal sealed class ResourceDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _displayNameResourceName;
        private bool _resourceLoaded;

        public ResourceDisplayNameAttribute(string displayNameResourceName)
        {
            this._displayNameResourceName = displayNameResourceName;
        }

        public override string DisplayName
        {
            get
            {
                if (!this._resourceLoaded)
                {
                    this._resourceLoaded = true;
                    base.DisplayNameValue = AtlasWeb.ResourceManager.GetString(this._displayNameResourceName, AtlasWeb.Culture);
                }
                return base.DisplayName;
            }
        }
    }
}

