namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Class, Inherited=true), ComVisible(false)]
    public sealed class PrivateComponentAttribute : Attribute, IConfigurationAttribute
    {
        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.Whistler, "PrivateComponentAttribute");
            ((ICatalogObject) info["Component"]).SetValue("IsPrivateComponent", true);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");
    }
}

