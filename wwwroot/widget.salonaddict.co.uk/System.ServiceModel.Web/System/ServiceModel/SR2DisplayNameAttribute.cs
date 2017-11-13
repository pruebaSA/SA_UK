namespace System.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Resources;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SR2DisplayNameAttribute : DisplayNameAttribute
    {
        public SR2DisplayNameAttribute(string name)
        {
            base.DisplayNameValue = SR2.ResourceManager.GetString(name, SR2.Culture);
        }

        public SR2DisplayNameAttribute(string name, string resourceSet)
        {
            base.DisplayNameValue = new ResourceManager(resourceSet, Assembly.GetExecutingAssembly()).GetString(name);
        }
    }
}

