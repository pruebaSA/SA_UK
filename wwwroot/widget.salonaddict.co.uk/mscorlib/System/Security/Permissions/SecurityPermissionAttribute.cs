namespace System.Security.Permissions
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [Serializable, AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true, Inherited=false), ComVisible(true)]
    public sealed class SecurityPermissionAttribute : CodeAccessSecurityAttribute
    {
        private SecurityPermissionFlag m_flag;

        public SecurityPermissionAttribute(SecurityAction action) : base(action)
        {
        }

        public override IPermission CreatePermission()
        {
            if (base.m_unrestricted)
            {
                return new SecurityPermission(PermissionState.Unrestricted);
            }
            return new SecurityPermission(this.m_flag);
        }

        public bool Assertion
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.Assertion) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.Assertion) : (this.m_flag & ~SecurityPermissionFlag.Assertion);
            }
        }

        public bool BindingRedirects
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.BindingRedirects) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.BindingRedirects) : (this.m_flag & ~SecurityPermissionFlag.BindingRedirects);
            }
        }

        public bool ControlAppDomain
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.ControlAppDomain) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.ControlAppDomain) : (this.m_flag & ~SecurityPermissionFlag.ControlAppDomain);
            }
        }

        public bool ControlDomainPolicy
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.ControlDomainPolicy) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.ControlDomainPolicy) : (this.m_flag & ~SecurityPermissionFlag.ControlDomainPolicy);
            }
        }

        public bool ControlEvidence
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.ControlEvidence) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.ControlEvidence) : (this.m_flag & ~SecurityPermissionFlag.ControlEvidence);
            }
        }

        public bool ControlPolicy
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.ControlPolicy) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.ControlPolicy) : (this.m_flag & ~SecurityPermissionFlag.ControlPolicy);
            }
        }

        public bool ControlPrincipal
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.ControlPrincipal) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.ControlPrincipal) : (this.m_flag & ~SecurityPermissionFlag.ControlPrincipal);
            }
        }

        public bool ControlThread
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.ControlThread) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.ControlThread) : (this.m_flag & ~SecurityPermissionFlag.ControlThread);
            }
        }

        public bool Execution
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.Execution) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.Execution) : (this.m_flag & ~SecurityPermissionFlag.Execution);
            }
        }

        public SecurityPermissionFlag Flags
        {
            get => 
                this.m_flag;
            set
            {
                this.m_flag = value;
            }
        }

        [ComVisible(true)]
        public bool Infrastructure
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.Infrastructure) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.Infrastructure) : (this.m_flag & ~SecurityPermissionFlag.Infrastructure);
            }
        }

        public bool RemotingConfiguration
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.RemotingConfiguration) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.RemotingConfiguration) : (this.m_flag & ~SecurityPermissionFlag.RemotingConfiguration);
            }
        }

        public bool SerializationFormatter
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.SerializationFormatter) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.SerializationFormatter) : (this.m_flag & ~SecurityPermissionFlag.SerializationFormatter);
            }
        }

        public bool SkipVerification
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.SkipVerification) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.SkipVerification) : (this.m_flag & ~SecurityPermissionFlag.SkipVerification);
            }
        }

        public bool UnmanagedCode
        {
            get => 
                ((this.m_flag & SecurityPermissionFlag.UnmanagedCode) != SecurityPermissionFlag.NoFlags);
            set
            {
                this.m_flag = value ? (this.m_flag | SecurityPermissionFlag.UnmanagedCode) : (this.m_flag & ~SecurityPermissionFlag.UnmanagedCode);
            }
        }
    }
}

