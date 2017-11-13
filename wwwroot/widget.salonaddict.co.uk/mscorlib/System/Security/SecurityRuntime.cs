namespace System.Security
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class SecurityRuntime
    {
        internal const bool StackContinue = true;
        internal const bool StackHalt = false;

        private SecurityRuntime()
        {
        }

        internal static void Assert(PermissionSet permSet, ref StackCrawlMark stackMark)
        {
            FrameSecurityDescriptor descriptor = CodeAccessSecurityEngine.CheckNReturnSO(CodeAccessSecurityEngine.AssertPermissionToken, CodeAccessSecurityEngine.AssertPermission, ref stackMark, 1, 1);
            if (descriptor == null)
            {
                if (SecurityManager._IsSecurityOn())
                {
                    throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
                }
            }
            else
            {
                if (descriptor.HasImperativeAsserts())
                {
                    throw new SecurityException(Environment.GetResourceString("Security_MustRevertOverride"));
                }
                descriptor.SetAssert(permSet);
            }
        }

        internal static void AssertAllPossible(ref StackCrawlMark stackMark)
        {
            FrameSecurityDescriptor securityObjectForFrame = GetSecurityObjectForFrame(ref stackMark, true);
            if (securityObjectForFrame == null)
            {
                if (SecurityManager._IsSecurityOn())
                {
                    throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
                }
            }
            else
            {
                if (securityObjectForFrame.GetAssertAllPossible())
                {
                    throw new SecurityException(Environment.GetResourceString("Security_MustRevertOverride"));
                }
                securityObjectForFrame.SetAssertAllPossible();
            }
        }

        [SecurityCritical]
        private static bool CheckDynamicMethodHelper(DynamicResolver dynamicResolver, IPermission demandIn, PermissionToken permToken, RuntimeMethodHandle rmh)
        {
            bool flag;
            CompressedStack securityContext = dynamicResolver.GetSecurityContext();
            try
            {
                flag = securityContext.CheckDemandNoHalt((CodeAccessPermission) demandIn, permToken, rmh);
            }
            catch (SecurityException exception)
            {
                throw new SecurityException(Environment.GetResourceString("Security_AnonymouslyHostedDynamicMethodCheckFailed"), exception);
            }
            return flag;
        }

        [SecurityCritical]
        private static bool CheckDynamicMethodSetHelper(DynamicResolver dynamicResolver, PermissionSet demandSet, out PermissionSet alteredDemandSet, RuntimeMethodHandle rmh)
        {
            bool flag;
            CompressedStack securityContext = dynamicResolver.GetSecurityContext();
            try
            {
                flag = securityContext.CheckSetDemandWithModificationNoHalt(demandSet, out alteredDemandSet, rmh);
            }
            catch (SecurityException exception)
            {
                throw new SecurityException(Environment.GetResourceString("Security_AnonymouslyHostedDynamicMethodCheckFailed"), exception);
            }
            return flag;
        }

        internal static void Deny(PermissionSet permSet, ref StackCrawlMark stackMark)
        {
            FrameSecurityDescriptor securityObjectForFrame = GetSecurityObjectForFrame(ref stackMark, true);
            if (securityObjectForFrame == null)
            {
                if (SecurityManager._IsSecurityOn())
                {
                    throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
                }
            }
            else
            {
                if (securityObjectForFrame.HasImperativeDenials())
                {
                    throw new SecurityException(Environment.GetResourceString("Security_MustRevertOverride"));
                }
                securityObjectForFrame.SetDeny(permSet);
            }
        }

        private static bool FrameDescHelper(FrameSecurityDescriptor secDesc, IPermission demandIn, PermissionToken permToken, RuntimeMethodHandle rmh) => 
            secDesc.CheckDemand((CodeAccessPermission) demandIn, permToken, rmh);

        private static bool FrameDescSetHelper(FrameSecurityDescriptor secDesc, PermissionSet demandSet, out PermissionSet alteredDemandSet, RuntimeMethodHandle rmh) => 
            secDesc.CheckSetDemand(demandSet, out alteredDemandSet, rmh);

        internal static MethodInfo GetMethodInfo(RuntimeMethodHandle rmh)
        {
            if (rmh.IsNullHandle())
            {
                return null;
            }
            PermissionSet.s_fullTrust.Assert();
            return (RuntimeType.GetMethodBase(rmh.GetDeclaringType(), rmh) as MethodInfo);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern FrameSecurityDescriptor GetSecurityObjectForFrame(ref StackCrawlMark stackMark, bool create);
        private static int OverridesHelper(FrameSecurityDescriptor secDesc) => 
            (OverridesHelper2(secDesc, false) + OverridesHelper2(secDesc, true));

        private static int OverridesHelper2(FrameSecurityDescriptor secDesc, bool fDeclarative)
        {
            int num = 0;
            if (secDesc.GetPermitOnly(fDeclarative) != null)
            {
                num++;
            }
            if (secDesc.GetDenials(fDeclarative) != null)
            {
                num++;
            }
            return num;
        }

        internal static void PermitOnly(PermissionSet permSet, ref StackCrawlMark stackMark)
        {
            FrameSecurityDescriptor securityObjectForFrame = GetSecurityObjectForFrame(ref stackMark, true);
            if (securityObjectForFrame == null)
            {
                if (SecurityManager._IsSecurityOn())
                {
                    throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
                }
            }
            else
            {
                if (securityObjectForFrame.HasImperativeRestrictions())
                {
                    throw new SecurityException(Environment.GetResourceString("Security_MustRevertOverride"));
                }
                securityObjectForFrame.SetPermitOnly(permSet);
            }
        }

        internal static void RevertAll(ref StackCrawlMark stackMark)
        {
            FrameSecurityDescriptor securityObjectForFrame = GetSecurityObjectForFrame(ref stackMark, false);
            if (securityObjectForFrame != null)
            {
                securityObjectForFrame.RevertAll();
            }
            else if (SecurityManager._IsSecurityOn())
            {
                throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
            }
        }

        internal static void RevertAssert(ref StackCrawlMark stackMark)
        {
            FrameSecurityDescriptor securityObjectForFrame = GetSecurityObjectForFrame(ref stackMark, false);
            if (securityObjectForFrame != null)
            {
                securityObjectForFrame.RevertAssert();
            }
            else if (SecurityManager._IsSecurityOn())
            {
                throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
            }
        }

        internal static void RevertDeny(ref StackCrawlMark stackMark)
        {
            FrameSecurityDescriptor securityObjectForFrame = GetSecurityObjectForFrame(ref stackMark, false);
            if (securityObjectForFrame != null)
            {
                securityObjectForFrame.RevertDeny();
            }
            else if (SecurityManager._IsSecurityOn())
            {
                throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
            }
        }

        internal static void RevertPermitOnly(ref StackCrawlMark stackMark)
        {
            FrameSecurityDescriptor securityObjectForFrame = GetSecurityObjectForFrame(ref stackMark, false);
            if (securityObjectForFrame != null)
            {
                securityObjectForFrame.RevertPermitOnly();
            }
            else if (SecurityManager._IsSecurityOn())
            {
                throw new ExecutionEngineException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
            }
        }
    }
}

