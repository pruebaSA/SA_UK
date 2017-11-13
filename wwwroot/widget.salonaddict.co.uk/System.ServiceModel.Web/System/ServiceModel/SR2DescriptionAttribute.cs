namespace System.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Resources;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SR2DescriptionAttribute : DescriptionAttribute
    {
        public SR2DescriptionAttribute(string description)
        {
            base.DescriptionValue = SR2.ResourceManager.GetString(description, SR2.Culture);
        }

        public SR2DescriptionAttribute(string description, string resourceSet)
        {
            base.DescriptionValue = new ResourceManager(resourceSet, Assembly.GetExecutingAssembly()).GetString(description);
        }
    }
}

