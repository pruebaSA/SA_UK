namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Class, Inherited=true), ComVisible(false)]
    public sealed class ComponentAccessControlAttribute : Attribute, IConfigurationAttribute
    {
        private bool _value;

        public ComponentAccessControlAttribute() : this(true)
        {
        }

        public ComponentAccessControlAttribute(bool val)
        {
            this._value = val;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.MTS, "ComponentAccessControlAttribute");
            ICatalogObject obj2 = (ICatalogObject) info["Component"];
            if (Platform.IsLessThan(Platform.W2K))
            {
                obj2.SetValue("SecurityEnabled", this._value ? "Y" : "N");
            }
            else
            {
                obj2.SetValue("ComponentAccessChecksEnabled", this._value);
            }
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");

        public bool Value =>
            this._value;
    }
}

