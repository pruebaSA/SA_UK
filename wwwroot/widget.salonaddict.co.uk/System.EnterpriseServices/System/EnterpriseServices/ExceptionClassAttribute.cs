namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Class, Inherited=true), ComVisible(false)]
    public sealed class ExceptionClassAttribute : Attribute, IConfigurationAttribute
    {
        private string _value;

        public ExceptionClassAttribute(string name)
        {
            this._value = name;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "ExceptionClassAttribute");
            ((ICatalogObject) info["Component"]).SetValue("ExceptionClass", this._value);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");

        public string Value =>
            this._value;
    }
}

