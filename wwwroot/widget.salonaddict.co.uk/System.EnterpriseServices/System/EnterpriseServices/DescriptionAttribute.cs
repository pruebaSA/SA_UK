namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [ComVisible(false), AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, Inherited=true)]
    public sealed class DescriptionAttribute : Attribute, IConfigurationAttribute
    {
        private string _desc;

        public DescriptionAttribute(string desc)
        {
            this._desc = desc;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.MTS, "DescriptionAttribute");
            string str = (string) info["CurrentTarget"];
            ((ICatalogObject) info[str]).SetValue("Description", this.Description);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s)
        {
            if (((s != "Application") && (s != "Component")) && (s != "Interface"))
            {
                return (s == "Method");
            }
            return true;
        }

        private string Description =>
            this._desc;
    }
}

