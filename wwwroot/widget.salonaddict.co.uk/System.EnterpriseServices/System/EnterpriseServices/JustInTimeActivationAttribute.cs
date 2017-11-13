namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Class, Inherited=true), ComVisible(false)]
    public sealed class JustInTimeActivationAttribute : Attribute, IConfigurationAttribute
    {
        private bool _enabled;

        public JustInTimeActivationAttribute() : this(true)
        {
        }

        public JustInTimeActivationAttribute(bool val)
        {
            this._enabled = val;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "JustInTimeActivationAttribute");
            ICatalogObject obj2 = (ICatalogObject) info["Component"];
            obj2.SetValue("JustInTimeActivation", this._enabled);
            if (this._enabled && (((int) obj2.GetValue("Synchronization")) == 0))
            {
                obj2.SetValue("Synchronization", SynchronizationOption.Required);
            }
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");

        public bool Value =>
            this._enabled;
    }
}

