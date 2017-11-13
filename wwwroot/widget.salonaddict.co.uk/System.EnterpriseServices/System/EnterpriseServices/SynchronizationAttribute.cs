namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [ComVisible(false), AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public sealed class SynchronizationAttribute : Attribute, IConfigurationAttribute
    {
        private SynchronizationOption _value;

        public SynchronizationAttribute() : this(SynchronizationOption.Required)
        {
        }

        public SynchronizationAttribute(SynchronizationOption val)
        {
            this._value = val;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "SynchronizationAttribute");
            ((ICatalogObject) info["Component"]).SetValue("Synchronization", this._value);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");

        public SynchronizationOption Value =>
            this._value;
    }
}

