namespace System.Security
{
    using System;
    using System.Collections;
    using System.Deployment.Internal.Isolation.Manifest;
    using System.Reflection;
    using System.Runtime.Hosting;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Policy;

    [Serializable, ComVisible(true), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class HostSecurityManager
    {
        [SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
        public virtual ApplicationTrust DetermineApplicationTrust(Evidence applicationEvidence, Evidence activatorEvidence, TrustManagerContext context)
        {
            if (applicationEvidence == null)
            {
                throw new ArgumentNullException("applicationEvidence");
            }
            IEnumerator hostEnumerator = applicationEvidence.GetHostEnumerator();
            ActivationArguments current = null;
            ApplicationTrust applicationTrust = null;
            while (hostEnumerator.MoveNext())
            {
                if (current == null)
                {
                    current = hostEnumerator.Current as ActivationArguments;
                }
                if (applicationTrust == null)
                {
                    applicationTrust = hostEnumerator.Current as ApplicationTrust;
                }
                if ((current != null) && (applicationTrust != null))
                {
                    break;
                }
            }
            if (current == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Policy_MissingActivationContextInAppEvidence"));
            }
            ActivationContext activationContext = current.ActivationContext;
            if (activationContext == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Policy_MissingActivationContextInAppEvidence"));
            }
            if ((applicationTrust != null) && !CmsUtils.CompareIdentities(applicationTrust.ApplicationIdentity, current.ApplicationIdentity, ApplicationVersionMatch.MatchExactVersion))
            {
                applicationTrust = null;
            }
            if (applicationTrust == null)
            {
                if ((AppDomain.CurrentDomain.ApplicationTrust != null) && CmsUtils.CompareIdentities(AppDomain.CurrentDomain.ApplicationTrust.ApplicationIdentity, current.ApplicationIdentity, ApplicationVersionMatch.MatchExactVersion))
                {
                    applicationTrust = AppDomain.CurrentDomain.ApplicationTrust;
                }
                else
                {
                    applicationTrust = ApplicationSecurityManager.DetermineApplicationTrustInternal(activationContext, context);
                }
            }
            ApplicationSecurityInfo info = new ApplicationSecurityInfo(activationContext);
            if (((applicationTrust != null) && applicationTrust.IsApplicationTrustedToRun) && !info.DefaultRequestSet.IsSubsetOf(applicationTrust.DefaultGrantSet.PermissionSet))
            {
                throw new InvalidOperationException(Environment.GetResourceString("Policy_AppTrustMustGrantAppRequest"));
            }
            return applicationTrust;
        }

        public virtual Evidence ProvideAppDomainEvidence(Evidence inputEvidence) => 
            inputEvidence;

        public virtual Evidence ProvideAssemblyEvidence(Assembly loadedAssembly, Evidence inputEvidence) => 
            inputEvidence;

        public virtual PermissionSet ResolvePolicy(Evidence evidence) => 
            SecurityManager.PolicyManager.ResolveHelper(evidence);

        public virtual PolicyLevel DomainPolicy =>
            null;

        public virtual HostSecurityManagerOptions Flags =>
            HostSecurityManagerOptions.AllFlags;
    }
}

