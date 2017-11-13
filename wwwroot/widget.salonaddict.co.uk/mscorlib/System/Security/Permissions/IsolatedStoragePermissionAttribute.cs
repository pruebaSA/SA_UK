namespace System.Security.Permissions
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true, Inherited=false), ComVisible(true)]
    public abstract class IsolatedStoragePermissionAttribute : CodeAccessSecurityAttribute
    {
        internal IsolatedStorageContainment m_allowed;
        internal long m_userQuota;

        protected IsolatedStoragePermissionAttribute(SecurityAction action) : base(action)
        {
        }

        public IsolatedStorageContainment UsageAllowed
        {
            get => 
                this.m_allowed;
            set
            {
                this.m_allowed = value;
            }
        }

        public long UserQuota
        {
            get => 
                this.m_userQuota;
            set
            {
                this.m_userQuota = value;
            }
        }
    }
}

