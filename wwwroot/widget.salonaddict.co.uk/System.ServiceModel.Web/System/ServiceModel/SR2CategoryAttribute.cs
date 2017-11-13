namespace System.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Resources;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SR2CategoryAttribute : CategoryAttribute
    {
        private string resourceSet;

        public SR2CategoryAttribute(string category) : base(category)
        {
            this.resourceSet = string.Empty;
        }

        public SR2CategoryAttribute(string category, string resourceSet) : base(category)
        {
            this.resourceSet = string.Empty;
            this.resourceSet = resourceSet;
        }

        protected override string GetLocalizedString(string value)
        {
            if (this.resourceSet.Length > 0)
            {
                ResourceManager manager = new ResourceManager(this.resourceSet, Assembly.GetExecutingAssembly());
                return manager.GetString(value);
            }
            return SR2.ResourceManager.GetString(value, SR2.Culture);
        }
    }
}

