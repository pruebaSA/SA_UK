namespace System.IdentityModel.Tokens
{
    using System;
    using System.IdentityModel;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;

    public class X509WindowsSecurityToken : X509SecurityToken
    {
        private bool disposed;
        private System.Security.Principal.WindowsIdentity windowsIdentity;

        public X509WindowsSecurityToken(X509Certificate2 certificate, System.Security.Principal.WindowsIdentity windowsIdentity) : this(certificate, windowsIdentity, true)
        {
        }

        internal X509WindowsSecurityToken(X509Certificate2 certificate, System.Security.Principal.WindowsIdentity windowsIdentity, bool clone) : this(certificate, windowsIdentity, SecurityUniqueId.Create().Value, clone)
        {
        }

        public X509WindowsSecurityToken(X509Certificate2 certificate, System.Security.Principal.WindowsIdentity windowsIdentity, string id) : this(certificate, windowsIdentity, id, true)
        {
        }

        internal X509WindowsSecurityToken(X509Certificate2 certificate, System.Security.Principal.WindowsIdentity windowsIdentity, string id, bool clone) : base(certificate, id, clone)
        {
            if (windowsIdentity == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("windowsIdentity");
            }
            this.windowsIdentity = clone ? System.IdentityModel.SecurityUtils.CloneWindowsIdentityIfNecessary(windowsIdentity) : windowsIdentity;
        }

        public override void Dispose()
        {
            try
            {
                if (!this.disposed)
                {
                    this.disposed = true;
                    this.windowsIdentity.Dispose();
                }
            }
            finally
            {
                base.Dispose();
            }
        }

        public System.Security.Principal.WindowsIdentity WindowsIdentity
        {
            get
            {
                base.ThrowIfDisposed();
                return this.windowsIdentity;
            }
        }
    }
}

