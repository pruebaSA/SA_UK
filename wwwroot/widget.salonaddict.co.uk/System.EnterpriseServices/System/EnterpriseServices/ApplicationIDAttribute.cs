namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=true), ComVisible(false)]
    public sealed class ApplicationIDAttribute : Attribute, IConfigurationAttribute
    {
        private Guid _value;

        public ApplicationIDAttribute(string guid)
        {
            this._value = new Guid(guid);
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info) => 
            false;

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Application");

        public Guid Value =>
            this._value;
    }
}

