﻿namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [ComVisible(false), AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public sealed class IISIntrinsicsAttribute : Attribute, IConfigurationAttribute
    {
        private bool _value;

        public IISIntrinsicsAttribute() : this(true)
        {
        }

        public IISIntrinsicsAttribute(bool val)
        {
            this._value = val;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "IISIntrinsicsAttribute");
            ((ICatalogObject) info["Component"]).SetValue("IISIntrinsics", this._value);
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");

        public bool Value =>
            this._value;
    }
}

