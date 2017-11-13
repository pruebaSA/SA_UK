namespace System.Management.Instrumentation
{
    using System;
    using System.Security.Permissions;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class ManagementEntityAttribute : Attribute
    {
        private bool _isExternalClass;
        private bool _isSingleton;
        private string _nounName;

        public bool External
        {
            get => 
                this._isExternalClass;
            set
            {
                this._isExternalClass = value;
            }
        }

        public string Name
        {
            get => 
                this._nounName;
            set
            {
                this._nounName = value;
            }
        }

        public bool Singleton
        {
            get => 
                this._isSingleton;
            set
            {
                this._isSingleton = value;
            }
        }
    }
}

