namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [ComVisible(false), AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public sealed class EventClassAttribute : Attribute, IConfigurationAttribute
    {
        private bool _allowInprocSubscribers = true;
        private string _filter = null;
        private bool _fireInParallel = false;

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "EventClassAttribute");
            ICatalogObject obj2 = (ICatalogObject) info["Component"];
            obj2.SetValue("FireInParallel", this._fireInParallel);
            obj2.SetValue("AllowInprocSubscribers", this._allowInprocSubscribers);
            if (this._filter != null)
            {
                obj2.SetValue("MultiInterfacePublisherFilterCLSID", this._filter);
            }
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Component");

        public bool AllowInprocSubscribers
        {
            get => 
                this._allowInprocSubscribers;
            set
            {
                this._allowInprocSubscribers = value;
            }
        }

        public bool FireInParallel
        {
            get => 
                this._fireInParallel;
            set
            {
                this._fireInParallel = value;
            }
        }

        public string PublisherFilter
        {
            get => 
                this._filter;
            set
            {
                this._filter = value;
            }
        }
    }
}

