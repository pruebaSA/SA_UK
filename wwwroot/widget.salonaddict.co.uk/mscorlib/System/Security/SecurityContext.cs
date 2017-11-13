﻿namespace System.Security
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Threading;

    public sealed class SecurityContext
    {
        private static bool _alwaysFlowImpersonationPolicy = (GetImpersonationFlowMode() == WindowsImpersonationFlowMode.IMP_ALWAYSFLOW);
        private System.Threading.CompressedStack _compressedStack;
        internal SecurityContextDisableFlow _disableFlow;
        private System.Threading.ExecutionContext _executionContext;
        private static SecurityContext _fullTrustSC;
        private static bool _LegacyImpersonationPolicy = (GetImpersonationFlowMode() == WindowsImpersonationFlowMode.IMP_NOFLOW);
        private System.Security.Principal.WindowsIdentity _windowsIdentity;
        internal static RuntimeHelpers.CleanupCode cleanupCode;
        internal bool isNewCapture;
        internal static RuntimeHelpers.TryCode tryCode;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal SecurityContext()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static SecurityContext Capture()
        {
            if (IsFlowSuppressed() || !SecurityManager._IsSecurityOn())
            {
                return null;
            }
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            SecurityContext context = Capture(Thread.CurrentThread.GetExecutionContextNoCreate(), ref lookForMyCaller);
            if (context == null)
            {
                context = CreateFullTrustSecurityContext();
            }
            return context;
        }

        internal static SecurityContext Capture(System.Threading.ExecutionContext currThreadEC, ref StackCrawlMark stackMark)
        {
            if (IsFlowSuppressed() || !SecurityManager._IsSecurityOn())
            {
                return null;
            }
            if (CurrentlyInDefaultFTSecurityContext(currThreadEC))
            {
                return null;
            }
            SecurityContext context = new SecurityContext {
                isNewCapture = true
            };
            if (!IsWindowsIdentityFlowSuppressed())
            {
                context._windowsIdentity = GetCurrentWI(currThreadEC);
            }
            else
            {
                context._disableFlow = SecurityContextDisableFlow.WI;
            }
            context.CompressedStack = System.Threading.CompressedStack.GetCompressedStack(ref stackMark);
            return context;
        }

        public SecurityContext CreateCopy()
        {
            if (!this.isNewCapture)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotNewCaptureContext"));
            }
            SecurityContext context = new SecurityContext {
                isNewCapture = true,
                _disableFlow = this._disableFlow,
                _windowsIdentity = this.WindowsIdentity
            };
            if (this._compressedStack != null)
            {
                context._compressedStack = this._compressedStack.CreateCopy();
            }
            return context;
        }

        internal static SecurityContext CreateFullTrustSecurityContext()
        {
            SecurityContext context = new SecurityContext {
                isNewCapture = true
            };
            if (IsWindowsIdentityFlowSuppressed())
            {
                context._disableFlow = SecurityContextDisableFlow.WI;
            }
            context.CompressedStack = new System.Threading.CompressedStack(null);
            return context;
        }

        internal static bool CurrentlyInDefaultFTSecurityContext(System.Threading.ExecutionContext threadEC) => 
            (IsDefaultThreadSecurityInfo() && (GetCurrentWI(threadEC) == null));

        internal static SecurityContext GetCurrentSecurityContextNoCreate()
        {
            System.Threading.ExecutionContext executionContextNoCreate = Thread.CurrentThread.GetExecutionContextNoCreate();
            if (executionContextNoCreate != null)
            {
                return executionContextNoCreate.SecurityContext;
            }
            return null;
        }

        internal static System.Security.Principal.WindowsIdentity GetCurrentWI(System.Threading.ExecutionContext threadEC)
        {
            if (_alwaysFlowImpersonationPolicy)
            {
                return System.Security.Principal.WindowsIdentity.GetCurrentInternal(TokenAccessLevels.MaximumAllowed, true);
            }
            SecurityContext securityContext = threadEC?.SecurityContext;
            if (securityContext != null)
            {
                return securityContext.WindowsIdentity;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern WindowsImpersonationFlowMode GetImpersonationFlowMode();
        internal bool IsDefaultFTSecurityContext()
        {
            if (this.WindowsIdentity != null)
            {
                return false;
            }
            if (this.CompressedStack != null)
            {
                return (this.CompressedStack.CompressedStackHandle == null);
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern bool IsDefaultThreadSecurityInfo();
        public static bool IsFlowSuppressed() => 
            IsFlowSuppressed(SecurityContextDisableFlow.All);

        internal static bool IsFlowSuppressed(SecurityContextDisableFlow flags)
        {
            SecurityContext currentSecurityContextNoCreate = GetCurrentSecurityContextNoCreate();
            return ((currentSecurityContextNoCreate != null) && ((currentSecurityContextNoCreate._disableFlow & flags) == flags));
        }

        public static bool IsWindowsIdentityFlowSuppressed()
        {
            if (!_LegacyImpersonationPolicy)
            {
                return IsFlowSuppressed(SecurityContextDisableFlow.WI);
            }
            return true;
        }

        public static void RestoreFlow()
        {
            SecurityContext currentSecurityContextNoCreate = GetCurrentSecurityContextNoCreate();
            if ((currentSecurityContextNoCreate == null) || (currentSecurityContextNoCreate._disableFlow == SecurityContextDisableFlow.Nothing))
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotRestoreUnsupressedFlow"));
            }
            currentSecurityContextNoCreate._disableFlow = SecurityContextDisableFlow.Nothing;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static void Run(SecurityContext securityContext, ContextCallback callback, object state)
        {
            if (securityContext == null)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullContext"));
            }
            if (!securityContext.isNewCapture)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotNewCaptureContext"));
            }
            securityContext.isNewCapture = false;
            if (CurrentlyInDefaultFTSecurityContext(Thread.CurrentThread.GetExecutionContextNoCreate()) && securityContext.IsDefaultFTSecurityContext())
            {
                callback(state);
            }
            else
            {
                RunInternal(securityContext, callback, state);
            }
        }

        [PrePrepareMethod]
        internal static void runFinallyCode(object userData, bool exceptionThrown)
        {
            SecurityContextRunData data = (SecurityContextRunData) userData;
            data.scsw.Undo();
        }

        internal static void RunInternal(SecurityContext securityContext, ContextCallback callBack, object state)
        {
            if (cleanupCode == null)
            {
                tryCode = new RuntimeHelpers.TryCode(SecurityContext.runTryCode);
                cleanupCode = new RuntimeHelpers.CleanupCode(SecurityContext.runFinallyCode);
            }
            SecurityContextRunData userData = new SecurityContextRunData(securityContext, callBack, state);
            RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(tryCode, cleanupCode, userData);
        }

        internal static void runTryCode(object userData)
        {
            SecurityContextRunData data = (SecurityContextRunData) userData;
            data.scsw = SetSecurityContext(data.sc, Thread.CurrentThread.ExecutionContext.SecurityContext);
            data.callBack(data.state);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static SecurityContextSwitcher SetSecurityContext(SecurityContext sc, SecurityContext prevSecurityContext)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return SetSecurityContext(sc, prevSecurityContext, ref lookForMyCaller);
        }

        internal static SecurityContextSwitcher SetSecurityContext(SecurityContext sc, SecurityContext prevSecurityContext, ref StackCrawlMark stackMark)
        {
            SecurityContextDisableFlow flow = sc._disableFlow;
            sc._disableFlow = SecurityContextDisableFlow.Nothing;
            SecurityContextSwitcher switcher = new SecurityContextSwitcher {
                currSC = sc
            };
            System.Threading.ExecutionContext executionContext = Thread.CurrentThread.ExecutionContext;
            switcher.currEC = executionContext;
            switcher.prevSC = prevSecurityContext;
            executionContext.SecurityContext = sc;
            if (sc != null)
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    switcher.wic = null;
                    if (!_LegacyImpersonationPolicy)
                    {
                        if (sc.WindowsIdentity != null)
                        {
                            switcher.wic = sc.WindowsIdentity.Impersonate(ref stackMark);
                        }
                        else if ((((flow & SecurityContextDisableFlow.WI) == SecurityContextDisableFlow.Nothing) && (prevSecurityContext != null)) && (prevSecurityContext.WindowsIdentity != null))
                        {
                            switcher.wic = System.Security.Principal.WindowsIdentity.SafeImpersonate(SafeTokenHandle.InvalidHandle, null, ref stackMark);
                        }
                    }
                    switcher.cssw = System.Threading.CompressedStack.SetCompressedStack(sc.CompressedStack, prevSecurityContext?.CompressedStack);
                }
                catch
                {
                    switcher.UndoNoThrow();
                    throw;
                }
            }
            return switcher;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static AsyncFlowControl SuppressFlow() => 
            SuppressFlow(SecurityContextDisableFlow.All);

        internal static AsyncFlowControl SuppressFlow(SecurityContextDisableFlow flags)
        {
            if (IsFlowSuppressed(flags))
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotSupressFlowMultipleTimes"));
            }
            if (Thread.CurrentThread.ExecutionContext.SecurityContext == null)
            {
                Thread.CurrentThread.ExecutionContext.SecurityContext = new SecurityContext();
            }
            AsyncFlowControl control = new AsyncFlowControl();
            control.Setup(flags);
            return control;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static AsyncFlowControl SuppressFlowWindowsIdentity() => 
            SuppressFlow(SecurityContextDisableFlow.WI);

        internal System.Threading.CompressedStack CompressedStack
        {
            get => 
                this._compressedStack;
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            set
            {
                this._compressedStack = value;
            }
        }

        internal System.Threading.ExecutionContext ExecutionContext
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            set
            {
                this._executionContext = value;
            }
        }

        internal static SecurityContext FullTrustSecurityContext
        {
            get
            {
                if (_fullTrustSC == null)
                {
                    _fullTrustSC = CreateFullTrustSecurityContext();
                }
                return _fullTrustSC;
            }
        }

        internal System.Security.Principal.WindowsIdentity WindowsIdentity
        {
            get => 
                this._windowsIdentity;
            set
            {
                this._windowsIdentity = value;
            }
        }

        internal class SecurityContextRunData
        {
            internal ContextCallback callBack;
            internal SecurityContext sc;
            internal SecurityContextSwitcher scsw;
            internal object state;

            internal SecurityContextRunData(SecurityContext securityContext, ContextCallback cb, object state)
            {
                this.sc = securityContext;
                this.callBack = cb;
                this.state = state;
                this.scsw = new SecurityContextSwitcher();
            }
        }
    }
}

