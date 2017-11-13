﻿namespace System.Security
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Util;
    using System.Threading;

    [Serializable, ComVisible(true), SecurityPermission(SecurityAction.InheritanceDemand, ControlEvidence=true, ControlPolicy=true)]
    public abstract class CodeAccessPermission : IPermission, ISecurityEncodable, IStackWalk
    {
        protected CodeAccessPermission()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Assert()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            CodeAccessSecurityEngine.Assert(this, ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void AssertAllPossible()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            SecurityRuntime.AssertAllPossible(ref lookForMyCaller);
        }

        internal bool CanUnrestrictedOverride() => 
            CanUnrestrictedOverride(this);

        internal static bool CanUnrestrictedOverride(IPermission ip) => 
            (CodeAccessSecurityEngine.DoesFullTrustMeanFullTrust() || (ip is IUnrestrictedPermission));

        internal bool CheckAssert(CodeAccessPermission asserted) => 
            this.IsSubsetOf(asserted);

        internal bool CheckDemand(CodeAccessPermission grant) => 
            this.IsSubsetOf(grant);

        internal bool CheckDeny(CodeAccessPermission denied)
        {
            IPermission permission = this.Intersect(denied);
            if (permission != null)
            {
                return permission.IsSubsetOf(null);
            }
            return true;
        }

        internal bool CheckPermitOnly(CodeAccessPermission permitted) => 
            this.IsSubsetOf(permitted);

        public abstract IPermission Copy();
        internal static SecurityElement CreatePermissionElement(IPermission perm, string permname)
        {
            SecurityElement element = new SecurityElement("IPermission");
            XMLUtil.AddClassAttribute(element, perm.GetType(), permname);
            element.AddAttribute("version", "1");
            return element;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Demand()
        {
            if (!this.CheckDemand(null))
            {
                StackCrawlMark lookForMyCallersCaller = StackCrawlMark.LookForMyCallersCaller;
                CodeAccessSecurityEngine.Check(this, ref lookForMyCallersCaller);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void DemandInternal(PermissionType permissionType)
        {
            StackCrawlMark lookForMyCallersCaller = StackCrawlMark.LookForMyCallersCaller;
            CodeAccessSecurityEngine.SpecialDemand(permissionType, ref lookForMyCallersCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Deny()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            CodeAccessSecurityEngine.Deny(this, ref lookForMyCaller);
        }

        [ComVisible(false)]
        public override bool Equals(object obj)
        {
            IPermission target = obj as IPermission;
            if ((obj != null) && (target == null))
            {
                return false;
            }
            try
            {
                if (!this.IsSubsetOf(target))
                {
                    return false;
                }
                if ((target != null) && !target.IsSubsetOf(this))
                {
                    return false;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        public abstract void FromXml(SecurityElement elem);
        [ComVisible(false)]
        public override int GetHashCode() => 
            base.GetHashCode();

        public abstract IPermission Intersect(IPermission target);
        public abstract bool IsSubsetOf(IPermission target);
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PermitOnly()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            CodeAccessSecurityEngine.PermitOnly(this, ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RevertAll()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            SecurityRuntime.RevertAll(ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RevertAssert()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            SecurityRuntime.RevertAssert(ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RevertDeny()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            SecurityRuntime.RevertDeny(ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RevertPermitOnly()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            SecurityRuntime.RevertPermitOnly(ref lookForMyCaller);
        }

        public override string ToString() => 
            this.ToXml().ToString();

        public abstract SecurityElement ToXml();
        public virtual IPermission Union(IPermission other)
        {
            if (other != null)
            {
                throw new NotSupportedException(Environment.GetResourceString("NotSupported_SecurityPermissionUnion"));
            }
            return this.Copy();
        }

        internal static void ValidateElement(SecurityElement elem, IPermission perm)
        {
            if (elem == null)
            {
                throw new ArgumentNullException("elem");
            }
            if (!XMLUtil.IsPermissionElement(perm, elem))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NotAPermissionElement"));
            }
            string str = elem.Attribute("version");
            if ((str != null) && !str.Equals("1"))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidXMLBadVersion"));
            }
        }

        internal bool VerifyType(IPermission perm) => 
            ((perm != null) && (perm.GetType() == base.GetType()));
    }
}

