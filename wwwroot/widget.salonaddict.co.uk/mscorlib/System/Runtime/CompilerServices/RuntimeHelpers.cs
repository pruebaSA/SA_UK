namespace System.Runtime.CompilerServices
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;
    using System.Threading;

    public static class RuntimeHelpers
    {
        private static TryCode s_EnterMonitor = new TryCode(RuntimeHelpers.EnterMonitorAndTryCode);
        private static CleanupCode s_ExitMonitor = new CleanupCode(RuntimeHelpers.ExitMonitorOnBackout);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _CompileMethod(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void _PrepareMethod(IntPtr method, RuntimeTypeHandle[] instantiation);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void _RunClassConstructor(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void _RunModuleConstructor(IntPtr module);
        private static void EnterMonitorAndTryCode(object helper)
        {
            ExecuteWithLockHelper helper2 = (ExecuteWithLockHelper) helper;
            Monitor.ReliableEnter(helper2.m_lockObject, ref helper2.m_tookLock);
            helper2.m_userCode(helper2.m_userState);
        }

        public static bool Equals(object o1, object o2) => 
            object.InternalEquals(o1, o2);

        [PrePrepareMethod]
        internal static void ExecuteBackoutCodeHelper(object backoutCode, object userData, bool exceptionThrown)
        {
            ((CleanupCode) backoutCode)(userData, exceptionThrown);
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static extern void ExecuteCodeWithGuaranteedCleanup(TryCode code, CleanupCode backoutCode, object userData);
        [HostProtection(SecurityAction.LinkDemand, Synchronization=true), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal static void ExecuteCodeWithLock(object lockObject, TryCode code, object userState)
        {
            ExecuteWithLockHelper userData = new ExecuteWithLockHelper(lockObject, code, userState);
            ExecuteCodeWithGuaranteedCleanup(s_EnterMonitor, s_ExitMonitor, userData);
        }

        [PrePrepareMethod]
        private static void ExitMonitorOnBackout(object helper, bool exceptionThrown)
        {
            ExecuteWithLockHelper helper2 = (ExecuteWithLockHelper) helper;
            if (helper2.m_tookLock)
            {
                Monitor.Exit(helper2.m_lockObject);
            }
        }

        public static int GetHashCode(object o) => 
            object.InternalGetHashCode(o);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern object GetObjectValue(object obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void InitializeArray(Array array, RuntimeFieldHandle fldHandle);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void PrepareConstrainedRegions()
        {
            ProbeForSufficientStack();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void PrepareConstrainedRegionsNoOP()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static extern void PrepareDelegate(Delegate d);
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void PrepareMethod(RuntimeMethodHandle method)
        {
            _PrepareMethod(method.Value, null);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void PrepareMethod(RuntimeMethodHandle method, RuntimeTypeHandle[] instantiation)
        {
            _PrepareMethod(method.Value, instantiation);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static extern void ProbeForSufficientStack();
        public static void RunClassConstructor(RuntimeTypeHandle type)
        {
            _RunClassConstructor(type.Value);
        }

        public static unsafe void RunModuleConstructor(ModuleHandle module)
        {
            _RunModuleConstructor(new IntPtr(module.Value));
        }

        public static int OffsetToStringData =>
            12;

        public delegate void CleanupCode(object userData, bool exceptionThrown);

        private class ExecuteWithLockHelper
        {
            internal object m_lockObject;
            internal bool m_tookLock;
            internal RuntimeHelpers.TryCode m_userCode;
            internal object m_userState;

            internal ExecuteWithLockHelper(object lockObject, RuntimeHelpers.TryCode userCode, object userState)
            {
                this.m_lockObject = lockObject;
                this.m_userCode = userCode;
                this.m_userState = userState;
            }
        }

        public delegate void TryCode(object userData);
    }
}

