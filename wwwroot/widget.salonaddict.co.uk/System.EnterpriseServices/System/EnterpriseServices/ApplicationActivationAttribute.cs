namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=true), ComVisible(false)]
    public sealed class ApplicationActivationAttribute : Attribute, IConfigurationAttribute
    {
        private string _SoapMailbox;
        private string _SoapVRoot;
        private ActivationOption _value;

        public ApplicationActivationAttribute(ActivationOption opt)
        {
            this._value = opt;
        }

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info)
        {
            bool flag = false;
            if (this._SoapVRoot != null)
            {
                ICatalogObject obj2 = (ICatalogObject) info["Application"];
                Platform.Assert(Platform.Whistler, "ApplicationActivationAttribute.SoapVRoot");
                obj2.SetValue("SoapActivated", true);
                obj2.SetValue("SoapVRoot", this._SoapVRoot);
                flag = true;
            }
            if (this._SoapMailbox != null)
            {
                ICatalogObject obj3 = (ICatalogObject) info["Application"];
                Platform.Assert(Platform.Whistler, "ApplicationActivationAttribute.SoapMailbox");
                obj3.SetValue("SoapActivated", true);
                obj3.SetValue("SoapMailTo", this._SoapMailbox);
                flag = true;
            }
            return flag;
        }

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.MTS, "ApplicationActivationAttribute");
            ICatalogObject obj2 = (ICatalogObject) info["Application"];
            if (Platform.IsLessThan(Platform.W2K))
            {
                switch (this._value)
                {
                    case ActivationOption.Library:
                        obj2.SetValue("Activation", "Inproc");
                        break;

                    case ActivationOption.Server:
                        obj2.SetValue("Activation", "Local");
                        break;
                }
            }
            else
            {
                obj2.SetValue("Activation", this._value);
            }
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Application");

        public string SoapMailbox
        {
            get => 
                this._SoapMailbox;
            set
            {
                this._SoapMailbox = value;
            }
        }

        public string SoapVRoot
        {
            get => 
                this._SoapVRoot;
            set
            {
                this._SoapVRoot = value;
            }
        }

        public ActivationOption Value =>
            this._value;
    }
}

