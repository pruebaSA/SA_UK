namespace System.EnterpriseServices.CompensatingResourceManager
{
    using System;
    using System.Collections;
    using System.EnterpriseServices;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [ComVisible(false), ProgId("System.EnterpriseServices.Crm.ApplicationCrmEnabledAttribute"), AttributeUsage(AttributeTargets.Assembly, Inherited=true)]
    public sealed class ApplicationCrmEnabledAttribute : Attribute, IConfigurationAttribute
    {
        private bool _value;

        public ApplicationCrmEnabledAttribute() : this(true)
        {
        }

        public ApplicationCrmEnabledAttribute(bool val)
        {
            this._value = val;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "CrmEnabledAttribute");
            ((ICatalogObject) info["Application"]).SetValue("CRMEnabled", this._value);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Application");

        public bool Value =>
            this._value;
    }
}

