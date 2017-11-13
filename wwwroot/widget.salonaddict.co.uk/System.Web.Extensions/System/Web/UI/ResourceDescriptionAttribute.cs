namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Web.Resources;

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Property | AttributeTargets.Class, Inherited=true, AllowMultiple=false)]
    internal sealed class ResourceDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _descriptionResourceName;
        private bool _resourceLoaded;

        public ResourceDescriptionAttribute(string descriptionResourceName)
        {
            this._descriptionResourceName = descriptionResourceName;
        }

        public override string Description
        {
            get
            {
                if (!this._resourceLoaded)
                {
                    this._resourceLoaded = true;
                    base.DescriptionValue = AtlasWeb.ResourceManager.GetString(this._descriptionResourceName, AtlasWeb.Culture);
                }
                return base.Description;
            }
        }
    }
}

