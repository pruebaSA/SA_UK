namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [ComVisible(false), AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public sealed class MustRunInClientContextAttribute : Attribute, IConfigurationAttribute
    {
        private bool _value;

        public MustRunInClientContextAttribute() : this(true)
        {
        }

        public MustRunInClientContextAttribute(bool val)
        {
            this._value = val;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "MustRunInClientContextAttribute");
            ((ICatalogObject) info["Component"]).SetValue("MustRunInClientContext", this._value);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");

        public bool Value =>
            this._value;
    }
}

