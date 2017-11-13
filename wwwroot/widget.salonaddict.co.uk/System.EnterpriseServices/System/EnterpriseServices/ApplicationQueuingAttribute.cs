namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=true), ComVisible(false)]
    public sealed class ApplicationQueuingAttribute : Attribute, IConfigurationAttribute
    {
        private bool _enabled = true;
        private bool _listen = false;
        private int _maxthreads = 0;

        bool IConfigurationAttribute.AfterSaveChanges(Hashtable info) => 
            false;

        bool IConfigurationAttribute.Apply(Hashtable info)
        {
            Platform.Assert(Platform.W2K, "ApplicationQueueingAttribute");
            ICatalogObject obj2 = (ICatalogObject) info["Application"];
            obj2.SetValue("QueuingEnabled", this._enabled);
            obj2.SetValue("QueueListenerEnabled", this._listen);
            if (this._maxthreads != 0)
            {
                Platform.Assert(Platform.Whistler, "ApplicationQueuingAttribute.MaxListenerThreads");
                obj2.SetValue("QCListenerMaxThreads", this._maxthreads);
            }
            return true;
        }

        bool IConfigurationAttribute.IsValidTarget(string s) => 
            (s == "Application");

        public bool Enabled
        {
            get => 
                this._enabled;
            set
            {
                this._enabled = value;
            }
        }

        public int MaxListenerThreads
        {
            get => 
                this._maxthreads;
            set
            {
                this._maxthreads = value;
            }
        }

        public bool QueueListenerEnabled
        {
            get => 
                this._listen;
            set
            {
                this._listen = value;
            }
        }
    }
}

