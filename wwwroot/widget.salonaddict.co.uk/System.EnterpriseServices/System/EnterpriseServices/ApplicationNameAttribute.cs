namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=true), ComVisible(false)]
    public sealed class ApplicationNameAttribute : Attribute, IConfigurationAttribute
    {
        private string _value;

        public ApplicationNameAttribute(string name)
        {
            this._value = name;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.MTS, "ApplicationNameAttribute");
            ((ICatalogObject) info["Application"]).SetValue("Name", this._value);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Application");

        public string Value =>
            this._value;
    }
}

