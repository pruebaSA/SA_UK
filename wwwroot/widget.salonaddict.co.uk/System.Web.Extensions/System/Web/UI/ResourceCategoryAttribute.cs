namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Web.Resources;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class ResourceCategoryAttribute : CategoryAttribute
    {
        internal ResourceCategoryAttribute(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value)
        {
            string localizedString = base.GetLocalizedString(value);
            if (localizedString == null)
            {
                localizedString = AtlasWeb.ResourceManager.GetString("Category_" + value, AtlasWeb.Culture);
            }
            return localizedString;
        }

        public override object TypeId =>
            typeof(CategoryAttribute);
    }
}

