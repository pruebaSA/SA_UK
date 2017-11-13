namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Method, Inherited=true), ComVisible(false)]
    public sealed class AutoCompleteAttribute : Attribute, IConfigurationAttribute
    {
        private bool _value;

        public AutoCompleteAttribute() : this(true)
        {
        }

        public AutoCompleteAttribute(bool val)
        {
            this._value = val;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "AutoCompleteAttribute");
            ((ICatalogObject) info["Method"]).SetValue("AutoComplete", this._value);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Method");

        public bool Value =>
            this._value;
    }
}

